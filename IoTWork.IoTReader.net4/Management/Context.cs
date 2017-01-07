using IoTWork.XML;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IoTWork.IoTReader.Interfaces;
using IoTWork.Infrastructure.Interfaces;
using IoTWork.IoTReader.Networking;
using IoTWork.Protocol;
using IoTWork.Protocol.Types;
using System.Net;

namespace IoTWork.IoTReader.Management
{
    internal class Context
    {
        public uint NetworkId { get; internal set; }
        public uint RingId { get; internal set; }
        public uint RegionId { get; internal set; }
        public uint DeviceId { get; internal set; }
        public string DeviceName { get; internal set; }

        #region framework
        internal iotreader configuration { get; set; }
        #endregion

        #region iotreader objects
        internal ICompressor dispatcher_iotopacket_compressor;
        internal IFormatter<IIotPacket> dispatcher_iotopacket_formatter;
        internal ISigner<IIotPacket> dispatcher_iotopacket_signer;
        internal ICompressor dispatcher_payload_compressor;
        internal IFormatter<Payload> dispatcher_payload_formatter;
        internal ISigner<Payload> dispatcher_payload_signer;

        internal ICompressor manager_iotopacket_compressor;
        internal IFormatter<IIotPacket> manager_iotopacket_formatter;
        internal ISigner<IIotPacket> manager_iotopacket_signer;
        internal ICompressor manager_payload_compressor;
        internal IFormatter<Payload> manager_payload_formatter;
        internal ISigner<Payload> manager_payload_signer;

        internal IClient client;
        internal IClient manager_client;

        internal Dictionary<string, ISensorDefinition> sensorDefinitions;
        internal Dictionary<string, IChainDefinition> chainDefinitions;
        internal Dictionary<string, ITriggerDefinition> triggerDefinitions;

        internal Dictionary<string, ISensor> sensorsMap;
        internal Dictionary<string, Interfaces.ITrigger> triggersMap;
        internal Dictionary<string, IChain> chainsMap;

        internal Dictionary<string, string> sensorsToChains;
        internal Dictionary<string, string> sensorsToTriggers;
        internal Dictionary<string, string> triggersToSensors;
        internal Dictionary<string, string> chainsToSensors;
        #endregion

        #region communicator
        internal IDispatcher dispatcher;

        internal void InitCommunicator()
        {
            dispatcher = new Dispatcher();
        }
        #endregion

        #region management
        public IPAddress LocalIpAddress { get; internal set; }
        public DateTime StartedAt { get; internal set; }
        public ushort AliveTimeoutInMilliseconds { get; internal set; }
        #endregion

        #region protocol
        public PacketFactory PacketFactory { get; internal set; }
        public byte ProtocolVersionMajor { get; internal set; }
        public byte ProtocolVersionMinor { get; internal set; }
        public bool IsWindows { get; internal set; }
        public bool IsLinux { get; internal set; }
        #endregion

        public Context()
        {
            sensorsMap = new Dictionary<string, ISensor>();
            triggersMap = new Dictionary<string, Interfaces.ITrigger>();
            chainsMap = new Dictionary<string, IChain>();

            sensorsToChains = new Dictionary<string, string>();
            sensorsToTriggers = new Dictionary<string, string>();
            triggersToSensors = new Dictionary<string, string>();
            chainsToSensors = new Dictionary<string, string>();
        }
    }
}
