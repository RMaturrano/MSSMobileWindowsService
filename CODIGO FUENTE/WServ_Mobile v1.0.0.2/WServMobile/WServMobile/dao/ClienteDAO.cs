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
    public class ClienteDAO : RestCall
    {
        public static List<ClienteBean> obtenerClientes(string url)
        {
            var mList = new List<ClienteBean>();

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
                MainProcess.log.Error("ClienteDAO > obtenerClientes() > " + ex.Message);
            }

            return mList;
        }

        public static bool validarCliente(string url, ClienteBean orden)
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
                            actualizarPropiedades(orden.ClaveMovil,
                                MainProcess.mConn.urlPatchSocioNegocio +
                                                "?empId=" + orden.EMPRESA +
                                                "&bpId=" + orden.ClaveMovil,
                                "{\"Migrado\":\"Y\", \"CARDCODE\": \"" + cardCode + "\"}");
                        }
                        else if(objCount > 1)
                        {
                            exists = true;
                            MainProcess.log.Error("ClienteDAO > validarCliente() > Document " + orden.ClaveMovil + " > El documento ya fue creado en SAP");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exists = true;
                MainProcess.log.Error("ClienteDAO > validarCliente() > " + ex.Message);
            }

            return exists;
        }

        public static string registrarCliente(string sessionId, string routeId, string urlSL, ClienteBean cliente)
        {
            string res = string.Empty;
            try
            {
                var document = transformBusinessPartner(cliente);
                if (document != null)
                {
                    File.WriteAllText(Util.castURL(MainProcess.mConn.pathJSONLog, "\\") + "CLIENTE_" + cliente.ClaveMovil
                                    + ".json",
                                    SimpleJson.SerializeObject(document));
                    IRestResponse response = makeRequest(Util.castURL(urlSL, "/") + Constant.BUSINESS_PARTNERS, Method.POST, sessionId, routeId, document);
                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        JObject jObject = JObject.Parse(response.Content.ToString());
                        res = jObject["CardCode"].ToString().Trim();
                    }
                    else
                    {
                        res = string.Empty;
                        MainProcess.log.Error("ClienteDAO > registrarCliente() > Document BusinessPartner " +
                            cliente.ClaveMovil + " > " + response.Content);
                        actualizarPropiedades(cliente.ClaveMovil,
                            MainProcess.mConn.urlPatchSocioNegocio +
                                            "?empId=" + cliente.EMPRESA +
                                            "&bpId=" + cliente.ClaveMovil,
                            "{\"Migrado\":\"N\", \"MENSAJE\": \"" + Util.replaceEscChar(response.Content) + "\"}");
                    }
                }
            }
            catch (Exception ex)
            {
                res = string.Empty;
                MainProcess.log.Error("ClienteDAO > registrarCliente() > " + ex.Message);
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
                        MainProcess.log.Error("ClienteDAO > actualizarPropiedades() > Document " +
                        claveMovil + " > " + messageError);
                    }
                }
                else
                {
                    res = false;
                    MainProcess.log.Error("ClienteDAO > actualizarPropiedades() > Document " +
                        claveMovil + " > " + response.Content);
                }
            }
            catch (Exception ex)
            {
                MainProcess.log.Error("ClienteDAO > actualizarPropiedades() > " + ex.Message);
            }

            return res;
        }

        public static BusinessPartnerBean transformBusinessPartner(ClienteBean ordr)
        {
            try
            {
                var businessPartner = new BusinessPartnerBean();

                businessPartner.CardCode = "C" + ordr.NumeroDocumento.PadLeft(11, '0');
                businessPartner.U_MSSL_BTP = ordr.TipoPersona;
                businessPartner.U_MSSL_BTD = ordr.TipoDocumento;
                businessPartner.U_MSSL_BAP = ordr.ApellidoPaterno;
                businessPartner.U_MSSL_BAM = ordr.ApellidoMaterno;
                businessPartner.U_MSSL_BN1 = ordr.PrimerNombre;
                businessPartner.U_MSSL_BN2 = ordr.SegundoNombre;
                businessPartner.FederalTaxID = ordr.NumeroDocumento;
                businessPartner.CardName = ordr.NombreRazonSocial;
                businessPartner.CardForeignName = ordr.NombreComercial;
                businessPartner.Phone1 = ordr.Telefono1;
                businessPartner.Phone2 = ordr.Telefono2;
                businessPartner.Cellular = ordr.TelefonoMovil;
                businessPartner.EmailAddress = ordr.CorreoElectronico;
                businessPartner.GroupCode = ordr.GrupoSocio;
                if(ordr.CondicionPago != -1)
                    businessPartner.PayTermsGrpCode = ordr.CondicionPago;
                if(ordr.ListaPrecio != -1)
                    businessPartner.PriceListNum = ordr.ListaPrecio;
                businessPartner.Indicator = ordr.Indicador;
                businessPartner.U_MSSM_CLM = ordr.ClaveMovil;
                businessPartner.U_MSSM_TRM = "05";
                businessPartner.U_MSSM_POA = ordr.POSEEACTIVOS;
                businessPartner.ProjectCode = ordr.PROYECTO;
                businessPartner.U_MSSM_TRE = ordr.TipoRegistro;

                if(ordr.Contacts.Count > 0)
                {
                    var contacts = new List<ContactBean>();
                    foreach (var c in ordr.Contacts)
                    {
                        contacts.Add(new ContactBean()
                        {
                            Name = c.IdContacto,
                            FirstName = c.PrimerNombre,
                            MiddleName = c.SegundoNombre,
                            LastName = c.Apellidos,
                            Address = c.Direccion,
                            E_Mail = c.CorreoElectronico,
                            Phone1 = c.Telefono1,
                            Phone2 = c.Telefono2,
                            MobilePhone = c.TelefonoMovil,
                            Position = c.Posicion
                        });
                    }

                    businessPartner.ContactEmployees = contacts;
                }
                

                if(ordr.Directions.Count > 0)
                {
                    var addresses = new List<AddressBean>();
                    foreach (var d in ordr.Directions)
                    {
                        addresses.Add(new AddressBean()
                        {
                            AddressType = d.Tipo.Equals("B") ? Constant.BILL_TO : Constant.SHIP_TO,
                            AddressName = d.Codigo,
                            Country = d.Pais,
                            State = d.Departamento,
                            City = d.Provincia,
                            County = d.Distrito,
                            Street = d.Calle,
                            U_MSSM_LAT = d.Latitud,
                            U_MSSM_LON = d.Longitud,
                            U_MSS_COVE = ordr.VENDEDOR,
                            U_MSS_RUTA = d.Ruta,
                            U_MSS_ZONA = d.Zona,
                            U_MSS_CANA = d.Canal,
                            U_MSS_GIRO = d.Giro 
                        });
                    }

                    businessPartner.BPAddresses = addresses;
                }
                

                return businessPartner;
            }
            catch (Exception ex)
            {
                MainProcess.log.Error("ClienteDAO > transformBusinessPartner() > " + ex.Message);
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
            public List<ClienteBean> value { get; set; }
        }
        #endregion
    }
}
