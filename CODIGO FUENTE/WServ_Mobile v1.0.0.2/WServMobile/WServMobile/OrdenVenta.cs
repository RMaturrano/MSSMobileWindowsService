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
    public class OrdenVenta
    {
        public static void registrarOrdenesEnSAP(CompanyBean sociedad)
        {
            string SessionId = string.Empty, RouteId = string.Empty;

            try
            {
                var listOrdenVenta = OrdenVentaDAO.obtenerOrdenVenta(MainProcess.mConn.urlGetOrdenVenta + "?id=" + sociedad.id);

                if (listOrdenVenta.Count > 0)
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
                        foreach (var ordenVenta in listOrdenVenta)
                        {
                            ordenVenta.motivoTraslado = sociedad.MOTIVO;

                            if (!OrdenVentaDAO.validarOrdenVenta(MainProcess.mConn.urlValidarOrdenVenta
                                                        + "?empId=" + sociedad.id
                                                        + "&clave=" + ordenVenta.ClaveMovil, ordenVenta, sociedad.EST_ORDR))
                            {
                                bool isLocEnabled = true;
                                if (sociedad.LOCALIZACION == null || sociedad.LOCALIZACION.Trim().Equals("N"))
                                    isLocEnabled = false;

                                int newDoc = OrdenVentaDAO.registrarOrdenVenta(SessionId, RouteId, MainProcess.mConn.urlServiceLayer, ordenVenta, sociedad.EST_ORDR, isLocEnabled);
                                if (newDoc > 0)
                                {
                                    OrdenVentaDAO.actualizarPropiedades(ordenVenta.ClaveMovil, MainProcess.mConn.urlPatchOrdenVenta +
                                        "?empId=" + sociedad.id +
                                        "&ordrId=" + ordenVenta.ClaveMovil,
                                        "{\"Migrado\":\"Y\",\"DocEntry\":" + newDoc + ", \"Mensaje\":\"" +
                                            (sociedad.EST_ORDR.Equals(Constant.DOCUMENTO_BORRADOR)
                                                    ? "Borrador creado" : "Documento creado") + "\"}");
                                }
                                else if (newDoc == -99)
                                {
                                    OrdenVentaDAO.actualizarPropiedades(ordenVenta.ClaveMovil, MainProcess.mConn.urlPatchOrdenVenta +
                                        "?empId=" + sociedad.id +
                                        "&ordrId=" + ordenVenta.ClaveMovil,
                                        "{\"Migrado\":\"Y\", \"Mensaje\":\"Borrador (pendiente autorización) creado\"}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MainProcess.log.Error(sociedad.descripcion + " > OrdenVenta > registrarOrdenesEnSAP() > " + ex.Message);
            }
        }

        public static void actualizarOrdenesEnBDMobile(CompanyBean sociedad)
        {
            try
            {

            }
            catch (Exception ex)
            {
                MainProcess.log.Error("OrdenVenta > actualizarOrdenesEnBDMobile() > " + ex.Message);
            }
        }

    }
}
