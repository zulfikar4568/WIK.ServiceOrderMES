using Camstar.WCF.ObjectStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIK.ServiceOrderMES.Entity
{
    public class OrderBOMSaved
    {
        public string MfgOrderName { get; set; }
        public List<MfgOrderMaterialListItmChanges> MaterialList { get; set; }
        public string ERPRouteName { get; set; }
    }
}
