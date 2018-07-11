using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WServMobile_Test.entity
{
    public class ClienteBean
    {
        public ClienteBean()
        {
            Contacts = new List<ClienteContactoBean>();
            Directions = new List<ClienteDireccionBean>();
        }

        public string ClaveMovil { get; set; }
        public string TransaccionMovil { get; set; }
        public string TipoPersona { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string NombreRazonSocial { get; set; }
        public string NombreComercial { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string Telefono1 { get; set; }
        public string Telefono2 { get; set; }
        public string TelefonoMovil { get; set; }
        public string CorreoElectronico { get; set; }
        public int ListaPrecio { get; set; }
        public int GrupoSocio { get; set; }
        public int CondicionPago { get; set; }
        public string Indicador { get; set; }
        public string Zona { get; set; }
        public string Migrado { get; set; }
        public string Actualizado { get; set; }
        public string Finalizado { get; set; }
        public string POSEEACTIVOS { get; set; }
        public string VENDEDOR { get; set; }
        public string MENSAJE { get; set; }
        public string CARDCODE { get; set; }
        public string EMPRESA { get; set; }
        public List<ClienteContactoBean> Contacts { get; set; }
        public List<ClienteDireccionBean> Directions { get; set; }
    }
}
