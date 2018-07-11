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
                    IRestResponse loginResp = LoginDAO.iniciarSesion(sociedad, MainProcess.mConn.urlServiceLayer);
                    if (loginResp.StatusCode == HttpStatusCode.OK)
                    {
                        SessionId = loginResp.Cookies[0].Value.ToString();
                        RouteId = loginResp.Cookies[1].Value.ToString();

                        foreach (var pago in listPagos)
                        {
                            if (pago.Migrado.Equals("N"))
                            {
                                string docEntryPago; 
                                bool existePago = PagoDAO.validarPago(MainProcess.mConn.urlValidarPagoRecibido
                                                            + "?empId=" + sociedad.id
                                                            + "&clave=" + pago.ClaveMovil, pago, sociedad.EST_ORCT, out docEntryPago);

                                //1. Registro
                                if (!existePago)
                                {
                                    int newDoc = PagoDAO.registrarPago(SessionId, RouteId, MainProcess.mConn.urlServiceLayer, pago, sociedad.EST_ORCT);
                                    if (newDoc > 0)
                                    {
                                        PagoDAO.actualizarPropiedades(pago.ClaveMovil, MainProcess.mConn.urlPatchPagoRecibido +
                                            "?empId=" + sociedad.id +
                                            "&icId=" + pago.ClaveMovil,
                                            "{\"Migrado\":\"Y\",\"DocEntry\":" + newDoc + ", \"Mensaje\":\"" +
                                            (sociedad.EST_ORCT.Equals(Constant.DOCUMENTO_BORRADOR) 
                                                    ? "Borrador creado":"Documento creado") + "\"}");
                                    }
                                }
                                //2. Actualización
                                else if (existePago && pago.Actualizado.Equals("Y"))
                                {
                                    pago.DocEntry = docEntryPago;
                                    bool succesUpdate = PagoDAO.actualizarPago(SessionId, RouteId, MainProcess.mConn.urlServiceLayer, pago, sociedad.EST_ORCT);
                                    if (succesUpdate)
                                    {
                                        PagoDAO.actualizarPropiedades(pago.ClaveMovil, MainProcess.mConn.urlPatchPagoRecibido +
                                            "?empId=" + sociedad.id +
                                            "&icId=" + pago.ClaveMovil,
                                            "{\"Migrado\":\"Y\",\"Actualizado\":\"Y\",\"DocEntry\":"+pago.DocEntry+", \"Mensaje\":\"" +
                                            (sociedad.EST_ORCT.Equals(Constant.DOCUMENTO_BORRADOR)
                                                    ? "Borrador actualizado" : "Documento actualizado") + "\"}");
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
                MainProcess.log.Error("PagoRecibido > registrarPagosEnSAP() > " + ex.Message);
            }
            finally
            {
                if (!string.IsNullOrEmpty(SessionId) && !string.IsNullOrEmpty(RouteId))
                    LoginDAO.cerrarSesion(SessionId, RouteId, MainProcess.mConn.urlServiceLayer);
            }
        }
    }
}
