using IoTWork.Infrastructure.Statistics;
using IoTWork.IoTReader.Management;
using IoTWork.IoTReader.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.IoTReader.Interfaces
{
    internal interface IServerForReceive
    {
        void Start(Int32 port);

        void Close();

        bool TryReceive(out byte[] data);

        Statistics GetStatistics();

        ErrorResume GetErrors();
    }
}
