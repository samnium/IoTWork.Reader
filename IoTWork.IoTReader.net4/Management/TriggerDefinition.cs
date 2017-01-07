using IoTWork.IoTReader.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IoTWork.XML;

namespace IoTWork.IoTReader.Management
{
    internal class TriggerDefinition : ITriggerDefinition
    {
        public TriggerDefinition(iotreaderTrigger t)
        {
            this.UniqueId = t.UniqueId;
            this.UniqueName = t.UniqueName;
            this.TypeName = t.type;
            this.RepeatForever = t.RepeatForever;
            this.WithIntervalInMilliseconds = t.WithIntervalInMilliseconds;
        }

        public ushort UniqueId
        {
            get;
            set;
        }

        public bool RepeatForever
        {
            get;
            set;
        }

        public string TypeName
        {
            get;
            set;
        }

        public string UniqueName
        {
            get;
            set;
        }

        public uint WithIntervalInMilliseconds
        {
            get;
            set;
        }
    }
}
