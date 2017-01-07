using IoTWork.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Pi.SHat.Sensor.Temperature
{
    [DataContract]
    public class PiSHatSample_Temperature : IIoTSample
    {
        [DataMember]
        public Double Value { get; set; }
    }
}
