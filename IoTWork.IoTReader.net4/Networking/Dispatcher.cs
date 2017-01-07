using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IoTWork.IoTReader.Interfaces;
using IoTWork.IoTReader.Pipes;
using IoTWork.Infrastructure.Interfaces;
using System.Threading;
using IoTWork.Infrastructure.Networking;
using IoTWork.Protocol;
using IoTWork.IoTReader.Management;
using IoTWork.Protocol.Types;
using IoTWork.IoTReader.Utils;
using IoTWork.Infrastructure.Management;
using IoTWork.Infrastructure;
using IoTWork.Infrastructure.Statistics;
using IoTWork.IoTReader.Helper;
using IotWork.Utils.Helpers;
using IoTWork.Protocol.Devices;

namespace IoTWork.IoTReader.Networking
{
    internal class Dispatcher: IDispatcher
    {
        Context _context;

        IJunction<ISample> _junction;

        IClient _client;

        ICompressor _iotpacket_compressor;
        IFormatter<IIotPacket> _iotpacket_formatter;
        ISigner<IIotPacket> _iotpacket_signer;

        ICompressor _payload_compressor;
        IFormatter<Payload> _payload_formatter;
        ISigner<Payload> _payload_signer;

        IPacketManagerOutput _packetManagerOutput;
        IPacketManagerInput _packetManagerInput;
        NetworkManager<IotPacket> _networkManager;

        bool _mustrun;
        Task _thread;

        private Journal<ExceptionContainer> _exceptions;
        private Dictionary<string, string> _statistics;

        private object _exlock;
        private object _stlock;

        internal Dispatcher()
        {
            JunctionName = "Dispatcher";

            _junction = null;

            _client = null;

            _iotpacket_compressor = null;
            _iotpacket_formatter = null;
            _iotpacket_signer = null;

            _payload_compressor = null;
            _payload_formatter = null;
            _payload_signer = null;

            _mustrun = false;
            _thread = new Task(() => _Run());

            _exceptions = new Journal<ExceptionContainer>(200);
            _statistics = new Dictionary<string, string>();

            _exlock = new object();
            _stlock = new object();
        }

        public string JunctionName
        {
            get;
            set;
        }

        public virtual void Close()
        {
            _mustrun = false;
            while (!_thread.Wait(100)) ;
        }

        public virtual void Pause()
        {
        }

        public virtual void Play()
        {
        }

        public void SetClient(IClient client)
        {
            _client = client;
        }

        public void SetPacketCompressor(ICompressor compressor)
        { 
        
            _iotpacket_compressor = compressor;
        }

        public void SetPacketFormatter(IFormatter<IIotPacket> formatter)
        {
            _iotpacket_formatter = formatter;
        }

        public void SetPacketSigner(ISigner<IIotPacket> signer)
        {
            _iotpacket_signer = signer;
        }

        public void SetPayloadCompressor(ICompressor compressor)
        {
            _payload_compressor = compressor;
        }

        public void SetPayloadFormatter(IFormatter<Payload> formatter)
        {
            _payload_formatter = formatter;
        }

        public void SetPayloadSigner(ISigner<Payload> signer)
        {
            _payload_signer = signer;
        }

        public void SetJunction(IJunction<ISample> junction)
        {
        }

        public void SetJunction(IJunction<ISample> junction, JunctionPointDirection Direction)
        {
            if (Direction == JunctionPointDirection.Destination)
                _junction = junction;
        }

        public void SetContext(Context context)
        {
            _context = context;
        }

        public void Build()
        {
            _packetManagerOutput = new PacketManagerOutput();
            _packetManagerInput = new PacketManagerInput();
            _networkManager = new NetworkManager<IotPacket>();

            _packetManagerOutput.SetPacketFormatter(_iotpacket_formatter);
            _packetManagerOutput.SetPayloadFormatter(_payload_formatter);
            _packetManagerOutput.SetCompressor(_iotpacket_compressor);
            _packetManagerOutput.SetPacketSigner(_iotpacket_signer);
            _packetManagerOutput.SetPayloadSigner(_payload_signer);

            _networkManager.SetPacketManagerOutput(_packetManagerOutput);
            _networkManager.SetPacketManagerInput(_packetManagerInput);
            _networkManager.SetClient(_client);
        }

