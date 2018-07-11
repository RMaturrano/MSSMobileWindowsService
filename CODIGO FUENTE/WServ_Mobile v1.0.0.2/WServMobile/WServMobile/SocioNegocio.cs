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
                        SessionId = loginResp.Cookies[0].Value.ToString();
                        RouteId = loginResp.Cookies[1].Value.ToString();

                        foreach (var cliente in listClientes)
                        {
                            if (cliente.Migrado.Equals("N"))
                            {
                                if (!ClienteDAO.validarCliente(MainProcess.mConn.urlValidarSocioNegocio
                                                            + "?empId=" + sociedad.id
                                                            + "&clave=" + cliente.ClaveMovil, cliente))
                                {
                                    string newDoc = ClienteDAO.registrarCliente(SessionId, RouteId, MainProcess.mConn.urlServiceLayer, cliente);
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

                        LoginDAO.cerrarSesion(SessionId, RouteId, MainProcess.mConn.urlServiceLayer);
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
            finally
            {
                if (!string.IsNullOrEmpty(SessionId) && !string.IsNullOrEmpty(RouteId))
                    LoginDAO.cerrarSesion(SessionId, RouteId, MainProcess.mConn.urlServiceLayer);
            }
        }
    }
}
