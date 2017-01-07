using IoTWork.Infrastructure.Compressors;
using IoTWork.Infrastructure.Formatters;
using IoTWork.Infrastructure.Interfaces;
using IoTWork.Infrastructure.Networking;
using IoTWork.Infrastructure.Statistics;
using IoTWork.IoTReader.Exceptions;
using IoTWork.IoTReader.Interfaces;
using IoTWork.IoTReader.Management;
using IoTWork.IoTReader.Networking;
using IoTWork.IoTReader.Pipes;
using IoTWork.IoTReader.Sensors;
using IoTWork.IoTReader.Triggers;
using IoTWork.Protocol;
using IoTWork.XML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using IoTWork.Infrastructure.Management;

namespace IoTWork.IoTReader.Helper
{
    public static class IoTReaderHelper
    {
        public static bool IsLinux { get; internal set; }

        #region Allocate Compressor

        private static ICompressor __AllocateCompressor(string type)
        {
            ICompressor compressor = new VoidCompressor();

            switch (type)
            {
                case "none":
                    compressor = new VoidCompressor();
                    break;
                case "gzip":
                    compressor = new GZipCompressor();
                    break;
                default:
                    throw new ParserException("Invalid Compressor type " + type);
            }

            return compressor;
        }

        internal static ICompressor AllocateCompressor(iotreaderManager manager)
        {
            ICompressor compressor = new VoidCompressor();

            if (manager != null && manager.compressor != null)
            {
                if (!String.IsNullOrEmpty(manager.compressor.type))
                {
                    compressor = IoTReaderHelper.__AllocateCompressor(manager.compressor.type);
                }
            }

            return compressor;
        }

        internal static IPAddress GetLocalIpAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }

        internal static ICompressor AllocateCompressor(iotreaderDispatcher dispatcher)
        {
            ICompressor compressor = new VoidCompressor();

            if (dispatcher != null && dispatcher.compressor != null)
            {
                if (!String.IsNullOrEmpty(dispatcher.compressor.type))
                {
                    compressor = IoTReaderHelper.__AllocateCompressor(dispatcher.compressor.type);
                }
            }

            return compressor;
        }

        #endregion

        #region Allocate Formatter

        private static IFormatter<T> __AllocateFormatter<T>(string type)
        {
            IFormatter<T> formatter = new VoidFormatter<T>();

            switch (type)
            {
                case "none":
                    formatter = new VoidFormatter<T>();
                    break;
                case "binary":
                    formatter = new BinaryFormatter<T>();
                    break;
                case "json":
                    formatter = new JSonFormatter<T>();
                    break;
                case "xml":
                    formatter = new XmlFormatter<T>();
                    break;
                default:
                    throw new ParserException("Invalid Formatter type " + type);

            }

            return formatter;
        }

        internal static IFormatter<T> AllocateFormatter<T>(iotreaderManager manager)
        {
            IFormatter<T> formatter = new VoidFormatter<T>();

            if (manager != null && manager.formatter != null)
            {
                if (!String.IsNullOrEmpty(manager.formatter.type))
                {
                    formatter = IoTReaderHelper.__AllocateFormatter<T>(manager.formatter.type);
                }
            }

            return formatter;
        }

        internal static IFormatter<T> AllocateFormatter<T>(iotreaderDispatcher dispatcher)
        {
            IFormatter<T> formatter = new VoidFormatter<T>();

            if (dispatcher != null && dispatcher.formatter != null)
            {
                if (!String.IsNullOrEmpty(dispatcher.formatter.type))
                {
                    formatter = IoTReaderHelper.__AllocateFormatter<T>(dispatcher.formatter.type);
                }
            }

            return formatter;
        }

        #endregion

        #region Allocate Signer

        private static ISigner<T> __AllocateSigner<T>(string type)
        {
            ISigner<T> signer = new VoidSigner<T>();

            switch (type)
            {
                case "none":
                    signer = new VoidSigner<T>();
                    break;
                case "hmac-sha1":
                    signer = new VoidSigner<T>();
                    break;
                default:
                    throw new ParserException("Invalid Signer type " + type);

            }

            return signer;
        }

        internal static ISigner<T> AllocateSigner<T>(iotreaderManager manager)
        {
            ISigner<T> signer = new VoidSigner<T>();

            if (manager != null && manager.signer != null)
            {
                if (!String.IsNullOrEmpty(manager.signer.type))
                {
                    signer = IoTReaderHelper.__AllocateSigner<T>(manager.signer.type);
                }
            }

            return signer;
        }

        internal static ISigner<T> AllocateSigner<T>(iotreaderDispatcher dispatcher)
        {
            ISigner<T> signer = new VoidSigner<T>();

            if (dispatcher != null && dispatcher.signer != null)
            {
                if (!String.IsNullOrEmpty(dispatcher.signer.type))
                {
                    signer = IoTReaderHelper.__AllocateSigner<T>(dispatcher.signer.type);
                }
            }

            return signer;
        }

        #endregion

        #region Client

