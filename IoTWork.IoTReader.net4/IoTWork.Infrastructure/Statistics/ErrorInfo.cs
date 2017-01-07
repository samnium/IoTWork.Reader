using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTWork.Infrastructure.Statistics
{
    public class ErrorItem
    {
        public DateTime Timestamp { get; set; }

        public string Module { get; internal set; }

        public String Message { get; set; }

        public Exception Exception { get; set;  }

        public DateTime GetWhen()
        {
            return Timestamp;
        }

        public string GetModule()
        {
            return Module;
        }

        public string GetMessage()
        {
            return Message;
        }

        public Exception GetValue()
        {
            return Exception;
        }
    }

    public class ErrorResume: IEnumerable<ErrorItem>
    {
        List<ErrorItem> _items;

        public ErrorResume()
        {
            _items = new List<ErrorItem>();
        }

        public int Count
        {
            get
            {
                return _items.Count;
            }
        }

        public IEnumerator<ErrorItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Add(DateTime Timestamp, String Module, String Message, Exception Exception)
        {
            ErrorItem item = new ErrorItem();
            item.Timestamp = Timestamp;
            item.Module = Module;
            item.Message = Message;
            item.Exception = Exception;
            _items.Add(item);
        }

        public void Add(ErrorResume resume)
        {
            foreach(var r in resume)
            {
                _items.Add(r);
            }
        }

        public void Add(DateTime dateTime)
        {
            throw new NotImplementedException();
        }
    }
}
