using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using WServMobile.dao;
using WServMobile.entity;

namespace WServMobile
{
    public class SocioNegocio
    {
        public static void registrarSociosEnSAP(CompanyBean sociedad)
        {
            string SessionId = string.Empty, RouteId = string.Empty;

            try
            {
                var listClientes = ClienteDAO.obtenerClientes(MainProcess.mConn.urlGetSocioNegocio + "?id=" + sociedad.id + "&mig='N'");

                if (listClientes.Count > 0)
                {
                    IRestResponse loginResp = LoginDAO.iniciarSesion(sociedad, MainProcess.mConn.urlServiceLayer);
                    if (loginResp.StatusCode == HttpStatusCode.OK)
                    {
                        sociedad.inSession = true;
                        SessionId = loginResp.Cookies[0].Value.ToString();
                        RouteId = loginResp.Cookies[1].Value.ToString();
                        sociedad.sessionId = SessionId;
                        sociedad.routeId = RouteId;

                        foreach (var cliente in listClientes)
                        {
                            if (cliente.Migrado.Equals("N"))
                            {
                                if (!ClienteDAO.validarCliente(MainProcess.mConn.urlValidarSocioNegocio
                                                            + "?empId=" + sociedad.id
                                                            + "&clave=" + cliente.ClaveMovil, cliente))
                                {
                                    bool locEnabled = true;
                                    if (sociedad.LOCALIZACION == null)
                                        locEnabled = false;
                                    else if (sociedad.LOCALIZACION.Trim().Equals("N"))
                                        locEnabled = false;

                                    string newDoc = ClienteDAO.registrarCliente(SessionId, RouteId, MainProcess.mConn.urlServiceLayer, cliente, locEnabled);
                                    if (!string.IsNullOrEmpty(newDoc))
                                    {
                                        ClienteDAO.actualizarPropiedades(cliente.ClaveMovil, MainProcess.mConn.urlPatchSocioNegocio +
                                            "?empId=" + sociedad.id +
                                            "&bpId=" + cliente.ClaveMovil,
                                            "{\"Migrado\":\"Y\",\"CARDCODE\":\"" + newDoc + "\"}");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        MainProcess.log.Error("Login Failed >" + sociedad.descripcion + " > " + loginResp.Content);
                    }
                }
            }
            catch (Exception ex)
            {
                MainProcess.log.Error("SocioNegocio > registrarSociosEnSAP() > " + ex.Message);
            }
        }
    }
}
