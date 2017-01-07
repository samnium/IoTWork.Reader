using IoTWork.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IoTWork.IoTReader.Management;
using IoTWork.Protocol;
using IoTWork.Protocol.Types;
using IoTWork.IoTReader.Utils;
using IoTWork.Infrastructure.Statistics;

namespace IoTWork.IoTReader.Interfaces
{
    internal interface IDispatcher: IJunctionPoint<ISample>
    {
        void SetClient(IClient client);

        void SetPacketCompressor(ICompressor compressor);

        void SetPacketFormatter(IFormatter<IIotPacket> formatter);

        void SetPacketSigner(ISigner<IIotPacket> signer);

        void SetPayloadCompressor(ICompressor compressor);

        void SetPayloadFormatter(IFormatter<Payload> formatter);

        void SetPayloadSigner(ISigner<Payload> signer);

        void SetContext(Context context);

        void Start();

        void Pause();

        void Skip();

        void Play();

        void Close();

        void Build();

        Statistics GetStatistics();

        ErrorResume GetErrors();

        bool IsFree();
    }
}
