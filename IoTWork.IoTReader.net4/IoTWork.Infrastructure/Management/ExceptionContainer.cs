using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.Infrastructure.Management
{
    public class ExceptionContainer
    {
        public String UniqueName { get; set; }

        public DateTime When { get; set; }

        public String Module { get; set; }

        public Int32 Order { get; set; }

        public Exception ex { get; set; }
    }
}
