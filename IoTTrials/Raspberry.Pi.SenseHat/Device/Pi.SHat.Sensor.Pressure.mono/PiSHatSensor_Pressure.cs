using IoTWork.Contracts;
using PiSHat.Sensors.Devices.LPS25H;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi.SHat.Sensor.Pressure
{
    public class PiSHatSensor_Pressure : IIoTSensor
    {
        private bool initialized;
        private readonly LPS25HPressureSensor sensor;

        public PiSHatSensor_Pressure()
        {
            try
            {
                initialized = false;

                Console.WriteLine("==> DEBUG PiSHatSensor_Pressure entering constructor {0}", DateTime.Now);

                sensor = new LPS25HPressureSensor(LPS25HDefines.ADDRESS0);

                Console.WriteLine("==> DEBUG InitSensorAsync on {0}", sensor.Name);

                sensor.InitAsync();

                Console.WriteLine("==> DEBUG PiSHatSensor_Pressure exiting constructor {0}", DateTime.Now);

                initialized = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine(ex.InnerException.Message);
            }
        }

        public IIoTSample Acquire()
        {
            PiSHatSample_Pressure sample = new PiSHatSample_Pressure();
            sample.Value = -1;

            Console.WriteLine("==> DEBUG Sensor: Pressure {0}", DateTime.Now);

            if (initialized && sensor.Initiated)
            {
                try
                {
                    if (sensor.Update())
                    {
                        var readings = sensor.Readings;
                        if (readings.PressureValid)
                            sample.Value = readings.Pressure;
                        else
                            sample.Value = -2;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    if (ex.InnerException != null)
                        Console.WriteLine(ex.InnerException.Message);
                    sample.Value = -3;
                }
            }

            return sample;
        }

        public bool Close(string Parameters)
        {
            bool done = true;

            try
            {
                Console.WriteLine("==> DEBUG PiSHatSensor_Pressure entering Close {0}", DateTime.Now);

                if (initialized)
                {
                    sensor.Dispose();
                }

                Console.WriteLine("==> DEBUG PiSHatSensor_Pressure exiting Close {0}", DateTime.Now);

                initialized = false;
            }
            catch (Exception ex)
            {
                done = false;

                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine(ex.InnerException.Message);
            }

            return done;
        }

        public string GetErrors()
        {
            return String.Empty;
        }

        public string GetStats()
        {
            return String.Empty;
        }

        public bool Init(string Parameters)
        {
            return true;
        }

        public bool Pause()
        {
            return true;
        }

        public bool Play()
        {
            return true;
        }

        public bool Stop()
        {
            return true;
        }
    }
}
