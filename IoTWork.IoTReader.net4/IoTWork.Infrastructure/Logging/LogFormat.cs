using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.Infrastructure
{
    public static class LogFormat
    {
        private static uint _id;

        public static void Init(uint id)
        {
            _id = id;
        }

        public static String Format(string formatString, params object[] args)
        {
            return String.Format(_id + "::" + formatString, args);
        }
    }
}
