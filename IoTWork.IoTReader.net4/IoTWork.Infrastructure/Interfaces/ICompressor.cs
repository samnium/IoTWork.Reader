using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.Infrastructure.Interfaces
{
    public interface ICompressor
    {
        byte[] Compress(byte[] data);

        byte[] Uncompress(byte[] data);
    }
}
