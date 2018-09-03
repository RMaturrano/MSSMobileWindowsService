using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WServMobile.entity.sl
{
    public class SalesOrderNoLocBean
    {
        public int Series { get; set; }
        public string CardCode { get; set; }
        public string NumAtCard { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string DocCurrency { get; set; }
        public int SalesPersonCode { get; set; }
        public string Project { get; set; }
        //Direccion fiscal
        public string PayToCode { get; set; }
        //direccion entrega
        public string ShipToCode { get; set; }
        public int PaymentGroupCode { get; set; }
        public string Indicator { get; set; }
        public string Comments { get; set; }
        public string U_MSSM_CRM { get; set; }
        public string U_MSSM_CLM { get; set; }
        public string U_MSSM_TRM { get; set; }
        public string U_MSSM_MOL { get; set; }
        public string U_MSSM_LAT { get; set; }
        public string U_MSSM_LON { get; set; }
        public string U_MSSM_HOR { get; set; }
        public string U_MSSM_RAN { get; set; }
        public List<SalesOrderLineBean> DocumentLines { get; set; }
    }
}
