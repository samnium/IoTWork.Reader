using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.IoTReader.Exceptions
{
    [DataContract]
    public class ChainException : Exception
    {
        public string ResourceReferenceProperty { get; set; }

        public ChainException()
        {
        }

        public ChainException(string message)
            : base(message)
        {
        }

        public ChainException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
