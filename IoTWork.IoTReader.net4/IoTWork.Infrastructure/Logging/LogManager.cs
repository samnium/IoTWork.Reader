using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.Infrastructure
{
    public static class LogManager
    {
        // http://stackoverflow.com/questions/166438/what-would-a-log4net-wrapper-class-look-like

        private static String _appName;

        public static void Init(String ConfigurationFilePath, String AppName)
        {
            _appName = AppName;
        }

        public static void Debug(object message, Exception ex = null)
        {
            String _message = (String)message;
            String _stacktrace = ex != null ? ex.StackTrace : String.Empty;
            System.Console.WriteLine("{0}  {1} {2}", DateTime.Now, _message, _stacktrace);
        }

        public static void Debug(string formatString, params object[] args)
        {
            var _message = String.Format(formatString, args);
            System.Console.WriteLine("{0}  {1}", DateTime.Now, _message);
        }

        public static void Info(object message, Exception ex = null)
        {
            String _message = (String)message;
            String _stacktrace = ex != null ? ex.StackTrace : String.Empty;
            System.Console.WriteLine("{0}  {1} {2}", DateTime.Now, _message, _stacktrace);
        }

        public static void Info(string formatString, params object[] args)
        {
            var _message = String.Format(formatString, args);
            System.Console.WriteLine("{0}  {1}", DateTime.Now, _message);
        }

        public static void Warn(object message, Exception ex = null)
        {
            String _message = (String)message;
            String _stacktrace = ex != null ? ex.StackTrace : String.Empty;
            System.Console.WriteLine("{0}  {1} {2}", DateTime.Now, _message, _stacktrace);
        }

        public static void Error(object message, Exception ex = null)
        {
            String _message = (String)message;
            String _stacktrace = ex != null ? ex.StackTrace : String.Empty;
            System.Console.WriteLine("{0}  {1} {2}", DateTime.Now, _message, _stacktrace);
        }

        public static void Fatal(object message, Exception ex = null)
        {
            String _message = (String)message;
            String _stacktrace = ex != null ? ex.StackTrace : String.Empty;
            System.Console.WriteLine("{0}  {1} {2}", DateTime.Now, _message, _stacktrace);
        }
    }
}
