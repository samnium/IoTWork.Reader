using IoTWork.Contracts;
using IoTWork.IoTReader.Exceptions;
using IoTWork.IoTReader.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using IoTWork.Infrastructure;

namespace IoTWork.IoTReader.Sensors
{
    internal class CustomSensor : BaseSensor
    {
        IIoTSensor sensor = null;

        public override void Build()
        {
            // http://stackoverflow.com/questions/1137781/c-sharp-correct-way-to-load-assembly-find-class-and-call-run-method
            // http://stackoverflow.com/questions/23735122/get-all-c-sharp-types-that-implements-an-interface-first-but-no-derived-classes

            try
            {
                LogManager.Debug("CustomSensor LoadFile {0}", LibraryPath);

                Assembly assembly = Assembly.LoadFile(LibraryPath);

                if (assembly == null)
                    throw new SensorException("Assembly  " + LibraryPath + " not found. Sensor can't be allocated");

                LogManager.Debug("CustomSensor Assembly {0}", assembly.FullName);

                Type type = assembly.GetTypes().Where(x => typeof(IIoTSensor).IsAssignableFrom(x)).FirstOrDefault();

                if (type == null)
                    throw new SensorException("Instance of IIoTSensor not found on dll " + LibraryPath + ". Sensor can't be allocated");

                LogManager.Debug("CustomSensor Type {0}", type.FullName);

                sensor = Activator.CreateInstance(type) as IIoTSensor;

                LogManager.Debug("CustomSensor Sensor {0}", sensor.GetType());

                if (sensor == null)
                    throw new SensorException("Instance of IIoTSensor can't be activated on dll " + LibraryPath + ". Sensor can't be allocated");

                if (!sensor.Init(ClassForAcquire))
                    throw new SensorException("Error in init sensor " + UniqueName + "." + sensor.GetErrors());

                LogManager.Debug("CustomSensor Done");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine(ex.InnerException.Message);

                throw ex;
            }
        }

        public override IIoTSample Acquire(object Locker)
        {
            IIoTSample sample;

            LogManager.Debug("CustomSensor Acquire with {0}", sensor.GetType());

            lock (Locker)
            {
                sample = sensor.Acquire();
            }

            LogManager.Debug("CustomSensor Sample is {0}", sample.GetType());

            return sample;
        }

        public override bool Close()
        {
            return sensor.Close(ClassForAcquire_Parameter_Close);
        }
    }
}
