using IoTWork.IoTReader.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IoTWork.IoTReader.Management;
using System.Threading;
using System.Collections.Concurrent;
using IoTWork.Infrastructure.Management;
using IoTWork.Infrastructure.Statistics;
using IoTWork.IoTReader.Helper;
using IotWork.Utils.Helpers;
using IoTWork.Protocol.Devices;

namespace IoTWork.IoTReader.Networking
{
    internal class UdpClient : IClient
    {
        // http://www.codeproject.com/Articles/10649/An-Introduction-to-Socket-Programming-in-NET-using
        // http://forum.unity3d.com/threads/is-there-a-way-to-force-a-udp-client-recieve-timeout-solved.288916/
        // https://social.msdn.microsoft.com/Forums/en-US/51e3fb86-f0ec-4c6e-886e-ae2ac39c5997/c-setting-udp-socket-receive-timeout?forum=netfxnetcom

        Uri _uri;
        System.Net.IPAddress _ipaddress;
        int _port;
        System.Net.Sockets.UdpClient _client = null;

        bool _mustrun;
        Task _thread;

        byte[] _tmpReceiveBuffer;
        ConcurrentQueue<byte[]> _inputqueue;
        ConcurrentQueue<byte[]> _outputqueue;

        private Journal<ExceptionContainer> _exceptions;
        private object _exlock;

        private Int32 SequenceNumber;

        private Int64 BytesSent;
        private Int64 BytesReceived;
        private Int64 PacketsSent;
        private Int64 PacketsReceived;

        private String ModuleName;

        public UdpClient()
        {
            _client = null;
            _uri = null;
            _thread = null;
            _mustrun = false;

            _inputqueue = new ConcurrentQueue<byte[]>();
            _outputqueue = new ConcurrentQueue<byte[]>();

            _exceptions = new Journal<ExceptionContainer>(100);
            _exlock = new object();

            SequenceNumber = 0;

            BytesSent = 0;
            BytesReceived = 0;

            PacketsSent = 0;
            PacketsReceived = 0;

            ModuleName = String.Empty;
        }

        public void SetModuleName(string ModuleName)
        {
            this.ModuleName = ModuleName;
        }

        public void SetUri(Uri uri)
        {
            _uri = uri;
            _ipaddress = _uri.Host == "localhost" ? System.Net.IPAddress.Loopback : System.Net.IPAddress.Parse(_uri.Host);
            _port = _uri.Port;
        }

        public void Connect()
        {
            try
            {
                if (_uri != null)
                {
                    _tmpReceiveBuffer = null;

                    _inputqueue = new ConcurrentQueue<byte[]>();
                    _outputqueue = new ConcurrentQueue<byte[]>();

                    // http://stackoverflow.com/questions/2727609/best-way-to-create-ipendpoint-from-string
                    System.Net.IPEndPoint ep =
                        new System.Net.IPEndPoint(_ipaddress, _port);

                    _client = new System.Net.Sockets.UdpClient();
                    _client.Client.Connect(ep);

                    _mustrun = true;
                    _thread = new Task(() => this._Run());

                    _thread.Start();
                }
            }
            catch (Exception ex)
            {
                RegisterException("UdpClient", "Connect", DateTime.Now, 0, ex);
            }
        }

        public void Close()
        {
            if (_thread != null)
            {
                _mustrun = false;
                _thread.Wait();
                _thread = null;
            }
            if (_client != null)
            {
                _client.Close();
                _client = null;
            }
        }

        public Infrastructure.Statistics.Statistics GetStatistics()
        {
            Infrastructure.Statistics.Statistics statistics = new Infrastructure.Statistics.Statistics();

            statistics.Add("Udp", "BytesSent", BytesSent.ToString(), NoteDomain.Network);
            statistics.Add("Udp", "BytesReceived", BytesReceived.ToString(), NoteDomain.Network);
            statistics.Add("Udp", "PacketsSent", PacketsSent.ToString(), NoteDomain.Network);
            statistics.Add("Udp", "PacketsReceived", PacketsReceived.ToString(), NoteDomain.Network);

            return statistics;
        }

        public void Send(byte[] data)
        {
            _outputqueue.Enqueue(data);
        }

        public bool TryReceive(out byte[] data)
        {
            data = null;
            return _inputqueue.TryDequeue(out data);
        }

        public void Reconnect()
        {
            throw new NotImplementedException();
        }

