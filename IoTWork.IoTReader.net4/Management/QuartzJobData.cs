using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.IoTReader.Management
{
    internal static class QuartzJobData
    {
        public static string SensorUniqueName = "JD0";
        public static string SequenceNumber = "JD1";
        public static string FirstTriggeredOn = "JD2";
        public static string LastTriggeredOn = "JD3";
        public static string MinDuration = "JD4";
        public static string MaxDuration = "JD5";
        public static string MeanDuration = "JD6";
    }
}
