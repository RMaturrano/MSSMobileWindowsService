using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WServMobile_Test.entity
{
    public class GeolocalizacionBean
    {
        public string ClaveMovil { get; set; }
        public string CodigoCliente { get; set; }
        public string CodigoDireccion { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public int Empresa { get; set; }
        public string Tipo { get; set; }
        public int RowNum { get; set; }
    }
}
