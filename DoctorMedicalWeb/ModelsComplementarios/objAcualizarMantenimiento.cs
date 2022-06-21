using DoctorMedicalWeb.Libreria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoctorMedicalWeb.ModelsComplementarios
{
    
    public class objAcualizarMantenimiento
    {
        public Maintenance Maintenance { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }
}