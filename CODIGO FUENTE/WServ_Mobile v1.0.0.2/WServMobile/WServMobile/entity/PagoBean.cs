using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WServMobile.entity
{
    public class PagoBean
    {
        public PagoBean()
        {
            U_MSSM_CRM = "Y";
        }

        public string ClaveMovil { get; set; }
        public string TransaccionMovil { get; set; }
        public string SocioNegocio { get; set; }
        public string EmpleadoVenta { get; set; }
        public string Comentario { get; set; }
        public string Glosa { get; set; }
        public string FechaContable { get; set; }
        public string TipoPago { get; set; }
        public string Moneda { get; set; }
        public string ChequeCuenta { get; set; }
        public string ChequeBanco { get; set; }
        public string ChequeVencimiento { get; set; }
        public double? ChequeImporte { get; set; }
        public int? ChequeNumero { get; set; }
        public string TransferenciaCuenta { get; set; }
        public string TransferenciaReferencia { get; set; }
        public double? TransferenciaImporte { get; set; }
        public string EfectivoCuenta { get; set; }
        public double? EfectivoImporte { get; set; }
        public string Migrado { get; set; }
        public string Actualizado { get; set; }
        public string Finalizado { get; set; }
        public string DocEntry { get; set; }
        public string Mensaje { get; set; }
        public int EMPRESA { get; set; }
        public string U_MSSM_CRM { get; set; }
        public string U_MSSM_CLM { get; set; }
        public string U_MSSM_TRM { get; set; }
        public List<PagoDetalleBean> Lineas { get; set; }
    }
}
