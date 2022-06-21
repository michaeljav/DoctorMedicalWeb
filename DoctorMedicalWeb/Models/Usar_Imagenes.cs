using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoctorMedicalWeb.Models
{
    public class Usar_Imagenes
    {


        public int? DoctSecuencia_fk { get; set; }
        public int? PaisSecuencia_fk { get; set; }
        public int? ClinSecuencia_fk { get; set; }
        public int? ConsSecuencia_fk { get; set; }

        public int? ImagSecuencia { get; set; }
        public string ImagCodigo { get; set; }
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "Favor introducir nombre")]
        public string ImagNombre { get; set; }
        [Display(Name = "Descripción")]
        public string ImagDescripcion { get; set; }
        public bool EstaDesabilitado { get; set; }





    }
}