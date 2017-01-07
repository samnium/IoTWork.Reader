using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.IoTReader.Exceptions
{
    [DataContract]
    public class SensorException : Exception
    {
        public string ResourceReferenceProperty { get; set; }

        public SensorException()
        {
        }

        public SensorException(string message)
            : base(message)
        {
        }

        public SensorException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
