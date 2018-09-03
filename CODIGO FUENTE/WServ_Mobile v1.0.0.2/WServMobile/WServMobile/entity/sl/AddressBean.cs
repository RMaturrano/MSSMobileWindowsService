using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WServMobile.entity
{
    public class AddressBean
    {
        //AddressType
        //bo_BillTo => Si tipo es B
        //Sino bo_ShipTo => Entrega

        public string AddressType { get; set; }
        public string AddressName { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Block { get; set; }
        public string Street { get; set; }
        public string U_MSSM_LAT { get; set; }
        public string U_MSSM_LON { get; set; }
        public string U_MSS_COVE { get; set; }
        public string U_MSS_RUTA { get; set; }
        public string U_MSS_ZONA { get; set; }
        public string U_MSS_CANA { get; set; }
        public string U_MSS_GIRO { get; set; }
    }
}
