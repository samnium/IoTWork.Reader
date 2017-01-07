using IoTWork.IoTReader.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IoTWork.IoTReader.Management;
using IoTWork.Infrastructure.Statistics;

namespace IoTWork.IoTReader.Networking
{
    internal class VoidClient : IClient
    {
        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Connect()
        {
            throw new NotImplementedException();
        }

        public void Connect(Uri uri)
        {
            throw new NotImplementedException();
        }

        public Statistics GetStatistics()
        {
            throw new NotImplementedException();
        }

        public ErrorResume GetErrors()
        {
            throw new NotImplementedException();
        }

        public bool TryReceive(out byte[] data)
        {
            throw new NotImplementedException();
        }

        public void Send(byte[] data)
        {
            throw new NotImplementedException();
        }

        public void Reconnect()
        {
            throw new NotImplementedException();
        }

        public void SetUri(Uri uri)
        {
            throw new NotImplementedException();
        }

        public bool IsOutputVoid()
        {
            throw new NotImplementedException();
        }

        public void SetModuleName(string ModuleName)
        {
            throw new NotImplementedException();
        }
    }
}
