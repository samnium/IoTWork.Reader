using IoTWork.Contracts;
using IoTWork.IoTReader.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.IoTReader.Sensors
{
    public class IIoTSampleRandom: IIoTSample
    {
        public long value { get; set; }
    }

    internal class RandomSensor : BaseSensor
    {
        Random rnd;

        public override void Build()
        {
            rnd = new Random();
        }

        public override IIoTSample Acquire(object Locker)
        {
            return new IIoTSampleRandom() { value = rnd.Next() };
        }
    }
}
