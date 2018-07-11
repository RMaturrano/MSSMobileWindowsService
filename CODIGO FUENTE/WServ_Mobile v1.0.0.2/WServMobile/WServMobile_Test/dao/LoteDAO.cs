using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using WServMobile_Test.helpers;

namespace WServMobile_Test.dao
{
    public class LoteDAO : RestCall
    {
        public static void invoke(string sessionId, string routeId, string urlSL)
        {
            IRestResponse response = makeRequest(Util.castURL(urlSL, "/") + "Returns(34)", Method.GET, sessionId, routeId, null);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

            }
        }
    }
}
