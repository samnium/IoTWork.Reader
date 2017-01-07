using IoTWork.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IoTWork.XML
{
    public class XSDParser : IParser
    {
        public bool ParseIoTConfigurationFile<T>(string xml, out T obj)
        {
            bool deserialized = false;

            obj = default(T);

            try
            {
                using (StringReader reader = new StringReader(xml))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    obj = (T)serializer.Deserialize(reader);
                    deserialized = true;
                }
            }
            catch (Exception ex)
            {
                LogManager.Fatal("Error in XML confguration file: " + ex.Message, ex);
                deserialized = false;
            }

            return deserialized;
        }
    }
}
