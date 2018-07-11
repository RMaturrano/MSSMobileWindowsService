using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using WServMobile.entity;

namespace WServMobile.helpers
{
    public class RestCall
    {
        public static IRestResponse makeRequest(string urlRequest, RestSharp.Method method, string sessionID = null, string routeID = null, object body = null, RequestParBean param = null)
        {

            var res = new RestClient(urlRequest);
            ServicePointManager.ServerCertificateValidationCallback = MyCertHandler;

            var request = new RestRequest(method);

            if (sessionID != null)
                request.AddCookie("B1SESSION", sessionID);

            if (routeID != null)
                request.AddCookie("ROUTEID", routeID);

            if (param != null)
            {
                request.AddParameter("empId", param.empresaId);
                request.AddParameter("ordrId", param.claveDocumento);
            }

            if (body != null)
            {
                request.AddJsonBody(body);
            }

            IRestResponse response = res.Execute(request);
            return response;
        }

        static bool MyCertHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors error)
        {
            return true;
        }
    }
}
