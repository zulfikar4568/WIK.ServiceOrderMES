using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIK.ServiceOrderMES.UseCase.Abstraction
{
    public interface IUseCase
    {
        Task MainLogic(string delimiter, string sourceFile);
    }
}
