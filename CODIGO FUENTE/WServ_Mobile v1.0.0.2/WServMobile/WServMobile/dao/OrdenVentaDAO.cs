using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WServMobile.entity;
using WServMobile.helpers;

namespace WServMobile.dao
{
    public class OrdenVentaDAO : RestCall
    {
        public static List<OrdenVentaBean> obtenerOrdenVenta(string url)
        {
            var mList = new List<OrdenVentaBean>();

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
                MainProcess.log.Error("OrdenVentaDAO > obtenerOrdenVenta() > " + ex.Message);
            }

            return mList;
        }

        public static bool validarOrdenVenta(string url, OrdenVentaBean orden, string tipo)
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
                            actualizarPropiedades(orden.ClaveMovil,
                                MainProcess.mConn.urlPatchOrdenVenta +
                                                "?empId=" + orden.EMPRESA +
                                                "&ordrId=" + orden.ClaveMovil,
                                "{\"Migrado\":\"Y\", \"DocEntry\": \"" + docEntry + "\", \"Mensaje\":\"" +
                                            (tipo.Equals(Constant.DOCUMENTO_BORRADOR)
                                                    ? "Borrador creado" : "Documento creado") + "\"}");
                        }
                        else if (objCount > 1)
                        {
                            exists = true;
                            MainProcess.log.Error("OrdenVentaDAO > validarOrdenVenta() > Document " + orden.ClaveMovil + " > El documento ya fue creado en SAP");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exists = true;
                MainProcess.log.Error("OrdenVentaDAO > validarOrdenVenta() > " + ex.Message);
            }

            return exists;
        }

        public static int registrarOrdenVenta(string sessionId, string routeId, string urlSL, OrdenVentaBean orden, string tipoOrden = null)
        {
            int res = -1;
            try
            {
                if (string.IsNullOrEmpty(tipoOrden) || tipoOrden.Equals(Constant.DOCUMENTO_BORRADOR))
                {
                    var document = transformOrdrToDraft(orden);
                    if (document != null)
                    {
                        File.WriteAllText(Util.castURL(MainProcess.mConn.pathJSONLog, "\\") + "ORDEN_" + orden.ClaveMovil
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
                            MainProcess.log.Error("OrdenVentaDAO > registrarOrdenVenta() > Document Draft " +
                                orden.ClaveMovil + " > " + response.Content);
                        }
                    }
                }
                else
                {
                    var document = transformOrdr(orden);
                    if (document != null)
                    {
                        File.WriteAllText(Util.castURL(MainProcess.mConn.pathJSONLog, "\\") + orden.ClaveMovil
                                        + ".json",
                                        SimpleJson.SerializeObject(document));
                        IRestResponse response = makeRequest(Util.castURL(urlSL, "/") + Constant.ORDERS, Method.POST, sessionId, routeId, document);
                        if (response.StatusCode == System.Net.HttpStatusCode.Created)
                        {
                            JObject jObject = JObject.Parse(response.Content.ToString());
                            res = int.Parse(jObject["DocEntry"].ToString().Trim());
                        }
                        else
                        {
                            JObject jObject = JObject.Parse(response.Content.ToString());
                            string codeError = jObject["error"]["code"].ToString();

                            if (!string.IsNullOrEmpty(codeError) && codeError.Equals("-2028"))
                            {
                                res = -99;
                            }
                            else
                            {
                                res = -1;
                                MainProcess.log.Error("OrdenVentaDAO > registrarOrdenVenta() > Document Order " +
                                    orden.ClaveMovil + " > " + response.Content);
                                actualizarPropiedades(orden.ClaveMovil,
                                    MainProcess.mConn.urlPatchOrdenVenta +
                                                    "?empId=" + orden.EMPRESA +
                                                    "&ordrId=" + orden.ClaveMovil,
                                    "{\"Migrado\":\"N\", \"Mensaje\": \"" + Util.replaceEscChar(response.Content) + "\"}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res = -1;
                MainProcess.log.Error("OrdenVentaDAO > registrarOrdenVenta() > " + ex.Message);
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
                        MainProcess.log.Error("OrdenVentaDAO > actualizarEstado() > Document " +
                        claveMovil + " > " + messageError);
                    }
                }
                else
                {
                    res = false;
                    MainProcess.log.Error("OrdenVentaDAO > actualizarEstado() > Document " +
                        claveMovil + " > " + response.Content);
                }
            }
            catch (Exception ex)
            {
                MainProcess.log.Error("OrdenVentaDAO > actualizarEstado() > " + ex.Message);
            }

            return res;
        }

        public static DraftBean transformOrdrToDraft(OrdenVentaBean ordr)
        {
            try
            {
                var draft = new DraftBean();

                draft.DocObjectCode = Constant.OBJCODE_SALES_ORDER;

                if(!string.IsNullOrEmpty(ordr.Serie))
                    draft.Series = int.Parse(ordr.Serie);

                draft.CardCode = ordr.SocioNegocio;
                draft.DocDate = DateTime.ParseExact(ordr.FechaContable, "yyyyMMdd", CultureInfo.InvariantCulture);
                draft.DocDueDate = DateTime.ParseExact(ordr.FechaVencimiento, "yyyyMMdd", CultureInfo.InvariantCulture);
                draft.TaxDate = draft.DocDate;
                draft.Project = ordr.Proyecto;
                draft.DocCurrency = ordr.Moneda;
                draft.SalesPersonCode = int.Parse(ordr.EmpleadoVenta);
                draft.PayToCode = ordr.DireccionFiscal;
                draft.ShipToCode = ordr.DireccionEntrega;
                draft.PaymentGroupCode = int.Parse(ordr.CondicionPago);
                draft.Indicator = ordr.Indicador;
                //draft.Comments = ordr.Referencia;
                draft.U_MSSM_CRM = "Y";
                draft.U_MSSM_CLM = ordr.ClaveMovil;
                draft.U_MSSM_TRM = "02";
                draft.U_MSSM_MOL = ordr.ModoOffLine;
                draft.U_MSSM_LAT = ordr.Latitud;
                draft.U_MSSM_LON = ordr.Longitud;
                draft.U_MSSM_HOR = ordr.Hora;
                draft.U_MSSM_RAN = ordr.RangoDireccion;
                draft.U_MSSL_MTR = ordr.motivoTraslado;

                var detalle = new List<DraftLineBean>();
                foreach (var l in ordr.Lineas)
                {
                    detalle.Add(new DraftLineBean()
                    {
                        ItemCode = l.Articulo,
                        UoMEntry = int.Parse(l.UnidadMedida),
                        WarehouseCode = l.Almacen,
                        Quantity = int.Parse(l.Cantidad, NumberStyles.AllowDecimalPoint),
                        UnitPrice = double.Parse(l.PrecioUnitario),
                        DiscountPercent = double.Parse(l.PorcentajeDescuento),
                        TaxCode = l.Impuesto,
                        ProjectCode = ordr.Proyecto
                    });
                }

                draft.DocumentLines = detalle;

                return draft;
            }
            catch (Exception ex)
            {
                MainProcess.log.Error("OrdenVentaDAO > transformOrdrToDraft() > " + ex.Message);
                return null;
            }
        }

        public static SalesOrderBean transformOrdr(OrdenVentaBean ordr)
        {
            try
            {
                var salesOrder = new SalesOrderBean();

                if (!string.IsNullOrEmpty(ordr.Serie))
                    salesOrder.Series = int.Parse(ordr.Serie);

                salesOrder.CardCode = ordr.SocioNegocio;
                salesOrder.DocDate = DateTime.ParseExact(ordr.FechaContable, "yyyyMMdd", CultureInfo.InvariantCulture);
                salesOrder.DocDueDate = DateTime.ParseExact(ordr.FechaVencimiento, "yyyyMMdd", CultureInfo.InvariantCulture);
                salesOrder.TaxDate = salesOrder.DocDate;
                salesOrder.DocCurrency = ordr.Moneda;
                salesOrder.SalesPersonCode = int.Parse(ordr.EmpleadoVenta);
                salesOrder.PayToCode = ordr.DireccionFiscal;
                salesOrder.ShipToCode = ordr.DireccionEntrega;
                salesOrder.Project = ordr.Proyecto;
                salesOrder.PaymentGroupCode = int.Parse(ordr.CondicionPago);
                salesOrder.Indicator = ordr.Indicador;
                salesOrder.Comments = ordr.Referencia;
                salesOrder.U_MSSM_CRM = "Y";
                salesOrder.U_MSSM_CLM = ordr.ClaveMovil;
                salesOrder.U_MSSM_TRM = "05";
                salesOrder.U_MSSM_MOL = ordr.ModoOffLine;
                salesOrder.U_MSSM_LAT = ordr.Latitud;
                salesOrder.U_MSSM_LON = ordr.Longitud;
                salesOrder.U_MSSM_HOR = ordr.Hora;
                salesOrder.U_MSSM_RAN = ordr.RangoDireccion;
                salesOrder.U_MSSL_MTR = ordr.motivoTraslado;

                var detalle = new List<SalesOrderLineBean>();
                foreach (var l in ordr.Lineas)
                {
                    detalle.Add(new SalesOrderLineBean()
                    {
                        ItemCode = l.Articulo,
                        UoMEntry = int.Parse(l.UnidadMedida),
                        WarehouseCode = l.Almacen,
                        Quantity = int.Parse(l.Cantidad, NumberStyles.AllowDecimalPoint),
                        UnitPrice = double.Parse(l.PrecioUnitario),
                        DiscountPercent = double.Parse(l.PorcentajeDescuento),
                        TaxCode = l.Impuesto,
                        ProjectCode = ordr.Proyecto
                    });
                }

                salesOrder.DocumentLines = detalle;

                return salesOrder;
            }
            catch (Exception ex)
            {
                MainProcess.log.Error("OrdenVentaDAO > transformOrdr() > " + ex.Message);
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
            public List<OrdenVentaBean> value { get; set; }
        }
        #endregion
    }
}
