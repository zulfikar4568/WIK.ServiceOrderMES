using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WIK.ServiceOrderMES.Config;
using WIK.ServiceOrderMES.Driver;
using WIK.ServiceOrderMES.Util;

namespace WIK.ServiceOrderMES.Repository
{
    public interface IOrderBOMCached
    {
        Task<Entity.OrderBOM> GetOrderBOM(string OrderID, bool IgnoreException = true);
        Task<bool> SaveOrder(string OrderID, Entity.OrderBOM Data, TimeSpan? ExpireTime = null, bool IgnoreException = true);
    }
    public class OrderBOMCached : IOrderBOMCached
    {
        public async Task<Entity.OrderBOM> GetOrderBOM(string OrderID, bool IgnoreException = true)
        {
            try
            {
                Entity.OrderBOM data = await Redis.GetRecordAsync<Entity.OrderBOM>(OrderID);
                return data;
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
                if (!IgnoreException) throw ex;
                return null;
            }
        }
        public async Task<bool> SaveOrder(string OrderID, Entity.OrderBOM Data, TimeSpan? ExpireTime = null, bool IgnoreException = true)
        {
            try
            {
                await Redis.SetRecordAsync<Entity.OrderBOM>(OrderID, Data, ExpireTime);
                return true;
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
                if (!IgnoreException) throw ex;
                return false;
            }
        }
    }
}