        internal static IClient __AllocateClient(string type)
        {
            IClient client = new VoidClient();

            switch (type)
            {
                case "none":
                    client = new VoidClient();
                    break;
                case "udp":
                    client = new Networking.UdpClient();
                    break;
                case "web":
                    client = new Networking.WebClient();
                    break;
                default:
                    throw new ParserException("Invalid Client type " + type);
            }

            return client;
        }

        internal static IClient AllocateClient(iotreaderManager manager)
        {
            IClient client = new VoidClient();

            if (manager != null && manager.client != null)
            {
                if (!String.IsNullOrEmpty(manager.client.type))
                {
                    client = IoTReaderHelper.__AllocateClient(manager.client.type);
                }
            }

            return client;
        }

        internal static IClient AllocateClient(iotreaderDispatcher dispatcher)
        {
            IClient client = new VoidClient();

            if (dispatcher != null && dispatcher.client != null)
            {
                if (!String.IsNullOrEmpty(dispatcher.client.type))
                {
                    client = IoTReaderHelper.__AllocateClient(dispatcher.client.type);
                }
            }

            return client;
        }
        #endregion

        internal static Dictionary<string, ISensorDefinition> BuildSensorDefinitions(iotreader configuration)
        {
            Dictionary<string, ISensorDefinition> definitions = new Dictionary<string, ISensorDefinition>();

            if (configuration.sensors == null)
                throw new ParserException("Sensors definition is null.");
            if (configuration.sensors.Count() == 0)
                throw new ParserException("Sensors definition is void.");

            foreach (var s in configuration.sensors)
            {
                ISensorDefinition sd = new SensorDefinition(s);

                if (String.IsNullOrEmpty(sd.UniqueName))
                    throw new ParserException("Missing tag or value for UniqueName");
                if (sd.UniqueId == 0)
                    throw new ParserException("UniqueId is 0 for sensor " + sd.UniqueName);
                if (String.IsNullOrEmpty(sd.TypeName))
                    throw new ParserException("Missing tag or value for TypeName of sensor " + sd.UniqueName);
                if (String.IsNullOrEmpty(sd.TriggerUniqueName))
                    throw new ParserException("Missing tag or value for TriggerUniqueName of sensor " + sd.UniqueName);
                if (String.IsNullOrEmpty(sd.ChainUniqueName))
                    throw new ParserException("Missing tag or value for TriggerUniqueName of sensor " + sd.UniqueName);

                if (String.IsNullOrEmpty(sd.LibraryPath))
                    throw new ParserException("Missing tag or value for LibraryPath of sensor " + sd.UniqueName);


                if (definitions.ContainsKey(sd.UniqueName))
                    throw new ParserException("Duplicate UniqueName for sensor " + sd.UniqueName);
                else
                    definitions.Add(sd.UniqueName, sd);
            }

            return definitions;
        }

        internal static Dictionary<string, ITriggerDefinition> BuildTriggerDefinitions(iotreader configuration)
        {
            Dictionary<string, ITriggerDefinition> definitions = new Dictionary<string, ITriggerDefinition>();

            if (configuration.triggers == null)
                throw new ParserException("Triggers definition is null.");
            if (configuration.triggers.Count() == 0)
                throw new ParserException("Triggers definition is void.");

            foreach (var t in configuration.triggers)
            {
                ITriggerDefinition td = new TriggerDefinition(t);

                if (String.IsNullOrEmpty(td.UniqueName))
                    throw new ParserException("Missing tag or value for UniqueName");
                if (td.UniqueId == 0)
                    throw new ParserException("UniqueId is 0 for trigger " + td.UniqueName);
                if (String.IsNullOrEmpty(td.TypeName))
                    throw new ParserException("Missing tag or value for TypeName of trigger " + td.UniqueName);

                if (definitions.ContainsKey(td.UniqueName))
                    throw new ParserException("Duplicate UniqueName for trigger " + td.UniqueName);
                else
                    definitions.Add(td.UniqueName, td);
            }

            return definitions;
        }

        internal static NetworkManager<IotPacket> AllocateNetworkManagerForManagement(Context context)
        {
            IPacketManagerOutput packetManagerOutput = new PacketManagerOutput();
            IPacketManagerInput packetManagerInput = new PacketManagerInput();
            NetworkManager<IotPacket> networkManager = new NetworkManager<IotPacket>();

            packetManagerOutput.SetPacketFormatter(context.manager_iotopacket_formatter);
            packetManagerOutput.SetPayloadFormatter(context.manager_payload_formatter);
            packetManagerOutput.SetCompressor(context.manager_iotopacket_compressor);
            packetManagerOutput.SetPacketSigner(context.manager_iotopacket_signer);
            packetManagerOutput.SetPayloadSigner(context.manager_payload_signer);

            networkManager.SetPacketManagerOutput(packetManagerOutput);
            networkManager.SetPacketManagerInput(packetManagerInput);
            networkManager.SetClient(context.manager_client);

            return networkManager;
        }

