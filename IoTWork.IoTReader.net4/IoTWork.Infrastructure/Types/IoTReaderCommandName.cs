using IoTWork.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.Infrastructure.Types
{
    [DataContract]
    public enum IoTReaderCommandName
    {
        #region public
        [EnumMember]
        RestoreFactory,
        [EnumMember]
        RestoreFactoryAndRestartDevice,
        [EnumMember]
        RestartDevice,
        [EnumMember]
        RestartApplication,
        [EnumMember]
        StopAcquire,
        [EnumMember]
        RestartAcquire,

        [EnumMember]
        AskForStatistics,
        [EnumMember]
        AskForErrors,
        [EnumMember]
        AskForAlive,
        [EnumMember]
        AskForUpTime,

        [EnumMember]
        ListOfDllFilesForSensors,
        [EnumMember]
        ListOfDllFilesForPipes,

        [EnumMember]
        UploadRequestForDllFileForSensor,
        [EnumMember]
        UploadRequestForDllFileForPipe,
        [EnumMember]
        UploadConfigurationDeviceFile,
        [EnumMember]
        UploadConfigurationLogFile,
        #endregion

        #region internals
        [EnumMember]
        ExitApplication,
        [EnumMember]
        Unknown
        #endregion
    }
}
