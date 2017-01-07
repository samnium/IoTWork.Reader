using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.IO;

namespace IoTWork.Infrastructure.Interfaces
{
    //
    // http://stackoverflow.com/questions/1935040/how-to-handle-large-file-uploads-via-wcf
    // http://www.c-sharpcorner.com/uploadfile/dhananjaycoder/multiple-service-contracts-in-wcf-service/
    // https://msdn.microsoft.com/en-us/library/ms731913.aspx
    // https://msdn.microsoft.com/en-us/library/ms789010.aspx
    //

    [ServiceContract(Namespace = "http://iotwork.iotreader/commands")]
    public interface IDeviceManagementInterface: IDeviceService, IQueryService, ITransferService
    {

    }


    [ServiceContract(Namespace = "http://iotwork.iotreader/commands")]
    public interface IDeviceService
    {
        [OperationContract]
        void RestoreFactory();

        [OperationContract]
        void RestoreFactoryAndRestartDevice();

        [OperationContract]
        void RestartDevice();

        [OperationContract]
        void RestartApplication();

        [OperationContract]
        void StopAcquire();

        [OperationContract]
        void RestartAcquire();
    }

    [ServiceContract(Namespace = "http://iotwork.iotreader/commands")]
    public interface IQueryService
    {
        [OperationContract]
        void AskForStatistics();

        [OperationContract]
        void AskForErrors();

        [OperationContract]
        void AskForAlive();

        [OperationContract]
        void AskForUpTime();
    }

    [ServiceContract(Namespace = "http://iotwork.iotreader/commands")]
    public interface ITransferService
    {
        [OperationContract]
        void UploadRequestForDllFileForSensor(String FilePath, String FileContent, String Signature, Boolean SkipIfPresent);

        [OperationContract]
        void UploadRequestForDllFileForPipe(String FilePath, String FileContent, String Signature, Boolean SkipIfPresent);

        [OperationContract]
        void UploadConfigurationDeviceFile(String stream, String Signature);

        [OperationContract]
        void UploadConfigurationLogFile(String stream, String Signature);
    }
}
