using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIK.ServiceOrderMES.UseCase
{
    public interface IOrder : Abstraction.IUseCase
    {
        new void MainLogic(string delimiter, string sourceFile);
    }
    public class Order : IOrder
    {
        private readonly Repository.IOrderCsv _repositoryCsv;
        private readonly Repository.IOrderCached _repositoryCached;
        private readonly Repository.MaintenanceTransaction _repositoryMaintenanceTxn;
        public Order(Repository.IOrderCsv repositoryCsv, Repository.IOrderCached repositoryCached, Repository.MaintenanceTransaction repositoryMaintenanceTxn)
        {
            _repositoryCsv = repositoryCsv;
            _repositoryCached = repositoryCached;
            _repositoryMaintenanceTxn = repositoryMaintenanceTxn;
        }
        public void MainLogic(string delimiter, string sourceFile)
        {

        }
    }
}
