using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIK.ServiceOrderMES.Config;

namespace WIK.ServiceOrderMES.Driver.FileWatcherInstance
{
    class OrderFileWatcherInstance : BaseFileWatcherInstance
    {
        public OrderFileWatcherInstance(string path)
        {
            Instance = new FileSystemWatcher(path);
            Instance.Filter = AppSettings.OrderBOMFile;
        }
    }
}
