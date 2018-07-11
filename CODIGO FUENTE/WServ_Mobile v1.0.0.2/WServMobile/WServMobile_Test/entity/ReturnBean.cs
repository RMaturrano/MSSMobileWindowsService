using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WServMobile_Test.entity
{
    public class ReturnBean
    {
        public string CardCode { get; set; }
        public string NumAtCard { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string DocCurrency { get; set; }
        public int SalesPersonCode { get; set; }
        //Direccion fiscal
        public string PayToCode { get; set; }
        //direccion entrega
        public string ShipToCode { get; set; }
        public int PaymentGroupCode { get; set; }
        public string Indicator { get; set; }
        public string Comments { get; set; }
        public string U_MSSM_CRM { get; set; }
        public string U_MSSM_CLM { get; set; }
        public List<ReturnLineBean> DocumentLines { get; set; }
    }
}
