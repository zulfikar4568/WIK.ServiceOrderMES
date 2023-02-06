using Camstar.WCF.ObjectStack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WIK.ServiceOrderMES.Config;
using WIK.ServiceOrderMES.Util;

namespace WIK.ServiceOrderMES.UseCase
{
    public interface IOrderBOM : Abstraction.IUseCase
    {
        new Task MainLogic(string delimiter, string path);
    }
    public class OrderBOM : IOrderBOM
    {
        private readonly Repository.IOrderBOMCsv _repositoryCsv;
        private readonly Repository.IOrderBOMCached _repositoryCached;
        private readonly Repository.MaintenanceTransaction _repositoryMaintenanceTxn;
        public OrderBOM(Repository.IOrderBOMCsv repositoryCsv, Repository.IOrderBOMCached repositoryCached, Repository.MaintenanceTransaction repositoryMaintenanceTxn)
        {
            _repositoryCsv = repositoryCsv;
            _repositoryCached = repositoryCached;
            _repositoryMaintenanceTxn = repositoryMaintenanceTxn;
        }

        public async Task MainLogic(string delimiter, string path)
        {
            List<Entity.OrderBOM> orderBOMs = _repositoryCsv.Reading(delimiter, path);

            #if DEBUG
                Console.WriteLine("Doing Trim PO");
            #endif
            orderBOMs.ForEach(x => { x.ProductionOrder = x.ProductionOrder.TrimStart('0'); x.MaterialGroup = x.MaterialGroup.TrimStart('0'); });

            #if DEBUG
                Console.WriteLine("Doing Product Type");
            #endif
            CreateDefaultProductType();

            string[] serverMfgOrderList = _repositoryMaintenanceTxn.ListMfgOrderInfo().Where(x => x != null).Select(x => x.Name.ToString()).ToArray();
            string[] mergeMfgOrder = serverMfgOrderList.Intersect(orderBOMs.Select(x => x.ProductionOrder).Distinct().ToList()).ToArray();
            List<MfgOrderChanges> mfgOrders = await FindMfgOrder(mergeMfgOrder);

            // Save All orderBOM in redis
            await MigrateCSVToCached(orderBOMs);

            // Go trough Order that has been merge
            try
            {
                foreach (var mfgOrder in mfgOrders)
                {

                    ERPRouteChanges oERPRoute = await GetERPRouteFromMfgOrder(mfgOrder);
                    if (oERPRoute == null) continue;
                    if (mfgOrder.Qty == null && mfgOrder.Containers != null)
                    {
                        EventLogUtil.LogEvent($"Production or Manufacturing Order: {mfgOrder.Name} can't be used, it might be Production Order have a Container or doesn't have Qty!.\nTry to remove this {mfgOrder.Name} data on material list file.", System.Diagnostics.EventLogEntryType.Warning, 3);
                        continue;
                    }
                    List<Entity.OrderBOM> bomInformations = await _repositoryCached.GetOrderBOMInfoPattern($"POBOM{mfgOrder.Name}*");

                    // Setting Up BOM
                    List<dynamic> materialList = BOMLogic(bomInformations, oERPRoute, mfgOrder);

                    //Create Object to be saved
                    Entity.OrderBOMSaved dataToBeSaved = new Entity.OrderBOMSaved() { MfgOrderName = mfgOrder.Name.ToString(), ERPRouteName = oERPRoute.Name != null ? oERPRoute.Name.Value : "" , MaterialList = materialList};
                    
                    // Transaction MES
                    bool resultMfgOrder = _repositoryMaintenanceTxn.SaveMfgOrder(dataToBeSaved.MfgOrderName, "", "", "", "", "", "", 0, dataToBeSaved.MaterialList, dataToBeSaved.ERPRouteName);
                    #if DEBUG
                        string msg = resultMfgOrder == true ? "Success" : "Failed";
                        Console.WriteLine($"{mfgOrder.Name} result: {msg}");
                    #endif

                    // If Transaction Fail, it must be executed later
                    if (!resultMfgOrder)
                    {
                        await _repositoryCached.SaveMfgOrderFail(dataToBeSaved.MfgOrderName, dataToBeSaved);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogUtil.LogErrorEvent(AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source, ex);
            }
        }

        public List<dynamic> BOMLogic(List<Entity.OrderBOM> orderBOMs, ERPRouteChanges oERPRoute, MfgOrderChanges mfgOrder)
        {
            List<dynamic> MaterialList = new List<dynamic>();
            foreach (var item in orderBOMs)
            {
                //Console.WriteLine($"{item.ProductionOrder} - {item.Material} - {item.MaterialGroup} - {item.Qty} - {item.Scanning}");
                
                string operationNumber = AppSettings.DefaultOperationNumber;
                if (!ConvertToDoubleCommaDecimal(item.Qty, out double number)) continue;
                if (item.Scanning != AppSettings.ScanningFilter) continue;
                if (AppSettings.PPAFilter.Length > 0)
                {
                    foreach (var ppaFilter in AppSettings.PPAFilter)
                    {
                        if (item.MaterialGroup.Contains(ppaFilter))
                        {
                            operationNumber = "2";
                            break;
                        }
                        else
                        {
                            operationNumber = AppSettings.DefaultOperationNumber;
                        }
                    }
                }

                bool ObjectExists = _repositoryMaintenanceTxn.ProductExists(item.Material);
                if (!ObjectExists && item.MaterialGroup != "" && item.Material != "")
                {
                    _repositoryMaintenanceTxn.SaveProductFamily(item.Material);
                    _repositoryMaintenanceTxn.SaveProduct(item.Material, "1", "", AppSettings.DefaultProductDesc, "", AppSettings.DefaultProductType, "", "", "", "", "", item.MaterialGroup);
                }
                if (oERPRoute.RouteSteps != null)
                {
                    if (oERPRoute.RouteSteps.Length == 0) continue;
                    foreach (var routeStep in oERPRoute.RouteSteps)
                    {
                        if (routeStep.Sequence == null) continue;
                        if (routeStep.Sequence.Value == operationNumber && routeStep.Name != null)
                        {
                            MaterialList.Add(new MfgOrderMaterialListItmChanges() { Product = new RevisionedObjectRef(item.Material), QtyRequired = number / mfgOrder.Qty.Value, IssueControl = IssueControlEnum.LotAndStockPoint, RouteStep = new NamedSubentityRef(routeStep.Name.Value), wikScanning = new Primitive<string>() { Value = item.Scanning } });
                            #if DEBUG
                                Console.WriteLine($"{item.ProductionOrder} - {item.Material} - {item.MaterialGroup} - {item.Qty} - {item.Scanning}");
                            #endif
                        }
                    }
                }
                else
                {
                    EventLogUtil.LogEvent($"ERP Route: {oERPRoute.Name.Value} doesn't have routeSteps!. Material will included when updated!", System.Diagnostics.EventLogEntryType.Warning, 3);
                }
            }
            return MaterialList;
        }
        public async Task<ProductChanges> GetProductFromCached(string ProductName, string ProductRevision = "")
        {
            if (await _repositoryCached.GetProduct(ProductName, ProductRevision) is null)
            {
                ProductChanges product = _repositoryMaintenanceTxn.GetProduct(ProductName, ProductRevision);
                await _repositoryCached.SaveProduct(ProductName, product, ProductRevision, TimeSpan.FromHours(1));
                return product;
            }
            else
            {
                #if DEBUG
                    Console.WriteLine($"Product From Cached {ProductName}");
                #endif
                return await _repositoryCached.GetProduct(ProductName, ProductRevision);
            }
        }
        public async Task<ERPRouteChanges> GetERPRouteFromCached(string ERPRouteName, string ERPRouteRevision = "")
        {
            if (await _repositoryCached.GetERPRoute(ERPRouteName, ERPRouteRevision) is null)
            {
                ERPRouteChanges erpRoute = _repositoryMaintenanceTxn.GetERPRoute(ERPRouteName, ERPRouteRevision);
                await _repositoryCached.SaveERPRoute(ERPRouteName, erpRoute, ERPRouteRevision, TimeSpan.FromHours(1));
                return erpRoute;
            }
            else
            {
                #if DEBUG
                    Console.WriteLine($"ERPRoute From Cached {ERPRouteName}");
                #endif
                return await _repositoryCached.GetERPRoute(ERPRouteName, ERPRouteRevision);
            }
        }
        public async Task<WorkflowChanges> GetWorkflowFromCached(string WorkflowName, string WorkflowRevision = "")
        {
            if (await _repositoryCached.GetWorkflow(WorkflowName, WorkflowRevision) is null)
            {
                WorkflowChanges workflow = _repositoryMaintenanceTxn.GetWorkflow(WorkflowName, WorkflowRevision);
                await _repositoryCached.SaveWorkflow(WorkflowName, workflow, WorkflowRevision, TimeSpan.FromHours(1));
                return workflow;
            }
            else
            {
                #if DEBUG
                    Console.WriteLine($"Workflow From Cached {WorkflowName}");
                #endif
                return await _repositoryCached.GetWorkflow(WorkflowName, WorkflowRevision);
            }
        }
        public async Task<ERPRouteChanges> GetERPRouteFromMfgOrder(MfgOrderChanges oMfgOrder, bool IgnoreException = true)
        {
            try
            {
                if (oMfgOrder != null)
                {
                    if (oMfgOrder.Product != null)
                    {
                        ProductChanges oProduct = await GetProductFromCached(oMfgOrder.Product.Name);
                        if (oProduct.Workflow != null)
                        {
                            WorkflowChanges oWorkflow = await GetWorkflowFromCached(oProduct.Workflow.Name);
                            if (oWorkflow.ERPRoute != null)
                            {
                                ERPRouteChanges oERPRoute = await GetERPRouteFromCached(oWorkflow.ERPRoute.Name);
                                return oERPRoute;
                            }
                            else
                            {
                                EventLogUtil.LogEvent($"(Trying to get ERP Route from Mfg/Production Order): Manufacturing Order doesn't have a ERP Route because, Workflow: {oWorkflow.Name.Value} doesn't have a ERP Route", System.Diagnostics.EventLogEntryType.Warning, 3);
                                return null;
                            }
                        }
                        else
                        {
                            EventLogUtil.LogEvent($"(Trying to get ERP Route from Mfg/Production Order): Manufacturing Order doesn't have a ERP Route because, Product : {oProduct.Name.Value} doesn't have a workflow!", System.Diagnostics.EventLogEntryType.Warning, 3);
                            return null;
                        }
                    }
                    else
                    {
                        EventLogUtil.LogEvent($"(Trying to get ERP Route from Mfg/Production Order): Manufacturing Order doesn't have a ERP Route because, Production or Manufacturing Order: {oMfgOrder.Name.Value} doesn't have a product!", System.Diagnostics.EventLogEntryType.Warning, 3);
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
                if (!IgnoreException) throw ex;
                return null;
            }
        }
        public async Task MigrateCSVToCached(List<Entity.OrderBOM> orderBOMs)
        {
            foreach (var orderBOM in orderBOMs)
            {
                if (await _repositoryCached.GetOrderBOM($"POBOM{orderBOM.ProductionOrder}{orderBOM.Material}") is null)
                {
                    await _repositoryCached.SaveOrderBOM($"POBOM{orderBOM.ProductionOrder}{orderBOM.Material}", orderBOM, TimeSpan.FromHours(1));
                } else
                {
                    #if DEBUG
                        Console.WriteLine($"POBOM From Cached POBOM{orderBOM.ProductionOrder}{orderBOM.Material}");
                    #endif
                }
            }
        }
        public async Task<List<MfgOrderChanges>> FindMfgOrder(string[] MergeMfgOrders)
        {
            List<MfgOrderChanges> mfgOrders = new List<MfgOrderChanges>();
            foreach (var mfgOrder in MergeMfgOrders)
            {
                var mfgOrderCached = await _repositoryCached.GetMfgOrder(mfgOrder);
                if (mfgOrderCached is null)
                {
                    MfgOrderChanges mfgOrderInfo = _repositoryMaintenanceTxn.GetMfgOrder(mfgOrder);
                    if (mfgOrderInfo != null)
                    {
                        await _repositoryCached.SaveMfgOrder(mfgOrder, mfgOrderInfo, TimeSpan.FromHours(1));
                        mfgOrders.Add(mfgOrderInfo);
                    }
                    else
                    {
                        EventLogUtil.LogEvent($"Production or Manufacturing Order: {mfgOrder} is not found!", System.Diagnostics.EventLogEntryType.Warning, 3);
                    }
                }
                else
                {
                    mfgOrders.Add(mfgOrderCached);
                    Console.WriteLine($"Mfg Order From Cached {mfgOrderCached.Name}");
                }
            }
            return mfgOrders;
        }

        public void CreateDefaultProductType()
        {
            try
            {
                bool ObjectExists = _repositoryMaintenanceTxn.ProductTypeExists(AppSettings.DefaultProductType);
                if (!ObjectExists && AppSettings.DefaultProductType != "") _repositoryMaintenanceTxn.SaveProductType(AppSettings.DefaultProductType);
            }
            catch (Exception ex)
            {
                EventLogUtil.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Warning, 3);
            }
        }

        private bool ConvertToDoubleCommaDecimal(string value, out double result)
        {
            CultureInfo provider = new CultureInfo("en-US");
            NumberStyles styles = NumberStyles.Integer | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
            bool status = double.TryParse(value, styles, provider, out result);
            return status;
        }
    }
}
