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
    public interface IOrderCached
    {
        Task<Entity.Order> GetOrder(string OrderID, bool IgnoreException = true);
        Task<bool> SaveOrder(string OrderID, Entity.Order Data, TimeSpan? ExpireTime = null, bool IgnoreException = true);
        Task<bool> SaveString(string StringData, TimeSpan? ExpireTime = null, bool IgnoreException = true);
        Task<string> GetString(string StringData, bool IgnoreException = true);
        Task<Boolean> DeleteOrder(string OrderID, bool IgnoreException = true);
    }
    public class OrderCached : IOrderCached
    {
        public async Task<bool> SaveString(string StringData, TimeSpan? ExpireTime = null, bool IgnoreException = true)
        {
            try
            {
                await Redis.SetRecordAsync<string>(StringData, StringData, ExpireTime);
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
        public async Task<string> GetString(string StringData, bool IgnoreException = true)
        {
            try
            {
                string data = await Redis.GetRecordAsync<string>(StringData);
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
        public async Task<Entity.Order> GetOrder(string OrderID, bool IgnoreException = true)
        {
            try
            {
                Entity.Order data = await Redis.GetRecordAsync<Entity.Order>(OrderID);
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
        public async Task<Boolean> DeleteOrder(string OrderID, bool IgnoreException = true)
        {
            try
            {
                await Redis.DeleteRecordAsync(OrderID);
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
        public async Task<bool> SaveOrder(string OrderID, Entity.Order Data, TimeSpan? ExpireTime = null, bool IgnoreException = true)
        {
            try
            {
                await Redis.SetRecordAsync<Entity.Order>(OrderID, Data, ExpireTime);
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
