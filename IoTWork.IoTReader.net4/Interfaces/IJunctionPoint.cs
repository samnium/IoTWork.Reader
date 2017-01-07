using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.IoTReader.Interfaces
{
    internal enum JunctionPointDirection
    {
        Source,
        Destination
    }

    internal interface IJunctionPoint<T>
    {
        String JunctionName { get; set; }

        void SetJunction(IJunction<T> junction, JunctionPointDirection Direction);

        //bool TryEject(out T data);
    }
}
