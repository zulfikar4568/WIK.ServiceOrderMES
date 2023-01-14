using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIK.ServiceOrderMES.Entity
{
    public class Order
    {
        [Index(0)]
        public string WorkCenter { get; set; }
        [Index(1)]
        public string ProductionOrder { get; set; }
        [Index(2)]
        public string Material { get; set; }
        [Index(4)]
        public string OrderType { get; set; }
        [Index(7)]
        public string TargetQty { get; set; }
        [Index(11)]
        public string StartTime { get; set; }
        [Index(12)]
        public string EndTime { get; set; }
        [Index(13)]
        public string SystemStatus { get; set; }
    }
}
