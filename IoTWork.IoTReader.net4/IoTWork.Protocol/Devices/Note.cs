using IoTWork.Contracts;
using IoTWork.Protocol.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.Protocol.Devices
{
    [DataContract(Namespace = "http://iotwork.protocol/device")]
    public enum NoteDomain
    {
        [EnumMember]
        System,
        [EnumMember]
        Network,
        [EnumMember]
        Chain,
        [EnumMember]
        Sensor,
        [EnumMember]
        Error
    }

    [DataContract(Namespace = "http://iotwork.protocol/device")]
    public class Note : Payload
    {
        [DataMember(Name = "DMN", Order = 0)]
        public NoteDomain Domain { get; set; }

        [DataMember(Name = "WHN", Order = 0)]
        public DateTime? When { get; set; }

        [DataMember(Name = "COD", Order = 0)]
        public string Module { get; set; }

        [DataMember(Name = "KEY", Order = 1)]
        public string Name { get; set; }

        [DataMember(Name = "VAL", Order = 2)]
        public string Value { get; set; }
    }
}
