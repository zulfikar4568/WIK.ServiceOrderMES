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
        private readonly Repository.IOrderBOM _repository;
        public OrderBOM(Repository.IOrderBOM repository)
        {
            _repository = repository;
        }

        public void MainLogic(string delimiter, string path)
        {

        }
    }
}
