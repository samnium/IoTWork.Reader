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
    internal interface IClient
    {
        void SetUri(Uri uri);

        void Connect();

        void Reconnect();

        void Close();

        bool TryReceive(out byte[] data);

        void Send(byte[] data);

        Statistics GetStatistics();

        ErrorResume GetErrors();

        bool IsOutputVoid();

        void SetModuleName(string ModuleName);
    }
}
