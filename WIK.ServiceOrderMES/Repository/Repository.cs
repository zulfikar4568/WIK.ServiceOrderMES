﻿using Autofac;
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
            moduleBuilder.RegisterType<Order>().As<IOrder>();
            moduleBuilder.RegisterType<OrderBOM>().As<IOrderBOM>();
        }
    }
}
