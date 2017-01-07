using IoTWork.Infrastructure;
using IoTWork.Infrastructure.Interfaces;
using IoTWork.Infrastructure.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.IoTDeviceManager
{
    internal class ManagementService : IDeviceManagementInterface
    {
        internal ConcurrentQueue<Tuple<IoTReaderCommandName, String, List<object>>> _commands;

        private int _requestlimit = 20;

        public String GUID;

        public ManagementService()
        {
            _commands = new ConcurrentQueue<Tuple<IoTReaderCommandName, String, List<object>>>();
        }

        public void AskForAlive()
        {
            if (_commands.Count < _requestlimit)
                _commands.Enqueue(new Tuple<IoTReaderCommandName, String, List<object>>(IoTReaderCommandName.AskForAlive, GUID, null));
            else
            {
                SetTooManyRequestsHttpResponse();
            }
        }

        public void AskForErrors()
        {
            if (_commands.Count < _requestlimit)
                _commands.Enqueue(new Tuple<IoTReaderCommandName, String, List<object>>(IoTReaderCommandName.AskForErrors, GUID, null));
            else
            {
                SetTooManyRequestsHttpResponse();
            }
        }

        public void AskForStatistics()
        {
            if (_commands.Count < _requestlimit)
                _commands.Enqueue(new Tuple<IoTReaderCommandName, String, List<object>>(IoTReaderCommandName.AskForStatistics, GUID, null));
            else
            {
                SetTooManyRequestsHttpResponse();
            }
        }

        public void AskForUpTime()
        {
            if (_commands.Count < _requestlimit)
                _commands.Enqueue(new Tuple<IoTReaderCommandName, String, List<object>>(IoTReaderCommandName.AskForUpTime, GUID, null));
            else
            {
                SetTooManyRequestsHttpResponse();
            }
        }

        public void RestoreFactory()
        {
            if (_commands.Count < _requestlimit)
                _commands.Enqueue(new Tuple<IoTReaderCommandName, String, List<object>>(IoTReaderCommandName.RestoreFactory, GUID, null));
            else
            {
                SetTooManyRequestsHttpResponse();
            }
        }

        public void RestoreFactoryAndRestartDevice()
        {
            if (_commands.Count < _requestlimit)
                _commands.Enqueue(new Tuple<IoTReaderCommandName, String, List<object>>(IoTReaderCommandName.RestoreFactoryAndRestartDevice, GUID, null));
            else
            {
                SetTooManyRequestsHttpResponse();
            }
        }

        public void RestartAcquire()
        {
            if (_commands.Count < _requestlimit)
                _commands.Enqueue(new Tuple<IoTReaderCommandName, String, List<object>>(IoTReaderCommandName.RestartAcquire, GUID, null));
            else
            {
                SetTooManyRequestsHttpResponse();
            }
        }

        public void RestartApplication()
        {
            if (_commands.Count < _requestlimit)
                _commands.Enqueue(new Tuple<IoTReaderCommandName, String, List<object>>(IoTReaderCommandName.RestartApplication, GUID, null));
            else
            {
                SetTooManyRequestsHttpResponse();
            }
        }

        public void RestartDevice()
        {
            if (_commands.Count < _requestlimit)
                _commands.Enqueue(new Tuple<IoTReaderCommandName, String, List<object>>(IoTReaderCommandName.RestartDevice, GUID, null));
            else
            {
                SetTooManyRequestsHttpResponse();
            }
        }

        public void StopAcquire()
        {
            if (_commands.Count < _requestlimit)
                _commands.Enqueue(new Tuple<IoTReaderCommandName, String, List<object>>(IoTReaderCommandName.StopAcquire, GUID, null));
            else
            {
                SetTooManyRequestsHttpResponse();
            }
        }

        public void UploadConfigurationDeviceFile(String stream, string Signature)
        {
            if (_commands.Count < _requestlimit)
            {
                List<object> parameters = new List<object>();
                parameters.Add(stream);
                parameters.Add(Signature);
                _commands.Enqueue(new Tuple<IoTReaderCommandName, String, List<object>>(IoTReaderCommandName.UploadConfigurationDeviceFile, GUID, parameters));
            }
            else
            {
                SetTooManyRequestsHttpResponse();
            }
        }

        public void UploadConfigurationLogFile(string stream, string Signature)
        {
            if (_commands.Count < _requestlimit)
            {
                List<object> parameters = new List<object>();
                parameters.Add(stream);
                parameters.Add(Signature);
                _commands.Enqueue(new Tuple<IoTReaderCommandName, String, List<object>>(IoTReaderCommandName.UploadConfigurationLogFile, GUID, parameters));
            }
            else
            {
                SetTooManyRequestsHttpResponse();
            }
        }

        public void UploadRequestForDllFileForPipe(String FilePath, String FileContent, string Signature, bool SkipIfPresent)
        {
            if (_commands.Count < _requestlimit)
            {
                List<object> parameters = new List<object>();
                parameters.Add(FilePath);
                parameters.Add(FileContent);
                parameters.Add(Signature);
                parameters.Add(SkipIfPresent);
                _commands.Enqueue(new Tuple<IoTReaderCommandName, String, List<object>>(IoTReaderCommandName.UploadRequestForDllFileForPipe, GUID, parameters));
            }
            else
            {
                SetTooManyRequestsHttpResponse();
            }
        }

        public void UploadRequestForDllFileForSensor(String FilePath, String FileContent, string Signature, bool SkipIfPresent)
        {
            if (_commands.Count < _requestlimit)
            {
                List<object> parameters = new List<object>();
                parameters.Add(FilePath);
                parameters.Add(FileContent);
                parameters.Add(Signature);
                parameters.Add(SkipIfPresent);
                _commands.Enqueue(new Tuple<IoTReaderCommandName, String, List<object>>(IoTReaderCommandName.UploadRequestForDllFileForSensor, GUID, parameters));
            }
            else
            {
                SetTooManyRequestsHttpResponse();
            }
        }

        internal bool TryGetCommand(out Tuple<IoTReaderCommandName, String, List<object>> command)
        {
            return _commands.TryDequeue(out command);
        }

        private void SetTooManyRequestsHttpResponse()
        {
            throw new Exception("Queue is full");
        }
    }
}
