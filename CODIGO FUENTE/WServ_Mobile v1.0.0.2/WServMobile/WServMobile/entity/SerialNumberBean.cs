using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WServMobile.entity
{
    public class SerialNumberBean
    {
        public string ManufacturerSerialNumber { get; set; }
        public string InternalSerialNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string ManufactureDate { get; set; }
        public string BatchID { get; set; }
        public string SystemSerialNumber { get; set; }
        public string BaseLineNumber { get; set; }
        public string Quantity { get; set; }
        public string TrackingNote { get; set; }
        public string TrackingNoteLine { get; set; }
    }
}
