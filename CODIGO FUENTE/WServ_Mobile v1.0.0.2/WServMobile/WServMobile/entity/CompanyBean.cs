using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WServMobile.entity
{
    public class CompanyBean
    {
        public CompanyBean()
        {
            inSession = false;
            sessionId = string.Empty;
            routeId = string.Empty;
        }

        public int id { get; set; }
        public string descripcion { get; set; }
        public string base_datos { get; set; }
        public string usuario { get; set; }
        public string clave { get; set; }
        public string observacion { get; set; }
        public int LINEAS_ORDR { get; set; }
        public string EST_ORDR { get; set; }
        public string EST_ORCT { get; set; }
        public string MOTIVO { get; set; }
        public string LOCALIZACION { get; set; }
        public bool inSession { get; set; }
        public string sessionId { get; set; }
        public string routeId { get; set; }
    }
}
