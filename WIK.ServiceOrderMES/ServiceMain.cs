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
    public class ServiceMain
    {
        private readonly Driver.IFileWatcher<UseCase.IOrder, Driver.FileWatcherInstance.OrderFileWatcherInstance> _watcherOrder;
        private readonly Driver.IFileWatcher<UseCase.IOrderBOM, Driver.FileWatcherInstance.OrderBOMFileWatcherInstance> _watcherOrderBOM;
        public ServiceMain(Driver.IFileWatcher<UseCase.IOrder, Driver.FileWatcherInstance.OrderFileWatcherInstance> watcherOrder, Driver.IFileWatcher<UseCase.IOrderBOM, Driver.FileWatcherInstance.OrderBOMFileWatcherInstance> watcherOrderBOM)
        {
            _watcherOrder = watcherOrder;
            _watcherOrderBOM = watcherOrderBOM;
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
