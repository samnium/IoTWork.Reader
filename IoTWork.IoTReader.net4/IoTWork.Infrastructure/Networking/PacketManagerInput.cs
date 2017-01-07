using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using IoTWork.Infrastructure.Interfaces;
using IoTWork.Protocol;
using IoTWork.Protocol.Types;
using IotWork.Utils.Helpers;

namespace IoTWork.Infrastructure.Networking
{
    public class PacketManagerInput: IPacketManagerInput
    {
        ICompressor _compressor;
        IFormatter<IIotPacket> _packetFormatter;
        IFormatter<Payload> _payloadFormatter;
        ISigner<IIotPacket> _packetSigner;
        ISigner<Payload> _payloadSigner;

        byte _compressorCode;
        byte _packetFormatterCode;
        byte _payloadFormatterCode;
        byte _packetSignerCode;
        byte _payloadSignerCode;

        public PacketManagerInput()
        {
            _compressor = null;
            _packetFormatter = null;
            _payloadFormatter = null;
            _packetSigner = null;

            _compressorCode = 0;
            _packetFormatterCode = 0;
            _payloadFormatterCode = 0;
            _packetSignerCode = 0;
            _payloadSignerCode = 0;
        }

        public IIotPacket Decode(byte[] buffer)
        {
            _compressorCode = buffer[0];
            _packetFormatterCode = buffer[1];
            _payloadFormatterCode = buffer[2];
            _packetSignerCode = buffer[3];
            _payloadSignerCode = buffer[4];
            var stuff1 = buffer[5];
            var stuff2 = buffer[6];
            var stuff3 = buffer[7];

            _compressor = IoTWorkHelper.AllocateCompressor(_compressorCode);
            _packetFormatter = IoTWorkHelper.AllocateFormatter<IIotPacket>(_packetFormatterCode);
            _payloadFormatter = IoTWorkHelper.AllocateFormatter<Payload>(_payloadFormatterCode);
            _packetSigner = IoTWorkHelper.AllocateSigner<IIotPacket>(_packetSignerCode);
            _payloadSigner = IoTWorkHelper.AllocateSigner<Payload>(_payloadSignerCode);

            buffer = IoTWorkHelper.SubArray<byte>(buffer, 8, buffer.Length - 8);

            IIotPacket decoded = null;

            var uncompressed = _compressor.Uncompress(buffer);

            var unformatted = _packetFormatter.Unformat(uncompressed);

            var message = unformatted;

            var payload = message.GetPayload();
            var header = message.GetHeader();

            var signed_packet = header.HMacPacket;
            var signed_payload = header.HMacPayload;

            message.SetPacketSignature(String.Empty);
            message.SetPayloadSignature(String.Empty);

            var formatted_Packet_WithouthSigning = _packetFormatter.Format(message);
            var formatted_Payload_WithouthSigning = _payloadFormatter.Format(payload);

            var sign_packet = _packetSigner.Calculate(formatted_Packet_WithouthSigning, "hello");
            var sign_payload = _payloadSigner.Calculate(formatted_Payload_WithouthSigning, "hello");

            if (signed_packet == sign_packet && signed_payload == sign_payload)
                decoded = message;
            else
                decoded = null;

            return decoded;
        }

        public ICompressor SetCompressor()
        {
            return _compressor;
        }

        public IFormatter<IIotPacket> SetPacketFormatter()
        {
            return _packetFormatter;
        }

        public IFormatter<Payload> SetPayloadFormatter()
        {
            return _payloadFormatter;
        }

        public ISigner<IIotPacket> SetPacketSigner()
        {
            return _packetSigner;
        }

        public ISigner<Payload> SetPayloadSigner()
        {
            return _payloadSigner;
        }
    }
}
