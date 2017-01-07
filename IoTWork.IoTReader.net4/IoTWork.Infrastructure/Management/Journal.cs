using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.Infrastructure.Management
{
    public class JournalItem<T>
    {
        public Double MillisecondsFromBegin { get; set; }

        public Double MillisecondsFromLast { get; set; }

        public DateTime When { get; set; }

        public T Data { get; set; } 
    }    

    public class Journal<T>
    {
        private List<JournalItem<T>> _items;
        private Int32 _size;
        private DateTime _startedOn;
        private DateTime? _lastInserteddOn;


        public Journal(Int32 size)
        {
            _size = size;
            _startedOn = DateTime.Now;
            _lastInserteddOn = null;
            _items = new List<JournalItem<T>>();
        }

        public void Add(T data)
        {
            Double elapsedMilliseconds = (DateTime.Now - _startedOn).TotalMilliseconds;
            Double elapsedFromLastInsertion = 0;
            if (_lastInserteddOn.HasValue)
                elapsedFromLastInsertion = (DateTime.Now - _lastInserteddOn.Value).TotalMilliseconds;
            else
                elapsedFromLastInsertion = elapsedMilliseconds;

            JournalItem<T> item = new JournalItem<T>();
            item.When = DateTime.Now;
            item.MillisecondsFromBegin = elapsedMilliseconds;
            item.MillisecondsFromLast = elapsedFromLastInsertion;
            item.Data = data;

            if (_items.Count == _size)
                _items.RemoveAt(_items.Count - 1);
            _items.Insert(0, item);

            _lastInserteddOn = DateTime.Now;
        }

        public int Count
        {
            get
            {
                return _items.Count;
            }
        }

        public JournalItem<T> GetAt(int i)
        {
            var _item = _items.ElementAt(i);
            _items.RemoveAt(i);
            return _item;
        }
    }
}
