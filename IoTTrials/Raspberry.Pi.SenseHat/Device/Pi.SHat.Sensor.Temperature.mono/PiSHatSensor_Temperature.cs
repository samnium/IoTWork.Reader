using IoTWork.Contracts;
using PiSHat.Sensors.Devices.HTS221;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi.SHat.Sensor.Temperature
{
    public class PiSHatSensor_Temperature : IIoTSensor
    {
        private bool initialized;
        private readonly HTS221HumiditySensor sensor;

        public PiSHatSensor_Temperature()
        {
            try
            {
                initialized = false;

                Console.WriteLine("==> DEBUG PiSHatSensor_Temperature entering constructor {0}", DateTime.Now);

                sensor = new HTS221HumiditySensor(HTS221Defines.ADDRESS);

                Console.WriteLine("==> DEBUG InitSensorAsync on {0}", sensor.Name);

                sensor.InitAsync();

                Console.WriteLine("==> DEBUG PiSHatSensor_Temperature exiting constructor {0}", DateTime.Now);

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
            PiSHatSample_Temperature sample = new PiSHatSample_Temperature();
            sample.Value = -275;

            Console.WriteLine("==> DEBUG Sensor: Temperature {0}", DateTime.Now);

            if (initialized && sensor.Initiated)
            {
                try
                {
                    if (sensor.Update())
                    {
                        var readings = sensor.Readings;
                        if (readings.TemperatureValid)
                            sample.Value = readings.Temperature;
                        else
                            sample.Value = -274;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    if (ex.InnerException != null)
                        Console.WriteLine(ex.InnerException.Message);
                    sample.Value = -276;
                }
            }

            return sample;
        }

        public bool Close(string Parameters)
        {
            bool done = true;

            try
            {
                Console.WriteLine("==> DEBUG PiSHatSensor_Temperature entering Close {0}", DateTime.Now);

                if (initialized)
                {
                    sensor.Dispose();
                }

                Console.WriteLine("==> DEBUG PiSHatSensor_Temperature exiting Close {0}", DateTime.Now);

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
