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
    public class ManagerStopped: Payload
    {
        [DataMember(Name = "UPT", Order = 0)]
        public TimeSpan UpTime { get; set; }
    }
}
