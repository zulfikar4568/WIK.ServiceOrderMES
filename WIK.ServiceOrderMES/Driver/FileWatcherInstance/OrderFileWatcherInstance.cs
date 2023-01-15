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
    class OrderFileWatcherInstance : BaseFileWatcherInstance
    {
        public OrderFileWatcherInstance(NetworkUNC network)
        {
            try
            {
                network.Connect();
            }
            catch (Exception ex)
            {
                EventLogUtil.LogErrorEvent(AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source, ex);
            }
            Instance = new FileSystemWatcher(AppSettings.OrderFolder);
            Instance.Filter = AppSettings.OrderFile;
        }
    }
}
