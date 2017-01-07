using IoTWork.Infrastructure.Formatters;
using IoTWork.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.Infrastructure.Formatters
{
    public class BinaryFormatter<T> : IFormatter<T>
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