        public void _Run()
        {
            BytesSent = 0;
            BytesReceived = 0;

            PacketsSent = 0;
            PacketsReceived = 0;

            while (_mustrun)
            {
                byte[] tosend = null;
                bool havetosend = false;

                try
                {
                    SequenceNumber++;

                    havetosend = _outputqueue.TryDequeue(out tosend);

                    if (havetosend && _client != null)
                    {
                        BytesSent += tosend.Length;
                        PacketsSent++;
                        SendFrame(tosend);
                    }
                }
                catch (Exception ex)
                {
                    RegisterException("UdpClient", "Send", DateTime.Now, SequenceNumber, ex);
                }

                if (!_mustrun)
                    break;

                try
                {
                    SequenceNumber++;

                    byte[] toreceive = null;

                    // handle receive by packets

                    if (_client != null)
                        toreceive = TryReceiveFrame();

                    if (toreceive != null)
                    {
                        BytesReceived += toreceive.Length;
                        PacketsReceived++;
                        _inputqueue.Enqueue(toreceive);
                    }
                }
                catch (Exception ex)
                {
                    RegisterException("UdpClient", "Receive", DateTime.Now, 0, ex);
                }

                // just to avoid 30% CPU
                Thread.Sleep(50);
            }
        }

        private void SendFrame(byte[] data)
        {
            byte[] buffer = new byte[data.Length + 4];
            uint length = (uint)data.Length;

            byte[] lengthbytes = BitConverter.GetBytes(length);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(lengthbytes);

            System.Buffer.BlockCopy(lengthbytes, 0, buffer, 0, 4);
            System.Buffer.BlockCopy(data, 0, buffer, 4, (int)length);

            _client.Client.Send(buffer);
        }

        private byte[] TryReceiveFrame()
        {
            Boolean gotdata = false;
            byte[] receivedbytes = null;

            try
            {
                // get data
                if (_client.Available > 0)
                {
                    System.Net.EndPoint ep = null;
                    System.Net.IPEndPoint ipep = null;

                    byte[] data = null;
                    var receiveres = _client.Client.ReceiveFrom(data, ref ep);

                    if (receiveres > 0 && data != null)
                    {
                        ipep = (System.Net.IPEndPoint)ep;
                        if (ipep.Address.Equals(_ipaddress))
                        {
                            if (_tmpReceiveBuffer == null)
                            {
                                _tmpReceiveBuffer = data;
                            }
                            else
                            {
                                var a1 = _tmpReceiveBuffer;
                                var a2 = data;

                                byte[] rv = new byte[a1.Length + a2.Length];
                                System.Buffer.BlockCopy(a1, 0, rv, 0, a1.Length);
                                System.Buffer.BlockCopy(a2, 0, rv, a1.Length, a2.Length);

                                _tmpReceiveBuffer = rv;
                            }
                            gotdata = true;
                        }
                    }
                }

                // try build a frame
                if (gotdata && _tmpReceiveBuffer.Length > 4)
                {
                    byte[] lengthbytes = new byte[4];

                    lengthbytes[0] = _tmpReceiveBuffer[0];
                    lengthbytes[1] = _tmpReceiveBuffer[1];
                    lengthbytes[2] = _tmpReceiveBuffer[2];
                    lengthbytes[3] = _tmpReceiveBuffer[3];

                    uint length = BitConverter.ToUInt32(lengthbytes, 0);

                    if (_tmpReceiveBuffer.Length + 4 >= length)
                    {
                        receivedbytes = new byte[length];

                        Array.Copy(_tmpReceiveBuffer, 4, receivedbytes, 0, (int)length);

                        if (_tmpReceiveBuffer.Length + 4 >= length + 4)
                        {
                            _tmpReceiveBuffer = null;
                        }
                        else
                        {
                            byte[] tb = new byte[_tmpReceiveBuffer.Length - 4 - length];
                            Array.Copy(_tmpReceiveBuffer, (int)(length + 4), tb, 0, (int)(_tmpReceiveBuffer.Length - 4 - length));
                            _tmpReceiveBuffer = tb;
                        }
                    }
                }
            }
            catch (Exception)
            {
                receivedbytes = null;
            }

            return receivedbytes;
        }

        private void RegisterException(String UniqueName, String Module, DateTime When, Int32 SequenceNumber, Exception Exception)
        {
            ExceptionContainer ec = new ExceptionContainer();
            ec.UniqueName = UniqueName;
            ec.When = When;
            ec.Module = ModuleName + "." + Module;
            ec.Order = SequenceNumber;
            ec.ex = Exception;

            lock (_exlock)
            {
                _exceptions.Add(ec);
            }
        }

        public ErrorResume GetErrors()
        {
            ErrorResume _resume = new ErrorResume();

            lock (_exlock)
            {
                _resume = IotWork.Utils.Helpers.IoTWorkHelper.ToErrorResume(_exceptions);
            }

            return _resume;
        }

        public bool IsOutputVoid()
        {
            return (_outputqueue.Count == 0);
        }
    }
}
