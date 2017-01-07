using IotWork.Utils.Helpers;
using IoTWork.Infrastructure;
using IoTWork.Infrastructure.Signer;
using IoTWork.Infrastructure.Types;
using IoTWork.IoTDeviceManager;
using IoTWork.IoTReader.Core.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IoTWork.IoTDeviceManager
{
    internal class TcpManagementHost
    {
        ManagementService _managementService;
        TcpServer _server;
        KeyManager _keyManager;

        Boolean _mustrun;
        Task _thread;

        public TcpManagementHost(ManagementService ManagementService, String TcpServerAddress, KeyManager KeyManager)
        {
            _mustrun = false;
            _thread = null;
            _managementService = ManagementService;
            _server = new TcpServer(TcpServerAddress);
            _keyManager = KeyManager;
        }

        internal void Start()
        {
            _mustrun = true;
            _thread = new Task(() => _Run());
            _thread.Start();
            _server.Start(Constants.Network_DeviceManagementPort);
        }

        internal void Close()
        {
            _mustrun = false;
            while (!_thread.Wait(100)) ;
        }

        private void _Run()
        {
            while (_mustrun)
            {
                byte[] packet = null;
                Boolean dequeued = false;

                try
                {
                    dequeued = _server.TryReceive(out packet);
                }
                catch (Exception ex)
                {
                    LogManager.Error(ex.Message, ex);
                    dequeued = false;
                }

                if (dequeued && packet != null)
                {
                    DateTime UtcPacketTime;
                    String Guid;
                    byte[] Payload;

                    if (ParseAndValidatePacket(packet, out UtcPacketTime, out Guid, out Payload))
                    {
                        Thread.Sleep(150);
                        continue;
                    }

                    packet = Payload;
                    var commandCode = packet[0];
                    var commandName = (IoTReaderCommandName)commandCode;

                    LogManager.Debug("TcpManagementHost executing " + commandName.ToString());

                    switch (commandName)
                    {
                        case IoTReaderCommandName.AskForAlive:
                            {
                                _managementService.GUID = Guid;
                                _managementService.AskForAlive();
                            }
                            break;
                        case IoTReaderCommandName.AskForErrors:
                            {
                                _managementService.GUID = Guid;
                                _managementService.AskForErrors();
                            }
                            break;
                        case IoTReaderCommandName.AskForStatistics:
                            {
                                _managementService.GUID = Guid;
                                _managementService.AskForStatistics();
                            }
                            break;
                        case IoTReaderCommandName.AskForUpTime:
                            {
                                _managementService.GUID = Guid;
                                _managementService.AskForUpTime();
                            }
                            break;
                        case IoTReaderCommandName.RestartAcquire:
                            {
                                _managementService.GUID = Guid;
                                _managementService.RestartAcquire();
                            }
                            break;
                        case IoTReaderCommandName.RestartApplication:
                            {
                                _managementService.GUID = Guid;
                                _managementService.RestartApplication();
                            }
                            break;
                        case IoTReaderCommandName.RestoreFactory:
                            {
                                _managementService.GUID = Guid;
                                _managementService.RestoreFactory();
                            }
                            break;
                        case IoTReaderCommandName.RestoreFactoryAndRestartDevice:
                            {
                                _managementService.GUID = Guid;
                                _managementService.RestoreFactoryAndRestartDevice();
                            }
                            break;
                        case IoTReaderCommandName.RestartDevice:
                            {
                                _managementService.GUID = Guid;
                                _managementService.RestartDevice();
                            }
                            break;
                        case IoTReaderCommandName.StopAcquire:
                            {
                                _managementService.GUID = Guid;
                                _managementService.StopAcquire();
                            }
                            break;
                        case IoTReaderCommandName.UploadConfigurationDeviceFile:
                            {
                                var xx = Encoding.UTF8.GetString(packet);
                                var yy = Encoding.UTF8.GetBytes(xx);

                                // greater than packet type + length1 + legth2
                                if (packet.Length > 1 + 4 + 4)
                                {
                                    var length1 = BitConverter.ToInt32(packet, 1);

                                    if (packet.Length > 1 + 4 + length1 + 4)
                                    {
                                        byte[] b1 = new byte[length1];
                                        Array.Copy(packet, 1 + 4, b1, 0, length1);

                                        var length2 = BitConverter.ToInt32(packet, 1 + 4 + length1);

                                        byte[] b2 = new byte[length2];
                                        Array.Copy(packet, 1 + 4 + length1 + 4, b2, 0, length2);

                                        String File = Encoding.UTF8.GetString(b1);
                                        String Signature = Encoding.UTF8.GetString(b2);

                                        _managementService.GUID = Guid;
                                        _managementService.UploadConfigurationDeviceFile(File, Signature);
                                    }
                                }
                            }
                            break;
                        case IoTReaderCommandName.UploadConfigurationLogFile:
                            {
                            }
                            break;
                        case IoTReaderCommandName.UploadRequestForDllFileForPipe:
                            {
                                // greater than packet type + length1 + legth2 + length3
                                if (packet.Length > 1 + 4 + 4 + 4)
                                {
                                    var length1 = BitConverter.ToInt32(packet, 1);

                                    if (packet.Length > 1 + 4 + length1 + 4 + 4)
                                    {
                                        byte[] b1 = new byte[length1];
                                        Array.Copy(packet, 1 + 4, b1, 0, length1);

                                        var length2 = BitConverter.ToInt32(packet, 1 + 4 + length1);

                                        if (packet.Length > 1 + 4 + length1 + 4 + length2 + 4)
                                        {
                                            byte[] b2 = new byte[length2];
                                            Array.Copy(packet, 1 + 4 + length1 + 4, b2, 0, length2);

                                            var length3 = BitConverter.ToInt32(packet, 1 + 4 + length1 + 4 + length2);

                                            if (packet.Length == 1 + 4 + length1 + 4 + length2 + 4 + length3)
                                            {
                                                byte[] b3 = new byte[length3];
                                                Array.Copy(packet, 1 + 4 + length1 + 4 + length2 + 4, b3, 0, length3);

                                                String FilePath = Encoding.UTF8.GetString(b1);
                                                String FileContent = Encoding.UTF8.GetString(b2);
                                                String Signature = Encoding.UTF8.GetString(b3);

                                                LogManager.Debug("TcpManagementHost   FilePath    :" + FilePath);
                                                LogManager.Debug("TcpManagementHost   Signature   :" + Signature);

                                                _managementService.GUID = Guid;
                                                _managementService.UploadRequestForDllFileForPipe(FilePath, FileContent, Signature, false);
                                            }
                                        }
                                    }
                                }

                            }
                            break;
                        case IoTReaderCommandName.UploadRequestForDllFileForSensor:
                            {
                                // greater than packet type + length1 + legth2 + length3
                                if (packet.Length > 1 + 4 + 4 + 4)
                                {
                                    var length1 = BitConverter.ToInt32(packet, 1);

                                    if (packet.Length > 1 + 4 + length1 + 4 + 4)
                                    {
                                        byte[] b1 = new byte[length1];
                                        Array.Copy(packet, 1 + 4, b1, 0, length1);

                                        var length2 = BitConverter.ToInt32(packet, 1 + 4 + length1);

                                        if (packet.Length > 1 + 4 + length1 + 4 + length2 + 4)
                                        {
                                            byte[] b2 = new byte[length2];
                                            Array.Copy(packet, 1 + 4 + length1 + 4, b2, 0, length2);

                                            var length3 = BitConverter.ToInt32(packet, 1 + 4 + length1 + 4 + length2);

                                            if (packet.Length == 1 + 4 + length1 + 4 + length2 + 4 + length3)
                                            {
                                                byte[] b3 = new byte[length3];
                                                Array.Copy(packet, 1 + 4 + length1 + 4 + length2 + 4, b3, 0, length3);

                                                String FilePath = Encoding.UTF8.GetString(b1);
                                                String FileContent = Encoding.UTF8.GetString(b2);
                                                String Signature = Encoding.UTF8.GetString(b3);

                                                LogManager.Debug("TcpManagementHost   FilePath    :" + FilePath);
                                                LogManager.Debug("TcpManagementHost   Signature   :" + Signature);

                                                _managementService.GUID = Guid;
                                                _managementService.UploadRequestForDllFileForSensor(FilePath, FileContent, Signature, false);
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }

                Thread.Sleep(100);
            }
        }

        private bool ParseAndValidatePacket(byte[] packet, out DateTime utcPacketTime, out string guid, out byte[] payload)
        {
            bool discarded = false;

            utcPacketTime = DateTime.UtcNow;
            guid = String.Empty;
            payload = null;

            try
            {
                int lenghttoread = 0;
                byte[] length = new byte[4];
                byte[] sinceepoch = new byte[8];
                byte[] guidarray = null;
                byte[] signaturearray = null;
                byte[] arraytosign = null;
                byte[] signaturecalculated = null;

                int pos = 0;

                Array.Copy(packet, pos, sinceepoch, 0, 8);
                pos += 8;

                pos += 1;

                Array.Copy(packet, pos, length, 0, 4);
                pos += 4;
                lenghttoread = BitConverter.ToInt32(length, 0);

                guidarray = new byte[lenghttoread];
                Array.Copy(packet, pos, guidarray, 0, lenghttoread);
                pos += lenghttoread;

                Array.Copy(packet, pos, length, 0, 4);
                pos += 4;
                lenghttoread = BitConverter.ToInt32(length, 0);

                if (lenghttoread > 0)
                {
                    payload = new byte[lenghttoread + 1];
                    payload[0] = packet[8];
                    Array.Copy(packet, pos, payload, 1, lenghttoread);
                    pos += lenghttoread;
                }
                else
                {
                    payload = new byte[1];
                    payload[0] = packet[8];
                }

                arraytosign = new byte[pos];
                Array.Copy(packet, 0, arraytosign, 0, pos);

                lenghttoread = packet.Length - pos;

                signaturearray = new byte[lenghttoread];
                Array.Copy(packet, pos, signaturearray, 0, lenghttoread);

                utcPacketTime = IoTWorkHelper.FromUnixTime(BitConverter.ToInt64(sinceepoch, 0));
                guid = Encoding.UTF8.GetString(guidarray);
                signaturecalculated = IoTWorkHelper.ComputeHmacSha1(arraytosign, _keyManager.PayloadKey);

                if ((DateTime.Now - utcPacketTime.ToLocalTime()).TotalSeconds > 20)
                    discarded = true;
                if (!signaturecalculated.SequenceEqual(signaturearray))
                    discarded = true;
            }
            catch (Exception)
            {
                discarded = true;
            }

            return discarded;
        }
    }
}
