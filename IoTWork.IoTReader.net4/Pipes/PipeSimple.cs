using IoTWork.Contracts;
using IoTWork.IoTReader.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.IoTReader.Pipes
{
    internal class PipeSimple : Pipe
    {
        public override void Build()
        {
        }

        public override IIoTSample CrossIt(IIoTSample sample)
        {
            return sample;
        }
    }
}
