using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.Contracts
{
    public interface IIoTSensor
    {
        bool Init(String Parameters);

        bool Close(String Parameters);

        bool Pause();

        bool Stop();

        bool Play();

        IIoTSample Acquire();

        string GetStats();

        string GetErrors();
    }
}
