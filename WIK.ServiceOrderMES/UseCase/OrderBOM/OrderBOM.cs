using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIK.ServiceOrderMES.UseCase
{
    public interface IOrderBOM : Abstraction.IUseCase
    {
        new Task MainLogic(string delimiter, string path);
    }
    public class OrderBOM : IOrderBOM
    {
        private readonly Repository.IOrderBOMCsv _repositoryCsv;
        private readonly Repository.IOrderCached _repositoryCached;
        private readonly Repository.MaintenanceTransaction _repositoryMaintenanceTxn;
        public OrderBOM(Repository.IOrderBOMCsv repositoryCsv, Repository.IOrderCached repositoryCached, Repository.MaintenanceTransaction repositoryMaintenanceTxn)
        {
            _repositoryCsv = repositoryCsv;
            _repositoryCached = repositoryCached;
            _repositoryMaintenanceTxn = repositoryMaintenanceTxn;
        }

        public async Task MainLogic(string delimiter, string path)
        {
            Console.WriteLine("Main Logic Order BOM");
        }
    }
}
