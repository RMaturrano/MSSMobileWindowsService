using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
using WServMobile_Test.entity;
using WServMobile_Test.helpers;

namespace WServMobile_Test.dao
{
    public class DevolucionDAO:RestCall
    {

        public static List<DevolucionBean> obtenerDevolucion(string url)
        {
            var mList = new List<DevolucionBean>();

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
                MainProcess.log.Error("DevolucionDAO > obtenerDevolucion() > " + ex.Message);
            }

            return mList;
        }

        public static bool validarDevolucion(string url, DevolucionBean devolucion)
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
                            actualizarPropiedades(devolucion.ClaveMovil,
                                MainProcess.mConn.urlPatchDevolucion +
                                                "?empId=" + devolucion.EMPRESA +
                                                "&rtId=" + devolucion.ClaveMovil,
                                "{\"MIGRADO\":\"Y\", \"DOCENTRY\": \"" + docEntry + "\", \"MENSAJE\":\"" +
                                            ("Documento creado") + "\"}");
                        }
                        else if (objCount > 1)
                        {
                            exists = true;
                            MainProcess.log.Error("DevolucionDAO > validarDevolucion() > Document " + devolucion.ClaveMovil + " > El documento ya fue creado en SAP");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exists = true;
                MainProcess.log.Error("DevolucionDAO > validarDevolucion() > " + ex.Message);
            }

            return exists;
        }

        public static int registrarDevolucion(string sessionId, string routeId, string urlSL, DevolucionBean devolucion)
        {
            int res = -1;
            try
            {
                var document = transformOrdn(devolucion);
                if (document != null)
                {
                    File.WriteAllText(Util.castURL(MainProcess.mConn.pathJSONLog, "\\") + "DEVOLUCION_" + devolucion.ClaveMovil
                                    + ".json",
                                    SimpleJson.SerializeObject(document));
                    IRestResponse response = makeRequest(Util.castURL(urlSL, "/") + Constant.RETURNS, Method.POST, sessionId, routeId, document);
                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        JObject jObject = JObject.Parse(response.Content.ToString());
                        res = int.Parse(jObject["DocEntry"].ToString().Trim());
                    }
                    else
                    {
                        res = -1;
                        MainProcess.log.Error("DevolucionDAO > registrarDevolucion() > Document Return " +
                            devolucion.ClaveMovil + " > " + response.Content);
                        actualizarPropiedades(devolucion.ClaveMovil,
                            MainProcess.mConn.urlPatchDevolucion +
                                            "?empId=" + devolucion.EMPRESA +
                                            "&rtId=" + devolucion.ClaveMovil,
                            "{\"MIGRADO\":\"N\", \"MENSAJE\": \"" + Util.replaceEscChar(response.Content) + "\"}");
                    }
                }
                
            }
            catch (Exception ex)
            {
                res = -1;
                MainProcess.log.Error("DevolucionDAO > registrarDevolucion() > " + ex.Message);
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
                        MainProcess.log.Error("DevolucionDAO > actualizarEstado() > Document " +
                        claveMovil + " > " + messageError);
                    }
                }
                else
                {
                    res = false;
                    MainProcess.log.Error("DevolucionDAO > actualizarEstado() > Document " +
                        claveMovil + " > " + response.Content);
                }
            }
            catch (Exception ex)
            {
                MainProcess.log.Error("DevolucionDAO > actualizarEstado() > " + ex.Message);
            }

            return res;
        }

        public static ReturnBean transformOrdn(DevolucionBean ordn)
        {
            try
            {
                var oReturn = new ReturnBean();

                oReturn.CardCode = ordn.SocioNegocio;
                oReturn.DocDate = DateTime.ParseExact(ordn.FechaContable, "yyyyMMdd", CultureInfo.InvariantCulture);
                oReturn.DocDueDate = DateTime.ParseExact(ordn.FechaVencimiento, "yyyyMMdd", CultureInfo.InvariantCulture);
                oReturn.TaxDate = oReturn.DocDate;
                oReturn.DocCurrency = ordn.Moneda;
                oReturn.SalesPersonCode = int.Parse(ordn.EmpleadoVenta);
                oReturn.PayToCode = ordn.DireccionFiscal;
                oReturn.ShipToCode = ordn.DireccionEntrega;
                oReturn.PaymentGroupCode = int.Parse(ordn.CondicionPago);
                oReturn.Indicator = ordn.Indicador;
                oReturn.Comments = ordn.Comentario;
                oReturn.U_MSSM_CRM = "Y";
                oReturn.U_MSSM_CLM = ordn.ClaveMovil;

                var detalle = new List<ReturnLineBean>();
                foreach (var l in ordn.Lineas)
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

                    detalle.Add(new ReturnLineBean()
                    {
                        LineNum = l.Linea,
                        ItemCode = l.Articulo,
                        WarehouseCode = l.Almacen,
                        Quantity = int.Parse(l.Cantidad, NumberStyles.AllowDecimalPoint),
                        UnitPrice = double.Parse(l.PrecioUnitario),
                        DiscountPercent = double.Parse(l.PorcentajeDescuento),
                        TaxCode = l.Impuesto,
                        BaseLine = int.Parse(l.LineaBase),
                        BaseEntry = int.Parse(ordn.ClaveBase),
                        BatchNumbers = lotes
                    });
                }

                oReturn.DocumentLines = detalle;

                return oReturn;
            }
            catch (Exception ex)
            {
                MainProcess.log.Error("DevolucionDAO > transformOrdn() > " + ex.Message);
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
            public List<DevolucionBean> value { get; set; }
        }
        #endregion

    }
}
