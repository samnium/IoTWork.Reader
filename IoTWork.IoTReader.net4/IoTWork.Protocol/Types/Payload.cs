using IoTWork.Protocol.Datas;
using IoTWork.Protocol.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.Protocol.Types
{
    [DataContract(Namespace = "http://iotwork.protocol/types")]
    [KnownType(typeof(Alive))]
    [KnownType(typeof(Note))]
    [KnownType(typeof(ManagerStarted))]
    [KnownType(typeof(ManagerStopped))]
    [KnownType(typeof(Sample))]
    [KnownType(typeof(Measures))]
    [KnownType(typeof(Statistics))]
    [KnownType(typeof(Errors))]
    [KnownType(typeof(Message))]
    public class Payload
    {

    }
}
