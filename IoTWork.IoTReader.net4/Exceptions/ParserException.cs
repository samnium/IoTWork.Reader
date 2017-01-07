using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.IoTReader.Exceptions
{
    [DataContract]
    public class ParserException : Exception
    {
        public string ResourceReferenceProperty { get; set; }

        public ParserException()
        {
        }

        public ParserException(string message)
            : base(message)
        {
        }

        public ParserException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
