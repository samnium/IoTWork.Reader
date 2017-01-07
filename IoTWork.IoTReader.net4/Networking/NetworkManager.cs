using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IoTWork.IoTReader.Interfaces;
using IoTWork.IoTReader.Pipes;
using IoTWork.Infrastructure.Interfaces;
using System.Collections.Concurrent;
using IoTWork.Protocol;

namespace IoTWork.IoTReader.Networking
{
    internal class NetworkManager<T>
    {
        IClient _client;
        IPacketManagerOutput _packetManagerOutput;
        IPacketManagerInput _packetManagerInput;

        internal NetworkManager()
        {
            _client = null;
            _packetManagerOutput = null;
            _packetManagerInput = null;
        }

        public bool DiscardInputMessages
        {
            get;
            set;
        }

        public void Start()
        {
            _client.Connect();
        }

        public void Close()
        {
            _client.Close();
        }

        public void SetClient(IClient client)
        {
            _client = client;
        }

        public void SetPacketManagerOutput(IPacketManagerOutput PacketManager)
        {
            _packetManagerOutput = PacketManager;
        }

        public void SetPacketManagerInput(IPacketManagerInput packetManagerInput)
        {
            _packetManagerInput = packetManagerInput;
        }

        public bool TryReceiveMessage(out IIotPacket message)
        {
            message = null;
            byte[] buffer;
            var received = _client.TryReceive(out buffer);
            if (received)
            {
                if (buffer != null)
                {
                    message = _packetManagerInput.Decode(buffer);
                }
                else
                    received = false;
            }
            return received;
        }

        public void SendMessage(IIotPacket message)
        {
            var buffer = _packetManagerOutput.Encode(message);
            _client.Send(buffer);
        }

        internal bool IsOutputVoid()
        {
            return _client.IsOutputVoid();
        }
    }
}
