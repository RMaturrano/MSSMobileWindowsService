﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WServMobile_Test.entity
{
    public class CompanyBean
    {
        public int id { get; set; }
        public string descripcion { get; set; }
        public string base_datos { get; set; }
        public string usuario { get; set; }
        public string clave { get; set; }
        public string observacion { get; set; }
        public int LINEAS_ORDR { get; set; }
        public string EST_ORDR { get; set; }
        public string EST_ORCT { get; set; }
    }
}
