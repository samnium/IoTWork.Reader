using IoTWork.IoTReader.Exceptions;
using IoTWork.IoTReader.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.IoTReader.Triggers
{
    class SimpleTrigger: Trigger
    {
        public override void Build()
        {
            if (WithIntervalInMilliseconds <= 0)
                throw new TriggerException("Simple Trigger has value WithIntervalInMilliseconds <= 0");

            try
            {

            }
            catch(Exception ex)
            {
                throw new TriggerException("Unable to build Quartz Trigger " + ex.Message + ".");
            }
        }

        public override bool IsQuartzTrigger()
        {
            return true;
        }
    }
}
