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
    public class PacketManagerOutput : IPacketManagerOutput
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

        public PacketManagerOutput()
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

        public void SetCompressor(ICompressor compressor)
        {
            _compressor = compressor;
            _compressorCode = IoTWorkHelper.CodeOfCompressor(compressor);
        }

        public void SetPacketFormatter(IFormatter<IIotPacket> formatter)
        {
            _packetFormatter = formatter;
            _packetFormatterCode = IoTWorkHelper.CodeOfFormatter(formatter);
        }

        public void SetPayloadFormatter(IFormatter<Payload> formatter)
        {
            _payloadFormatter = formatter;
            _payloadFormatterCode = IoTWorkHelper.CodeOfFormatter(formatter);
        }

        public void SetPacketSigner(ISigner<IIotPacket> signer)
        {
            _packetSigner = signer;
            _packetSignerCode = IoTWorkHelper.CodeOfSigner(signer);
        }

        public void SetPayloadSigner(ISigner<Payload> signer)
        {
            _payloadSigner = signer;
            _payloadSignerCode = IoTWorkHelper.CodeOfSigner(signer);
        }

        public byte[] Encode(IIotPacket message)
        {
            var payload = message.GetPayload();
            var header = message.GetHeader();

            var formatted_Packet_WithouthSigning = _packetFormatter.Format(message);
            var formatted_Payload_WithouthSigning = _payloadFormatter.Format(payload);

            var sign_packet = _packetSigner.Calculate(formatted_Packet_WithouthSigning, "hello");
            var sign_payload = _payloadSigner.Calculate(formatted_Payload_WithouthSigning, "hello");

            message.SetPacketSignature(sign_packet);
            message.SetPayloadSignature(sign_payload);

            var formatted = _packetFormatter.Format(message);

            var compressed = _compressor.Compress(formatted);

            byte[] packet_prefix = new byte[8];

            packet_prefix[0] = _compressorCode;
            packet_prefix[1] = _packetFormatterCode;
            packet_prefix[2] = _payloadFormatterCode;
            packet_prefix[3] = _packetSignerCode;
            packet_prefix[4] = _payloadSignerCode;
            packet_prefix[5] = 0;
            packet_prefix[6] = 0;
            packet_prefix[7] = 0;

            var merged = IoTWorkHelper.ArrayMerge<byte>(packet_prefix, compressed);

            return merged;
        }
    }
}
