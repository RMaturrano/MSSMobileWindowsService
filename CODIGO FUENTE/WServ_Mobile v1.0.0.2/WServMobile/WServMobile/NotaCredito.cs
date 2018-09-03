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
    public class NotaCredito
    {
        public static void registrarNotasCreditoEnSAP(CompanyBean sociedad)
        {
            string SessionId = string.Empty, RouteId = string.Empty;

            try
            {
                var listNotaCredito = NotaCreditoDAO.obtenerNotaCredito(MainProcess.mConn.urlGetNotaCredito + "?id=" + sociedad.id);

                if (listNotaCredito.Count > 0)
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
                        foreach (var notacredito in listNotaCredito)
                        {
                            if (!NotaCreditoDAO.validarNotaCredito(MainProcess.mConn.urlValidarNotaCredito
                                                        + "?empId=" + sociedad.id
                                                        + "&clave=" + notacredito.ClaveMovil, notacredito))
                            {
                                int newDoc = NotaCreditoDAO.registraNotaCredito(SessionId, RouteId, MainProcess.mConn.urlServiceLayer, notacredito);
                                if (newDoc > 0)
                                {
                                    NotaCreditoDAO.actualizarPropiedades(notacredito.ClaveMovil, MainProcess.mConn.urlPatchNotaCredito +
                                        "?empId=" + sociedad.id +
                                        "&cmId=" + notacredito.ClaveMovil,
                                        "{\"MIGRADO\":\"Y\",\"DOCENTRY\":" + newDoc + ", \"MENSAJE\":\"Borrador creado\"}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MainProcess.log.Error("NotaCredito > registrarNotasCreditoEnSAP() > " + ex.Message);
            }
        }
    }
}
