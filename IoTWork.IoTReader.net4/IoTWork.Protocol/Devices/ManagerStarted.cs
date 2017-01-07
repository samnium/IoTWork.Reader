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
    public class ManagerStarted: Payload
    {
        [DataMember(Name = "SAT", Order = 0)]
        public DateTime StartedAt { get; set; }
    }
}
