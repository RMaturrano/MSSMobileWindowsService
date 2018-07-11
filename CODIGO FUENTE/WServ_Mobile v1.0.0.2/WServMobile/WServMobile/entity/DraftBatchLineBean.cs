using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WServMobile.entity
{
    public class DraftBatchLineBean
    {
        public DraftBatchLineBean()
        {
            this.ItemCode = "";
            this.DiscountPercent = 0;
            this.UoMEntry = -1;

            BatchNumbers = new List<BatchNumberBean>();
        }

        public int LineNum { get; set; }
        public string ItemCode { get; set; }
        public int UoMEntry { get; set; }
        public string WarehouseCode { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double DiscountPercent { get; set; }
        public string TaxCode { get; set; }
        public int BaseType { get; set; }
        public int BaseEntry { get; set; }
        public int BaseLine { get; set; }
        public List<BatchNumberBean> BatchNumbers { get; set; }
        public List<DraftBeanAllocationBean> DocumentLinesBinAllocations { get; set; }
    }
}
