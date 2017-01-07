using IoTWork.Contracts;
using IoTWork.IoTReader.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IoTWork.IoTReader.Pipes
{
    internal class PipeWait : Pipe
    {
        public override void Build()
        {
        }

        public override IIoTSample CrossIt(IIoTSample sample)
        {
            Thread.Sleep((int)WithIntervalInMilliseconds);
            return sample;
        }
    }
}
