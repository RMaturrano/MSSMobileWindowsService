using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WServMobile_Test.entity
{
    public class SalesOrderLineBean
    {
        public SalesOrderLineBean()
        {
            this.ItemCode = "";
            this.DiscountPercent = 0;
            this.UoMEntry = -1;
        }

        public string ItemCode { get; set; }
        public int UoMEntry { get; set; }
        public string WarehouseCode { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double DiscountPercent { get; set; }
        public string TaxCode { get; set; }
    }
}
