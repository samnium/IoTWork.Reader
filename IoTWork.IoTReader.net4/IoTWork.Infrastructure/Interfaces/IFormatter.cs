using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.Infrastructure.Interfaces
{
    public interface IFormatter<T>
    {
        byte[] Format(T data);

        T Unformat(byte[] data);
    }
}
