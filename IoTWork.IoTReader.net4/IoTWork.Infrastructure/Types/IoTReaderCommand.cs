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
    public class IoTReaderCommand
    {
        [DataMember]
        public IoTReaderCommandName name { get; set; }
    }
}
