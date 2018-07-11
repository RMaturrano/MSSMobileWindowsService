using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WServMobile_Test.entity;
using WServMobile_Test.helpers;

namespace WServMobile_Test.dao
{
    public class LoginDAO:RestCall
    {
        public static IRestResponse iniciarSesion(CompanyBean e, string baseUrl)
        {
            try
            {
                var entity = new SLLoginBean()
                {
                    CompanyDB = e.base_datos,
                    UserName = e.usuario,
                    Password = e.clave
                };

                IRestResponse response = makeRequest(Util.castURL(baseUrl, "/") + Constant.LOGIN, Method.POST, null, null, entity);
                return response;
            }
            catch (Exception ex) 
            {
                MainProcess.log.Error("LoginDAO > iniciarSesion() > " + ex.Message);
                return null;
            }
        }

        public static void cerrarSesion(string sessionId, string routeId, string baseUrl)
        {
            try
            {
                makeRequest(Util.castURL(baseUrl, "/") + Constant.LOGOUT, Method.POST, sessionId, routeId);
            }
            catch (Exception ex)
            {
                MainProcess.log.Error("LoginDAO > cerrarSesion() > " + ex.Message);
            }
        }
    }
}
