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
        private readonly Repository.IOrder _repository;
        public Order(Repository.IOrder repository)
        {
            _repository = repository;
        }
        public void MainLogic(string delimiter, string sourceFile)
        {

        }
    }
}
