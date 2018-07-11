using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WServMobile.entity;
using WServMobile.helpers;
using WServMobile;

namespace WServMobile.dao
{
    public class CompanyDAO : RestCall
    {
        public static List<CompanyBean> obtenerSociedades(string url)
        {
            var mlist = new List<CompanyBean>();

            try
            {
                IRestResponse response = makeRequest(url, Method.GET);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (response.Content.Contains(Constant.STATUS_SUCCESS))
                    {
                        ResponseBean objResponse = SimpleJson.DeserializeObject<ResponseBean>(response.Content);
                        if (objResponse.ResponseStatus.Equals(Constant.STATUS_SUCCESS))
                        {
                            if (objResponse.Response.message.value != null &&
                                objResponse.Response.message.value.Count > 0)
                                mlist.AddRange(objResponse.Response.message.value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MainProcess.log.Error("CompanyDAO > obtenerSociedades() > " + ex.Message);
            }

            return mlist;
        }
    }

    #region MODEL_RESPONSE
    public class ResponseBean
    {
        public string ResponseStatus { get; set; }
        public string ResponseType { get; set; }
        public int ResponseCount { get; set; }
        public Response Response { get; set; }
    }

    public class Response
    {
        public int code { get; set; }
        public Message message { get; set; }
    }

    public class Message
    {
        public string lang { get; set; }
        public List<CompanyBean> value { get; set; }
    }
    #endregion
}
