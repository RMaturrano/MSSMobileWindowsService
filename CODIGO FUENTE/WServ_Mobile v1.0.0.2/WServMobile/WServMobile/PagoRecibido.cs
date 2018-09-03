using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WServMobile.dao;
using WServMobile.entity;
using WServMobile.helpers;

namespace WServMobile
{
    public class PagoRecibido
    {
        public static void registrarPagosEnSAP(CompanyBean sociedad)
        {
            string SessionId = string.Empty, RouteId = string.Empty;

            try
            {
                var listPagos = PagoDAO.obtenerPagos(MainProcess.mConn.urlGetPagoRecibido + "?id=" + sociedad.id);

                if (listPagos.Count > 0)
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
                        foreach (var pago in listPagos)
                        {
                            if (pago.Migrado.Equals("N"))
                            {
                                if (!PagoDAO.validarPago(MainProcess.mConn.urlValidarPagoRecibido
                                                            + "?empId=" + sociedad.id
                                                            + "&clave=" + pago.ClaveMovil, pago, sociedad.EST_ORCT))
                                {
                                    int newDoc = PagoDAO.registrarPago(SessionId, RouteId, MainProcess.mConn.urlServiceLayer, pago, sociedad.EST_ORCT);
                                    if (newDoc > 0)
                                    {
                                        PagoDAO.actualizarPropiedades(pago.ClaveMovil, MainProcess.mConn.urlPatchPagoRecibido +
                                            "?empId=" + sociedad.id +
                                            "&icId=" + pago.ClaveMovil,
                                            "{\"Migrado\":\"Y\",\"DocEntry\":" + newDoc + ", \"Mensaje\":\"" +
                                            (sociedad.EST_ORCT.Equals(Constant.DOCUMENTO_BORRADOR)
                                                    ? "Borrador creado" : "Documento creado") + "\"}");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MainProcess.log.Error("PagoRecibido > registrarPagosEnSAP() > " + ex.Message);
            }
        }
    }
}
