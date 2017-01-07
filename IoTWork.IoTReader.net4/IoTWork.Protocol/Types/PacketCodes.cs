using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.Protocol.Types
{
    public enum PacketCodes
    {
        ManagerStarted = 1,
        ManagerStopped = 2,
        Alive = 3,
        Statistics = 4,
        UpTime = 5,
        Errors = 6,
        Message = 7,

        Sample = 100,
        Measure = 101,
    }
}
