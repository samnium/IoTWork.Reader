using IoTWork.IoTReader.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using IoTWork.Contracts;

namespace IoTWork.IoTReader.Management
{
    internal class Sample : ISample
    {
        private Stopwatch _sw;

        public String Source
        {
            get;
            set;
        }

        public SortedList<int, long> ComputationHistory
        {
            get;
            set;
        }

        public IIoTSample CurrentSample
        {
            get;
            set;
        }

        public SortedList<int, Exception> ExceptionHistory
        {
            get;
            set;
        }

        public IIoTSample OriginalSample
        {
            get;
            set;
        }

        public SortedList<int, DateTime> PipeHistory
        {
            get;
            set;
        }

        public DateTime ProducedAt
        {
            get;
            set;
        }

        public SortedList<int, IIoTSample> ValueHistory
        {
            get;
            set;
        }

        public Sample(IIoTSample sample)
        {
            ProducedAt = DateTime.Now;
            OriginalSample = sample;
            CurrentSample = sample;
            ValueHistory = new SortedList<int, IIoTSample>();
            PipeHistory = new SortedList<int, DateTime>();
            ComputationHistory = new SortedList<int, long>();
            ExceptionHistory = new SortedList<int, Exception>();

            _sw = new Stopwatch();
        }

        public Sample(ISensor sensor, IIoTSample sensorSample) : this(sensorSample)
        {
            Source = sensor.UniqueName;
        }

        public void StartTraversingPipe(int stage)
        {
            ValueHistory[stage] = CurrentSample;
            PipeHistory[stage] = DateTime.Now;

            _sw.Restart();
        }

        public void EndTraversingPipe(int stage, Exception ex)
        {
            _sw.Stop();

            ComputationHistory[stage] = _sw.ElapsedMilliseconds;

            if (ex != null)
                ExceptionHistory[stage] = ex;
        }

        public void UpdateValue(IIoTSample sample)
        {
            CurrentSample = sample;
        }
    }
}