        internal static Dictionary<string, IChainDefinition> BuildChainDefinitions(iotreader configuration)
        {
            Dictionary<string, IChainDefinition> definitions = new Dictionary<string, IChainDefinition>();

            if (configuration.chains == null)
                throw new ParserException("Chains definition is null.");
            if (configuration.chains.Count() == 0)
                throw new ParserException("Chains definition is void.");

            foreach (var c in configuration.chains)
            {
                IChainDefinition cd = new ChainDefinition(c);

                if (String.IsNullOrEmpty(cd.UniqueName))
                    throw new ParserException("Missing tag or value for UniqueName");
                if (cd.UniqueId == 0)
                    throw new ParserException("UniqueId is 0 for chain " + cd.UniqueName);

                if (c.pipes == null)
                    throw new ParserException("Pipes definition is null for chain " + cd.UniqueName);
                if (c.pipes.Count() == 0)
                    throw new ParserException("Pipes definition is void for chain " + cd.UniqueName);

                foreach (var p in c.pipes)
                {
                    IPipeDefinition pd = new PipeDefinition(p);

                    if (String.IsNullOrEmpty(pd.TypeName))
                        throw new ParserException("Missing tag or value for TypeName of pipe " + p.stage + " of chain " + cd.UniqueName);

                    cd.Attach(pd);
                }

                if (definitions.ContainsKey(cd.UniqueName))
                    throw new ParserException("Duplicate UniqueName for chain " + cd.UniqueName);
                else
                    definitions.Add(cd.UniqueName, cd);
            }

            return definitions;
        }

        internal static ISensor BuildSensor(ISensorDefinition sd)
        {
            ISensor sensor = null;

            switch (sd.TypeName)
            {
                case "random":
                    {
                        sensor = new RandomSensor();
                    }
                    break;
                case "void":
                    {
                        sensor = new VoidSensor();
                    }
                    break;
                case "custom":
                    {
                        sensor = new CustomSensor();
                    }
                    break;
            }

            if (sensor != null)
            {
                sensor.Mount(sd);
                sensor.Build();
            }

            return sensor;
        }

        internal static ITrigger BuildTrigger(ITriggerDefinition td, ISensor sensor)
        {
            ITrigger trigger = null;

            switch (td.TypeName)
            {
                case "simple":
                    {
                        trigger = new SimpleTrigger();
                    }
                    break;
            }

            if (trigger != null)
            {
                trigger.Mount(td);
                trigger.SetSensor(sensor);
                trigger.Build();

                sensor.RegisterTrigger(trigger);
            }

            return trigger;
        }

        internal static IChain BuildChain(IChainDefinition cd, ISensor sensor, IJunction<ISample> junctionChainToCommunicator)
        {
            IChain chain = new Chain();
            IJunction<ISample> junctionSensorToChain = new Junction<ISample>();

            chain.Mount(cd);
            chain.SetSensor(sensor);

            foreach (IPipeDefinition pd in cd)
            {
                IPipe pipe = null;

                switch (pd.TypeName)
                {
                    case "simple":
                        {
                            pipe = new PipeSimple();
                        }
                        break;
                    case "wait":
                        {
                            pipe = new PipeWait();
                        }
                        break;
                    case "custom":
                        {
                            pipe = new PipeCustom();
                        }
                        break;
                }

                if (pipe != null)
                {
                    pipe.Mount(pd);
                    pipe.Build();
                    chain.Attach(pipe);
                }
            }

            chain.Build();

            sensor.RegisterChain(chain);

            junctionSensorToChain.AttachSource(sensor);
            junctionSensorToChain.AttachDestination(chain);

            junctionChainToCommunicator.AttachSource(chain, chain.LinkPriority);

            return chain;
        }

        internal static Statistics MergeStatistics(Statistics managerStatistics, Statistics dispatcherStatistics, List<Statistics> sensorStatistics)
        {
            Statistics statistics = new Statistics();

            if (managerStatistics != null && dispatcherStatistics != null && sensorStatistics != null)
            {
                statistics.Add(managerStatistics);
                sensorStatistics.ForEach(x => statistics.Add(x));
                statistics.Add(dispatcherStatistics);

                return statistics;
            }
            else
                return null;
        }

        internal static ErrorResume MergeErrors(ErrorResume managerErrors, ErrorResume dispatcherErrors, ErrorResume sensorErrors)
        {
            ErrorResume resume = new ErrorResume();

            if (managerErrors != null && dispatcherErrors != null && sensorErrors != null)
            {
                resume.Add(managerErrors);
                resume.Add(dispatcherErrors);
                resume.Add(sensorErrors);

                return resume;
            }
            else
                return null;
        }

        internal static string ToLocalPath(string Path)
        {
            if (!IsLinux)
                Path = @"c:\" + Path;
            else 
                Path = "/iot/" + Path.Replace("\\", "/");
            return Path;
        }

        internal static string ConcatPath(string realPath, string filePath)
        {
            return realPath + "\\" + filePath;
        }
    }
}
