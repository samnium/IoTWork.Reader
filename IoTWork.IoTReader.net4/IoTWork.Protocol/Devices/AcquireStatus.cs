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
    public enum AcquireStatus
    {
        [EnumMember]
        Acquiring,

        [EnumMember]
        Stopped,

        [EnumMember]
        Paused,

        [EnumMember]
        Faulted
    }
}
