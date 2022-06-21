using DoctorMedicalWeb.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoctorMedicalWeb.ModelsComplementarios
{
    public class Usar_RecetaComplementaria
    {


        public  Usar_RecetaComplementaria()
        {

        }



        //public Usar_Receta usar_receta;
        //public Usar_RecetaAnalisisClinico usar_RecetaAnalisisClinico;
        //public Usar_RecetaImagenes usar_RecetaImagenes;
        //public Usar_RecetaMedicamento usar_RecetaMedicamento;

 
        /*Receta*/
        public int? DoctSecuencia_fk { get; set; }
        public int? PaisSecuencia_fk { get; set; }
        public int? ClinSecuencia_fk { get; set; }
        public int? ConsSecuencia_fk { get; set; }
        [Display(Name = "Paciente codigo")]
        public int? PaciSecuencia_fk { get; set; }
                [Display(Name = "Secuencia")]
        public int? ReceSecuencia { get; set; }
        public Nullable<int> CMHistSecuencia_fk { get; set; }
        [Display(Name = "Fecha")]
        public Nullable<System.DateTime> ReceFecha { get; set; }
        //public string ReceFechaString { get; set; }
      
        
        public string ReceFechaString
        {
            get
            {
                string fecha = "";
                if (ReceFecha != null)
                {
                    fecha = this.ReceFecha.Value.ToString("dd/MM/yyyy");

                }
                return fecha;
            }
            set
            {
                string fecha = "";
                if (ReceFecha != null)
                {
                    fecha = this.ReceFecha.Value.ToString("dd/MM/yyyy");

                }
                value = fecha;
            }
        }

        public string ReceInstruccionesFarmacia { get; set; }
        public string ReceInstruccionesAlPaciente { get; set; }
        [Display(Name = "Receta")]
        [DataType(DataType.MultilineText)]
        public string ReceComentario { get; set; }
        public Nullable<int> UsuaSecuenciaCreacion { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> UsuaFechaCreacion { get; set; }
        public string UsuaFechaCreacionString
        {
            get
            {
                string fecha = "";
                if (UsuaFechaCreacion != null)
                {
                    fecha = this.UsuaFechaCreacion.Value.ToString("dd/MM/yyyy");

                }
                return fecha;
            }
            set
            {
                string fecha = "";
                if (UsuaFechaCreacion != null)
                {
                    fecha = this.UsuaFechaCreacion.Value.ToString("dd/MM/yyyy");

                }
                value = fecha;
            }
        }
        public Nullable<int> UsuaSecuenciaModificacion { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> UsuaFechaModificacion { get; set; }

        public string UsuaFechaModificacionString
        {
            get
            {
                string fecha = "";
                if (UsuaFechaModificacion != null)
                {
                    fecha = this.UsuaFechaModificacion.Value.ToString("dd/MM/yyyy");

                }
                return fecha;
            }

            set
            {
                string fecha = "";
                if (UsuaFechaModificacion != null)
                {
                    fecha = this.UsuaFechaModificacion.Value.ToString("dd/MM/yyyy");

                }
                value = fecha;
            }
        }
        public bool EstaDesabilitado { get; set; }

        /*datos del paciente*/
        [Display(Name = "Documento")]
        //[Required(ErrorMessage = "Introduzca documento")]
        public string PaciDocumento { get; set; }
        [Display(Name = "Nombre")]
        //[Required(ErrorMessage = "Favor ingresar nombre")]
        public string PaciNombre { get; set; }
        [Display(Name = "Primer Apellido")]
        //[Required(ErrorMessage = "Favor ingresar primer apellido")]
        public string PaciApellido1 { get; set; }
        [Display(Name = "Segundo Apellido")]
        public string PaciApellido2 { get; set; }
            [Display(Name = "Nombre")]
        public string RecNombre { get; set; }

            public string RecSinConsultaNombre { get; set; }

        public string NombreCompleto
        {

            get
            {
                string compl = this.PaciNombre + " " + this.PaciApellido1 + " " + this.PaciApellido2;
                return compl;
            }

        }
                [Display(Name = "Código")]
        public string RecCodigo { get; set; }


        //public string NombreCompleto
        //{

        //    get;
        //    set;

        //}

        ///*mEDICAMENTOS DE LA RECETA*/
        //public List<int> MediSecuencia_fk { get; set; }

        ///*ANALISIS DE LA RECETA*/
        //public List<int> AClinSecuencia_fk { get; set; }

        ///*IMAGENES DE LA RECETA*/
        //public List<int> ImagSecuencia_fk { get; set; }

        /*mEDICAMENTOS DE LA RECETA*/
        public List<int> MediSecuencia_fk { get; set; }

        /*ANALISIS DE LA RECETA*/
        public List<int> AClinSecuencia_fk { get; set; }

        /*IMAGENES DE LA RECETA*/
        public List<int> ImagSecuencia_fk { get; set; }



    }
}