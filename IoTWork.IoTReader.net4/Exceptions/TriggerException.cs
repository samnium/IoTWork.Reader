using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.IoTReader.Exceptions
{
    [DataContract]
    public class TriggerException : Exception
    {
        public string ResourceReferenceProperty { get; set; }

        public TriggerException()
        {
        }

        public TriggerException(string message)
            : base(message)
        {
        }

        public TriggerException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
