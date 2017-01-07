using IoTWork.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.IoTReader.Interfaces
{
    internal interface ISample
    {
        String Source { get; set; }

        DateTime ProducedAt { get; set; }

        SortedList<int, DateTime> PipeHistory { get; set; }

        SortedList<int, long> ComputationHistory { get; set; }

        SortedList<int, IIoTSample> ValueHistory { get; set; }

        SortedList<int, Exception> ExceptionHistory { get; set; }

        IIoTSample OriginalSample { get; set; }

        IIoTSample CurrentSample { get; set; }

        void StartTraversingPipe(int stage);

        void EndTraversingPipe(int stage, Exception ex);

        void UpdateValue(IIoTSample sample);
    }
}
