using IoTWork.IoTReader.DataModel;
using IoTWork.IoTReader.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.IoTReader.Interfaces
{
    internal interface IJunction<T>
    {
        void AttachSource(IJunctionPoint<T> source, LinkPriority priority = LinkPriority.low);

        void AttachDestination(IJunctionPoint<T> destination);

        void Enqueue(IJunctionPoint<T> source, T sample);

        bool TryDequeue(out T sample);
    }
}
