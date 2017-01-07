using IoTWork.IoTReader.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.IoTReader.Management
{
    internal static class SensorArchive
    {
        internal static Dictionary<string, ISensor> map = null;

        internal static void Persist(Dictionary<string, ISensor> sm)
        {
            map = sm;
        }
    }
}
