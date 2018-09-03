using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WServMobile.entity
{
    public class IncomingPaymentBean
    {
        public IncomingPaymentBean()
        {
            DocType = "rCustomer";
            U_MSSM_CRM = "Y";
            BankChargeAmount = 0;
            TransferSum = 0;
            CashSum = 0;
            PaymentChecks = new List<PaymentCheckBean>();
            PaymentInvoices = new List<PaymentInvoiceBean>();
            DocObjectCode = "bopot_IncomingPayments";
        }

        public string DocType { get; set; }
        public string CardCode { get; set; }
        public string CounterReference { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime TaxDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Remarks { get; set; }
        public string JournalRemarks { get; set; }
        public double BankChargeAmount { get; set; }
        public string DocCurrency { get; set; }
        public string U_MSSM_CRM { get; set; }
        public string U_MSSM_CLM { get; set; }
        public string U_MSSM_TRM { get; set; }
        public string CheckAccount { get; set; }
        public string TransferAccount { get; set; }
        public DateTime TransferDate { get; set; }
        public string TransferReference { get; set; }
        public double TransferSum { get; set; }
        public string CashAccount { get; set; }
        public double CashSum { get; set; }
        public string DocObjectCode { get; set; }
        public List<PaymentCheckBean> PaymentChecks { get; set; }
        public List<PaymentInvoiceBean> PaymentInvoices { get; set; }


    }
}
