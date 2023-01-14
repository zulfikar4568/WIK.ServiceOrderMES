using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIK.ServiceOrderMES.Config;

namespace WIK.ServiceOrderMES.Driver
{
    public class Driver : Module
    {
        protected override void Load(ContainerBuilder moduleBuilder)
        {
            moduleBuilder.RegisterInstance(new FileWatcherInstance.OrderFileWatcherInstance(AppSettings.OrderFolder)).As<FileWatcherInstance.OrderFileWatcherInstance>();
            moduleBuilder.RegisterInstance(new FileWatcherInstance.OrderBOMFileWatcherInstance(AppSettings.OrderBOMFolder)).As<FileWatcherInstance.OrderBOMFileWatcherInstance>();

            moduleBuilder.RegisterType<FileWatcher<UseCase.IOrder, FileWatcherInstance.OrderFileWatcherInstance>>().As<IFileWatcher<UseCase.IOrder, FileWatcherInstance.OrderFileWatcherInstance>>();
            moduleBuilder.RegisterType<FileWatcher<UseCase.IOrderBOM, FileWatcherInstance.OrderBOMFileWatcherInstance>>().As<IFileWatcher<UseCase.IOrderBOM, FileWatcherInstance.OrderBOMFileWatcherInstance>>();
        }
    }
}
