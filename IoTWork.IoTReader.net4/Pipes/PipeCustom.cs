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

namespace IoTWork.IoTReader.Pipes
{
    internal class PipeCustom : Pipe
    {
        IIoTPipe pipe = null;

        public override void Build()
        {
            // http://stackoverflow.com/questions/1137781/c-sharp-correct-way-to-load-assembly-find-class-and-call-run-method
            // http://stackoverflow.com/questions/23735122/get-all-c-sharp-types-that-implements-an-interface-first-but-no-derived-classes

            LogManager.Debug("PipeCustom LoadFile {0}", LibraryPath);

            Assembly assembly = Assembly.LoadFile(LibraryPath);

            if (assembly == null)
                throw new Exceptions.PipeException("Assembly  " + LibraryPath + " not found. Pipe can't be allocated");

            LogManager.Debug("PipeCustom Assembly {0}", assembly.FullName);

            Type type = assembly.GetTypes().Where(x => typeof(IIoTPipe).IsAssignableFrom(x)).FirstOrDefault();

            if (type == null)
                throw new Exceptions.PipeException("Instance of IIotPipe not found on dll " + LibraryPath + ". Pipe can't be allocated");

            LogManager.Debug("PipeCustom Type {0}", type.FullName);

            pipe = Activator.CreateInstance(type) as IIoTPipe;

            LogManager.Debug("PipeCustom Sensor {0}", pipe.GetType());

            if (pipe == null)
                throw new Exceptions.PipeException("Instance of IIotPipe can't be activated on dll " + LibraryPath + ". Pipe can't be allocated");

            LogManager.Debug("PipeCustom Done");
        }
        public override IIoTSample CrossIt(IIoTSample sample)
        {
            return pipe.CrossIt(sample);
        }
    }
}
