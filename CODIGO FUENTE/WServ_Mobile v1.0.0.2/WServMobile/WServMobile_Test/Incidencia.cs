using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using WServMobile_Test.dao;
using WServMobile_Test.entity;

namespace WServMobile_Test
{
    public class Incidencia
    {
        public static void registrarIncidenciasEnSAP(CompanyBean sociedad)
        {
            string SessionId = string.Empty, RouteId = string.Empty;

            try
            {
                var listClientes = IncidenciaDAO.obtenerIncidencias(MainProcess.mConn.urlGetIncidencia + "?id=" + sociedad.id);

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
                                if (!IncidenciaDAO.validarIncidencia(MainProcess.mConn.urlValidarIncidencia
                                                            + "?empId=" + sociedad.id
                                                            + "&clave=" + cliente.ClaveMovil, cliente))
                                {
                                    string newDoc = IncidenciaDAO.registrarIncidencia(SessionId, RouteId, MainProcess.mConn.urlServiceLayer, cliente);
                                    if (!string.IsNullOrEmpty(newDoc))
                                    {
                                        IncidenciaDAO.actualizarPropiedades(cliente.ClaveMovil, MainProcess.mConn.urlPatchIncidencia +
                                            "?empId=" + sociedad.id +
                                            "&acId=" + cliente.ClaveMovil,
                                            "{\"MIGRADO\":\"Y\",\"CODIGOSAP\":" + newDoc + "}");
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
            finally
            {
                if (!string.IsNullOrEmpty(SessionId) && !string.IsNullOrEmpty(RouteId))
                    LoginDAO.cerrarSesion(SessionId, RouteId, MainProcess.mConn.urlServiceLayer);
            }
        }
    }
}
