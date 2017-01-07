using IoTWork.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Pi.SHat.Sensor.Pressure
{
    [DataContract]
    public class PiSHatSample_Pressure: IIoTSample
    {
        [DataMember]
        public Double Value { get; set; }
    }
}
