using Quartz;
using System;
using System.Threading.Tasks;
using WIK.ServiceOrderMES.Config;

namespace WIK.ServiceOrderMES.Job
{
    public class OrderJob : IJob
    {
        private readonly UseCase.IOrder _usecase;
        public OrderJob(UseCase.IOrder usecase)
        {
            _usecase = usecase ?? throw new ArgumentNullException(nameof(usecase));
        }
        async Task IJob.Execute(IJobExecutionContext context)
        {
            #if DEBUG
                await Console.Out.WriteLineAsync("Order Job Called!");
            #endif
            await _usecase.MainLogic(";", $"{AppSettings.OrderFolder}\\{AppSettings.OrderFile}");
        }
    }
}
