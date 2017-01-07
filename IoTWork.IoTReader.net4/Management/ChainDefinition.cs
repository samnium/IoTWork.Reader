using IoTWork.IoTReader.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IoTWork.XML;
using System.Collections;

namespace IoTWork.IoTReader.Management
{
    internal class ChainDefinition : IChainDefinition
    {
        internal SortedList<int, IPipeDefinition> pipeDefinitions;

        public ChainDefinition(iotreaderChain c)
        {
            this.UniqueId = c.UniqueId;
            this.UniqueName = c.UniqueName;
            this.Priority = c.priority;

            pipeDefinitions = new SortedList<int, IPipeDefinition>();
        }

        public ushort UniqueId
        {
            get;
            set;
        }

        public string UniqueName
        {
            get;
            set;
        }

        public string Priority
        {
            get;
            set;
        }

        public void Attach(IPipeDefinition pipe)
        {
            if (pipeDefinitions.ContainsKey(pipe.Stage))
                throw new Exception("Duplicated Pipe at stage " + pipe.Stage);
            pipeDefinitions.Add(pipe.Stage, pipe);
        }

        public IEnumerator<IPipeDefinition> GetEnumerator()
        {
            foreach(var pd in pipeDefinitions)
            {
                yield return pd.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
