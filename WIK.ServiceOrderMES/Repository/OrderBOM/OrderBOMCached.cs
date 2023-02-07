using Camstar.WCF.ObjectStack;
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
        Task<bool> SaveOrderBOM(string OrderID, Entity.OrderBOM Data, TimeSpan? ExpireTime = null, bool IgnoreException = true);
        Task<MfgOrderChanges> GetMfgOrder(string ProductionOrder, bool IgnoreException = true);
        Task<bool> SaveMfgOrder(string ProductionOrder, MfgOrderChanges Data, TimeSpan? ExpireTime = null, bool IgnoreException = true);
        Task<List<Entity.OrderBOM>> GetOrderBOMInfoPattern(string pattern, bool IgnoreException = true);
        Task<ERPRouteChanges> GetERPRoute(string ERPRoute, string ERPRouteRevision = "", bool IgnoreException = true);
        Task<bool> SaveERPRoute(string ERPRouteName, ERPRouteChanges Data, string ERPRouteRevision = "", TimeSpan? ExpireTime = null, bool IgnoreException = true);
        Task<WorkflowChanges> GetWorkflow(string Workflow, string WorkflowRevision = "", bool IgnoreException = true);
        Task<bool> SaveWorkflow(string Workflow, WorkflowChanges Data, string WorkflowRevision = "", TimeSpan? ExpireTime = null, bool IgnoreException = true);
        Task<ProductChanges> GetProduct(string Product, string ProductRevision = "", bool IgnoreException = true);
        Task<bool> SaveProduct(string Product, ProductChanges Data, string ProductRevision = "", TimeSpan? ExpireTime = null, bool IgnoreException = true);
        Task<List<Entity.OrderBOMSaved>> GetMfgOrderFailList(string pattern, bool IgnoreException = true);
        Task<bool> SaveMfgOrderFail(string ProductionOrder, Entity.OrderBOMSaved Data, TimeSpan? ExpireTime = null, bool IgnoreException = true);
        Task<bool> DeleteMfgOrderFail(string OrderID, bool IgnoreException = true);
    }
    public class OrderBOMCached : IOrderBOMCached
    {
        public async Task<ProductChanges> GetProduct(string Product, string ProductRevision = "", bool IgnoreException = true)
        {
            try
            {
                return await Redis.GetRecordAsync<ProductChanges>($"PRD{Product}{ProductRevision}");
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
                if (!IgnoreException) throw ex;
                return null;
            }
        }
        public async Task<bool> SaveProduct(string Product, ProductChanges Data, string ProductRevision = "", TimeSpan? ExpireTime = null, bool IgnoreException = true)
        {
            try
            {
                await Redis.SetRecordAsync<ProductChanges>($"PRD{Product}{ProductRevision}", Data, ExpireTime);
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
        public async Task<WorkflowChanges> GetWorkflow(string Workflow, string WorkflowRevision = "", bool IgnoreException = true)
        {
            try
            {
                return await Redis.GetRecordAsync<WorkflowChanges>($"WFL{Workflow}{WorkflowRevision}");
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
                if (!IgnoreException) throw ex;
                return null;
            }
        }
        public async Task<bool> SaveWorkflow(string Workflow, WorkflowChanges Data, string WorkflowRevision = "", TimeSpan? ExpireTime = null, bool IgnoreException = true)
        {
            try
            {
                await Redis.SetRecordAsync<WorkflowChanges>($"WFL{Workflow}{WorkflowRevision}", Data, ExpireTime);
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
        public async Task<ERPRouteChanges> GetERPRoute(string ERPRoute, string ERPRouteRevision = "", bool IgnoreException = true)
        {
            try
            {
                return await Redis.GetRecordAsync<ERPRouteChanges>($"ERPR{ERPRoute}{ERPRouteRevision}");
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
                if (!IgnoreException) throw ex;
                return null;
            }
        }
        public async Task<bool> SaveERPRoute(string ERPRouteName, ERPRouteChanges Data, string ERPRouteRevision = "", TimeSpan? ExpireTime = null, bool IgnoreException = true)
        {
            try
            {
                await Redis.SetRecordAsync<ERPRouteChanges>($"ERPR{ERPRouteName}{ERPRouteRevision}", Data, ExpireTime);
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
        public async Task<MfgOrderChanges> GetMfgOrder(string ProductionOrder, bool IgnoreException = true)
        {
            try
            {
                return await Redis.GetRecordAsync<MfgOrderChanges>($"PO{ProductionOrder}");
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
                if (!IgnoreException) throw ex;
                return null;
            }
        }
        public async Task<bool> SaveMfgOrder(string ProductionOrder, MfgOrderChanges Data, TimeSpan? ExpireTime = null, bool IgnoreException = true)
        {
            try
            {
                await Redis.SetRecordAsync<MfgOrderChanges>($"PO{ProductionOrder}", Data, ExpireTime);
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
        public async Task<bool> SaveOrderBOM(string OrderID, Entity.OrderBOM Data, TimeSpan? ExpireTime = null, bool IgnoreException = true)
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

        public async Task<List<Entity.OrderBOM>> GetOrderBOMInfoPattern(string pattern, bool IgnoreException = true)
        {
            List<Entity.OrderBOM> dataFounded = new List<Entity.OrderBOM>();
            try
            {
                var dataKeys = await Redis.GetRecordKeysPattern(pattern);
                foreach (var key in dataKeys)
                {
                    Entity.OrderBOM searchData = await Redis.GetRecordAsync<Entity.OrderBOM>(key);
                    if (searchData != null) dataFounded.Add(searchData);
                }
                return dataFounded;
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
                if (!IgnoreException) throw ex;
                return null;
            }
        }
        public async Task<bool> SaveMfgOrderFail(string ProductionOrder, Entity.OrderBOMSaved Data, TimeSpan? ExpireTime = null, bool IgnoreException = true)
        {
            try
            {
                await Redis.SetRecordAsync<Entity.OrderBOMSaved>($"PO_FAIL_{ProductionOrder}", Data, ExpireTime);
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
        public async Task<List<Entity.OrderBOMSaved>> GetMfgOrderFailList(string pattern, bool IgnoreException = true)
        {
            List<Entity.OrderBOMSaved> dataFounded = new List<Entity.OrderBOMSaved>();
            try
            {
                var dataKeys = await Redis.GetRecordKeysPattern(pattern);
                foreach (var key in dataKeys)
                {
                    Entity.OrderBOMSaved searchData = await Redis.GetRecordAsync<Entity.OrderBOMSaved>(key);
                    if (searchData != null) dataFounded.Add(searchData);
                }
                return dataFounded;
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
                if (!IgnoreException) throw ex;
                return null;
            }
        }
        public async Task<bool> DeleteMfgOrderFail(string ProductionOrder, bool IgnoreException = true)
        {
            try
            {
                await Redis.DeleteRecordAsync($"PO_FAIL_{ProductionOrder}");
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
