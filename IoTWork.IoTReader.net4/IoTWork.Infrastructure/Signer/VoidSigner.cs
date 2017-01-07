using IoTWork.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.Infrastructure.Formatters
{
    public class VoidSigner<T> : ISigner<T>
    {
        public string Calculate(byte[] data, string secret)
        {
            return string.Empty;
        }

        public bool Verified(byte[] data, string sign, string secret)
        {
            return true;
        }
    }
}
