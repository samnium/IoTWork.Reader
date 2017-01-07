using IoTWork.IoTReader.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IoTWork.IoTReader.DataModel;
using IoTWork.XML;

namespace IoTWork.IoTReader.Management
{
    internal class SensorDefinition : ISensorDefinition
    {
        public SensorDefinition(iotreaderSensor s)
        {
            this.UniqueId = s.UniqueId;
            this.UniqueName = s.UniqueName;
            this.TriggerUniqueName = s.TriggerUniqueName;
            this.ChainUniqueName = s.ChainUniqueName;
            this.TypeName = s.type;

            this.LibraryPath = s.LibraryPath;

            this.ClassForAcquire = String.Empty;
            this.ClassForSample = String.Empty;
            this.ClassForAcquire_Parameter_Init = s.Parameters;
            this.ClassForAcquire_Parameter_Close = String.Empty;
        }

        public ushort UniqueId
        {
            get;
            set;
        }

        public string ChainUniqueName
        {
            get;
            set;
        }

        public string ClassForSample
        {
            get;
            set;
        }

        public string ClassForAcquire
        {
            get;
            set;
        }

        public string ClassForAcquire_Parameter_Init
        {
            get;
            set;
        }

        public string ClassForAcquire_Parameter_Close
        {
            get;
            set;
        }

        public string LibraryPath
        {
            get;
            set;
        }

        public string TriggerUniqueName
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
    }
}
