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
    public class Devolucion
    {

        public static void registrarDevolucionesEnSAP(CompanyBean sociedad)
        {
            string SessionId = string.Empty, RouteId = string.Empty;

            try
            {
                var listDevolucion = DevolucionDAO.obtenerDevolucion(MainProcess.mConn.urlGetDevolucion + "?id=" + sociedad.id);

                if (listDevolucion.Count > 0)
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
                        foreach (var devolucion in listDevolucion)
                        {
                            if (!DevolucionDAO.validarDevolucion(MainProcess.mConn.urlValidarDevolucion
                                                        + "?empId=" + sociedad.id
                                                        + "&clave=" + devolucion.ClaveMovil, devolucion))
                            {
                                int newDoc = DevolucionDAO.registrarDevolucion(SessionId, RouteId, MainProcess.mConn.urlServiceLayer, devolucion);
                                if (newDoc > 0)
                                {
                                    DevolucionDAO.actualizarPropiedades(devolucion.ClaveMovil, MainProcess.mConn.urlPatchDevolucion +
                                        "?empId=" + sociedad.id +
                                        "&rtId=" + devolucion.ClaveMovil,
                                        "{\"MIGRADO\":\"Y\",\"DOCENTRY\":" + newDoc + ", \"MENSAJE\":\"Borrador creado\"}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MainProcess.log.Error("Devolucion > registrarDevolucionesEnSAP() > " + ex.Message);
            }
        }

    }
}
