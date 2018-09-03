using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WServMobile.entity
{
    public class BusinessPartnerBean
    {
        public BusinessPartnerBean()
        {
            CardType = "cLid";
            Currency = "##";
            Valid = "tYES";
            Frozen = "tNO";
            U_MSSM_CRM = "Y";
            BPAddresses = new List<AddressBean>();
            ContactEmployees = new List<ContactBean>();
        }

        public string CardType { get; set; }
        public string U_MSSL_BTP { get; set; }
        public string U_MSSL_BTD { get; set; }
        public string U_MSSL_BAP { get; set; }
        public string U_MSSL_BAM { get; set; }
        public string U_MSSL_BN1 { get; set; }
        public string U_MSSL_BN2 { get; set; }
        public string FederalTaxID { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string CardForeignName { get; set; }
        public string Currency { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Cellular { get; set; }
        public string EmailAddress { get; set; }
        public int? GroupCode { get; set; }
        public string Valid { get; set; }
        public string Frozen { get; set; }
        public int? PayTermsGrpCode { get; set; }
        public int? PriceListNum { get; set; }
        public string Indicator { get; set; }
        public string ProjectCode { get; set; }
        public string U_MSSM_CRM { get; set; }  //CreadoMovil
        public string U_MSSM_CLM { get; set; }  //ClaveMovil
        public string U_MSSM_TRM { get; set; }  //TransaccionMovil
        public string U_MSSM_POA { get; set; } //posee activos
        public string U_MSSM_TRE { get; set; }
        public List<AddressBean> BPAddresses { get; set; }
        public List<ContactBean> ContactEmployees { get; set; }
    }
}
