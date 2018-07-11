using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WServMobile_Test.helpers
{
    public class Constant
    {
        public static string STATUS_SUCCESS = "Success";
        public static string STATUS_ERROR = "Error";

        //SERVICE LAYER METHODS
        public static string LOGIN = "Login";
        public static string LOGOUT = "Logout";
        public static string DRAFTS = "Drafts";
        public static string PAYMENT_DRAFTS = "PaymentDrafts";
        public static string ORDERS = "Orders";
        public static string ACTIVITIES = "Activities";
        public static string INCOMING_PAYMENTS = "IncomingPayments";
        public static string BUSINESS_PARTNERS = "BusinessPartners";
        public static string RETURNS = "Returns";

        //OBJECT CODES
        public static string OBJCODE_SALES_ORDER = "17";
        public static string OBJCODE_INCOMING_PAYMENT = "24";
        public static string OBJCODE_BUSINESS_PARTNER = "2";
        public static string OBJCODE_ACTIVITIES = "33";
        public static string OBJCODE_CREDIT_NOTE = "14";

        //TIPOS DE DOCUMENTOS
        public static string DOCUMENTO_DEFINITIVO = "01";
        public static string DOCUMENTO_BORRADOR = "02";

        //TIPOS DE DIRECCION DE CLIENTE
        public static string BILL_TO  = "bo_BillTo";
        public static string SHIP_TO = "bo_ShipTo";

        //ORIGEN INCIDENCIAS
        public static string ORIGEN_ORDEN_VENTA = "Orden";
        public static string ORIGEN_ENTREGA = "Entrega";
        public static string ORIGEN_FACTURA = "Factura";
    }
}
