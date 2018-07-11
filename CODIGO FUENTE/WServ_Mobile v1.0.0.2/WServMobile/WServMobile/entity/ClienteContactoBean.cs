using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WServMobile.entity
{
    public class ClienteContactoBean
    {
        public string IdContacto { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string Apellidos { get; set; }
        public string Posicion { get; set; }
        public string Direccion { get; set; }
        public string CorreoElectronico { get; set; }
        public string Telefono1 { get; set; }
        public string Telefono2 { get; set; }
        public string TelefonoMovil { get; set; }
    }
}
