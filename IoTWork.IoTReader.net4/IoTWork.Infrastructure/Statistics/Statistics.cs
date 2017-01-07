using IoTWork.Protocol.Devices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.Infrastructure.Statistics
{
    public class StatisticsItem
    {
        internal String Module;
        internal String Name;
        internal String Value;
        internal NoteDomain Domain;

        public String GetModule()
        {
            return Module;
        }

        public String GetName()
        {
            return Name;
        }

        public String GetValue()
        {
            return Value;
        }

        public NoteDomain GetDomain()
        {
            return Domain;
        }
    }

    public class Statistics : IEnumerable<StatisticsItem>
    {
        private List<StatisticsItem> _items;

        public Statistics()
        {
            _items = new List<StatisticsItem>();
        }

        public IEnumerator<StatisticsItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Add(string Module, string Name, string Value, NoteDomain Domain)
        {
            StatisticsItem item = new StatisticsItem();
            item.Module = Module;
            item.Name = Name;
            item.Value = Value;
            item.Domain = Domain;
            _items.Add(item);
        }

        public void Add(Statistics Items)
        {
            foreach(var it in Items)
            {
                _items.Add(it);
            }
        }
    }
}
