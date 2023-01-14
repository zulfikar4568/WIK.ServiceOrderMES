using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIK.ServiceOrderMES.Util;

namespace WIK.ServiceOrderMES.Config
{
    public static class AppSettings
    {
        public static string AssemblyName { get; set; }
        #region SOURCE_FOLDER
        public static string OrderFile
        {
            get
            {
                return ConfigurationManager.AppSettings["OrderFile"];
            }
        }
        public static string OrderBOMFile
        {
            get
            {
                return ConfigurationManager.AppSettings["OrderBOMFile"];
            }
        }

        public static string OrderFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["OrderFolder"];
            }
        }
        public static string OrderBOMFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["OrderBOMFolder"];
            }
        }

        #endregion

        #region UNC PATH Folder
        public static string UNCPath
        {
            get
            {
                return ConfigurationManager.AppSettings["UNCPath"];
            }
        }
        internal static string UNCPathUsername
        {
            get
            {
                return ConfigurationManager.AppSettings["UNCPathUsername"];
            }
        }
        internal static string UNCPathPassword
        {
            get
            {
                if (ConfigurationManager.AppSettings["UNCPathPassword"] != "")
                {
                    Simple3Des oSimple3Des = new Simple3Des(ConfigurationManager.AppSettings["ExCorePasswordKey"]);
                    return oSimple3Des.DecryptData(ConfigurationManager.AppSettings["UNCPathPassword"]);
                }
                else
                {
                    return "";
                }
            }
        }
        #endregion
    }
}
