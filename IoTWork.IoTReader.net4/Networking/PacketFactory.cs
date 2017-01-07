using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IoTWork.Protocol.Devices;
using IoTWork.Protocol;
using IoTWork.Protocol.Types;
using IoTWork.IoTReader.Management;
using IoTWork.IoTReader.Interfaces;
using IoTWork.Protocol.Datas;
using IoTWork.IoTReader.Utils;
using IoTWork.Infrastructure.Statistics;
using IoTWork.Infrastructure.Helpers;

namespace IoTWork.IoTReader.Networking
{
    public class PacketFactory
    {
        private DateTime _now;
        private Context _context;

        private object _sequenceNumberDatasLocker;
        private ulong _sequenceNumberDatas = 0;

        private object _sequenceNumberDeviceLocker;
        private ulong _sequenceNumberDevice = 0;

        private static readonly Lazy<PacketFactory> _instance
            = new Lazy<PacketFactory>(() => new PacketFactory());

        private PacketFactory()
        {
            _sequenceNumberDatasLocker = new object();
            _sequenceNumberDeviceLocker = new object();
        }

        public static PacketFactory Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        internal void SetContext(Context context)
        {
            _context = context;
        }


        private ulong BuildSequenceNumber(ServiceCodes SCode)
        {
            ulong value = 0;
            if (SCode == ServiceCodes.Device)
            {
                lock (_sequenceNumberDatasLocker)
                {
                    value = ++_sequenceNumberDatas;
                }
            }
            else if (SCode == ServiceCodes.Datas)
            {
                lock (_sequenceNumberDeviceLocker)
                {
                    value = ++_sequenceNumberDevice;
                }
            }

            return value;
        }

        private void BuildNow()
        {
            _now = DateTime.Now;
        }

        private Header BuildHeader(ServiceCodes SCode, PacketCodes PCode, String SensorUniqueAddress = null, String GUID = null)
        {
            Header h = new Header();

            h.GUID = GUID;

            h.VersionMajor = _context.ProtocolVersionMajor;
            h.VersionMinor = _context.ProtocolVersionMinor;

            h.DeviceUniqueAddress = _context.NetworkId + "." + _context.RegionId + "." + _context.RingId + "." + _context.DeviceId;
            h.SensorUniqueAddress = String.IsNullOrEmpty(SensorUniqueAddress) ? String.Empty : SensorUniqueAddress;

            h.Region = String.Empty;

            h.SourceAdrress = _context.LocalIpAddress.ToString();
            h.Traversed = new List<string>();

            h.ServiceCode = (ushort)SCode;
            h.PacketCode = (ushort)PCode;

            h.SentAt = _now;
            h.SequenceNumber = BuildSequenceNumber(SCode);

            h.HMacPacket = String.Empty;
            h.HMacPayload = String.Empty;

            return h;
        }


        internal IotPacket BuildPacket_ManagerStarted(DateTime StartedAt)
        {
            IotPacket p = new IotPacket();
            ManagerStarted payload = new ManagerStarted();

            BuildNow();

            payload.StartedAt = StartedAt;

            p.Header = BuildHeader(ServiceCodes.Device, PacketCodes.ManagerStarted);
            p.Payload = payload;
            return p;
        }

        internal IotPacket BuildPacket_Alive(TimeSpan UpTime, AcquireStatus AcquireStatus, Dictionary<string, IoTWork.Infrastructure.Statistics.Statistics> Statistics)
        {
            IotPacket p = new IotPacket();
            Alive payload = new Alive();

            BuildNow();

            payload.UpTime = UpTime;
            payload.Status = AcquireStatus;
            payload.Notes = new List<Note>();
            foreach (var s in Statistics)
            {
                Note sensor = new Note();
                sensor.Name = s.Key;
                sensor.Value = s.Value.ToString();
            }

            p.Header = BuildHeader(ServiceCodes.Device, PacketCodes.Alive);
            p.Payload = payload;
            return p;
        }

        internal IotPacket BuildPacket_Measures(ISample sample)
        {
            IotPacket p = new IotPacket();
            Measures payload = new Measures();

            BuildNow();

            payload.datas = new List<Protocol.Datas.Sample>();
            payload.datas.Add(new Protocol.Datas.Sample() { AcquiredAt = sample.ProducedAt, data = sample.CurrentSample });

            p.Header = BuildHeader(ServiceCodes.Datas, PacketCodes.Measure, sample.Source);
            p.Payload = payload;
            return p;
        }

        internal IotPacket BuildPacket_ManagerStopped(DateTime startedAt)
        {
            IotPacket p = new IotPacket();
            ManagerStopped payload = new ManagerStopped();

            BuildNow();

            payload.UpTime = (DateTime.Now - startedAt);

            p.Header = BuildHeader(ServiceCodes.Device, PacketCodes.ManagerStopped);
            p.Payload = payload;
            return p;
        }

        internal IotPacket BuildPacket_Statistics(String GUID, IoTWork.Infrastructure.Statistics.Statistics statistics)
        {
            IotPacket p = new IotPacket();
            Protocol.Devices.Statistics payload = new Protocol.Devices.Statistics();

            BuildNow();

            payload.Notes = new List<Note>();

            if (statistics != null)
            {
                foreach (var s in statistics)
                {
                    string code = s.GetModule();
                    string name = s.GetName();
                    string value = s.GetValue();
                    NoteDomain domain = s.GetDomain();

                    payload.Notes.Add(new Note() { Domain = domain, When = null, Module = code, Name = name, Value = value });
                }
            }

            p.Header = BuildHeader(ServiceCodes.Device, PacketCodes.Statistics, null, GUID);
            p.Payload = payload;
            return p;
        }

        internal IotPacket BuildPacket_Errors(String GUID, ErrorResume errors)
        {
            IotPacket p = new IotPacket();
            Protocol.Devices.Errors payload = new Protocol.Devices.Errors();

            BuildNow();

            payload.Notes = new List<Note>();

            if (errors != null)
            {
                foreach (var s in errors)
                {
                    DateTime when = s.GetWhen();
                    string code = s.GetModule();
                    string name = s.GetMessage();
                    string value = SerializerHelper.JSonSerialize(s.GetValue());
                    NoteDomain domain = NoteDomain.Error;

                    payload.Notes.Add(new Note() { Domain = domain, When = when, Module = code, Name = name, Value = value });
                }
            }

            p.Header = BuildHeader(ServiceCodes.Device, PacketCodes.Errors, null, GUID);
            p.Payload = payload;
            return p;
        }

        internal IotPacket BuildPacket_UpTime(DateTime startedAt)
        {
            IotPacket p = new IotPacket();
            UpTime payload = new UpTime();

            BuildNow();

            payload.Value = (DateTime.Now - startedAt);

            p.Header = BuildHeader(ServiceCodes.Device, PacketCodes.UpTime);
            p.Payload = payload;
            return p;
        }

        internal IotPacket BuildPacket_Message(String GUID, bool IsAlert, bool Acknowledge, int Code, string Name, string Text)
        {
            IotPacket p = new IotPacket();
            Message payload = new Message();

            BuildNow();

            payload.IsAlert = IsAlert;
            payload.Acknowledge = Acknowledge;
            payload.Code = Code;
            payload.Name = Name;
            payload.Text = Text;

            p.Header = BuildHeader(ServiceCodes.Device, PacketCodes.Message, null, GUID);
            p.Payload = payload;
            return p;
        }
    }
}
