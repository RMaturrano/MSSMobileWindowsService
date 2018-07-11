using System;
using System.Collections.Generic;
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
    public class GeolocalizacionDAO : RestCall
    {

        public static List<GeolocalizacionBean> obtenerPendientes(string url)
        {
            var mList = new List<GeolocalizacionBean>();

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
                MainProcess.log.Error("GeolocalizacionDAO > obtenerPendientes() > " + ex.Message);
            }

            return mList;
        }

        public static string actualizarDireccion(string sessionId, string routeId, string urlSL, GeolocalizacionBean gelocation)
        {
            string res = string.Empty;
            try
            {
                var document = transformToBusinessPartner(gelocation);
                if (document != null)
                {
                    File.WriteAllText(Util.castURL(MainProcess.mConn.pathJSONLog, "\\") + "UBICACION_" + gelocation.ClaveMovil
                                    + ".json",
                                    SimpleJson.SerializeObject(document));
                    IRestResponse response = makeRequest(Util.castURL(urlSL, "/") + 
                        Constant.BUSINESS_PARTNERS + "('" + gelocation.CodigoCliente.Trim() + "')", 
                        Method.PATCH, sessionId, routeId, document);

                    if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
                    {
                        //res = string.Empty;
                        //MainProcess.log.Error("GeolocalizacionDAO > acualizarDireccion() > Ubicación " +
                        //    gelocation.ClaveMovil + " - " + gelocation.CodigoDireccion + " > " + response.Content);
                        //actualizarPropiedades(gelocation.ClaveMovil,
                        //    MainProcess.mConn.urlPatchIncidencia +whats
                        //                    "?empId=" + gelocation.Empresa +
                        //                    "&ccId=" + gelocation.CodigoCliente,
                        //    "{\"MIGRADO\":\"N\", \"MENSAJE\": \"" + Util.replaceEscChar(response.Content) + "\"}");
                    }
                    else
                    {
                        actualizarPropiedades(gelocation.ClaveMovil, MainProcess.mConn.urlPatchUbicacion +
                                    "?codEmpresa=" + gelocation.Empresa +
                                    "&codCliente=" + gelocation.CodigoCliente +
                                    "&codDireccion=" + gelocation.CodigoDireccion,
                                    "{\"Migrado\":\"Y\"}");
                    }
                }
            }
            catch (Exception ex)
            {
                res = string.Empty;
                MainProcess.log.Error("GeolocalizacionDAO > acualizarDireccion() > " + ex.Message);
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
                        MainProcess.log.Error("GeolocalizacionDAO > actualizarPropiedades() > Document " +
                        claveMovil + " > " + messageError);
                    }
                }
                else
                {
                    res = false;
                    MainProcess.log.Error("GeolocalizacionDAO > actualizarPropiedades() > Document " +
                        claveMovil + " > " + response.Content);
                }
            }
            catch (Exception ex)
            {
                MainProcess.log.Error("GeolocalizacionDAO > actualizarPropiedades() > " + ex.Message);
            }

            return res;
        }

        public static BusinessPartnerBeanLocationBean transformToBusinessPartner(GeolocalizacionBean incidencia)
        {
            try
            {
                var businessPartner = new BusinessPartnerBeanLocationBean();
                var addresses = new List<AddressLocationBean>();
                addresses.Add(new AddressLocationBean()
                {
                    AddressName = incidencia.CodigoDireccion,
                    AddressType = incidencia.Tipo.Equals("B") ? Constant.BILL_TO : Constant.SHIP_TO,
                    U_MSSM_LAT = incidencia.Latitud,
                    U_MSSM_LON = incidencia.Longitud,
                    RowNum = incidencia.RowNum
                });

                businessPartner.BPAddresses = addresses;

                return businessPartner;
            }
            catch (Exception ex)
            {
                MainProcess.log.Error("GeolocalizacionDAO > transformToBusinessPartner() > " + ex.Message);
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
            public List<GeolocalizacionBean> value { get; set; }
        }
        #endregion
    }
}
