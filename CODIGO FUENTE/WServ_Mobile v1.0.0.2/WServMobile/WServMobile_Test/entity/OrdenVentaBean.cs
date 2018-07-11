using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WServMobile_Test.entity
{
    public class OrdenVentaBean
    {
        public string ClaveMovil { get; set; }
        public string TransaccionMovil { get; set; }
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
        public string Actualizado { get; set; }
        public string Finalizado { get; set; }
        public string DocEntry { get; set; }
        public string Mensaje { get; set; }
        public string EMPRESA { get; set; }
        public string RangoDireccion { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string Hora { get; set; }
        public string ModoOffLine { get; set; }
        public List<OrdenVentaDetalleBean> Lineas { get; set; }
    }
}
