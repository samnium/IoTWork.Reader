using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IoTWork.IoTReader.DataModel;
using IoTWork.Contracts;
using IoTWork.IoTReader.Management;
using IoTWork.IoTReader.Utils;
using IoTWork.Infrastructure.Statistics;

namespace IoTWork.IoTReader.Interfaces
{
    internal interface ISensor: ISensorDefinition, IJunctionPointSource<ISample>
    {
        void Mount(ISensorDefinition definition);

        void Build();

        void RegisterTrigger(ITrigger trigger);

        ITrigger GetTrigger();

        void RegisterChain(IChain chain);

        IChain GetChain();

        IIoTSample Acquire(object Locker);

        bool Close();

        bool Stop();

        bool Pause();

        bool Play();

        void RegisterException(String UniqueName, DateTime When, Int32 SequenceNumber, Exception Exception);

        void RegisterStatistics(int SequenceNumber, DateTime FirstTriggeredOn, DateTime LastTriggeredOn, Double MinDuration, Double MaxDuration, Double MeanDuration);

        Statistics GetStatistics();

        ErrorResume GetErrors();
    }
}
