using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.IoTReader.Exceptions
{
    [DataContract]
    public class PipeException : Exception
    {
        public string ResourceReferenceProperty { get; set; }

        public PipeException()
        {
        }

        public PipeException(string message)
            : base(message)
        {
        }

        public PipeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
