using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.Contracts
{
    public interface IIoTPipe
    {
        IIoTSample CrossIt(IIoTSample sample);
    }
}
