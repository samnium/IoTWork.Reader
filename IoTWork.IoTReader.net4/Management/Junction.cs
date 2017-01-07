using IoTWork.IoTReader.DataModel;
using IoTWork.IoTReader.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.IoTReader.Management
{
    internal class Junction<T>: IJunction<T>
    {
        private SortedList<string, ConcurrentQueue<T>> _queues;
        private SortedList<string, LinkPriority> _sourcesWithPriority;
        private SortedList<LinkPriority, List<string>> _sourcesByPriority;

        private Boolean _hasHighPriority;
        private Boolean _hasMediumPriority;

        private object _HighPriorityLock;
        private object _MediumPriorityLock;

        private int _counter_high;
        private int _counter_medium;
        private int _idx_high;
        private int _idx_medium;

        private int _idx;

        public Junction()
        {
            _queues = new SortedList<string, ConcurrentQueue<T>>();
            _sourcesWithPriority = new SortedList<string, LinkPriority>();
            _sourcesByPriority = new SortedList<LinkPriority, List<string>>();

            _hasHighPriority = false;
            _hasMediumPriority = false;

            _HighPriorityLock = new object();
            _MediumPriorityLock = new object();

            _counter_high = 0;
            _counter_medium = 0;

            _idx_high = 0;
            _idx_medium = 0;

            _idx = 0;
        }

        public void AttachSource(IJunctionPoint<T> source, LinkPriority priority)
        {
            _queues.Add(source.JunctionName, new ConcurrentQueue<T>());

            _sourcesWithPriority.Add(source.JunctionName, priority);

            if (!_sourcesByPriority.ContainsKey(priority))
                _sourcesByPriority.Add(priority, new List<string>());

            _sourcesByPriority[priority].Add(source.JunctionName);

            source.SetJunction(this, JunctionPointDirection.Source);
        }

        public void AttachDestination(IJunctionPoint<T> destination)
        {
            destination.SetJunction(this, JunctionPointDirection.Destination);
        }

        public void Enqueue(IJunctionPoint<T> source, T sample)
        {
            _queues[source.JunctionName].Enqueue(sample);

            if (_sourcesWithPriority[source.JunctionName] == LinkPriority.high)
            {
                lock(_HighPriorityLock)
                {
                    _hasHighPriority = true;
                }
            }
            else if (_sourcesWithPriority[source.JunctionName] == LinkPriority.medium)
            {
                lock (_MediumPriorityLock)
                {
                    _hasMediumPriority = true;
                }
            }
        }

        public bool TryDequeue(out T sample)
        {
            bool dequeued = false;

            sample = default(T);

            Boolean _hasHighPriority_local = false;
            Boolean _hasMediumPriority_local = false;

            // TRY READ FROM HIGH PRIORITY QUEUE

            lock (_HighPriorityLock)
            {
                _hasHighPriority_local = _hasHighPriority;
            }

            if (_hasHighPriority_local && _counter_high < _sourcesByPriority[LinkPriority.high].Count * 3)
            {
                var name_high = _sourcesByPriority[LinkPriority.high].ElementAt(_idx_high);
                var queue_high = _queues[name_high];

                dequeued = queue_high.TryDequeue(out sample);

                _idx_high = (_idx_high + 1) % _sourcesByPriority[LinkPriority.high].Count;
                _counter_high++;
            }

            if (dequeued && sample != null)
                return dequeued;
            else
                dequeued = false;

            // TRY READ FROM MEDIUM PRIORITY QUEUE

            lock (_MediumPriorityLock)
            {
                _hasMediumPriority_local = _hasMediumPriority;
            }

            if (_hasMediumPriority_local && _counter_medium < _sourcesByPriority[LinkPriority.medium].Count * 2)
            {
                var name_medium = _sourcesByPriority[LinkPriority.medium].ElementAt(_idx_medium);
                var queue_medium = _queues[name_medium];

                dequeued = queue_medium.TryDequeue(out sample);

                _idx_medium = (_idx_medium + 1) % _sourcesByPriority[LinkPriority.medium].Count;
                _counter_medium++;
            }

            // TRY READ FROM THE NEXT QUEUE
            if (_queues.Count > 0)
            {
                var queue = _queues.ElementAt(_idx);

                dequeued = queue.Value.TryDequeue(out sample);

                _idx = (_idx + 1) % _queues.Count();
            }

            return dequeued;
        }

    }
}
