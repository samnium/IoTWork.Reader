using IoTWork.IoTReader.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using IoTWork.IoTReader.Management;
using System.Diagnostics;
using IoTWork.Contracts;
using IoTWork.Infrastructure;
using System.Threading;

namespace IoTWork.IoTReader.Management
{
    //[PersistJobDataAfterExecution]
    //[DisallowConcurrentExecution]
    //public class ActionJob : IJob
    //{
    //    public void Execute(IJobExecutionContext context)
    //    {
    //        var a = 1;
    //    }
    //}


    internal class SensorJob
    {
        private object _sensorLocker;

        private Context _context;

        private Interfaces.ISensor _sensor;
        private Interfaces.ITrigger _trigger;

        private Task _thread;
        private Boolean _mustrun;
        private Boolean _paused;

        public SensorJob()
        {
            _thread = new Task(() => Execute());
            _mustrun = false;
            _paused = false;
        }

        public SensorJob(Context context, object SensorLocker) : this()
        {
            _context = context;
            _sensorLocker = SensorLocker;
        }

        public void Execute()
        {
            Int32 SequenceNumber = 0;
            Double MinDuration = Double.MaxValue;
            Double MaxDuration = Double.MinValue;
            Double MeanDuration = 0;
            DateTime? FirstTriggeredOn = null;
            DateTime LastTriggeredOn = DateTime.Now;

            DateTime triggerTimer = DateTime.Now;

            while (_mustrun)
            {
                try
                {
                    if (!_paused && ((DateTime.Now - triggerTimer).TotalMilliseconds > _trigger.WithIntervalInMilliseconds))
                    {
                        Stopwatch sw = new Stopwatch();

                        SequenceNumber++;

                        if (!FirstTriggeredOn.HasValue)
                            FirstTriggeredOn = DateTime.Now;

                        LastTriggeredOn = DateTime.Now;

                        ISensor sensor = null;
                        IIoTSample sensorSample = null;

                        // Lookup for the sensor
                        try
                        {
                            LogManager.Debug("SensorJob ... mapping");

                            sensor = SensorArchive.map[_sensor.UniqueName];
                        }
                        catch (Exception ex)
                        {

                        }

                        // Acquire
                        sw.Start();
                        if (sensor != null)
                        {

                            try
                            {
                                LogManager.Debug("SensorJob {0} is {1}", sensor.UniqueName, sensor.GetType());

                                sensorSample = sensor.Acquire(_sensorLocker);

                                LogManager.Debug("Acquired {0} at {1}", sensor.UniqueName, DateTime.Now);
                            }
                            catch (Exception ex)
                            {
                                LogManager.Error("SensorJob Execute Error: " + ex.Message, ex);
                                sensor.RegisterException(_sensor.UniqueName, DateTime.Now, SequenceNumber, ex);
                            }
                        }
                        sw.Stop();

                        // Inject inside the chain
                        if (sensorSample != null)
                        {
                            try
                            {
                                LogManager.Debug("SensorJob {0} ... injecting", sensor.UniqueName);

                                ISample sample = new Sample(sensor, sensorSample);

                                sensor.Inject(sample);
                            }
                            catch (Exception ex)
                            {

                            }
                        }

                        var elapsed = sw.ElapsedMilliseconds;

                        if (elapsed > MaxDuration)
                            MaxDuration = elapsed;

                        if (elapsed < MinDuration)
                            MinDuration = elapsed;

                        if (SequenceNumber == 1)
                            MeanDuration = elapsed;
                        else
                            MeanDuration = ((MeanDuration * (SequenceNumber - 1)) + elapsed) / SequenceNumber;

                        sensor.RegisterStatistics(SequenceNumber,FirstTriggeredOn.Value,
                            LastTriggeredOn, MinDuration, MaxDuration, MeanDuration);

                        triggerTimer = DateTime.Now;
                    }
                    else
                    {
                        Thread.Sleep((int)(_trigger.WithIntervalInMilliseconds / 10));
                    }

                }
                catch (Exception ex)
                {
                    LogManager.Error("SensorJob Execute Error: " + ex.Message, ex);
                }
            }

        }
        
        internal void SetSensor(Interfaces.ISensor sensor)
        {
            this._sensor = sensor;
            this._trigger = sensor.GetTrigger();
        }

        internal void Schedule()
        {
            //if (_trigger != null && _sensor != null)
            //{
            //    if (_trigger.IsQuartzTrigger())
            //    {
            //        job = JobBuilder.Create<SensorJob>()
            //            .UsingJobData(QuartzJobData.SensorUniqueName, _sensor.UniqueName)
            //            .WithIdentity(_sensor.UniqueName, "Sensors").Build();

            //        _context.quartz_scheduler.ScheduleJob(job, _trigger.GetQuartzTrigger());
            //    }
            //}

            if (_trigger != null && _sensor != null)
            {
                _paused = false;
                _mustrun = true;
                _thread.Start();
            }

        }

        internal void Pause()
        {
            //_context.quartz_scheduler.PauseJob(job.Key);
            _paused = true;
        }

        internal void Resume()
        {
            //_context.quartz_scheduler.ResumeJob(job.Key);
            _paused = false;
        }

        internal void Stop()
        {
            _mustrun = false;
            while (!_thread.Wait(100)) Thread.Sleep(100);
            //_context.quartz_scheduler.DeleteJob(job.Key);
        }
    }
}
