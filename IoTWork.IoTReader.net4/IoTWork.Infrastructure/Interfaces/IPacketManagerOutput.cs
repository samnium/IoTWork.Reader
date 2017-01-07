using IoTWork.Protocol;
using IoTWork.Protocol.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.Infrastructure.Interfaces
{
    public interface IPacketManagerOutput
    {
        void SetCompressor(ICompressor compressor);

        void SetPacketFormatter(IFormatter<IIotPacket> formatter);

        void SetPayloadFormatter(IFormatter<Payload> formatter);

        void SetPacketSigner(ISigner<IIotPacket> signer);

        void SetPayloadSigner(ISigner<Payload> signer);

        byte[] Encode(IIotPacket message);
    }
}