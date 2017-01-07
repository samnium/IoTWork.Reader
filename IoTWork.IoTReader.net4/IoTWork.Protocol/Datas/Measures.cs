using IoTWork.Contracts;
using IoTWork.Protocol.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.Protocol.Datas
{
    [DataContract(Namespace = "http://iotwork.protocol/data")]
    public class Measures: Payload
    {
        [DataMember(Name = "DATS", Order = 0)]
        public List<Sample> datas { get; set; }
    }
}
