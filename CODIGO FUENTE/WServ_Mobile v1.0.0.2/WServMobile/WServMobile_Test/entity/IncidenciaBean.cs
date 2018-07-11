using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WServMobile_Test.entity
{
    public class IncidenciaBean
    {
        public string ClaveMovil { get; set; }
        public string Origen { get; set; }
        public string CodigoCliente { get; set; }
        public string CodigoContacto { get; set; }
        public string CodigoDireccion { get; set; }
        public int CodigoMotivo { get; set; }
        public string DescripcionMotivo { get; set; }
        public string Comentarios { get; set; }
        public int Vendedor { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string FechaCreacion { get; set; }
        public string HoraCreacion { get; set; }
        public string ModoOffLine { get; set; }
        public int ClaveFactura { get; set; }
        public string SerieFactura { get; set; }
        public int CorrelativoFactura { get; set; }
        public string TipoIncidencia { get; set; }
        public string FechaPago { get; set; }
        public string Migrado { get; set; }
        public string Actualizado { get; set; }
        public string Finalizado { get; set; }
        public int CodigoSAP { get; set; }
        public string Mensaje { get; set; }
        public int EMPRESA { get; set; }
    }
}
