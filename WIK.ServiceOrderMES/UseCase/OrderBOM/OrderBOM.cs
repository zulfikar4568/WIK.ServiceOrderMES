using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIK.ServiceOrderMES.UseCase
{
    public interface IOrderBOM : Abstraction.IUseCase
    {
        new void MainLogic(string delimiter, string path);
    }
    public class OrderBOM : IOrderBOM
    {
        private readonly Repository.IOrderBOMCsv _repositoryCsv;
        private readonly Repository.IOrderCached _repositoryCached;
        public OrderBOM(Repository.IOrderBOMCsv repositoryCsv, Repository.IOrderCached repositoryCached)
        {
            _repositoryCsv = repositoryCsv;
            _repositoryCached = repositoryCached;
        }

        public void MainLogic(string delimiter, string path)
        {

        }
    }
}
