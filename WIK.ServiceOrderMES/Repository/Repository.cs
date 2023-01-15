using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIK.ServiceOrderMES.Repository
{
    class Repository : Module
    {
        protected override void Load(ContainerBuilder moduleBuilder)
        {
            moduleBuilder.RegisterType<OrderCsv>().As<IOrderCsv>();
            moduleBuilder.RegisterType<OrderBOMCsv>().As<IOrderBOMCsv>();

            moduleBuilder.RegisterType<OrderCached>().As<IOrderCached>();
            moduleBuilder.RegisterType<OrderBOMCached>().As<IOrderBOMCached>();
        }
    }
}
