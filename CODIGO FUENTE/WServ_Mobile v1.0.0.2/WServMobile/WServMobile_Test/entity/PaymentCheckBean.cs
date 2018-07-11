using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WServMobile_Test.entity
{
    public class PaymentCheckBean
    {
        public PaymentCheckBean()
        {
            CountryCode = "PE";
        }

        public string CheckAccount { get; set; }
        public string CountryCode { get; set; }
        public string BankCode { get; set; }
        public DateTime? DueDate { get; set; }
        public double CheckSum { get; set; }
        public int CheckNumber { get; set; }
    }
}
