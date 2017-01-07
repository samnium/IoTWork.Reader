using IoTWork.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.Infrastructure.Compressors
{
    public class VoidCompressor : ICompressor
    {
        public byte[] Compress(byte[] data)
        {
            return data;
        }

        public byte[] Uncompress(byte[] data)
        {
            return data;
        }
    }
}
