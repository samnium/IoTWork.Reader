using IoTWork.Protocol.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.Protocol
{
    [ServiceContract(Namespace = "http://iotwork.protocol")]
    public class IotPacket: IIotPacket
    {
        [DataMember(Name = "ENDPOINT", Order = 0)]
        public String EndPoint { get; set; }

        [DataMember(Name = "HEADER", Order = 0)]
        public Header Header { get; set; }

        [DataMember(Name = "PAYLOAD", Order = 1)]
        public Payload Payload { get; set; }

        public Header GetHeader()
        {
            return Header;
        }

        public Payload GetPayload()
        {
            return Payload;
        }

        public IPEndPoint GetRemoteEndpoint()
        {
            string[] ep = EndPoint.Split(':');
            if (ep.Length < 2) throw new FormatException("Invalid endpoint format");
            IPAddress ip;
            if (ep.Length > 2)
            {
                if (!IPAddress.TryParse(string.Join(":", ep, 0, ep.Length - 1), out ip))
                {
                    throw new FormatException("Invalid ip-adress");
                }
            }
            else
            {
                if (!IPAddress.TryParse(ep[0], out ip))
                {
                    throw new FormatException("Invalid ip-adress");
                }
            }
            int port;
            if (!int.TryParse(ep[ep.Length - 1], NumberStyles.None, NumberFormatInfo.CurrentInfo, out port))
            {
                throw new FormatException("Invalid port");
            }
            return new IPEndPoint(ip, port);
        }

        public void SetHeader(Header Header)
        {
            this.Header = Header;
        }

        public void SetPacketSignature(string Signature)
        {
            this.Header.HMacPacket = Signature;
        }

        public void SetPayload(Payload Payload)
        {
            this.Payload = Payload;
        }

        public void SetPayloadSignature(string Signature)
        {
            this.Header.HMacPayload = Signature;
        }

        public void SetRemoteEndpoint(IPEndPoint EndPoint)
        {
            this.EndPoint = EndPoint.ToString();
        }
    }
}
