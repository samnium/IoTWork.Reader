using IoTWork.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.Samples.Sensors
{
    [Serializable]
    public class SensorTimeUpSample: IIoTSample
    {
        [DataMember]
        public Int64 ElapsedMilliseconds { get; set; }
    }

    public class SensorTimeUp : IIoTSensor
    {
        private Stopwatch sw = null;
        private Int64? ErrorsReading = null;
        private Int64? NumberOfAcquires = null;

        public IIoTSample Acquire()
        {
            NumberOfAcquires++;

            if (sw != null)
            {
                return new SensorTimeUpSample() { ElapsedMilliseconds = sw.ElapsedMilliseconds };
            }
            else
            {
                ErrorsReading++;
                return null;
            }
        }

        public bool Close(string Parameters)
        {
            if (sw != null)
            {
                sw.Reset();
                sw = null;
                return true;
            }
            else
                return false;
        }

        public string GetErrors()
        {
            return ErrorsReading.HasValue ? "ErrorsReading " + ErrorsReading : "No Errors";
        }

        public string GetStats()
        {
            return NumberOfAcquires.HasValue ? "NumberOfAcquires " + NumberOfAcquires : "No Statistics";
        }

        public bool Init(string Parameters)
        {
            if (sw == null)
            {
                ErrorsReading = 0;
                NumberOfAcquires = 0;

                sw = new Stopwatch();
                sw.Start();
                return true;
            }
            else
                return false;
        }

        public bool Pause()
        {
            if (sw != null)
            {
                sw.Stop();
                return true;
            }
            else
                return false;
        }

        public bool Play()
        {
            if (sw != null)
            {
                sw.Start();
                return true;
            }
            else
                return false;
        }

        public bool Stop()
        {
            if (sw != null)
            {
                sw.Reset();
                return true;
            }
            else
                return false;
        }
    }

    public class SubtractOneSecond : IIoTPipe
    {
        public IIoTSample CrossIt(IIoTSample sample)
        {
            SensorTimeUpSample realsample = sample as SensorTimeUpSample;

            if (realsample != null)
            {
                SensorTimeUpSample newvalue = new SensorTimeUpSample();

                newvalue.ElapsedMilliseconds = realsample.ElapsedMilliseconds - 1000;

                return newvalue;
            }
            else
                return sample;
        }
    }
}
