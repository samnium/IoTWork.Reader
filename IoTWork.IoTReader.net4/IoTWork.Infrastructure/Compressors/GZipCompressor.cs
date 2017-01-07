using IoTWork.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.Infrastructure.Compressors
{
    public class GZipCompressor : ICompressor
    {
        public byte[] Compress(byte[] data)
        {
            byte[] outputdata = null;

            if (data != null)
            {
                using (var msi = new MemoryStream(data))
                {
                    using (var mso = new MemoryStream())
                    {
                        using (var gs = new GZipStream(mso, CompressionMode.Compress))
                        {
                            msi.CopyTo(gs);
                        }
                        outputdata = mso.ToArray();
                    }
                }
            }

            return outputdata;
        }

        public byte[] Uncompress(byte[] data)
        {
            byte[] outputdata = null;

            if (data != null)
            {
                using (var msi = new MemoryStream(data))
                {
                    using (var mso = new MemoryStream())
                    {
                        using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                        {
                            gs.CopyTo(mso);
                        }

                        outputdata = mso.ToArray();
                    }
                }
            }

            return outputdata;
        }
    }
}
