using IoTWork.IoTReader.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IoTWork.IoTReader.Management;
using IoTWork.Contracts;

namespace IoTWork.IoTReader.Pipes
{
    internal class Pipe : IPipe
    {
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

        public virtual void Build()
        {
            throw new NotImplementedException();
        }

        public virtual IIoTSample CrossIt(IIoTSample sample)
        {
            throw new NotImplementedException();
        }

        public void Mount(IPipeDefinition pd)
        {
            this.Stage = pd.Stage;
            this.TypeName = pd.TypeName;
            this.LibraryPath = pd.LibraryPath;
            this.Parameter = pd.Parameter;
            this.WithIntervalInMilliseconds = pd.WithIntervalInMilliseconds;
        }
    }
}
