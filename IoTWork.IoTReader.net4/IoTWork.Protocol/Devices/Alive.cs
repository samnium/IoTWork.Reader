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
    public class Alive: Payload
    {
        [DataMember(Name = "UTM", Order = 0)]
        public TimeSpan UpTime { get; set; }

        [DataMember(Name = "STA", Order = 1)]
        public AcquireStatus Status { get; set; }

        [DataMember(Name = "NOT", Order = 2)]
        public List<Note> Notes { get; set; }
    }
}
