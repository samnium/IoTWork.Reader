using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.Infrastructure
{
    public static class Constants
    {
        public static String PathPrefix_Windows = @"c:";
        public static String PathPrefix_Linux = "/iot";

        public static String Path_ConfigurationFileFactory = @"\iotreader\conf\configuration_factory.xml";
        public static String Path_ConfigurationFile = @"\iotreader\conf\configuration_iotreader.xml";

        public static String Path_Pipes = @"\iotreader\pipes\";
        public static String Path_Sensors = @"\iotreader\sensors\";
        public static String Path_Modules = @"\iotreader\modules\";

        public static string Path_Tmp = @"\iotreader\tmp\";

        public static int Network_DeviceManagementPort = 41975;
    }
}
