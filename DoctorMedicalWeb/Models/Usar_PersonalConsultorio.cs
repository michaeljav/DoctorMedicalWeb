using DoctorMedicalWeb.App_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoctorMedicalWeb.Models
{
    public class Usar_PersonalConsultorio
    {
    
            public int DoctSecuencia_fk { get; set; }
            public int persSecuencia_fk { get; set; }
            public int clinSecuencia_fk { get; set; }
            public int ConsSecuencia_fk { get; set; }

            public virtual Doctor Doctor { get; set; }
			    public bool EstaDesabilitado { get; set; }
     
    }
}