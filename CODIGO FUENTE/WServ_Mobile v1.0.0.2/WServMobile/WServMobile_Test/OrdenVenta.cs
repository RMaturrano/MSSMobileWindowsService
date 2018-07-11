using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WServMobile_Test.dao;
using WServMobile_Test.entity;
using WServMobile_Test.helpers;

namespace WServMobile_Test
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
                    IRestResponse loginResp = LoginDAO.iniciarSesion(sociedad, MainProcess.mConn.urlServiceLayer);
                    if (loginResp.StatusCode == HttpStatusCode.OK)
                    {
                        SessionId = loginResp.Cookies[0].Value.ToString();
                        RouteId = loginResp.Cookies[1].Value.ToString();

                        foreach (var ordenVenta in listOrdenVenta)
                        {
                            if (!OrdenVentaDAO.validarOrdenVenta(MainProcess.mConn.urlValidarOrdenVenta
                                                        + "?empId=" + sociedad.id
                                                        + "&clave=" + ordenVenta.ClaveMovil, ordenVenta, sociedad.EST_ORDR))
                            {
                                int newDoc = OrdenVentaDAO.registrarOrdenVenta(SessionId, RouteId, MainProcess.mConn.urlServiceLayer, ordenVenta, sociedad.EST_ORDR);
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
                    else
                    {
                        MainProcess.log.Error("Login Failed >" + sociedad.descripcion + " > " + loginResp.Content);
                    }
                }
            }
            catch (Exception ex)
            {
                MainProcess.log.Error("OrdenVenta > registrarOrdenesEnSAP() > " + ex.Message);
            }
            finally
            {
                if (!string.IsNullOrEmpty(SessionId) && !string.IsNullOrEmpty(RouteId))
                    LoginDAO.cerrarSesion(SessionId, RouteId, MainProcess.mConn.urlServiceLayer);
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
