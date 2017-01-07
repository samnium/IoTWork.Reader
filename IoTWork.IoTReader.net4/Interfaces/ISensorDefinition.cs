using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.IoTReader.Interfaces
{
    public interface ISensorDefinition
    {
        ushort UniqueId { get; set; }

        string UniqueName { get; set; }

        string TypeName { get; set; }

        string ChainUniqueName { get; set; }
        string TriggerUniqueName { get; set; }

        string LibraryPath { get; set; }
        string ClassForSample { get; set; }
        string ClassForAcquire { get; set; }
        string ClassForAcquire_Parameter_Init { get; set; }
        string ClassForAcquire_Parameter_Close { get; set; }
    }
}
