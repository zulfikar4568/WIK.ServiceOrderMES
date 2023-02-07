using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIK.ServiceOrderMES.Config;

namespace WIK.ServiceOrderMES.Job
{
    public class OrderBOMFailedJob : IJob
    {
        private readonly UseCase.IOrderBOMFailed _usecase;
        public OrderBOMFailedJob(UseCase.IOrderBOMFailed usecase)
        {
            _usecase = usecase ?? throw new ArgumentNullException(nameof(usecase));
        }
        async Task IJob.Execute(IJobExecutionContext context)
        {
            #if DEBUG
                await Console.Out.WriteLineAsync("Order BOM Failed Job Called!");
            #endif
            await _usecase.MainLogic(";", $"{AppSettings.OrderFolder}\\{AppSettings.OrderFile}");
        }
    }
}
