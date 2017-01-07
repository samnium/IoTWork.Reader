using IoTWork.IoTReader.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.IoTReader.Interfaces
{
    internal interface IChainDefinition: IEnumerable<IPipeDefinition>
    {
        ushort UniqueId { get; set; }

        string UniqueName { get; set; }

        string Priority { get; set; }

        void Attach(IPipeDefinition pipe);
    }
}
