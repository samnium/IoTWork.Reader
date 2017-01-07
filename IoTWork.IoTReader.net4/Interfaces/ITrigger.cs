using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IoTWork.IoTReader.DataModel;

namespace IoTWork.IoTReader.Interfaces
{
    internal interface ITrigger: ITriggerDefinition
    {
        String RealUniqueName { get; set; }

        void Mount(ITriggerDefinition TriggerDefinition);

        void Build();

        Boolean IsQuartzTrigger();

        void SetSensor(ISensor sensor);
    }
}
