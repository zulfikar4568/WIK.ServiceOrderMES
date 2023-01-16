using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WIK.ServiceOrderMES.Config;
using WIK.ServiceOrderMES.Util;

namespace WIK.ServiceOrderMES.Driver.FileWatcherInstance
{
    public class OrderFileWatcherInstance : BaseFileWatcherInstance
    {
        public OrderFileWatcherInstance()
        {
            Instance = new FileSystemWatcher(AppSettings.OrderFolder);
            Instance.Filter = AppSettings.OrderFile;
        }
    }
}
