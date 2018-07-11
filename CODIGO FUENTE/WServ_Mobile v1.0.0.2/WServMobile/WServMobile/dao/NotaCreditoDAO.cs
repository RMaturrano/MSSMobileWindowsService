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
    public class NotaCreditoDAO : RestCall
    {
        public static List<NotaCreditoBean> obtenerNotaCredito(string url)
        {
            var mList = new List<NotaCreditoBean>();

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
                MainProcess.log.Error("NotaCreditoDAO > obtenerNotaCredito() > " + ex.Message);
            }

            return mList;
        }

        public static bool validarNotaCredito(string url, NotaCreditoBean notacredito)
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
                            string docEntry = objResponse["Response"]["message"]["value"].ToString();
                            actualizarPropiedades(notacredito.ClaveMovil,
                                MainProcess.mConn.urlPatchNotaCredito +
                                                "?empId=" + notacredito.EMPRESA +
                                                "&cmId=" + notacredito.ClaveMovil,
                                "{\"MIGRADO\":\"Y\", \"DOCENTRY\": \"" + docEntry + "\", \"MENSAJE\":\"" +
                                            ("Borrador creado") + "\"}");
                        }
                        else if (objCount > 1)
                        {
                            exists = true;
                            MainProcess.log.Error("NotaCreditoDAO > validarNotaCredito() > Document " + notacredito.ClaveMovil + " > El documento ya fue creado en SAP");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exists = true;
                MainProcess.log.Error("NotaCreditoDAO > validarNotaCredito() > " + ex.Message);
            }

            return exists;
        }

        public static int registraNotaCredito(string sessionId, string routeId, string urlSL, NotaCreditoBean notacredito)
        {
            int res = -1;
            try
            {
                var document = transformDrf(notacredito);
                if (document != null)
                {
                    File.WriteAllText(Util.castURL(MainProcess.mConn.pathJSONLog, "\\") + "NOTACREDITO_" + notacredito.ClaveMovil
                                    + ".json",
                                    SimpleJson.SerializeObject(document));
                    IRestResponse response = makeRequest(Util.castURL(urlSL, "/") + Constant.DRAFTS, Method.POST, sessionId, routeId, document);
                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        JObject jObject = JObject.Parse(response.Content.ToString());
                        res = int.Parse(jObject["DocEntry"].ToString().Trim());
                    }
                    else
                    {
                        res = -1;
                        MainProcess.log.Error("NotaCreditoDAO > registraNotaCredito() > Document Credit Memo " +
                            notacredito.ClaveMovil + " > " + response.Content);
                        actualizarPropiedades(notacredito.ClaveMovil,
                            MainProcess.mConn.urlPatchNotaCredito +
                                            "?empId=" + notacredito.EMPRESA +
                                            "&cmId=" + notacredito.ClaveMovil,
                            "{\"MIGRADO\":\"N\", \"MENSAJE\": \"" + Util.replaceEscChar(response.Content) + "\"}");
                    }
                }
            }
            catch (Exception ex)
            {
                res = -1;
                MainProcess.log.Error("NotaCreditoDAO > registraNotaCredito() > " + ex.Message);
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
                        MainProcess.log.Error("NotaCreditoDAO > actualizarEstado() > Document " +
                        claveMovil + " > " + messageError);
                    }
                }
                else
                {
                    res = false;
                    MainProcess.log.Error("NotaCreditoDAO > actualizarEstado() > Document " +
                        claveMovil + " > " + response.Content);
                }
            }
            catch (Exception ex)
            {
                MainProcess.log.Error("NotaCreditoDAO > actualizarEstado() > " + ex.Message);
            }

            return res;
        }

        public static DraftBatchBean transformDrf(NotaCreditoBean orin)
        {
            try
            {
                var oReturn = new DraftBatchBean();

                oReturn.DocObjectCode = Constant.OBJCODE_CREDIT_NOTE;
                oReturn.CardCode = orin.SocioNegocio;
                oReturn.DocDate = DateTime.ParseExact(orin.FechaContable, "yyyyMMdd", CultureInfo.InvariantCulture);
                oReturn.DocDueDate = DateTime.ParseExact(orin.FechaVencimiento, "yyyyMMdd", CultureInfo.InvariantCulture);
                oReturn.TaxDate = oReturn.DocDate;
                oReturn.DocCurrency = orin.Moneda;
                oReturn.SalesPersonCode = int.Parse(orin.EmpleadoVenta);
                oReturn.PayToCode = orin.DireccionFiscal;
                oReturn.ShipToCode = orin.DireccionEntrega;
                oReturn.PaymentGroupCode = int.Parse(orin.CondicionPago);
                oReturn.Indicator = orin.Indicador;
                oReturn.Comments = orin.Comentario;
                oReturn.U_MSSM_CRM = "Y";
                oReturn.U_MSSM_CLM = orin.ClaveMovil;
                oReturn.U_MSSM_TRM = "02";

                var detalle = new List<DraftBatchLineBean>();
                foreach (var l in orin.Lineas)
                {
                    List<BatchNumberBean> lotes = new List<BatchNumberBean>();
                    if (l.Lotes != null)
                    {
                        foreach (var lot in l.Lotes)
                        {
                            BatchNumberBean batchNumber = new BatchNumberBean();
                            batchNumber.BaseLineNumber = lot.LineaBase;
                            batchNumber.Quantity = lot.Cantidad;
                            batchNumber.BatchNumber = lot.Lote;
                            lotes.Add(batchNumber);
                        }
                    }

                    detalle.Add(new DraftBatchLineBean()
                    {
                        LineNum = l.Linea,
                        ItemCode = l.Articulo,
                        WarehouseCode = l.Almacen,
                        Quantity = int.Parse(l.Cantidad, NumberStyles.AllowDecimalPoint),
                        UnitPrice = double.Parse(l.PrecioUnitario),
                        DiscountPercent = double.Parse(l.PorcentajeDescuento),
                        TaxCode = l.Impuesto,
                        BaseLine = int.Parse(l.LineaBase),
                        BaseEntry = int.Parse(orin.ClaveBase),
                        BaseType = 13,
                        BatchNumbers = lotes
                    });
                }

                oReturn.DocumentLines = detalle;

                return oReturn;
            }
            catch (Exception ex)
            {
                MainProcess.log.Error("NotaCreditoDAO > transformDrf() > " + ex.Message);
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
            public List<NotaCreditoBean> value { get; set; }
        }
        #endregion
    }
}
