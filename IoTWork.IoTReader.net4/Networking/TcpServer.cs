using IoTWork.IoTReader.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IoTWork.Infrastructure.Statistics;
using System.Net;
using System.Net.Sockets;
using IoTWork.IoTReader.Helper;
using System.Collections.Concurrent;
using System.Text;
using IoTWork.Infrastructure;
using System.Threading;

namespace IoTWork.IoTReader.Core.Networking
{
    public class TcpServer : IServerForReceive
    {
        // https://msdn.microsoft.com/it-it/library/6y0e13d3(v=vs.110).aspx

        Socket _listener;
        Int32 _port;

        Boolean _mustrun;
        Thread _thread;

        String _serveraddress;

        ConcurrentQueue<byte[]> _outputqueue;

        public TcpServer(String TcpServerAddress)
        {
            _listener = null;
            _mustrun = false;
            _thread = null;
            _outputqueue = new ConcurrentQueue<byte[]>();
            _serveraddress = TcpServerAddress;
        }

        public void Close()
        {
            _mustrun = false;
            try
            {
                _listener.Dispose();
            }
            catch(Exception ex)
            {

            }
            while (!_thread.Join(100)) ;
        }

        public ErrorResume GetErrors()
        {
            throw new NotImplementedException();
        }

        public Statistics GetStatistics()
        {
            throw new NotImplementedException();
        }

        public void Start(Int32 port)
        {
            _port = port;

            _mustrun = true;
            _thread = new Thread(this._Run);
            _thread.Start();
        }

        public bool TryReceive(out byte[] data)
        {
            data = null;
            return _outputqueue.TryDequeue(out data);
        }

        private void _Run()
        {
            while(_mustrun)
            {
                // Bind the socket to the local endpoint and 
                // listen for incoming connections.

                TcpListener server = null;

                try
                {
                    byte[] bytes = new Byte[1024];
                    string data = null;

                    #region Accept
                    try
                    {
                        IPAddress ipAddress = null;
                        if (!String.IsNullOrEmpty(_serveraddress))
                            ipAddress = IPAddress.Parse(_serveraddress);
                        else
                            ipAddress = IoTReaderHelper.GetLocalIpAddress();
                        LogManager.Debug("Start TcpServer at {0} o port {1}", ipAddress.ToString(), _port);
                        server = new TcpListener(ipAddress, _port);
                        server.Start();
                    }
                    catch(Exception ex)
                    {
                        LogManager.Error(ex.Message, ex);
                        server = null;
                    }
                    #endregion

                    while (_mustrun)
                    {
                        
                        if (server != null)
                        {
                            TcpClient client = server.AcceptTcpClient();
                            NetworkStream stream = client.GetStream();

                            #region read
                            try
                            {
                                int i;
                                data = String.Empty;
                                int readdata = 0;
                                byte[] dataarray = new byte[2 * 1024 * 1024];
                                byte[] readbytes = new byte[8 * 1024];
                                while ((i = stream.Read(readbytes, 0, readbytes.Length)) != 0)
                                {
                                    Array.Copy(readbytes, 0, dataarray, readdata, i);
                                    readdata += i;
                                }

                                byte[] receivedBytes = new byte[readdata];
                                Array.Copy(dataarray, 0, receivedBytes, 0, readdata);
                                _outputqueue.Enqueue(receivedBytes);
                            }
                            catch (Exception ex)
                            {
                                LogManager.Error(ex.Message, ex);
                            }
                            #endregion

                            #region shutdown
                            try
                            {
                                client.Close();
                            }
                            catch (Exception ex)
                            {
                                LogManager.Error(ex.Message, ex);
                            }
                            #endregion
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.Error(ex.Message, ex);
                }


                Thread.Sleep(100);
            }
        }

        public static bool IsSocketConnected(Socket s)
        {
            return !((s.Poll(1000, SelectMode.SelectRead) && (s.Available == 0)) || !s.Connected);

            /* The long, but simpler-to-understand version:

                    bool part1 = s.Poll(1000, SelectMode.SelectRead);
                    bool part2 = (s.Available == 0);
                    if ((part1 && part2 ) || !s.Connected)
                        return false;
                    else
                        return true;

            */
        }
    }
}
