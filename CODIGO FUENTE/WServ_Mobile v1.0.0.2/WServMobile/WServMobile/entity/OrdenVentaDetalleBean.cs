using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WServMobile.entity
{
    public class OrdenVentaDetalleBean
    {
        public string Linea { get; set; }
        public string Articulo { get; set; }
        public string UnidadMedida { get; set; }
        public string Almacen { get; set; }
        public string Cantidad { get; set; }
        public string ListaPrecio { get; set; }
        public string PrecioUnitario { get; set; }
        public string PorcentajeDescuento { get; set; }
        public string Impuesto { get; set; }
    }
}
