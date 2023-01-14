using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
