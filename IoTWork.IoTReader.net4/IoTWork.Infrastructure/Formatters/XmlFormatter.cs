using IoTWork.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.Infrastructure.Formatters
{
    public class XmlFormatter<T> : IFormatter<T>
    {
        public byte[] Format(T data)
        {
            throw new NotImplementedException();
        }

        public T Unformat(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
