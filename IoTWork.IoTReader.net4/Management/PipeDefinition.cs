using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IoTWork.XML;
using IoTWork.IoTReader.Interfaces;

namespace IoTWork.IoTReader.Management
{
    internal class PipeDefinition : IPipeDefinition
    {
        public PipeDefinition(iotreaderChainPipe p)
        {
            this.Stage = p.stage;
            this.TypeName = p.type;
            this.LibraryPath = p.LibraryPath;
            this.Parameter = p.Parameters;
            this.WithIntervalInMilliseconds = p.WithIntervalInMilliseconds;
        }

        public string Parameter
        {
            get;
            set;
        }

        public string LibraryPath
        {
            get;
            set;
        }

        public int Stage
        {
            get;
            set;
        }

        public string TypeName
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
