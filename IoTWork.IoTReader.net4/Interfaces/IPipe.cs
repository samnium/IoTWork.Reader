using IoTWork.Contracts;
using IoTWork.IoTReader.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.IoTReader.Interfaces
{
    interface IPipe: IPipeDefinition
    {
        void Mount(IPipeDefinition pd);

        void Build();

        IIoTSample CrossIt(IIoTSample sample);
    }
}
