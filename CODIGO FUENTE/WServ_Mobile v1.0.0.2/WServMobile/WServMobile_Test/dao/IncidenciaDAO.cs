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
    public class IncidenciaDAO : RestCall
    {
        public static List<IncidenciaBean> obtenerIncidencias(string url)
        {
            var mList = new List<IncidenciaBean>();

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
                MainProcess.log.Error("IncidenciaDAO > obtenerIncidencias() > " + ex.Message);
            }

            return mList;
        }

        public static bool validarIncidencia(string url, IncidenciaBean incidencia)
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
                            actualizarPropiedades(incidencia.ClaveMovil,
                                MainProcess.mConn.urlPatchIncidencia +
                                                "?empId=" + incidencia.EMPRESA +
                                                "&acId=" + incidencia.ClaveMovil,
                                "{\"MIGRADO\":\"Y\", \"CODIGOSAP\": " + cardCode + "}");
                        }
                        else if (objCount > 1)
                        {
                            exists = true;
                            MainProcess.log.Error("IncidenciaDAO > validarIncidencia() > Document " + incidencia.ClaveMovil + " > El documento ya fue creado en SAP");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exists = true;
                MainProcess.log.Error("IncidenciaDAO > validarIncidencia() > " + ex.Message);
            }

            return exists;
        }

        public static string registrarIncidencia(string sessionId, string routeId, string urlSL, IncidenciaBean incidencia)
        {
            string res = string.Empty;
            try
            {
                var document = transformActivity(incidencia);
                if (document != null)
                {
                    File.WriteAllText(Util.castURL(MainProcess.mConn.pathJSONLog, "\\") + "CLIENTE_" + incidencia.ClaveMovil
                                    + ".json",
                                    SimpleJson.SerializeObject(document));
                    IRestResponse response = makeRequest(Util.castURL(urlSL, "/") + Constant.ACTIVITIES, Method.POST, sessionId, routeId, document);
                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        JObject jObject = JObject.Parse(response.Content.ToString());
                        res = jObject["ActivityCode"].ToString().Trim();
                    }
                    else
                    {
                        res = string.Empty;
                        MainProcess.log.Error("IncidenciaDAO > registrarIncidencia() > Document Activity " +
                            incidencia.ClaveMovil + " > " + response.Content);
                        actualizarPropiedades(incidencia.ClaveMovil,
                            MainProcess.mConn.urlPatchIncidencia +
                                            "?empId=" + incidencia.EMPRESA +
                                            "&acId=" + incidencia.ClaveMovil,
                            "{\"MIGRADO\":\"N\", \"MENSAJE\": \"" + Util.replaceEscChar(response.Content) + "\"}");
                    }
                }
            }
            catch (Exception ex)
            {
                res = string.Empty;
                MainProcess.log.Error("IncidenciaDAO > registrarIncidencia() > " + ex.Message);
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
                        MainProcess.log.Error("IncidenciaDAO > actualizarPropiedades() > Document " +
                        claveMovil + " > " + messageError);
                    }
                }
                else
                {
                    res = false;
                    MainProcess.log.Error("IncidenciaDAO > actualizarPropiedades() > Document " +
                        claveMovil + " > " + response.Content);
                }
            }
            catch (Exception ex)
            {
                MainProcess.log.Error("IncidenciaDAO > actualizarPropiedades() > " + ex.Message);
            }

            return res;
        }

        public static ActivityBean transformActivity(IncidenciaBean incidencia)
        {
            try
            {
                var activity = new ActivityBean();

                if (!string.IsNullOrEmpty(incidencia.CodigoCliente))
                {
                    activity.CardCode = incidencia.CodigoCliente;
                    activity.ContactPersonCode = string.IsNullOrEmpty(incidencia.CodigoContacto) ? null : (int?)int.Parse(incidencia.CodigoContacto);
                    //activity.AddressType = incidencia.TipoDocumento;
                }

                string activityDescription;
                int activityType;
                if (incidencia.Origen.StartsWith(Constant.ORIGEN_ORDEN_VENTA))
                {
                    activityDescription = "cn_Meeting";
                    activityType = 1;
                }
                else if (incidencia.Origen.StartsWith(Constant.ORIGEN_ENTREGA))
                {
                    activityDescription = "cn_Meeting";
                    activityType = 2;
                }
                else
                {
                    activityDescription = "cn_Task";
                    activityType = 3;
                }

                activity.Activity = activityDescription;
                activity.ActivityType = activityType;
                activity.SalesEmployee = incidencia.Vendedor;
                activity.ActivityDate = DateTime.ParseExact(incidencia.FechaCreacion, "yyyyMMdd", CultureInfo.InvariantCulture);

                if (!incidencia.Origen.StartsWith(Constant.ORIGEN_FACTURA))
                {
                    activity.StartTime = incidencia.HoraCreacion.Replace(":", "");
                    activity.EndTime = DateTime.ParseExact(activity.StartTime, "HHmm", CultureInfo.InvariantCulture).AddMinutes(5).ToString("HHmm");
                }
                else
                {
                    activity.StartTime = "0900";
                    activity.EndTime = "1800";
                }

                activity.StartDate = activity.ActivityDate;
                activity.EndDueDate = activity.ActivityDate;

                if (!string.IsNullOrEmpty(incidencia.SerieFactura))
                { 
                    activity.DocType = "13";
                    activity.DocEntry = incidencia.ClaveFactura != -1 ? incidencia.ClaveFactura.ToString() : null;
                    activity.U_MSSM_SER = incidencia.SerieFactura;
                    activity.U_MSSM_COR = incidencia.CorrelativoFactura.ToString();
                }

                if(!string.IsNullOrEmpty(incidencia.CodigoDireccion))
                    activity.AddressName = incidencia.CodigoDireccion;
                activity.U_MSSM_CLM = incidencia.ClaveMovil;
                activity.U_MSSM_TRM = "05";
                activity.U_MSSM_MOL = incidencia.ModoOffLine;
                activity.U_MSSM_LAT = incidencia.Latitud;
                activity.U_MSSM_LON = incidencia.Longitud;
                activity.U_MSSM_FEC = activity.ActivityDate;
                activity.U_MSSM_HOR = incidencia.HoraCreacion;
                activity.U_MSSM_MOT = incidencia.CodigoMotivo != -1 ? incidencia.DescripcionMotivo : null;

                activity.U_MSSM_FCP = !string.IsNullOrEmpty(incidencia.FechaPago) ? 
                   (DateTime?) DateTime.ParseExact(incidencia.FechaPago,"yyyyMMdd", CultureInfo.InvariantCulture) : null;

                activity.U_MSSM_TIP = incidencia.TipoIncidencia != null ? incidencia.TipoIncidencia : null;

                return activity;
            }
            catch (Exception ex)
            {
                MainProcess.log.Error("IncidenciaDAO > transformActivity() > " + ex.Message);
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
            public List<IncidenciaBean> value { get; set; }
        }
        #endregion
    }
}
