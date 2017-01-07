using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.IoTReader.Interfaces
{
    internal interface IJunctionPointSource<T>: IJunctionPoint<T>
    {
        void Inject(T data);
    }
}
