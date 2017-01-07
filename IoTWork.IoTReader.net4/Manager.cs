using IotWork.Utils.Helpers;
using IoTWork.Infrastructure;
using IoTWork.Infrastructure.Formatters;
using IoTWork.Infrastructure.Management;
using IoTWork.Infrastructure.Signer;
using IoTWork.Infrastructure.Statistics;
using IoTWork.Infrastructure.Types;
using IoTWork.IoTDeviceManager;
using IoTWork.IoTReader.Helper;
using IoTWork.IoTReader.Interfaces;
using IoTWork.IoTReader.Management; 
using IoTWork.IoTReader.Networking;
using IoTWork.IoTReader.Utils;
using IoTWork.Protocol;
using IoTWork.Protocol.Devices;
using IoTWork.Protocol.Types;
using IoTWork.XML;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace IoTWork.IoTReader
{
    public class Manager
    {
        private enum ManagerStatus
        {
            Running,
            Stopped
        }

        private Context _context;
        private ErrorResume _managerErrors;
        private Boolean _mustrun;

        private Journal<ExceptionContainer> _exceptions;

        #region sensors management

        #endregion

        #region jobs management
        private List<SensorJob> _jobs;
        #endregion

        private ManagementService _managementService;
        private TcpManagementHost _serviceHost;

        private KeyManager keyManager;

        public Manager()
        {
            _context = new Context();

            _context.StartedAt = DateTime.Now;
            _context.IsWindows = true;
            _context.IsLinux = false;

            LogManager.Info("***************************************************");
            LogManager.Info("* IoTReader                                       *");
            LogManager.Info("***************************************************");
            LogManager.Info("");
            LogManager.Info("IsWindows      {0}", _context.IsWindows);
            LogManager.Info("IsLinux        {0}", _context.IsLinux);
            LogManager.Info("");

            _jobs = new List<SensorJob>();

            _mustrun = false;
            _managerErrors = null;

            _exceptions = new Journal<ExceptionContainer>(100);

            _managementService = null;
            _serviceHost = null;

            keyManager = new KeyManager();
        }

        public void Run(String TcpServerAddress, Boolean IsLinux)
        {
            object SensorLocker = new object();

            IoTReaderHelper.IsLinux = IsLinux;

            #region Hosted Service
            _managementService = new ManagementService();
            _serviceHost = new TcpManagementHost(_managementService, TcpServerAddress, keyManager);
            _serviceHost.Start();
            #endregion

            ExecutionStep step = ExecutionStep.Start;

            String Configuration_FilePath_Log4Net = @"C:\iotreader\conf\log4net_iotreader.xml";
            LogManager.Init(Configuration_FilePath_Log4Net, "IoTReader");

            _context.ProtocolVersionMajor = 1;
            _context.ProtocolVersionMinor = 0;

            _context.LocalIpAddress = Helper.IoTReaderHelper.GetLocalIpAddress();

            _context.PacketFactory = PacketFactory.Instance;
            _context.PacketFactory.SetContext(_context);

            #region Add modules to the app.domain
            try
            {
                String modulefile = Helper.IoTReaderHelper.ToLocalPath(Constants.Path_Modules) + "modules.txt";
                LogManager.Debug("Checking for module list " + modulefile);
                if (File.Exists(modulefile))
                {
                    var modulestoload = File.ReadAllLines(modulefile);
                    if (modulestoload != null && modulestoload.Length > 0)
                    {
                        foreach (var m in modulestoload)
                        {
                            var libraryname = Helper.IoTReaderHelper.ToLocalPath(Constants.Path_Modules) + m;
                            try
                            {
                                LogManager.Debug("Loading module " + libraryname);
                                Assembly a = Assembly.LoadFile(libraryname);
                            }
                            catch (Exception ex)
                            {
                                LogManager.Error(libraryname + "::: " + ex.Message, ex);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                LogManager.Error(ex.Message, ex);
            }
            #endregion


            #region read configuration file
            try
            {
                iotreader configuration;
                String Configuration_FilePath_Main = String.Empty;

                Configuration_FilePath_Main = Helper.IoTReaderHelper.ToLocalPath(Constants.Path_ConfigurationFile);

                LogManager.Info(String.Format("Loading configuration path at {0}", Configuration_FilePath_Main));

                ILoader loader = new FileLoader();
                loader.SetPath(Configuration_FilePath_Main);
                var content = loader.Load();

                IParser parser = new XSDParser();
                var parsed = parser.ParseIoTConfigurationFile(content, out configuration);

                if (!parsed)
                    _context.configuration = null;
                else
                {
                    if (configuration.device != null)
                    {
                        _context.configuration = configuration;
                        _context.NetworkId = configuration.device.network;
                        _context.RingId = configuration.device.ring;
                        _context.RegionId = configuration.device.region;
                        _context.DeviceId = configuration.device.id;
                        _context.DeviceName = configuration.device.name;
                        _context.AliveTimeoutInMilliseconds = configuration.manager.AliveTimeoutInMilliseconds;

                        LogFormat.Init(_context.DeviceId);

                        step = ExecutionStep.Manager_Start_ConfigurationFileRead;
                    }
                    else
                    {
                        throw new FormatException("Missing device tag in configuration file.");
                    }
                }
            }
            catch(Exception ex)
            {
                LogManager.Fatal(ex.Message, ex);

                _context.configuration = null;
            }
            #endregion

            if (_context.configuration == null)
            {
                LogManager.Fatal("Configuration file not found or not well formed. Unable to start IoTReader");
            }
            else
            {
                if (_context.configuration.manager != null)
                {
                    _context.manager_iotopacket_compressor = Helper.IoTReaderHelper.AllocateCompressor(_context.configuration.manager);
                    _context.manager_iotopacket_formatter = Helper.IoTReaderHelper.AllocateFormatter<IIotPacket>(_context.configuration.manager);
                    _context.manager_iotopacket_signer = Helper.IoTReaderHelper.AllocateSigner<IIotPacket>(_context.configuration.manager);

                    _context.manager_payload_compressor = Helper.IoTReaderHelper.AllocateCompressor(_context.configuration.manager);
                    _context.manager_payload_formatter = Helper.IoTReaderHelper.AllocateFormatter<Payload>(_context.configuration.manager);
                    _context.manager_payload_signer = Helper.IoTReaderHelper.AllocateSigner<Payload>(_context.configuration.manager);

                    Uri uri = new Uri(_context.configuration.manager.client.Uri);
                    _context.manager_client = Helper.IoTReaderHelper.AllocateClient(_context.configuration.manager);
                    _context.manager_client.SetModuleName("Manager");
                    _context.manager_client.SetUri(uri);
                }

                IJunction<ISample> _junction = new Junction<ISample>();

                #region initizialization

                try
                {
                    LogManager.Info(LogFormat.Format("Initialization..."));

                    #region init dispatcher

                    _context.dispatcher_iotopacket_compressor = Helper.IoTReaderHelper.AllocateCompressor(_context.configuration.dispatcher);
                    _context.dispatcher_iotopacket_formatter = Helper.IoTReaderHelper.AllocateFormatter<IIotPacket>(_context.configuration.dispatcher);
                    _context.dispatcher_iotopacket_signer = Helper.IoTReaderHelper.AllocateSigner<IIotPacket>(_context.configuration.dispatcher);
                    _context.dispatcher_payload_compressor = Helper.IoTReaderHelper.AllocateCompressor(_context.configuration.dispatcher);
                    _context.dispatcher_payload_formatter = Helper.IoTReaderHelper.AllocateFormatter<Payload>(_context.configuration.dispatcher);
                    _context.dispatcher_payload_signer = Helper.IoTReaderHelper.AllocateSigner<Payload>(_context.configuration.dispatcher);

                    Uri uri = new Uri(_context.configuration.dispatcher.client.Uri);
                    _context.client = Helper.IoTReaderHelper.AllocateClient(_context.configuration.dispatcher);
                    _context.client.SetModuleName("Disaptcher");
                    _context.client.SetUri(uri);

                    LogManager.Info(LogFormat.Format("Compressor ... {0}", _context.dispatcher_iotopacket_formatter));
                    LogManager.Info(LogFormat.Format("Formatter  ... {0}", _context.dispatcher_iotopacket_compressor));
                    LogManager.Info(LogFormat.Format("Client     ... {0}", _context.client));

                    _context.InitCommunicator();
                    _context.dispatcher.SetClient(_context.client);
                    _context.dispatcher.SetPacketFormatter(_context.dispatcher_iotopacket_formatter);
                    _context.dispatcher.SetPacketCompressor(_context.dispatcher_iotopacket_compressor);
                    _context.dispatcher.SetPacketSigner(_context.dispatcher_iotopacket_signer);
                    _context.dispatcher.SetPayloadFormatter(_context.dispatcher_payload_formatter);
                    _context.dispatcher.SetPayloadCompressor(_context.dispatcher_payload_compressor);
                    _context.dispatcher.SetPayloadSigner(_context.dispatcher_payload_signer);

                    _junction.AttachDestination(_context.dispatcher);

                    _context.dispatcher.SetContext(_context);

                    _context.dispatcher.Build();
                    _context.dispatcher.Start();
                    #endregion

                    step = ExecutionStep.Manager_Start_BasicInitialization_Done;
                }
                catch (Exception ex)
                {
                    LogManager.Fatal(ex.Message, ex);

                    step = ExecutionStep.Manager_Start_BasicInitialization_Failed;
                }

                #endregion

                if (step == ExecutionStep.Manager_Start_BasicInitialization_Done)
                {
                    #region parse configuration file

                    LogManager.Info(String.Format("Parsing configuration file"));

                    try
                    {
                        _context.sensorDefinitions = Helper.IoTReaderHelper.BuildSensorDefinitions(_context.configuration);
                        _context.triggerDefinitions = Helper.IoTReaderHelper.BuildTriggerDefinitions(_context.configuration);
                        _context.chainDefinitions = Helper.IoTReaderHelper.BuildChainDefinitions(_context.configuration);

                        step = ExecutionStep.Manager_Start_ArchitectureParsing_Done;
                    }
                    catch (Exception ex)
                    {
                        LogManager.Fatal(ex.Message, ex);

                        step = ExecutionStep.Manager_Start_ArchitectureParsing_Failed;
                    }
                    #endregion

                    if (step == ExecutionStep.Manager_Start_ArchitectureParsing_Done)
                    {
                        #region init sensor jobs

                        LogManager.Info(String.Format("Allocating sensors"));

                        foreach (var sd in _context.sensorDefinitions)
                        {
                            IChainDefinition cd = null;
                            ITriggerDefinition td = null;

                            ISensor sensor = Helper.IoTReaderHelper.BuildSensor(sd.Value);

                            if (sensor != null)
                            {
                                cd = _context.chainDefinitions.ContainsKey(sensor.ChainUniqueName) ?
                                    _context.chainDefinitions[sensor.ChainUniqueName] : (IChainDefinition)null;

                                td = _context.triggerDefinitions.ContainsKey(sensor.TriggerUniqueName) ?
                                    _context.triggerDefinitions[sensor.TriggerUniqueName] : (ITriggerDefinition)null;
                            }

                            if (sensor != null && cd != null && td != null)
                            {
                                Interfaces.ITrigger trigger = Helper.IoTReaderHelper.BuildTrigger(td, sensor);
                                IChain chain = Helper.IoTReaderHelper.BuildChain(cd, sensor, _junction);

                                _context.sensorsMap.Add(sensor.UniqueName, sensor);
                                _context.chainsMap.Add(chain.RealUniqueName, chain);
                                _context.triggersMap.Add(trigger.RealUniqueName, trigger);

                                _context.chainsToSensors.Add(sensor.GetChain().RealUniqueName, sensor.UniqueName);
                                _context.triggersToSensors.Add(sensor.GetTrigger().RealUniqueName, sensor.UniqueName);

                                _context.sensorsToChains.Add(sensor.UniqueName, sensor.ChainUniqueName);
                                _context.sensorsToTriggers.Add(sensor.UniqueName, sensor.TriggerUniqueName);

                                SensorJob sensorjob = new SensorJob(_context, SensorLocker);
                                sensorjob.SetSensor(sensor);

                                _jobs.Add(sensorjob);
                            }
                        }
                        #endregion

                        #region open chains and schedule jobs

                        LogManager.Info(String.Format("Starting acquire process"));

                        SensorArchive.Persist(_context.sensorsMap);

                        foreach (var c in _context.chainsMap)
                        {
                            c.Value.Open();
                        }

                        foreach (var j in _jobs)
                        {
                            j.Schedule();
                        }

                        #endregion

                        #region Send Start Message

                        LogManager.Info(String.Format("Sending starting message"));

                        IotPacket packetManagerStarted = _context.PacketFactory.BuildPacket_ManagerStarted(_context.StartedAt);
                        NetworkManager<IotPacket> networkManager = Helper.IoTReaderHelper.AllocateNetworkManagerForManagement(_context);
                        networkManager.Start();
                        networkManager.SendMessage(packetManagerStarted);
                        
                        #endregion  

                        #region Management work cycle

                        DateTime aliveTimer = DateTime.Now;

                        _mustrun = true;
                        AcquireStatus acquireStatus = AcquireStatus.Acquiring;

                        #region Statistics
                        Int32 stats_SamplingSequenceNumber = 0;
                        Int32 stats_CommandExecuted = 0;
                        Int32 stats_AliveSent = 0;
                        Int32 stats_AliveTimeoutInMilliseconds = _context.AliveTimeoutInMilliseconds;
                        #endregion

                        Boolean RestartDeviceOnExit = false;

                        Boolean IsAlert = false;
                        Boolean Acknowledge = false;
                        Boolean GiveFeedback = false;
                        String FeedbackText = String.Empty;
                        IoTReaderCommand FeedbackCommand = null;
                        String FeedbackGuid = String.Empty;

                        while (_mustrun)
                        {
                            #region Feedback
                            if (GiveFeedback && FeedbackCommand != null)
                            {
                                var code = (Int32)FeedbackCommand.name;
                                var name = FeedbackCommand.name.ToString();
                                SendFeedback(networkManager, FeedbackGuid, IsAlert, Acknowledge, code, name, FeedbackText);
                                GiveFeedback = false;
                            }
                            #endregion

                            stats_SamplingSequenceNumber++;

                            Thread.Sleep(100);

                            #region Build and Send Alive

                            try
                            {
                                if ((DateTime.Now - aliveTimer).TotalMilliseconds > _context.AliveTimeoutInMilliseconds)
                                {
                                    stats_AliveSent++;

                                    LogManager.Info(String.Format("ALIVE"));

                                    SendAlive(networkManager, acquireStatus);
                                    aliveTimer = DateTime.Now;
                                }
                            }
                            catch (Exception ex)
                            {
                                LogManager.Error(ex.Message, ex);
                                RegisterException("Reader " + _context.DeviceName, "MainThread", DateTime.Now, stats_SamplingSequenceNumber, ex);
                            }
                            #endregion

                            #region Command Management
                            List<object> Parameters = null;
                            String GUID = null;
                            IoTReaderCommand command = CheckForIncomingCommand(out Parameters, out GUID);

                            if (command != null)
                            {
                                try
                                {
                                    GiveFeedback = false;
                                    Acknowledge = true;
                                    FeedbackText = String.Empty;
                                    FeedbackCommand = command;
                                    FeedbackText = "done";
                                    FeedbackGuid = GUID;

                                    stats_CommandExecuted++;

                                    switch (command.name)
                                    {
                                        case IoTReaderCommandName.ExitApplication:
                                            {
                                                LogManager.Info(String.Format("Executing command ExitApplication"));

                                                GiveFeedback = true;

                                                _mustrun = false;
                                                continue;
                                            }
                                        case IoTReaderCommandName.RestoreFactoryAndRestartDevice:
                                            {
                                                LogManager.Info(String.Format("Executing command RestoreFactoryAndRestartDevice"));

                                                var Path_ConfigurationFile = IoTReaderHelper.ToLocalPath(Constants.Path_ConfigurationFile);
                                                var Path_ConfigurationFileFactory = IoTReaderHelper.ToLocalPath(Constants.Path_ConfigurationFileFactory);

                                                LogManager.Debug("Path_ConfigurationFile          :" + Path_ConfigurationFile);
                                                LogManager.Debug("Path_ConfigurationFileFactory   :" + Path_ConfigurationFileFactory);

                                                File.Copy(Path_ConfigurationFileFactory, Path_ConfigurationFile, true);

                                                GiveFeedback = true;

                                                RestartDeviceOnExit = true;
                                                _mustrun = false;
                                                continue;
                                            }
                                        case IoTReaderCommandName.RestartDevice:
                                            {
                                                GiveFeedback = true;

                                                RestartDeviceOnExit = true;
                                                _mustrun = false;
                                                continue;
                                            }
                                        case IoTReaderCommandName.RestartApplication:
                                            {
                                                LogManager.Info(String.Format("Executing command RestartApplication"));

                                                GiveFeedback = true;

                                                _mustrun = false;
                                                continue;
                                            }
                                        case IoTReaderCommandName.StopAcquire:
                                            {
                                                LogManager.Info(String.Format("Executing command StopAcquire"));

                                                if (acquireStatus == AcquireStatus.Acquiring)
                                                {
                                                    foreach (var j in _jobs)
                                                    {
                                                        j.Pause();
                                                    }

                                                    acquireStatus = AcquireStatus.Paused;
                                                }

                                                GiveFeedback = true;
                                            }
                                            break;
                                        case IoTReaderCommandName.RestartAcquire:
                                            {
                                                LogManager.Info(String.Format("Executing command RestartAcquire"));

                                                if (acquireStatus == AcquireStatus.Paused)
                                                {
                                                    foreach (var j in _jobs)
                                                    {
                                                        j.Resume();
                                                    }

                                                    acquireStatus = AcquireStatus.Acquiring;
                                                }

                                                GiveFeedback = true;
                                            }
                                            break;

                                        case IoTReaderCommandName.AskForStatistics:
                                            {
                                                LogManager.Info(String.Format("Executing command AskForStatistics"));

                                                IoTWork.Infrastructure.Statistics.Statistics dispatcherStatistics = null;
                                                List<IoTWork.Infrastructure.Statistics.Statistics> sensorStatistics = new List<IoTWork.Infrastructure.Statistics.Statistics>();
                                                IoTWork.Infrastructure.Statistics.Statistics managerStatistics = new IoTWork.Infrastructure.Statistics.Statistics();

                                                dispatcherStatistics = _context.dispatcher.GetStatistics();
                                                foreach (var c in _context.chainsMap)
                                                {
                                                    var sensorName = _context.chainsToSensors[c.Value.RealUniqueName];
                                                    var sensor = _context.sensorsMap[sensorName];

                                                    sensorStatistics.Add(sensor.GetStatistics());
                                                    sensorStatistics.Add(c.Value.GetStatistics());
                                                }

                                                managerStatistics.Add("Reader " + _context.DeviceName, "SamplingSequenceNumber", stats_SamplingSequenceNumber.ToString(), NoteDomain.System);
                                                managerStatistics.Add("Reader " + _context.DeviceName, "CommandExecuted", stats_CommandExecuted.ToString(), NoteDomain.System);
                                                managerStatistics.Add("Reader " + _context.DeviceName, "AliveSent", stats_AliveSent.ToString(), NoteDomain.System);
                                                managerStatistics.Add("Reader " + _context.DeviceName, "AliveTimeoutInMilliseconds", stats_AliveTimeoutInMilliseconds.ToString(), NoteDomain.System);

                                                SendStatistics(networkManager, GUID, managerStatistics, dispatcherStatistics, sensorStatistics);
                                            }
                                            break;
                                        case IoTReaderCommandName.AskForErrors:
                                            {
                                                LogManager.Info(String.Format("Executing command AskForErrors"));

                                                ErrorResume dispatcherErrors = null;
                                                ErrorResume sensorErrors = new ErrorResume();
                                                ErrorResume managerErrors = null;

                                                dispatcherErrors = _context.dispatcher.GetErrors();
                                                foreach (var c in _context.chainsMap)
                                                {
                                                    var sensorName = _context.chainsToSensors[c.Value.RealUniqueName];
                                                    var sensor = _context.sensorsMap[sensorName];

                                                    sensorErrors.Add(sensor.GetErrors());
                                                    sensorErrors.Add(c.Value.GetErrors());
                                                }

                                                var UpTimeSeconds = (DateTime.Now - _context.StartedAt).TotalSeconds;

                                                managerErrors = IotWork.Utils.Helpers.IoTWorkHelper.ToErrorResume(_exceptions);

                                                var errors = Helper.IoTReaderHelper.MergeErrors(managerErrors, dispatcherErrors, sensorErrors);

                                                if (errors.Count > 0)
                                                {
                                                    LogManager.Debug("");
                                                    LogManager.Debug("ERROR RESUME");
                                                    foreach (var er in errors)
                                                    {
                                                        LogManager.Debug("-> " + er.GetWhen() + " :: " + er.GetModule() + " :: " + er.GetMessage());
                                                    }
                                                    LogManager.Debug("");
                                                }

                                                SendErrors(networkManager, GUID, errors);
                                            }
                                            break;
                                        case IoTReaderCommandName.AskForAlive:
                                            {
                                                LogManager.Info(String.Format("Executing command AskForAlive"));

                                                SendAlive(networkManager, acquireStatus);
                                                aliveTimer = DateTime.Now;
                                            }
                                            break;
                                        case IoTReaderCommandName.AskForUpTime:
                                            {
                                                LogManager.Info(String.Format("Executing command AskForUpTime"));

                                                SendUpTime(networkManager);
                                            }
                                            break;

                                        case IoTReaderCommandName.ListOfDllFilesForSensors:
                                            {
                                                // Do nothing: executed by the DeviceManager
                                            }
                                            break;
                                        case IoTReaderCommandName.ListOfDllFilesForPipes:
                                            {
                                                // Do nothing: executed by the DeviceManager
                                            }
                                            break;

                                        case IoTReaderCommandName.UploadRequestForDllFileForSensor:
                                            {
                                                LogManager.Info(String.Format("Executing command UploadRequestForDllFileForSensor"));

                                                String FilePath = (String)Parameters[0];
                                                String FileContent = (String)Parameters[1];
                                                String Signature = (String)Parameters[2];

                                                var hashvalue = IotWork.Utils.Helpers.IoTWorkHelper.ComputeHmacSha1(FileContent, keyManager.DeviceKey);

                                                LogManager.Debug("FilePath    :" + FilePath);
                                                LogManager.Debug("Signature   :" + Signature);
                                                LogManager.Debug("Calculated Signature   :" + hashvalue);

                                                // check signature
                                                if (!String.Equals(hashvalue, Signature))
                                                {
                                                    IsAlert = true;
                                                    FeedbackText = "Signature is not valid";
                                                    break;
                                                }

                                                String RealPath = IoTReaderHelper.ToLocalPath(Constants.Path_Sensors);
                                                RealPath = IoTReaderHelper.ConcatPath(RealPath, FilePath);
                                                byte[] Content = Convert.FromBase64String(FileContent);

                                                LogManager.Debug("writing " + Content.Length + " bytes to file " + RealPath);

                                                using (var _FileStream = new System.IO.FileStream(FilePath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                                                {
                                                    _FileStream.Write(Content, 0, Content.Length);
                                                    _FileStream.Flush();
                                                }

                                                GiveFeedback = true;
                                            }
                                            break;
                                        case IoTReaderCommandName.UploadRequestForDllFileForPipe:
                                            {
                                                LogManager.Info(String.Format("Executing command UploadRequestForDllFileForPipe"));

                                                String FilePath = (String)Parameters[0];
                                                String FileContent = (String)Parameters[1];
                                                String Signature = (String)Parameters[2];

                                                var hashvalue = IotWork.Utils.Helpers.IoTWorkHelper.ComputeHmacSha1(FileContent, keyManager.DeviceKey);

                                                LogManager.Debug("FilePath    :" + FilePath);
                                                LogManager.Debug("Signature   :" + Signature);
                                                LogManager.Debug("Calculated Signature   :" + hashvalue);

                                                // check signature
                                                if (!String.Equals(hashvalue, Signature))
                                                {
                                                    IsAlert = true;
                                                    FeedbackText = "Signature is not valid";
                                                    break;
                                                }

                                                String RealPath = IoTReaderHelper.ToLocalPath(Constants.Path_Pipes);
                                                RealPath = IoTReaderHelper.ConcatPath(RealPath, FilePath);
                                                byte[] Content = Convert.FromBase64String(FileContent);

                                                LogManager.Debug("writing " + Content.Length + " bytes to file " + RealPath);

                                                File.WriteAllBytes(FilePath.Replace("//", "/"), Content);

                                                GiveFeedback = true;
                                            }
                                            break;
                                        case IoTReaderCommandName.UploadConfigurationDeviceFile:
                                            {
                                                LogManager.Info(String.Format("Executing command UploadConfigurationDeviceFile"));

                                                String Content = (String)Parameters[0];
                                                String Signature = (String)Parameters[1];

                                                if (Content.Length > 256 * 1024)
                                                    break;

                                                var hashvalue = IotWork.Utils.Helpers.IoTWorkHelper.ComputeHmacSha1(Content, keyManager.DeviceKey);

                                                // check signature
                                                if (!String.Equals(hashvalue, Signature))
                                                {
                                                    IsAlert = true;
                                                    FeedbackText = "Signature is not valid";
                                                    break;
                                                }

                                                // check if it is a valid configuration file
                                                try
                                                {
                                                    iotreader configuration;

                                                    IParser parser = new XSDParser();

                                                    var parsed = parser.ParseIoTConfigurationFile(Content, out configuration);

                                                    if (configuration == null)
                                                    {
                                                        IsAlert = true;
                                                        FeedbackText = "configuration section is null";
                                                        break;
                                                    }

                                                    if (configuration.device == null)
                                                    {
                                                        IsAlert = true;
                                                        FeedbackText = "device section is null";
                                                        break;
                                                    }

                                                    if (configuration.manager == null)
                                                    {
                                                        IsAlert = true;
                                                        FeedbackText = "manager section is null";
                                                        break;
                                                    }

                                                    if (configuration.sensors == null || configuration.sensors.Length == 0)
                                                    {
                                                        IsAlert = true;
                                                        FeedbackText = "sensors section is null";
                                                        break;
                                                    }

                                                    if (configuration.chains == null || configuration.chains.Length == 0)
                                                    {
                                                        IsAlert = true;
                                                        FeedbackText = "chains section is null";
                                                        break;
                                                    }

                                                    if (configuration.triggers == null || configuration.triggers.Length == 0)
                                                    {
                                                        IsAlert = true;
                                                        FeedbackText = "triggers section is null";
                                                        break;
                                                    }

                                                    if (configuration.dispatcher == null)
                                                    {
                                                        IsAlert = true;
                                                        FeedbackText = "dispatcher section is null";
                                                        break;
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    IsAlert = true;
                                                    FeedbackText = "xml parsing error " + ex.Message;
                                                    break;
                                                }

                                                var path = IoTReaderHelper.ToLocalPath(Constants.Path_ConfigurationFile);
                                                IotWork.Utils.Helpers.IoTWorkHelper.WriteToFile(path, Content);

                                                GiveFeedback = true;
                                            }
                                            break;
                                        case IoTReaderCommandName.UploadConfigurationLogFile:
                                            {
                                                // Do nothing: executed by the DeviceManager
                                            }
                                            break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogManager.Error(ex.Message, ex);
                                    RegisterException("Reader " + _context.DeviceName, "MainThread", DateTime.Now, stats_SamplingSequenceNumber, ex);

                                    #region Feedback
                                    if (command != null)
                                    {
                                        var code = (Int32)command.name;
                                        var name = command.name.ToString();
                                        SendFeedback(networkManager, FeedbackGuid, true, true, code, name, ex.Message);
                                        GiveFeedback = false;
                                    }
                                    #endregion
                                }
                            }

                            #endregion
                        }

                        #region shutdown

                        #region Network Manager 
                        while (!networkManager.IsOutputVoid()) Thread.Sleep(500);
                        #endregion

                        #region Hosted Service
                        LogManager.Info(String.Format("Interface shuttingdown"));
                        if (_serviceHost != null)
                            _serviceHost.Close();
                        #endregion

                        #region close chains and stop jobs

                        LogManager.Info(String.Format("Jobs stopping"));

                        foreach (var j in _jobs)
                        {
                            j.Stop();
                        }

                        Boolean allChainsHadBeenClosed = false;

                        LogManager.Info(String.Format("Chains stopping"));

                        while (allChainsHadBeenClosed)
                        {
                            allChainsHadBeenClosed = true;

                            foreach (var c in _context.chainsMap)
                            {
                                if (!c.Value.IsEmpty())
                                    allChainsHadBeenClosed = false;
                            }

                            Thread.Sleep(100);
                        }

                        LogManager.Info(String.Format("Chains closing"));

                        foreach (var c in _context.chainsMap)
                        {
                            c.Value.Close();
                        }

                        LogManager.Info(String.Format("Dispatcher stopping"));

                        while (!_context.dispatcher.IsFree())
                            Thread.Sleep(100);

                        LogManager.Info(String.Format("Dispatcher closing"));

                        _context.dispatcher.Close();


                        #endregion

                        #endregion

                        #region Send Close Message

                        LogManager.Info(String.Format("Sending closing message"));

                        IotPacket packetManagerStopped = _context.PacketFactory.BuildPacket_ManagerStopped(_context.StartedAt);
                        networkManager.SendMessage(packetManagerStopped);

                        #endregion

                        acquireStatus = AcquireStatus.Stopped;

                        #endregion

                        LogManager.Info(String.Format("Application ending"));

                        if (RestartDeviceOnExit)
                        {
                            RestartDevice();
                        }
                    }
                }
            }
        }

        private void RestartDevice()
        {
            //if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            //{
            //    LogManager.Info(String.Format("Bye bye WINDOWS"));
            //}
            //else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            //{
            //    LogManager.Info(String.Format("Bye bye LINUX"));
            //}
        }

        private void RegisterException(String UniqueName, String Module, DateTime When, Int32 SequenceNumber, Exception Exception)
        {
            ExceptionContainer ec = new ExceptionContainer();
            ec.UniqueName = UniqueName;
            ec.When = When;
            ec.Module = Module;
            ec.Order = SequenceNumber;
            ec.ex = Exception;

            _exceptions.Add(ec);
        }

        private void SendFeedback(NetworkManager<IotPacket> networkManager, String GUID, bool IsAlert, bool Acknowledge, int Code, string Name, string Text)
        {
            IotPacket packet = _context.PacketFactory.BuildPacket_Message(GUID, IsAlert, Acknowledge, Code, Name, Text);

            LogManager.Debug(
                String.Format("Sending Feedback to command {0} with GUID {1}: alert is {2} and acknowledge is {3}", Name, GUID, IsAlert, Acknowledge));

            networkManager.SendMessage(packet);
        }

        private void SendStatistics(NetworkManager<IotPacket> networkManager, String GUID, IoTWork.Infrastructure.Statistics.Statistics managerStatistics, IoTWork.Infrastructure.Statistics.Statistics dispatcherStatistics, List<IoTWork.Infrastructure.Statistics.Statistics> sensorStatistics)
        {
            var statistics = Helper.IoTReaderHelper.MergeStatistics(managerStatistics, dispatcherStatistics, sensorStatistics);

            IotPacket packet = _context.PacketFactory.BuildPacket_Statistics(GUID, statistics);

            networkManager.SendMessage(packet);
        }

        private void SendErrors(NetworkManager<IotPacket> networkManager, String GUID, ErrorResume errors)
        {
            IotPacket packet = _context.PacketFactory.BuildPacket_Errors(GUID, errors);

            networkManager.SendMessage(packet);
        }

        private void SendUpTime(NetworkManager<IotPacket> networkManager)
        {
            IotPacket packet = _context.PacketFactory.BuildPacket_UpTime(_context.StartedAt);

            networkManager.SendMessage(packet);
        }

        private void SendAlive(NetworkManager<IotPacket> networkManager, AcquireStatus acquireStatus)
        {
            try
            {
                TimeSpan UpTime = TimeSpan.FromMilliseconds((DateTime.Now - _context.StartedAt).TotalMilliseconds);

                Dictionary<string, IoTWork.Infrastructure.Statistics.Statistics> chainStatistics = new Dictionary<string, IoTWork.Infrastructure.Statistics.Statistics>();
                foreach (var c in _context.chainsMap)
                {
                    var statistics = c.Value.GetStatistics();
                    chainStatistics.Add(c.Key, statistics);
                }

                IotPacket packetAlive = _context.PacketFactory.BuildPacket_Alive(UpTime, acquireStatus, chainStatistics);

                networkManager.SendMessage(packetAlive);
            }
            catch (Exception)
            {
            }
        }

        private IoTReaderCommand CheckForIncomingCommand(out List<object> Parameters, out String GUID)
        {
            bool dequeued = false;
            Tuple<IoTReaderCommandName, String, List<object>> command = null;
            Parameters = null;
            GUID = String.Empty;

            try
            {
                dequeued = _managementService.TryGetCommand(out command);
            }
            catch (Exception)
            {
                dequeued = false;
            }


            if (dequeued && command != null)
            {
                IoTReaderCommand retcmd = new IoTReaderCommand();
                retcmd.name = command.Item1;
                GUID = command.Item2;
                Parameters = command.Item3;
                return retcmd;
            }

            return null;
        }
    }
}
