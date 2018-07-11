using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WServMobile.entity
{
    public class DevolucionBean
    {
        public string ClaveMovil { get; set; }
        public string ClaveBase { get; set; }
        public string SocioNegocio { get; set; }
        public string ListaPrecio { get; set; }
        public string CondicionPago { get; set; }
        public string Indicador { get; set; }
        public string Referencia { get; set; }
        public string FechaContable { get; set; }
        public string FechaVencimiento { get; set; }
        public string Moneda { get; set; }
        public string EmpleadoVenta { get; set; }
        public string DireccionFiscal { get; set; }
        public string DireccionEntrega { get; set; }
        public string Comentario { get; set; }
        public string Migrado { get; set; }
        public string DocEntry { get; set; }
        public string Mensaje { get; set; }
        public string EMPRESA { get; set; }
        public List<DevolucionDetalleBean> Lineas { get; set; }
    }
}
