using IoTWork.IoTReader.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.Infrastructure.Signer
{
    public class KeyManager
    {
        public String DeviceKey { get; set; }

        public String HeaderKey { get; set; }

        public String PayloadKey { get; set; }

        public KeyManager()
        {
            String device_key = @"\iotreader\keys\device.key";
            String head_key = @"\iotreader\keys\header.key";
            String payload_key = @"\iotreader\keys\payload.key";

            var device_key_path = IoTReaderHelper.ToLocalPath(device_key);
            var head_key_path = IoTReaderHelper.ToLocalPath(head_key);
            var payload_key_path = IoTReaderHelper.ToLocalPath(payload_key);

            if (File.Exists(device_key_path))
            {
                DeviceKey = File.ReadAllText(device_key_path);
                LogManager.Debug("Read Device Key at  " + device_key_path);
            }

            if (File.Exists(head_key_path))
            {
                HeaderKey = File.ReadAllText(head_key_path);
                LogManager.Debug("Read Header Key at  " + head_key_path);
            }

            if (File.Exists(payload_key_path))
            {
                PayloadKey = File.ReadAllText(payload_key_path);
                LogManager.Debug("Read Payload Key at  " + payload_key_path);
            }
        }
    }
}
