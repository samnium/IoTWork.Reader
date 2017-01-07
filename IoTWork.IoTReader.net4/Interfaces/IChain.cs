using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IoTWork.IoTReader.Interfaces;
using System.Collections.Concurrent;
using IoTWork.IoTReader.DataModel;
using IoTWork.IoTReader.Management;
using IoTWork.IoTReader.Utils;
using IoTWork.Infrastructure.Statistics;

namespace IoTWork.IoTReader.Interfaces
{
    internal interface IChain: IChainDefinition, IEnumerable<IPipe>, IJunctionPoint<ISample>
    {
        String RealUniqueName { get; set; }

        LinkPriority LinkPriority { get; set; }

        void Mount(IChainDefinition cd);

        void Build();

        void Open();

        void Close();

        bool IsEmpty();

        void Attach(IPipe pipe);

        void SetSensor(ISensor sensor);

        Statistics GetStatistics();

        ErrorResume GetErrors();
    }
}
