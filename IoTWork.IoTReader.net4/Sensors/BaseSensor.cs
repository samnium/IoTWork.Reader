using IoTWork.IoTReader.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IoTWork.Contracts;
using IoTWork.Infrastructure.Management;
using IoTWork.IoTReader.Management;
using IoTWork.IoTReader.Utils;
using IoTWork.Infrastructure.Statistics;
using IoTWork.IoTReader.Helper;
using IotWork.Utils.Helpers;

namespace IoTWork.IoTReader.Sensors
{
    internal class BaseSensor : ISensor
    {
        private ITrigger _trigger;
        private IChain _chain;
        private IJunction<ISample> _junction;

        private Journal<ExceptionContainer> _exceptions;
        private Dictionary<string, string> _statistics;

        private object _exlock;
        private object _stlock;

        public BaseSensor()
        {
            _junction = null;
            _exceptions = new Journal<ExceptionContainer>(100);
            _statistics = new Dictionary<string, string>();

            _exlock = new object();
            _stlock = new object();
        }

        public ushort UniqueId
        {
            get;
            set;
        }

        public string JunctionName
        {
            get;
            set;
        }

        public string ChainUniqueName
        {
            get;
            set;
        }

        public string TriggerUniqueName
        {
            get;
            set;
        }

        public string TypeName
        {
            get;
            set;
        }

        public string UniqueName
        {
            get;
            set;
        }

        public string LibraryPath
        {
            get;
            set;
        }

        public string ClassForSample
        {
            get;
            set;
        }

        public string ClassForAcquire
        {
            get;
            set;
        }

        public string ClassForAcquire_Parameter_Init
        {
            get;
            set;
        }

        public string ClassForAcquire_Parameter_Close
        {
            get;
            set;
        }

        public void Mount(ISensorDefinition sd)
        {
            this.UniqueId = sd.UniqueId;
            this.UniqueName = sd.UniqueName;
            this.JunctionName = sd.UniqueName;
            this.TriggerUniqueName = sd.TriggerUniqueName;
            this.ChainUniqueName = sd.ChainUniqueName;
            this.TypeName = sd.TypeName;
            this.LibraryPath = sd.LibraryPath;
            this.ClassForSample = sd.ClassForSample;
            this.ClassForAcquire = sd.ClassForAcquire;
            this.ClassForAcquire_Parameter_Init = sd.ClassForAcquire_Parameter_Init;
            this.ClassForAcquire_Parameter_Close = sd.ClassForAcquire_Parameter_Close;
        }

        public void RegisterTrigger(ITrigger Trigger)
        {
            _trigger = Trigger;
        }

        public ITrigger GetTrigger()
        {
            return _trigger;
        }

        public void RegisterChain(IChain chain)
        {
            _chain = chain;
        }

        public IChain GetChain()
        {
            return _chain;
        }

        public virtual IIoTSample Acquire(object Locker)
        {
            throw new NotImplementedException();
        }

        public virtual bool Close()
        {
            throw new NotImplementedException();
        }

        public virtual void Build()
        {
            throw new NotImplementedException();
        }

        public virtual bool Stop()
        {
            throw new NotImplementedException();
        }

        public virtual bool Pause()
        {
            throw new NotImplementedException();
        }

        public virtual bool Play()
        {
            throw new NotImplementedException();
        }

        public void SetJunction(IJunction<ISample> junction, JunctionPointDirection Direction)
        {
            if (Direction == JunctionPointDirection.Source)
                _junction = junction;
        }

        public void Inject(ISample data)
        {
            if (_junction != null)
                _junction.Enqueue(this, data);
        }

        public void RegisterException(String UniqueName, DateTime When, Int32 SequenceNumber, Exception Exception)
        {
            ExceptionContainer ec = new ExceptionContainer();
            ec.UniqueName = UniqueName;
            ec.When = When;
            ec.Module = "Sensor";
            ec.Order = SequenceNumber;
            ec.ex = Exception;

            lock (_exlock)
            {
                _exceptions.Add(ec);
            }
        }

        public void RegisterStatistics(int SequenceNumber, DateTime FirstTriggeredOn, DateTime LastTriggeredOn, Double MinDuration, Double MaxDuration, Double MeanDuration)
        {
            lock (_stlock)
            {
                if (!_statistics.ContainsKey("SamplingSequenceNumber"))
                    _statistics.Add("SamplingSequenceNumber", SequenceNumber.ToString());
                else
                    _statistics["SamplingSequenceNumber"] = SequenceNumber.ToString();

                if (!_statistics.ContainsKey("FirstTriggeredOn"))
                    _statistics.Add("FirstTriggeredOn", FirstTriggeredOn.ToString());
                else
                    _statistics["FirstTriggeredOn"] = FirstTriggeredOn.ToString();

                if (!_statistics.ContainsKey("LastTriggeredOn"))
                    _statistics.Add("LastTriggeredOn", LastTriggeredOn.ToString());
                else
                    _statistics["LastTriggeredOn"] = LastTriggeredOn.ToString();

                if (!_statistics.ContainsKey("MinDuration"))
                    _statistics.Add("MinDuration", MinDuration.ToString("F2"));
                else
                    _statistics["MinDuration"] = MinDuration.ToString("F2");

                if (!_statistics.ContainsKey("MaxDuration"))
                    _statistics.Add("MaxDuration", MaxDuration.ToString("F2"));
                else
                    _statistics["MaxDuration"] = MaxDuration.ToString("F2");

                if (!_statistics.ContainsKey("MeanDuration"))
                    _statistics.Add("MeanDuration", MeanDuration.ToString("F2"));
                else
                    _statistics["MeanDuration"] = MeanDuration.ToString("F2");
            }
        }

        public Statistics GetStatistics()
        {
            Statistics statistics = new Statistics();

            lock (_stlock)
            {
                statistics.Add("Sensor " + UniqueName, "SamplingSequenceNumber", _statistics["SamplingSequenceNumber"], Protocol.Devices.NoteDomain.Sensor);
                statistics.Add("Sensor " + UniqueName, "FirstTriggeredOn", _statistics["FirstTriggeredOn"], Protocol.Devices.NoteDomain.Sensor);
                statistics.Add("Sensor " + UniqueName, "LastTriggeredOn", _statistics["LastTriggeredOn"], Protocol.Devices.NoteDomain.Sensor);
                statistics.Add("Sensor " + UniqueName, "MinDuration", _statistics["MinDuration"], Protocol.Devices.NoteDomain.Sensor);
                statistics.Add("Sensor " + UniqueName, "MaxDuration", _statistics["MaxDuration"], Protocol.Devices.NoteDomain.Sensor);
                statistics.Add("Sensor " + UniqueName, "MeanDuration", _statistics["MeanDuration"], Protocol.Devices.NoteDomain.Sensor);
            }

            return statistics;
        }

        public ErrorResume GetErrors()
        {
            ErrorResume _resume = new ErrorResume();

            lock (_exlock)
            {
                _resume = IotWork.Utils.Helpers.IoTWorkHelper.ToErrorResume(_exceptions);
            }

            return _resume;
        }
    }
}
