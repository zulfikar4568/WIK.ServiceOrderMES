using Camstar.WCF.ObjectStack;
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
        public static string AssemblyName
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            }
        }

        #region CRON EXPRESSION
        public static string OrderCronExpression
        {
            get
            {
                return ConfigurationManager.AppSettings["OrderCronExpression"];
            }
        }
        public static string OrderFailedCronExpression
        {
            get
            {
                return ConfigurationManager.AppSettings["OrderFailedCronExpression"];
            }
        }
        public static string OrderBOMCronExpression
        {
            get
            {
                return ConfigurationManager.AppSettings["OrderBOMCronExpression"];
            }
        }
        public static string OrderBOMFailedCronExpression
        {
            get
            {
                return ConfigurationManager.AppSettings["OrderBOMFailedCronExpression"];
            }
        }
        #endregion

        #region CONFIG ACCOUNT ExCore
        public static TimeSpan UTCOffset
        {
            get
            {
                string sUTCOffset = ConfigurationManager.AppSettings["UTCOffset"];
                string[] aUTCOffset = sUTCOffset.Split(':');
                return new TimeSpan(Int32.Parse(aUTCOffset[0]), Int32.Parse(aUTCOffset[1]), Int32.Parse(aUTCOffset[2]));
            }
        }
        private static string ExCoreHost
        {
            get
            {
                return ConfigurationManager.AppSettings["ExCoreHost"];
            }
        }
        private static string ExCorePort
        {
            get
            {
                return ConfigurationManager.AppSettings["ExCorePort"];
            }
        }
        private static string ExCoreUsername
        {
            get
            {
                return ConfigurationManager.AppSettings["ExCoreUsername"];
            }
        }
        private static string ExCorePassword
        {
            get
            {

                Simple3Des oSimple3Des = new Simple3Des(ConfigurationManager.AppSettings["ExCorePasswordKey"]);
                return oSimple3Des.DecryptData(ConfigurationManager.AppSettings["ExCorePassword"]);
            }
        }
        private static UserProfile _ExCoreUserProfile = null;
        public static UserProfile ExCoreUserProfile
        {
            get
            {
                if (_ExCoreUserProfile == null)
                {
                    _ExCoreUserProfile = new UserProfile(ExCoreUsername, ExCorePassword, UTCOffset);
                }
                if (_ExCoreUserProfile.Name != ExCoreUsername || _ExCoreUserProfile.Password.Value != ExCorePassword)
                {
                    _ExCoreUserProfile = new UserProfile(ExCoreUsername, ExCorePassword, UTCOffset);
                }
                return _ExCoreUserProfile;
            }
        }
        #endregion

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

        #region CACHED
        public static string CachedHost
        {
            get
            {
                return ConfigurationManager.AppSettings["CachedHost"];
            }
        }
        public static int CachedExpiration
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["CachedExpiration"]);
            }
        }
        #endregion

        #region ORDER FILTER
        public static string[] FilterOrderTypes
        {
            get
            {
                return ConfigurationManager.AppSettings["FilterOrderType"].Split(',');
            }
        }
        public static string[] FilterMfgLines
        {
            get
            {
                return ConfigurationManager.AppSettings["FilterWorkCenter"].Split(',');
            }
        }
        public static string[] FilterOrderStatus
        {
            get
            {
                return ConfigurationManager.AppSettings["FilterSystemStatus"].Split(',');
            }
        }
        #endregion

        #region ORDER BOM CONFIG
        public static string[] PPAFilter
        {
            get
            {
                return ConfigurationManager.AppSettings["PPAFilter"].Split(',');
            }
        }
        public static string ScanningFilter
        {
            get
            {
                return ConfigurationManager.AppSettings["ScanningFilter"];
            }
        }
        public static string DefaultProductType
        {
            get
            {
                return ConfigurationManager.AppSettings["DefaultProductType"];
            }
        }
        public static string DefaultOperationNumber
        {
            get
            {
                return ConfigurationManager.AppSettings["DefaultOperationNumber"];
            }
        }
        public static string DefaultProductDesc
        {
            get
            {
                return ConfigurationManager.AppSettings["DefaultProductDesc"];
            }
        }
        #endregion
    }
}
