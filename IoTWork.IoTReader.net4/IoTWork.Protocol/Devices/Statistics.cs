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
    public class Statistics: Payload
    {
        [DataMember(Name = "NOT", Order = 0)]
        public List<Note> Notes { get; set; }
    }
}
