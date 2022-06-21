using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoctorMedicalWeb.Models
{

    public class Usar_Medicamento
    {
        public int? DoctSecuencia_fk { get; set; }
        public int? PaisSecuencia_fk { get; set; }
        public int? ClinSecuencia_fk { get; set; }
        public int? ConsSecuencia_fk { get; set; }
        public int? MediSecuencia { get; set; }
        public string MediCodigo { get; set; }
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "Favor introducir nombre")]
        public string MediNombre { get; set; }
        [Display(Name = "Laboratorio")]
        public string MediLaboratorio { get; set; }
        [Display(Name = "Família")]
        public string MediFamilia { get; set; }
        [Display(Name = "Descripción")]
        public string MediDescripcion { get; set; }
        public bool EstaDesabilitado { get; set; }
    }

}