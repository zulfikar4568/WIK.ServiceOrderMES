using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIK.ServiceOrderMES.Config;

namespace WIK.ServiceOrderMES.CronJob
{
    public class OrderJob : IJob
    {
        private readonly UseCase.Order _usecase;
        public OrderJob(UseCase.Order usecase)
        {
            _usecase = usecase;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            await _usecase.MainLogic(";", $"{AppSettings.OrderFolder}\\{AppSettings.OrderFile}");
        }
    }
}
