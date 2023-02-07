using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIK.ServiceOrderMES.Entity
{
    public class OrderSaved
    {
        public Entity.Order Order { get; set; }
        public double Qty { get; set; }
        public string OrderStatus { get; set; }
        public string OrderType { get; set; }
    }
}
