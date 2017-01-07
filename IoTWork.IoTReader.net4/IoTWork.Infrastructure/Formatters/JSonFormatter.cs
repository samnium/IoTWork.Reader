using IoTWork.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IoTWork.Infrastructure.Formatters
{
    public class JSonFormatter<T> : IFormatter<T>
    {
        public byte[] Format(T data)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
            string json = JsonConvert.SerializeObject(data, settings);
            json = json.Replace(", IoTWork.IoTReader.Core\"", ", IoTWork.Protocol\"");
            json = json.Replace("IoTWork.Samples.Core", "IoTWork.Samples");
            var bytes = Encoding.UTF8.GetBytes(json);
            return bytes;
        }

        public T Unformat(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
