using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WIK.ServiceOrderMES.Config;
using WIK.ServiceOrderMES.Util;

namespace WIK.ServiceOrderMES
{
    public class ServiceMain
    {
        private readonly Driver.IFileWatcher<UseCase.IOrder, Driver.FileWatcherInstance.OrderFileWatcherInstance> _watcherOrder;
        private readonly Driver.IFileWatcher<UseCase.IOrderBOM, Driver.FileWatcherInstance.OrderBOMFileWatcherInstance> _watcherOrderBOM;
        private readonly IScheduler _scheduler;
        public ServiceMain(Driver.IFileWatcher<UseCase.IOrder, Driver.FileWatcherInstance.OrderFileWatcherInstance> watcherOrder, Driver.IFileWatcher<UseCase.IOrderBOM, Driver.FileWatcherInstance.OrderBOMFileWatcherInstance> watcherOrderBOM, IScheduler scheduler)
        {
            _watcherOrder = watcherOrder;
            _watcherOrderBOM = watcherOrderBOM;
            _scheduler = scheduler;
        }

        public void Start()
        {
            // Start the watcher file
            _watcherOrder.Init();
            _watcherOrderBOM.Init();

            // Start the CronJob
            ScheduleJob();
            _scheduler.Start().ConfigureAwait(true).GetAwaiter().GetResult();
        }

        private void ScheduleJob()
        {
            SchedulerCronJob<Job.OrderJob>(AppSettings.OrderCronExpression);
            SchedulerCronJob<Job.OrderFailedJob>(AppSettings.OrderFailedCronExpression);

            SchedulerCronJob<Job.OrderBOMJob>(AppSettings.OrderBOMCronExpression);
            SchedulerCronJob<Job.OrderBOMFailedJob>(AppSettings.OrderBOMFailedCronExpression);
        }

        private void SchedulerCronJob<T>(string cronExpression) where T : IJob
        {
            var jobName = typeof(T).Name;

            var job = JobBuilder
                .Create<T>()
                .WithIdentity(jobName, $"{jobName}-Group")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"{jobName}-Trigger", $"{jobName}-TriggerGroup")
                .StartNow()
                .WithCronSchedule(cronExpression)
                .Build();

            _scheduler.ScheduleJob(job, trigger).ConfigureAwait(true).GetAwaiter().GetResult();
        }

        public void Stop()
        {
            _watcherOrder.Exit();
            _watcherOrderBOM.Exit();
            _scheduler.Shutdown().ConfigureAwait(true).GetAwaiter().GetResult();
        }
    }
}
