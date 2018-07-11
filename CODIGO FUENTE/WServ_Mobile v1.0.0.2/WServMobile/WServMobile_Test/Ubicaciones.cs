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
    public class Ubicaciones
    {
        public static void actualizarUbicacionesEnSAP(CompanyBean sociedad)
        {
            string SessionId = string.Empty, RouteId = string.Empty;

            try
            {
                var listUbicaciones = GeolocalizacionDAO.obtenerPendientes(MainProcess.mConn.urlGetUbicaciones + "?id=" + sociedad.id);

                if (listUbicaciones.Count > 0)
                {
                    IRestResponse loginResp = LoginDAO.iniciarSesion(sociedad, MainProcess.mConn.urlServiceLayer);
                    if (loginResp.StatusCode == HttpStatusCode.OK)
                    {
                        SessionId = loginResp.Cookies[0].Value.ToString();
                        RouteId = loginResp.Cookies[1].Value.ToString();

                        foreach (var ubicacion in listUbicaciones)
                        {
                            GeolocalizacionDAO.actualizarDireccion(SessionId, RouteId, MainProcess.mConn.urlServiceLayer, ubicacion);
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
                MainProcess.log.Error("SocioNegocio > actualizarUbicacionesEnSAP() > " + ex.Message);
            }
            finally
            {
                if (!string.IsNullOrEmpty(SessionId) && !string.IsNullOrEmpty(RouteId))
                    LoginDAO.cerrarSesion(SessionId, RouteId, MainProcess.mConn.urlServiceLayer);
            }
        }
    }
}
