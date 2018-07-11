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
    public class PagoRecibidoDAO:RestCall
    {
        public static List<PagoRecibidoBean> obtenerPagoRecibido(string url)
        {
            var mList = new List<PagoRecibidoBean>();

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
                                mList.AddRange(objResponse.Response.message.value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MainProcess.log.Error("PagoRecibidoDAO > obtenerPagoRecibido() > " + ex.Message);
            }

            return mList;
        }
    }
}
