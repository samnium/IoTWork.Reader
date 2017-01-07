using IoTWork.Protocol.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.Protocol
{
    public interface IIotPacket
    {
        void SetRemoteEndpoint(IPEndPoint EndPoint);

        void SetHeader(Header Header);

        void SetPayload(Payload Payload);

        IPEndPoint GetRemoteEndpoint();

        Header GetHeader();

        Payload GetPayload();

        void SetPacketSignature(string Signature);

        void SetPayloadSignature(string Signature);
    }
}
