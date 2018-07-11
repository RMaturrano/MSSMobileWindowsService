using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WServMobile.entity
{
    public class PaymentInvoiceBean
    {
        public PaymentInvoiceBean()
        {
            InstallmentId = 1;
            DocLine = 0;
        }

        public int DocEntry { get; set; }
        public int InstallmentId { get; set; }
        public int DocLine { get; set; }
        public double SumApplied { get; set; }
    }
}
