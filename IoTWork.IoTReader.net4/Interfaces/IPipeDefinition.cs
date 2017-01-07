using IoTWork.IoTReader.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.IoTReader.Interfaces
{
    internal interface IPipeDefinition
    {
        int Stage { get; set; }

        string TypeName { get; set; }

        string LibraryPath { get; set; }

        string Parameter { get; set; }

        uint WithIntervalInMilliseconds { get; set; }
    }
}
