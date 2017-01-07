using IoTWork.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Pi.SHat.Sensor.Humidity
{
    [DataContract]
    public class PiSHatSample_Humidity: IIoTSample
    {
        [DataMember]
        public Double Value { get; set; }
    }
}
