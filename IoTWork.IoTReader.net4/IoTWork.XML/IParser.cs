using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.XML
{
    public interface IParser
    {
        bool ParseIoTConfigurationFile<T>(String xml, out T obj);
    }
}
