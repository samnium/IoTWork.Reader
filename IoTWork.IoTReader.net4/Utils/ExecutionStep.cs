using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.IoTReader.Utils
{
    internal enum ExecutionStep
    {
        Start,
        Manager_Start_ConfigurationFileRead,
        Manager_Start_BasicInitialization_Done,
        Manager_Start_BasicInitialization_Failed,
        Manager_Start_ArchitectureParsing_Done,
        Manager_Start_ArchitectureParsing_Failed
    }
}
