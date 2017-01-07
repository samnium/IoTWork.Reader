using Bridge.Linux;
using IoTWork.Contracts;
using Pi.SHat.Sensor.Humidity;
using Pi.SHat.Sensor.Pressure;
using Pi.SHat.Sensor.Temperature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pi.SHat.Sensor.Pipe.Dump
{
    public class PiSHatPipe_Dump : IIoTPipe
    {
        FrameBufferOfRaspberryPiSenseHat FrameBuffer;
        bool initialized;

        public PiSHatPipe_Dump()
        {
            try
            {
                FrameBuffer = null;
                initialized = false;

                Console.WriteLine("==> DEBUG PiSHatSensor_Humidity entering constructor {0}", DateTime.Now);

                FrameBuffer = FrameBufferOfRaspberryPiSenseHat.Instance;

                FrameBuffer.Open();

                Console.WriteLine("==> DEBUG PiSHatSensor_Humidity exiting constructor {0}", DateTime.Now);

                initialized = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine(ex.InnerException.Message);
            }
        }

        public IIoTSample CrossIt(IIoTSample sample)
        {
            if (sample.GetType() == typeof(PiSHatSample_Humidity))
            {
                System.Console.WriteLine("\t\tPIPE.Dump.Humidity {0}", ((PiSHatSample_Humidity)sample).Value);
                if (initialized && FrameBuffer != null)
                {
                    FrameBuffer.Draw('H');
                }
            }
            else if (sample.GetType() == typeof(PiSHatSample_Pressure))
            {
                System.Console.WriteLine("\t\t\tPIPE.Dump.Pressure {0}", ((PiSHatSample_Pressure)sample).Value);
                if (initialized && FrameBuffer != null)
                {
                    FrameBuffer.Draw('P');
                }
            }
            else if (sample.GetType() == typeof(PiSHatSample_Temperature))
            {
                System.Console.WriteLine("\t\t\t\tPIPE.Dump.Temperature {0}", ((PiSHatSample_Temperature)sample).Value);
                if (initialized && FrameBuffer != null)
                {
                    FrameBuffer.Draw('T');
                }
            }
            return sample;
        }
    }
}
