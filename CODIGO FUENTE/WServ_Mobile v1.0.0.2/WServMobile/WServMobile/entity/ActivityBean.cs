using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WServMobile.entity
{
    public class ActivityBean
    {
        public ActivityBean()
        {
            U_MSSM_CRM = "Y";
            Location = -2;
            CardCode = null;
        }
        //M => cn_Meeting => Actividad reunion
        //T => Task => Tarea

        public string CardCode { get; set; }
        public int? ContactPersonCode { get; set; }
        //public string AddressType { get; set; }
        public string Activity { get; set; }    //Reunion o tarea 
        public int Location { get; set; }
        public int ActivityType { get; set; }   //Tipo
        public int SalesEmployee { get; set; }
        public DateTime ActivityDate { get; set; }
        public string ActivityTime { get; set; }
        public DateTime StartDate { get; set; }
        public string StartTime { get; set; }
        public DateTime EndDueDate { get; set; }
        public string EndTime { get; set; }
        public string AddressName { get; set; }
        public string DocType { get; set; }
        public string DocNum { get; set; }
        public string DocEntry { get; set; }
        public string U_MSSM_CRM { get; set; }  //CreadoMovil
        public string U_MSSM_CLM { get; set; }  //ClaveMovil
        public string U_MSSM_TRM { get; set; }  //TransaccionMovil
        public string U_MSSM_MOL { get; set; }    //ModoOffline
        public string U_MSSM_LAT { get; set; }    //Latitud
        public string U_MSSM_LON { get; set; }    //Longitud
        public DateTime? U_MSSM_FEC { get; set; }  //FechaCreacion
        public string U_MSSM_HOR { get; set; }    //HoraCreacion
        public string U_MSSM_MOT { get; set; }    //Motivo
        public string U_MSSM_SER { get; set; }    //Serie
        public string U_MSSM_COR { get; set; }    //correlativo
        public string U_MSSM_TIP { get; set; }    //TipoIncidencia
        public DateTime? U_MSSM_FCP { get; set; }  //FechaCompromisoPago
    }
}
