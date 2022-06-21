using DoctorMedicalWeb.App_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoctorMedicalWeb.ModelsComplementarios
{
    public class UsuarioLoguiado
    {
      public  Usuario usuario { get; set; }
        public vw_UsuarioConsultorios Consultorio { get; set; }
        public int doctSecuencia { get; set; }
        public int persSecuencia { get; set; }
        public string imagPath { get; set; }
        public string NombreCompleto { get; set; }
       


  
    }
}