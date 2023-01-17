using Quartz;
using System;
using System.Threading.Tasks;
using WIK.ServiceOrderMES.Config;

namespace WIK.ServiceOrderMES.Job
{
    public class OrderBOMJob : IJob
    {
        private readonly UseCase.IOrderBOM _usecase;
        public OrderBOMJob(UseCase.IOrderBOM usecase)
        {
            _usecase = usecase ?? throw new ArgumentNullException(nameof(usecase));
        }
        async Task IJob.Execute(IJobExecutionContext context)
        {
            #if DEBUG
                await Console.Out.WriteLineAsync("Order BOM Job Called!");
            #endif
            await _usecase.MainLogic(";", $"{AppSettings.OrderBOMFolder}\\{AppSettings.OrderBOMFile}");
        }
    }
}
