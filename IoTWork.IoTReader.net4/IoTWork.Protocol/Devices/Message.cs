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
    public class Message: Payload
    {
        [DataMember(Name = "IA", Order = 1)]
        public Boolean IsAlert { get; set; }

        [DataMember(Name = "AK", Order = 2)]
        public Boolean Acknowledge { get; set; }

        [DataMember(Name = "CD", Order = 3)]
        public Int32 Code { get; set; }

        [DataMember(Name = "NM", Order = 4)]
        public String Name { get; set; }

        [DataMember(Name = "TX", Order = 5)]
        public String Text { get; set; }
    }
}
