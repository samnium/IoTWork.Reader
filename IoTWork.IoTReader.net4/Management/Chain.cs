using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IoTWork.IoTReader.Interfaces;
using System.Collections.Concurrent;
using System.Collections;
using IoTWork.IoTReader.DataModel;
using System.Threading;
using IoTWork.Infrastructure;
using IoTWork.IoTReader.Utils;
using System.Diagnostics;
using IoTWork.Infrastructure.Management;
using IoTWork.Infrastructure.Statistics;
using IoTWork.IoTReader.Helper;
using IotWork.Utils.Helpers;
using IoTWork.Protocol.Devices;
using IoTWork.Contracts;

namespace IoTWork.IoTReader.Management
{
    internal class Chain: IChain
    {
        internal SortedList<int, IPipe> _pipes;
        internal SortedList<int, int> _pipesSequenceNumber;
        internal SortedList<int, DateTime> _pipesFirstTriggeredOn;
        internal SortedList<int, DateTime> _pipesLastTriggeredOn;
        internal SortedList<int, Double> _pipesMaxTime;
        internal SortedList<int, Double> _pipesMinTime;
        internal SortedList<int, Double> _pipesMeanTime;

        internal IJunction<ISample> _junctionAsSource;
        internal IJunction<ISample> _junctionAsDestination;

        internal Task _thread;
        internal Boolean _running;

        private Journal<ExceptionContainer> _exceptions;
        private Dictionary<string, string> _statistics;
        internal SortedList<int, Dictionary<string, string>> _statisticsForPipes;

        private object _exlock;
        private object _stlock;

        public ushort UniqueId
        {
            get;
            set;
        }

        public String RealUniqueName
        {
            get;
            set;
        }

        public string JunctionName
        {
            get;
            set;
        }

        public string UniqueName
        {
            get;
            set;
        }

        public string Priority
        {
            get;
            set;
        }

        public LinkPriority LinkPriority
        {
            get;
            set;
        }

        public Chain()
        {
            _pipes = new SortedList<int, IPipe>();

            _pipesSequenceNumber = new SortedList<int, int>();
            _pipesFirstTriggeredOn = new SortedList<int, DateTime>();
            _pipesLastTriggeredOn = new SortedList<int, DateTime>();
            _pipesMaxTime = new SortedList<int, Double>();
            _pipesMinTime = new SortedList<int, Double>();
            _pipesMeanTime = new SortedList<int, Double>();

            _thread = new Task(() => _ChainRun());

            _exceptions = new Journal<ExceptionContainer>(200);
            _statistics = new Dictionary<string, string>();
            _statisticsForPipes = new SortedList<int, Dictionary<string, string>>();

            _exlock = new object();
            _stlock = new object();
        }

        public void Mount(IChainDefinition cd)
        {
            this.UniqueId = cd.UniqueId;
            this.UniqueName = cd.UniqueName;
            this.Priority = String.IsNullOrEmpty(cd.Priority) ? "low" : cd.Priority;
            this.JunctionName = cd.UniqueName;
            this.LinkPriority = (LinkPriority)Enum.Parse(typeof(LinkPriority), this.Priority.ToLower());
        }

        public void Build()
        {
        }

        public void Open()
        {
            _running = true;
            _thread.Start();
        }

        public void Close()
        {
            _running = false;
            while (!_thread.Wait(100)) ;
        }

        public void Attach(IPipe pipe)
        {
            _pipes.Add(pipe.Stage, pipe);

            _pipesSequenceNumber.Add(pipe.Stage, 0);
            _pipesFirstTriggeredOn.Add(pipe.Stage, DateTime.MinValue);
            _pipesLastTriggeredOn.Add(pipe.Stage, DateTime.MinValue);
            _pipesMaxTime.Add(pipe.Stage, Double.MinValue);
            _pipesMinTime.Add(pipe.Stage, Double.MaxValue);
            _pipesMeanTime.Add(pipe.Stage, 0);

            if (!_statisticsForPipes.ContainsKey(pipe.Stage))
                _statisticsForPipes.Add(pipe.Stage, new Dictionary<string, string>());
        }

        public void Attach(IPipeDefinition pipe)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IPipeDefinition> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator<IPipe> IEnumerable<IPipe>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void SetJunction(IJunction<ISample> junction, JunctionPointDirection Direction)
        {
            if (Direction == JunctionPointDirection.Source)
                _junctionAsSource = junction;
            else
                _junctionAsDestination = junction;
        }

        public void SetSensor(ISensor sensor)
        {
            this.JunctionName = this.UniqueName + "_onsensor_" + sensor.UniqueName;
            this.RealUniqueName = this.UniqueName + "_onsensor_" + sensor.UniqueName;
        }

