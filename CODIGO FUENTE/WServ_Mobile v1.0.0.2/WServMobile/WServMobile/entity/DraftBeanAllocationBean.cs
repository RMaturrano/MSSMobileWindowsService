using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WServMobile.entity
{
    public class DraftBeanAllocationBean
    {
        public DraftBeanAllocationBean()
        {
            AllowNegativeQuantity = "tNO";
            SerialAndBatchNumbersBaseLine = -1;
        }

        public int BinAbsEntry { get; set; }
        public int Quantity { get; set; }
        public string AllowNegativeQuantity { get; set; }
        public int SerialAndBatchNumbersBaseLine { get; set; }
        public int BaseLineNumber { get; set; }
    }
}
