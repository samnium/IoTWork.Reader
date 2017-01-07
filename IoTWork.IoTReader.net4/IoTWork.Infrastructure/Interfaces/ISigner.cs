using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.Infrastructure.Interfaces
{
    public interface ISigner<T>
    {
        string Calculate(byte[] data, string secret);

        bool Verified(byte[] data, string sign, string secret);
    }
}
