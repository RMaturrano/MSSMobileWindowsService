using RestSharp;
using System;
using System.Net;
using WServMobile.dao;
using WServMobile.entity;

namespace WServMobile
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
                    SessionId = sociedad.sessionId;
                    RouteId = sociedad.routeId;

                    if (!sociedad.inSession)
                    {
                        IRestResponse loginResp = LoginDAO.iniciarSesion(sociedad, MainProcess.mConn.urlServiceLayer);
                        if (loginResp.StatusCode == HttpStatusCode.OK)
                        {
                            sociedad.inSession = true;
                            SessionId = loginResp.Cookies[0].Value.ToString();
                            RouteId = loginResp.Cookies[1].Value.ToString();
                            sociedad.sessionId = SessionId;
                            sociedad.routeId = RouteId;
                        }
                        else
                            MainProcess.log.Error("Login Failed >" + sociedad.descripcion + " > " + loginResp.Content);
                    }

                    if (sociedad.inSession)
                    {
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
                                            "{\"MIGRADO\":\"Y\",\"CODIGOSAP\":" + newDoc + ", \"MENSAJE\":\"Documento creado\"}");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MainProcess.log.Error("Incidencia > registrarIncidenciasEnSAP() > " + ex.Message);
            }
        }
    }
}