        public virtual void Skip()
        {
        }

        public virtual void Start()
        {
            _networkManager.Start();

            _mustrun = true;
            _thread.Start();
        }

        private void _Run()
        {
            Int32 SequenceNumber = 0;
            DateTime? FirstTriggeredOn = null;
            DateTime LastTriggeredOn = DateTime.Now;

            while (_mustrun)
            {
                if (_junction != null)
                {
                    try
                    {
                        ISample sample = null;
                        bool dequeued = false;

                        try
                        {
                            dequeued = _junction.TryDequeue(out sample);
                        }
                        catch (Exception)
                        {
                            dequeued = false;
                        }

                        if (dequeued)
                        {
                            SequenceNumber++;

                            if (!FirstTriggeredOn.HasValue)
                                FirstTriggeredOn = DateTime.Now;
                            LastTriggeredOn = DateTime.Now;

                            IotPacket packetData = _context.PacketFactory.BuildPacket_Measures(sample);

                            _networkManager.SendMessage(packetData);

                            RegisterStatistics(SequenceNumber, FirstTriggeredOn.Value, LastTriggeredOn);
                        }
                    }
                    catch(Exception ex)
                    {
                        LogManager.Error(LogFormat.Format("Dispatcher error : {1}", ex.Message), ex);

                        RegisterException("Dispatcher", DateTime.Now, SequenceNumber, ex);

                    }
                    Thread.Sleep(50);
                }
            }
        }

        public Infrastructure.Statistics.Statistics GetStatistics()
        {
            Infrastructure.Statistics.Statistics statistics = new Infrastructure.Statistics.Statistics();

            var clientStatistics = _client.GetStatistics();

            statistics.Add(clientStatistics);

            lock (_stlock)
            {
                foreach(var s in _statistics)
                {
                    statistics.Add("Dispatcher", s.Key, s.Value, NoteDomain.Network);
                }
            }

            return statistics;
        }

        public ErrorResume GetErrors()
        {
            ErrorResume _resume = new ErrorResume();

            lock (_exlock)
            {
                _resume = IotWork.Utils.Helpers.IoTWorkHelper.ToErrorResume(_exceptions);
            }

            var clientErrors = _client.GetErrors();
            if (clientErrors != null)
            {
                _resume.Add(clientErrors);
            }

            return _resume;
        }

        public bool IsFree()
        {
            return true;
        }

        private void RegisterException(String UniqueName, DateTime When, Int32 SequenceNumber, Exception Exception)
        {
            ExceptionContainer ec = new ExceptionContainer();
            ec.UniqueName = UniqueName;
            ec.When = When;
            ec.Module = String.Empty;
            ec.Order = SequenceNumber;
            ec.ex = Exception;

            lock (_exlock)
            {
                _exceptions.Add(ec);
            }
        }

        public void RegisterStatistics(int SequenceNumber, DateTime FirstTriggeredOn, DateTime LastTriggeredOn)
        {
            lock (_stlock)
            {
                if (!_statistics.ContainsKey("SamplingSequenceNumber"))
                    _statistics.Add("SamplingSequenceNumber", SequenceNumber.ToString());
                else
                    _statistics["SamplingSequenceNumber"] = SequenceNumber.ToString();

                if (!_statistics.ContainsKey("FirstTriggeredOn"))
                    _statistics.Add("FirstTriggeredOn", FirstTriggeredOn.ToString());
                else
                    _statistics["FirstTriggeredOn"] = FirstTriggeredOn.ToString();

                if (!_statistics.ContainsKey("LastTriggeredOn"))
                    _statistics.Add("LastTriggeredOn", LastTriggeredOn.ToString());
                else
                    _statistics["LastTriggeredOn"] = LastTriggeredOn.ToString();
            }
        }
    }
}