        public Infrastructure.Statistics.Statistics GetStatistics()
        {
            Infrastructure.Statistics.Statistics statistics = new Infrastructure.Statistics.Statistics();

            foreach (var kv in _pipes)
            {
                var pipeUniqueName = UniqueName + "." + kv.Key + "p";
                var moduleName = "Pipe " + pipeUniqueName;

                lock (_stlock)
                {
                    statistics.Add(moduleName, "SamplingSequenceNumber", _statisticsForPipes[kv.Key]["SamplingSequenceNumber"], NoteDomain.Chain);
                    statistics.Add(moduleName, "FirstTriggeredOn", _statisticsForPipes[kv.Key]["FirstTriggeredOn"], NoteDomain.Chain);
                    statistics.Add(moduleName, "LastTriggeredOn", _statisticsForPipes[kv.Key]["LastTriggeredOn"], NoteDomain.Chain);
                    statistics.Add(moduleName, "MinDuration", _statisticsForPipes[kv.Key]["MinDuration"], NoteDomain.Chain);
                    statistics.Add(moduleName, "MaxDuration", _statisticsForPipes[kv.Key]["MaxDuration"], NoteDomain.Chain);
                    statistics.Add(moduleName, "MeanDuration", _statisticsForPipes[kv.Key]["MeanDuration"], NoteDomain.Chain);
                }
            }

            lock(_stlock)
            {
                statistics.Add("Chain " + UniqueName, "SamplingSequenceNumber", _statistics["SamplingSequenceNumber"], NoteDomain.Chain);
                statistics.Add("Chain " + UniqueName, "FirstTriggeredOn", _statistics["FirstTriggeredOn"], NoteDomain.Chain);
                statistics.Add("Chain " + UniqueName, "LastTriggeredOn", _statistics["LastTriggeredOn"], NoteDomain.Chain);
                statistics.Add("Chain " + UniqueName, "MinDuration", _statistics["MinDuration"], NoteDomain.Chain);
                statistics.Add("Chain " + UniqueName, "MaxDuration", _statistics["MaxDuration"], NoteDomain.Chain);
                statistics.Add("Chain " + UniqueName, "MeanDuration", _statistics["MeanDuration"], NoteDomain.Chain);
            }

            return statistics;
        }

        #region The CHAIN implementation

