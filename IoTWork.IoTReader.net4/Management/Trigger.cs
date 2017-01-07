using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IoTWork.IoTReader.Interfaces;

namespace IoTWork.IoTReader.Management
{
    internal class Trigger: Interfaces.ITrigger
    {
        public ushort UniqueId
        {
            get;
            set;
        }

        public String RealUniqueName
        {
            get;
            set;
        }

        public string UniqueName
        {
            get;
            set;
        }

        public string TypeName
        {
            get;
            set;
        }

        public bool RepeatForever
        {
            get;
            set;
        }

        public uint WithIntervalInMilliseconds
        {
            get;
            set;
        }

        public Trigger()
        {
        }

        public void Mount(ITriggerDefinition td)
        {
            this.UniqueId = td.UniqueId;
            this.UniqueName = td.UniqueName;
            this.TypeName = td.TypeName;
            this.WithIntervalInMilliseconds = td.WithIntervalInMilliseconds;
            this.RepeatForever = td.RepeatForever;
        }

        public virtual void Build()
        {
            throw new NotImplementedException();
        }

        public virtual bool IsQuartzTrigger()
        {
            throw new NotImplementedException();
        }

        public void SetSensor(ISensor sensor)
        {
            this.RealUniqueName = this.UniqueName + "_onsensor_" + sensor.UniqueName;
        }
    }
}
