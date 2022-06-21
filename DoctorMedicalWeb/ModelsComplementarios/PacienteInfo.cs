using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoctorMedicalWeb.ModelsComplementarios
{
    public class PacienteInfo
    {

        public int? DoctSecuencia_fk { get; set; }
        public int? PaisSecuencia_fk { get; set; }
        public int? ClinSecuencia_fk { get; set; }
        public int? ConsSecuencia_fk { get; set; }
        public int? PaciSecuencia { get; set; }
        public string PaciCodigo { get; set; }

        [Required(ErrorMessage = "Introduzca documento")]
        public string PaciDocumento { get; set; }
        [Display(Name = "Cédula o Pasaporte")]
        [Required(ErrorMessage = "Seleccione tipo documento")]
        public int TDSecuencia { get; set; }
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "Favor ingresar nombre")]
        public string PaciNombre { get; set; }
        [Display(Name = "Primer Apellido")]
        [Required(ErrorMessage = "Favor ingresar primer apellido")]
        public string PaciApellido1 { get; set; }
        [Display(Name = "Segundo Apellido")]
        public string PaciApellido2 { get; set; }
        public string NombreCompleto
        {

            get
            {
                string compl = this.PaciNombre + " " + this.PaciApellido1 + " " + this.PaciApellido2;
                return compl;
            }

        }



        [Display(Name = "Fecha Nacimiento")]
        [Required(ErrorMessage = "Favor ingresar fecha nacimiento")]
        public Nullable<System.DateTime> PaciFechaNacimiento { get; set; }

        public int GSangSecuencia_fk { get; set; }


        public string PaciFechaNacimientoString
        {
            get
            {
                string fecha = "";
                if (PaciFechaNacimiento != null)
                {
                    fecha = this.PaciFechaNacimiento.Value.ToString("dd/MM/yyyy");

                }
                return fecha;
            }
            set
            {
                string fecha = "";
                if (PaciFechaNacimiento != null)
                {
                    fecha = this.PaciFechaNacimiento.Value.ToString("dd/MM/yyyy");

                }
                value = fecha;
            }
        }

        [Display(Name = "Edad")]
        public int? PaciEdad { get; set; }
        string ultimaFechaDeconsulta { get; set; }
    
        public string PaciFotoPath { get; set; }
        public string PaciFotoNombre { get; set; }


    }
}