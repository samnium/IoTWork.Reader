using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace IoTWork.IoTReader.Core.Console
{
    public class Program
    {
        public static int Main(string[] args)
        {
            String TcpServerAddress = String.Empty;
            Boolean IsLinux = false;

            int i = 0;
            while (i < args.Length)
            {
                if (args[i] == "--tcpserver")
                {
                    TcpServerAddress = args[++i];
                }

                if (args[i] == "--islinux")
                {
                    IsLinux = true;
                }

                i++;
            }


            Manager manager = new Manager();
            manager.Run(TcpServerAddress, IsLinux);

            ProcessThreadCollection currentThreads = Process.GetCurrentProcess().Threads;
            foreach (var thread in currentThreads)
            {
                System.Console.WriteLine(thread);
            }
            Environment.Exit(0);
            return 0;
        }
    }
}
