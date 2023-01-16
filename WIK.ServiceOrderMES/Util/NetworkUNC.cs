using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIK.ServiceOrderMES.Config;

namespace WIK.ServiceOrderMES.Util
{
    public static class NetworkUNC
    {
        public static void Connect()
        {
            if (AppSettings.UNCPathUsername != "" && AppSettings.UNCPathPassword != "")
            {
                NetUtil.Connect(AppSettings.UNCPath, AppSettings.UNCPathUsername, AppSettings.UNCPathPassword);
            }
            if (AppSettings.OrderFolder != "")
            {
                if (!Directory.Exists(AppSettings.OrderFolder)) Directory.CreateDirectory(AppSettings.OrderFolder);
                if (!Directory.Exists(AppSettings.OrderBOMFolder)) Directory.CreateDirectory(AppSettings.OrderBOMFolder);
            }
        }
        public static void Disconnect()
        {
            if (AppSettings.OrderFolder != "" || AppSettings.OrderBOMFolder != "")
            {
                NetUtil.Disconnect(AppSettings.UNCPath);
            }
        }
    }
}
