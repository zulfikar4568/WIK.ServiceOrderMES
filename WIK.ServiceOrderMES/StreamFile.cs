﻿using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WIK.ServiceOrderMES.Config;
using WIK.ServiceOrderMES.Util;

namespace WIK.ServiceOrderMES
{
    public class StreamFile
    {
        private readonly Driver.IFileWatcher<UseCase.IOrder, Driver.FileWatcherInstance.OrderFileWatcherInstance> _watcherOrder;
        private readonly Driver.IFileWatcher<UseCase.IOrderBOM, Driver.FileWatcherInstance.OrderBOMFileWatcherInstance> _watcherOrderBOM;
        public StreamFile()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(new Driver.Driver());
            containerBuilder.RegisterModule(new Repository.Repository());
            containerBuilder.RegisterModule(new UseCase.UseCase());
            containerBuilder.RegisterModule(new Util.Util());

            var container = containerBuilder.Build();
            _watcherOrder = container.Resolve<Driver.IFileWatcher<UseCase.IOrder, Driver.FileWatcherInstance.OrderFileWatcherInstance>>();
            _watcherOrderBOM = container.Resolve<Driver.IFileWatcher<UseCase.IOrderBOM, Driver.FileWatcherInstance.OrderBOMFileWatcherInstance>>();
        }

        public void Start()
        {
            _watcherOrder.Init();
            _watcherOrderBOM.Init();
        }

        public void Stop()
        {
            _watcherOrder.Exit();
            _watcherOrderBOM.Exit();
        }
    }
}
