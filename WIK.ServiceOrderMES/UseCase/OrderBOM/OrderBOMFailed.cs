using Camstar.WCF.ObjectStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIK.ServiceOrderMES.Config;

namespace WIK.ServiceOrderMES.UseCase
{
    public interface IOrderBOMFailed : Abstraction.IUseCase
    {
        new Task MainLogic(string delimiter, string sourceFile);
    }
    public class OrderBOMFailed : IOrderBOMFailed
    {
        private readonly Repository.IOrderBOMCached _repositoryCached;
        private readonly Repository.MaintenanceTransaction _repositoryMaintenanceTxn;
        public OrderBOMFailed(Repository.IOrderBOMCached repositoryCached, Repository.MaintenanceTransaction repositoryMaintenanceTxn) 
        {
            _repositoryCached = repositoryCached;
            _repositoryMaintenanceTxn = repositoryMaintenanceTxn;
        }

        public async Task MainLogic(string delimiter, string path)
        {
            List<Entity.OrderBOMSaved> listOrderBOMFailed = await _repositoryCached.GetMfgOrderFailList("PO_FAIL_*");
            if (listOrderBOMFailed.Count == 0)
            {
                #if DEBUG
                    Console.WriteLine("There's no Order BOM Failed");
                    return;
                #endif
            }
            foreach (var orderBOM in listOrderBOMFailed)
            {
                bool resultMfgOrder = _repositoryMaintenanceTxn.SaveMfgOrder(orderBOM.MfgOrderName, "", "", "", "", "", "", 0, orderBOM.MaterialList, orderBOM.ERPRouteName);
                #if DEBUG
                    string msg = resultMfgOrder == true ? "Success" : "Failed";
                    Console.WriteLine($"{orderBOM.MfgOrderName} result: {msg}");
                #endif

                // If Transaction Fail, it must be executed later
                if (!resultMfgOrder)
                {
                    await _repositoryCached.SaveMfgOrderFail(orderBOM.MfgOrderName, orderBOM, TimeSpan.FromDays(AppSettings.CachedExpiration));
                } else
                {
                    // Delete MfgOrder Fail from cached
                    await _repositoryCached.DeleteMfgOrderFail(orderBOM.MfgOrderName);
                }
            }
        }
    }
}
