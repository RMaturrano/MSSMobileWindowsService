using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
using WServMobile.entity;
using WServMobile.helpers;

namespace WServMobile.dao
{
    public class PagoDAO : RestCall
    {
        public static List<PagoBean> obtenerPagos(string url)
        {
            var mList = new List<PagoBean>();

            try
            {
                IRestResponse response = makeRequest(url, Method.GET);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (response.Content.Contains(Constant.STATUS_SUCCESS))
                    {
                        ResponseBean objResponse = SimpleJson.DeserializeObject<ResponseBean>(response.Content);
                        if (objResponse.ResponseStatus.Equals(Constant.STATUS_SUCCESS))
                        {
                            if (objResponse.Response.message.value != null &&
                                objResponse.Response.message.value.Count > 0)
                                mList.AddRange(objResponse.Response.message.value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MainProcess.log.Error("PagoDAO > obtenerPagos() > " + ex.Message);
            }

            return mList;
        }

        public static bool validarPago(string url, PagoBean pago, string tipoDoc)
        {
            var exists = false;

            try
            {
                IRestResponse response = makeRequest(url, Method.GET);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (response.Content.Contains(Constant.STATUS_SUCCESS))
                    {
                        JObject objResponse = JObject.Parse(response.Content.ToString());
                        int objCount = (int)objResponse["ResponseCount"];
                        if (objCount == 1)
                        {
                            exists = true;
                            string cardCode = objResponse["Response"]["message"]["value"].ToString();
                            actualizarPropiedades(pago.ClaveMovil,
                                MainProcess.mConn.urlPatchSocioNegocio +
                                                "?empId=" + pago.EMPRESA +
                                                "&icId=" + pago.ClaveMovil,
                                "{\"Migrado\":\"Y\", \"DocEntry\": " + cardCode + ", \"Mensaje\":\"" +
                                            (tipoDoc.Equals(Constant.DOCUMENTO_BORRADOR)
                                                    ? "Borrador creado" : "Documento creado") + "\"}");
                        }
                        else if (objCount > 1)
                        {
                            exists = true;
                            MainProcess.log.Error("PagoDAO > validarPago() > Document " + pago.ClaveMovil + " > El documento ya fue creado en SAP");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exists = true;
                MainProcess.log.Error("PagoDAO > validarPago() > " + ex.Message);
            }

            return exists;
        }

        public static int registrarPago(string sessionId, string routeId, string urlSL, PagoBean pago, string tipoPago = null)
        {
            int res = -1;
            try
            {
                var document = transformIncomingPayment(pago, tipoPago);
                if (document != null)
                {
                    File.WriteAllText(Util.castURL(MainProcess.mConn.pathJSONLog, "\\") + "PAGO_" + pago.ClaveMovil
                                    + ".json",
                                    SimpleJson.SerializeObject(document));
                    IRestResponse response = makeRequest(Util.castURL(urlSL, "/") + (tipoPago.Equals(Constant.DOCUMENTO_BORRADOR) ?
                        Constant.PAYMENT_DRAFTS : Constant.INCOMING_PAYMENTS), Method.POST, sessionId, routeId, document);
                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        JObject jObject = JObject.Parse(response.Content.ToString());
                        res = int.Parse(jObject["DocEntry"].ToString().Trim());
                    }
                    else
                    {
                        res = -1;
                        MainProcess.log.Error("PagoDAO > registrarPago() > Document IncomingPayment " +
                            pago.ClaveMovil + " > " + response.Content);
                        actualizarPropiedades(pago.ClaveMovil,
                            MainProcess.mConn.urlPatchPagoRecibido +
                                            "?empId=" + pago.EMPRESA +
                                            "&icId=" + pago.ClaveMovil,
                            "{\"Migrado\":\"N\", \"Mensaje\": \"" + Util.replaceEscChar(response.Content) + "\"}");
                    }
                }
            }
            catch (Exception ex)
            {
                res = -1;
                MainProcess.log.Error("PagoDAO > registrarPago() > " + ex.Message);
            }

            return res;
        }

        public static bool actualizarPropiedades(string claveMovil, string url, string properties)
        {
            var res = true;

            try
            {
                IRestResponse response = makeRequest(url, Method.PATCH, null, null, properties);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (!response.Content.Contains(Constant.STATUS_SUCCESS))
                    {
                        string messageError = response.Content.ToString();
                        MainProcess.log.Error("PagoDAO > actualizarPropiedades() > Document " +
                        claveMovil + " > " + messageError);
                    }
                }
                else
                {
                    res = false;
                    MainProcess.log.Error("PagoDAO > actualizarPropiedades() > Document " +
                        claveMovil + " > " + response.Content);
                }
            }
            catch (Exception ex)
            {
                MainProcess.log.Error("PagoDAO > actualizarPropiedades() > " + ex.Message);
            }

            return res;
        }

        public static IncomingPaymentBean transformIncomingPayment(PagoBean orct, string tipoPago)
        {
            try
            {
                var incomingPayment = new IncomingPaymentBean();

                incomingPayment.CardCode = orct.SocioNegocio;
                incomingPayment.CounterReference = orct.EmpleadoVenta;
                incomingPayment.Remarks = orct.Comentario;
                incomingPayment.JournalRemarks = orct.Glosa;
                incomingPayment.DocDate = DateTime.ParseExact(orct.FechaContable, "yyyyMMdd", CultureInfo.InvariantCulture);
                incomingPayment.TaxDate = incomingPayment.DocDate;
                incomingPayment.DueDate = incomingPayment.DocDate;
                incomingPayment.DocCurrency = orct.Moneda;
                incomingPayment.U_MSSM_CLM = orct.ClaveMovil;
                incomingPayment.U_MSSM_TRM = tipoPago.Equals(Constant.DOCUMENTO_BORRADOR) ? "02": "05";

                switch (orct.TipoPago)
                {
                    case "C":
                        incomingPayment.PaymentChecks.Add(new PaymentCheckBean()
                        {
                            CheckAccount = orct.ChequeCuenta,
                            BankCode = orct.ChequeBanco,
                            DueDate = !orct.ChequeVencimiento.Equals("") ? 
                                    DateTime.ParseExact(orct.ChequeVencimiento, "yyyyMMdd", CultureInfo.InvariantCulture) : 
                                    incomingPayment.DocDate,
                            CheckSum = orct.ChequeImporte != null ? (double) orct.ChequeImporte : 0,
                            CheckNumber = orct.ChequeNumero != null ? (int) orct.ChequeNumero : 0
                        });
                        break;
                    case "T":
                        incomingPayment.TransferAccount = orct.TransferenciaCuenta;
                        incomingPayment.TransferDate = incomingPayment.DocDate;
                        incomingPayment.TransferReference = orct.TransferenciaReferencia;
                        incomingPayment.TransferSum = orct.TransferenciaImporte != null ? (double)orct.TransferenciaImporte : 0;
                        break;
                    case "F":
                        incomingPayment.CashAccount = orct.EfectivoCuenta;
                        incomingPayment.CashSum = orct.EfectivoImporte != null ? (double) orct.EfectivoImporte : 0;
                        break;
                    default:
                        break;
                }


                if (orct.Lineas.Count > 0)
                {
                    var Lines = new List<PaymentInvoiceBean>();
                    foreach (var c in orct.Lineas)
                    {
                        Lines.Add(new PaymentInvoiceBean()
                        {
                            DocEntry = c.FacturaCliente,
                            SumApplied = c.Importe
                        });
                    }

                    incomingPayment.PaymentInvoices = Lines;
                }

                return incomingPayment;
            }
            catch (Exception ex)
            {
                MainProcess.log.Error("PagoDAO > transformIncomingPayment() > " + ex.Message);
                return null;
            }
        }

        #region MODEL_RESPONSE
        private class ResponseBean
        {
            public string ResponseStatus { get; set; }
            public string ResponseType { get; set; }
            public int ResponseCount { get; set; }
            public Response Response { get; set; }
        }

        private class Response
        {
            public int code { get; set; }
            public Message message { get; set; }
        }

        private class Message
        {
            public string lang { get; set; }
            public List<PagoBean> value { get; set; }
        }
        #endregion
    }
}
