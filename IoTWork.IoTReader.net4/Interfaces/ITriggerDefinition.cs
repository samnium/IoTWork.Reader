using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IoTWork.IoTReader.DataModel;

namespace IoTWork.IoTReader.Interfaces
{
    public interface ITriggerDefinition
    {
        ushort UniqueId { get; set; }

        string UniqueName { get; set; }
        string TypeName { get; set; }

        bool RepeatForever { get; set; }

        uint WithIntervalInMilliseconds { get; set; }
    }
}