        public void _ChainRun()
        {
            Int32 SequenceNumber = 0;
            DateTime? FirstTriggeredOn = null;
            DateTime LastTriggeredOn = DateTime.Now;
            Double MinDuration = Double.MaxValue;
            Double MaxDuration = Double.MinValue;
            Double MeanDuration = 0;

            while (_running)
            {
                ISample sample = null;

                try
                {
                    var dequeued = _junctionAsDestination.TryDequeue(out sample);
                }
                catch(Exception ex)
                {
                    sample = null;

                    LogManager.Error(LogFormat.Format("Chain {0} error in dequeuing.", UniqueName), ex);
                }

                try
                {
                    if (sample != null)
                    {
                        SequenceNumber++;

                        if (!FirstTriggeredOn.HasValue)
                            FirstTriggeredOn = DateTime.Now;
                        LastTriggeredOn = DateTime.Now;

                        Stopwatch chainsw = new Stopwatch();
                        chainsw.Start();

                        Boolean reachedTheEndOfTheChain = true;

                        foreach (var kv in _pipes)
                        {
                            _pipesSequenceNumber[kv.Key]++;
                            if (_pipesSequenceNumber[kv.Key] == 1)
                            {
                                _pipesFirstTriggeredOn[kv.Key] = DateTime.Now;
                            }
                            _pipesLastTriggeredOn[kv.Key] = DateTime.Now;


                            var pipe = kv.Value;
                            var pipeUniqueName = UniqueName + "." + kv.Key + "p";

                            IIoTSample newsample = null;
                            Exception pipeex = null;

                            sample.StartTraversingPipe(pipe.Stage);

                            Stopwatch sw = new Stopwatch();

                            sw.Start();

                            try
                            {
                                newsample = pipe.CrossIt(sample.CurrentSample);
                            }
                            catch (Exception ex)
                            {
                                pipeex = ex;

                                LogManager.Error(LogFormat.Format("Pipe error at {0} : {1}", pipeUniqueName, ex.Message), ex);

                                RegisterException("Pipe", pipeUniqueName, DateTime.Now, _pipesSequenceNumber[kv.Key], ex);
                            }

                            sw.Stop();

                            sample.EndTraversingPipe(pipe.Stage, pipeex);

                            var elapsed = sw.ElapsedMilliseconds;

                            if (elapsed > _pipesMaxTime[kv.Key])
                                _pipesMaxTime[kv.Key] = elapsed;

                            if (elapsed < _pipesMinTime[kv.Key])
                                _pipesMinTime[kv.Key] = elapsed;

                            if (_pipesSequenceNumber[kv.Key] == 1)
                                _pipesMeanTime[kv.Key] = elapsed;
                            else
                                _pipesMeanTime[kv.Key] = ((_pipesMeanTime[kv.Key] * (_pipesSequenceNumber[kv.Key] - 1)) + elapsed) / _pipesSequenceNumber[kv.Key];

                            if (pipeex != null)
                            {
                                reachedTheEndOfTheChain = false;
                                break;
                            }

                            sample.UpdateValue(newsample);

                            RegisterPipeStatistics(kv.Key, _pipesSequenceNumber[kv.Key], _pipesFirstTriggeredOn[kv.Key],
                                _pipesLastTriggeredOn[kv.Key], _pipesMinTime[kv.Key], _pipesMaxTime[kv.Key], _pipesMeanTime[kv.Key]);

                            Thread.Sleep(50);
                        }

                        if (reachedTheEndOfTheChain)
                        {
                            _junctionAsSource.Enqueue(this, sample);

                            LogManager.Info(LogFormat.Format("Feed sensor {0} on chain {1}", sample.Source, UniqueName));
                        }

                        chainsw.Stop();

                        var chainelapsed = chainsw.ElapsedMilliseconds;

                        if (chainelapsed > MaxDuration)
                            MaxDuration = chainelapsed;

                        if (chainelapsed < MinDuration)
                            MinDuration = chainelapsed;

                        if (SequenceNumber == 1)
                            MeanDuration = chainelapsed;
                        else
                            MeanDuration = ((MeanDuration * (SequenceNumber - 1)) + chainelapsed) / SequenceNumber;

                        RegisterStatistics(SequenceNumber, FirstTriggeredOn.Value, LastTriggeredOn, MinDuration, MaxDuration, MeanDuration);

                        Thread.Sleep(50);
                    }
                    else
                        Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    LogManager.Error(LogFormat.Format("Chain error at {0} : {1}", UniqueName, ex.Message), ex);

                    RegisterException("Pipe", UniqueName, DateTime.Now, SequenceNumber, ex);

                    Thread.Sleep(50);
                }
            }
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

        public bool IsEmpty()
        {
            return true;
        }

        private void RegisterException(String Module, String UniqueName, DateTime When, Int32 SequenceNumber, Exception Exception)
        {
            ExceptionContainer ec = new ExceptionContainer();
            ec.UniqueName = UniqueName;
            ec.When = When;
            ec.Module = Module;
            ec.Order = SequenceNumber;
            ec.ex = Exception;

            lock (_exlock)
            {
                _exceptions.Add(ec);
            }
        }

        public void RegisterPipeStatistics(int Stage, int SequenceNumber, DateTime FirstTriggeredOn, DateTime LastTriggeredOn, Double MinDuration, Double MaxDuration, Double MeanDuration)
        {
            lock (_stlock)
            {
                if (!_statisticsForPipes[Stage].ContainsKey("SamplingSequenceNumber"))
                    _statisticsForPipes[Stage].Add("SamplingSequenceNumber", SequenceNumber.ToString());
                else
                    _statisticsForPipes[Stage]["SamplingSequenceNumber"] = SequenceNumber.ToString();

                if (!_statisticsForPipes[Stage].ContainsKey("FirstTriggeredOn"))
                    _statisticsForPipes[Stage].Add("FirstTriggeredOn", FirstTriggeredOn.ToString());
                else
                    _statisticsForPipes[Stage]["FirstTriggeredOn"] = FirstTriggeredOn.ToString();

                if (!_statisticsForPipes[Stage].ContainsKey("LastTriggeredOn"))
                    _statisticsForPipes[Stage].Add("LastTriggeredOn", LastTriggeredOn.ToString());
                else
                    _statisticsForPipes[Stage]["LastTriggeredOn"] = LastTriggeredOn.ToString();

                if (!_statisticsForPipes[Stage].ContainsKey("MinDuration"))
                    _statisticsForPipes[Stage].Add("MinDuration", MinDuration.ToString("F2"));
                else
                    _statisticsForPipes[Stage]["MinDuration"] = MinDuration.ToString("F2");

                if (!_statisticsForPipes[Stage].ContainsKey("MaxDuration"))
                    _statisticsForPipes[Stage].Add("MaxDuration", MaxDuration.ToString("F2"));
                else
                    _statisticsForPipes[Stage]["MaxDuration"] = MaxDuration.ToString("F2");

                if (!_statisticsForPipes[Stage].ContainsKey("MeanDuration"))
                    _statisticsForPipes[Stage].Add("MeanDuration", MeanDuration.ToString("F2"));
                else
                    _statisticsForPipes[Stage]["MeanDuration"] = MeanDuration.ToString("F2");
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

        #endregion
    }
}
