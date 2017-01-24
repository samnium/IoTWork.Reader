using IoTWork.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.IoTReader.Sensors
{
    internal class VoidSensor : BaseSensor
    {
        public override void Build()
        {

        }

        public override IIoTSample Acquire(object Locker)
        {
            return null;
        }
    }
}
