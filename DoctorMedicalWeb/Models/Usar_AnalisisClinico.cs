using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoctorMedicalWeb.Models
{
    public  class Usar_AnalisisClinico
    {
       

        public int? DoctSecuencia_fk { get; set; }
        public int? PaisSecuencia_fk { get; set; }
        public int? ClinSecuencia_fk { get; set; }
        public int? ConsSecuencia_fk { get; set; }
        public int? AClinSecuencia { get; set; }
        
        public string AClinCodigo { get; set; }
        [Display(Name = "Nombre")]
    
        [Required(ErrorMessage = "Favor introducir nombre")]
        public string AClinNombre { get; set; } 
        [Display(Name = "Descripción")]
        public string AClinDescripcion { get; set; }
        [Display(Name = "Tipo de Muestra")]
        public string AClinTipoDeMuestra { get; set; }
        [Display(Name = "Tiempo de Proceso")]
        public string AClinTiempoDeProceso { get; set; }
        [Display(Name = "Condición del paciente")]
        public string AClinCondicionesDelPaciente { get; set; }
        public bool EstaDesabilitado { get; set; }

     
    }
}