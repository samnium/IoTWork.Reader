using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.Protocol.Types
{
    [DataContract(Namespace = "http://iotwork.protocol")]
    public class Header
    {
        [DataMember(Name = "GUID", Order = 0)]
        public String GUID { get; set; }

        [DataMember(Name = "VMJ", Order = 0)]
        public byte VersionMajor { get; set; }

        [DataMember(Name = "VMM", Order = 1)]
        public byte VersionMinor { get; set; }

        [DataMember(Name = "DUA", Order = 3)]
        public string DeviceUniqueAddress { get; set; }

        [DataMember(Name = "SUA", Order = 4)]
        public string SensorUniqueAddress { get; set; }

        [DataMember(Name = "RGN", Order = 5)]
        public string Region { get; set; }

        [DataMember(Name = "SAD", Order = 6)]
        public string SourceAdrress { get; set; }

        [DataMember(Name = "TVD", Order = 7)]
        public List<string> Traversed { get; set; }

        [DataMember(Name = "SCD", Order = 8)]
        public ushort ServiceCode { get; set; }

        [DataMember(Name = "PCD", Order = 9)]
        public ushort PacketCode { get; set; }

        [DataMember(Name = "SAT", Order = 10)]
        public DateTime SentAt { get; set; }

        [DataMember(Name = "SNB", Order = 11)]
        public ulong SequenceNumber { get; set; }

        [DataMember(Name = "HMAC1", Order = 12)]
        public string HMacPayload { get; set; }

        [DataMember(Name = "HMAC2", Order = 13)]
        public string HMacPacket { get; set; }
    }
}
