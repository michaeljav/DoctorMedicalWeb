using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DoctorMedicalWeb.Models;
using System.ComponentModel.DataAnnotations;
using DoctorMedicalWeb.Libreria;
using System.Globalization;

namespace DoctorMedicalWeb.ModelsComplementarios
{
    public class Usar_Consultar
    {
        //inicializo las listas por si viene vacias
        public Usar_Consultar()
        {
            //this.MConsSecuencia_fk = new List<int>();
            //this.EFisiSecuencia_fk = new List<int>();
            //this.DiagSecuencia = new List<int>();
            //this.TratSecuencia_fk = new List<int>();
            //this.EnfeSecuenciaFamiliar = new List<int>();
            //this.EnfeSecuenciaPersonal = new List<int>();
            //this.ReceSecuencia = new List<int>();
            //this.MediSecuencia_fk = new List<int>();
            //this.AClinSecuencia_fk = new List<int>();
            //this.ImagSecuencia_fk = new List<int>();
        }

        public int? DoctSecuencia_fk { get; set; }
        public int? PaisSecuencia_fk { get; set; }
        public int? ClinSecuencia_fk { get; set; }
        public int? ConsSecuencia_fk { get; set; }

        [Required]
        public int PaciSecuencia_fk { get; set; }
        public int? CMHistSecuencia { get; set; }

        [Display(Name = "FUR")]
        //[DataType(DataType.DateTime)]
     
        public Nullable<System.DateTime> CMHistFechaUltimaRegla { get; set; }
        public string CMHistFechaUltimaReglaString
        {
            get
            {
                string fecha = "";
                if (CMHistFechaUltimaRegla != null)
                {
                    fecha = this.CMHistFechaUltimaRegla.Value.ToString("dd/MM/yyyy"); // me convertia la fecha en mes dia anio
                    //fecha = this.CMHistFechaUltimaRegla.Value.ToString();

                }
                return fecha;
            }
            set
            {
                string fecha = "";
                if (CMHistFechaUltimaRegla != null)
                {
                    fecha = this.CMHistFechaUltimaRegla.Value.ToString("dd/MM/yyyy");
                    //fecha = this.CMHistFechaUltimaRegla.Value.ToString();

                }
                value = fecha;
            }

        }
        [Display(Name = "IMC")]
        [RegularExpression(@"^\d+.?\d{0,2}$", ErrorMessage = "Ivalido Digitos; Máximo dos decimal.")]
        public Nullable<decimal> CMHistIndiceMasaCorporal { get; set; }



        public Nullable<System.DateTime> CMHistEmbarazadaFecha { get; set; }
      
        public string CMHistEmbarazadaFechaString
        {
            get
            {
                string fecha = "";
                if (CMHistEmbarazadaFecha != null)
                {
                    //fecha = this.CMHistEmbarazadaFecha.Value.ToString("dd/MM/yyyy");
//fecha = this.CMHistEmbarazadaFecha.Value.ToString();
                }
                return fecha;
            }
            set
            {
                string fecha = "";
                if (CMHistEmbarazadaFecha != null)
                {
                    //fecha = this.CMHistEmbarazadaFecha.Value.ToString("dd/MM/yyyy");
 //fecha = this.CMHistEmbarazadaFecha.Value.ToString();
                }
                value = fecha;
            }

        }


        public Nullable<int> CMHistEmbarazadaDias { get; set; }

        public Nullable<int> CMHistEmbarazadaMeses { get; set; }

        public Nullable<System.DateTime> CMHistEmbarazadaFechaProbableParto { get; set; }
        public string CMHistEmbarazadaFechaProbablePartoString
        {
            get
            {
                string fecha = "";
                if (CMHistEmbarazadaFechaProbableParto != null)
                {
                    fecha = this.CMHistEmbarazadaFechaProbableParto.Value.ToString("dd/MM/yyyy");
//fecha = this.CMHistEmbarazadaFechaProbableParto.Value.ToString();
                }
                return fecha;
            }
            set
            {
                string fecha = "";
                if (CMHistEmbarazadaFechaProbableParto != null)
                {
                    fecha = this.CMHistEmbarazadaFechaProbableParto.Value.ToString("dd/MM/yyyy");
  //fecha = this.CMHistEmbarazadaFechaProbableParto.Value.ToString();
                }
                value = fecha;
            }

        }

        [Display(Name = "Unidad de Peso")]

        public string CMHistUnidadesMedidaPeso { get; set; }
        [Display(Name = "Peso")]
        //[RegularExpression(@"^\d+.?\d{0,2}$", ErrorMessage = "Ivalido Digitos; Máximo dos decimal.")]
        [DataType(DataType.Currency)]
        // [RegularExpression("([0-9]+)", ErrorMessage = "favor de introducir solor números.")]
        public Nullable<decimal> CMHistPeso { get; set; }
        [Display(Name = "Unidad de Estatura")]
        public string CMHistUnidadesMedidaTalla { get; set; }
        [Display(Name = "Estatura")]
        //[RegularExpression(@"^\d+.?\d{0,2}$", ErrorMessage = "Ivalido Digitos; Máximo dos decimal.")]
        [DataType(DataType.Currency)]
        // [RegularExpression("([0-9]+)", ErrorMessage = "favor de introducir solor números.")]
        public Nullable<decimal> CMHistTalla { get; set; }
            [Display(Name = "Código")]
        public string CMHistCodigo { get; set; }

        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> CMHistFecha { get; set; }
        public string CMHistFechaString
        {
            get
            {
                string fecha = "";
                if (CMHistFecha != null)
                {
                    fecha = this.CMHistFecha.Value.ToString("dd/MM/yyyy");
 //fecha = this.CMHistFecha.Value.ToString();
                }
                return fecha;
            }
            set
            {
                string fecha = "";
                if (CMHistFecha != null)
                {
                    fecha = this.CMHistFecha.Value.ToString("dd/MM/yyyy");
 //fecha = this.CMHistFecha.Value.ToString();

                }
                value = fecha;
            }
        }
        //este campo lo utilizo para cuando  le de al boton editar, poder
        // cargar la ultima fecha de consulta.
        public string UltimaFechaConsulta { get; set; }
        [Display(Name = "Hora")]
        [DataType(DataType.Time)]
        public Nullable<System.TimeSpan> CMHistHora { get; set; }
        public string CMHistHoraString
        {

            get
            {
                string hora = "";
                if (CMHistHora != null)
                {

                    DateTime dt = Convert.ToDateTime(CMHistHora.ToString());
                    hora = dt.ToString("hh:mm:ss tt", new CultureInfo("en-US"));

                }
                return hora;
            }
            set
            {
                string hora = "";
                if (CMHistHora != null)
                {

                    DateTime dt = Convert.ToDateTime(CMHistHora.ToString());
                    hora = dt.ToString("hh:mm:ss tt", new CultureInfo("en-US"));

                }
                value = hora;

            }

        }

        [Display(Name = "Observación General")]
        public string CMHistComentario { get; set; }

        public int? CMediSecuencia { get; set; }

        public Nullable<System.DateTime> CMediFecha { get; set; }
        public Nullable<System.TimeSpan> CMediHora { get; set; }
        [Display(Name = "Unidad Estatura")]
        public string CMediUnidadesMedidaTalla { get; set; }
        [Display(Name = "Estatura")]
        public Nullable<decimal> CMediTalla { get; set; }
        [Display(Name = "Unidad Peso")]
        public string CMediUnidadesMedidaPeso { get; set; }
        [Display(Name = "Peso")]
        public Nullable<decimal> CMediPeso { get; set; }
        [Display(Name = "Grupo Sanguineo")]
        public Nullable<int> GSangSecuencia_fk { get; set; }
        [Display(Name = "Embarazada")]
        public bool CMedEmbarazada { get; set; }

        [Display(Name = "Embarazo", Prompt = "Fecha")]
        //[DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //[DataType(DataType.DateTime)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]

        //[DataType(DataType.Date), DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}", ApplyFormatInEditMode = true)]
        //[DataType(DataType.Date)]

        //[DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        //[Editable(false)]
        //[Display(Name = "Fech")]
        //[DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> CMedEmbarazadaFecha { get; set; }

        public string CMedEmbarazadaFechaString
        {
            get
            {
                string fecha = "";
                if (CMedEmbarazadaFecha != null)
                {
                    fecha = this.CMedEmbarazadaFecha.Value.ToString("dd/MM/yyyy");
                    //fecha = this.CMedEmbarazadaFecha.Value.ToString();
                }
                return fecha;
            }
            set
            {
                string fecha = "";
                if (CMedEmbarazadaFecha != null)
                {
                    fecha = this.CMedEmbarazadaFecha.Value.ToString("dd/MM/yyyy");
//fecha = this.CMedEmbarazadaFecha.Value.ToString();
                }
                value = fecha;
            }

        }
        [Display(Name = "Semanas")]
        public Nullable<int> CMedEmbarazadaSemanas { get; set; }
        [Display(Name = "Dias")]
        public Nullable<int> CMedEmbarazadaDias { get; set; }
        [Display(Name = "Mes(es)")]
        public Nullable<int> CMedEmbarazadaMeses { get; set; }
        [Display(Name = "Dias Act. Mes")]
        public Nullable<int> CMedEmbarazadaMesActualDias { get; set; }
        [Display(Name = "Tentativa de Parto", Prompt = "Fecha")]
        public Nullable<System.DateTime> CMedEmbarazadaFechaProbableParto { get; set; }
        public string CMedEmbarazadaFechaProbablePartoString
        {
            get
            {
                string fecha = "";
                if (CMedEmbarazadaFechaProbableParto != null)
                {
                    fecha = this.CMedEmbarazadaFechaProbableParto.Value.ToString("dd/MM/yyyy");
//fecha = this.CMedEmbarazadaFechaProbableParto.Value.ToString();
                }
                return fecha;
            }
            set
            {
                string fecha = "";
                if (CMedEmbarazadaFechaProbableParto != null)
                {
                    fecha = this.CMedEmbarazadaFechaProbableParto.Value.ToString("dd/MM/yyyy");
 //fecha = this.CMedEmbarazadaFechaProbableParto.Value.ToString();

                }
                value = fecha;
            }

        }


        public bool CMHistEmbarazada { get; set; }


        public Nullable<int> CMHistEmbarazadaSemanas { get; set; }



        public Nullable<int> CMHistEmbarazadaMesActualDias { get; set; }

        [Display(Name = "Menarquía")]
        public string CMediMenarquia { get; set; }
        [Display(Name = "Patrón Menstrual")]
        public string CMediPatronMenstrual { get; set; }
        [Display(Name = "Duración Menstrual")]
        public string CMediMensutracionDuracion { get; set; }
        [Display(Name = "Dismenorrea")]
        public bool CMediDismenorrea { get; set; }
        [Display(Name = "Primer Coito")]
        public Nullable<int> CMediPrimerCoito { get; set; }

        [Display(Name = "Dispareunia")]
        public bool CMediDispareunia { get; set; }
        [Display(Name = "Vida Sex. Activa")]
        public bool CMediVidaSexualActiva { get; set; }
        [Display(Name = "# Parejas")]
        public Nullable<int> CMediNumeroParejasSexual { get; set; }
        [Display(Name = "FUP")]
        public Nullable<System.DateTime> CMediFechaUltimoParto { get; set; }
        public string CMediFechaUltimoPartoString
        {
            get
            {
                string fecha = "";
                if (CMediFechaUltimoParto != null)
                {
                    fecha = this.CMediFechaUltimoParto.Value.ToString("dd/MM/yyyy");
                    //fecha = this.CMediFechaUltimoParto.Value.ToString();

                }
                return fecha;
            }
            set
            {
                string fecha = "";
                if (CMediFechaUltimoParto != null)
                {
                    fecha = this.CMediFechaUltimoParto.Value.ToString("dd/MM/yyyy");
//fecha = this.CMediFechaUltimoParto.Value.ToString();
                }
                value = fecha;
            }
        }
        [Display(Name = "FUA")]
        public Nullable<System.DateTime> CMediFechaUltimoAborto { get; set; }
        public string CMediFechaUltimoAbortoString
        {
            get
            {
                string fecha = "";
                if (CMediFechaUltimoAborto != null)
                {
                    fecha = this.CMediFechaUltimoAborto.Value.ToString("dd/MM/yyyy");
//fecha = this.CMediFechaUltimoAborto.Value.ToString();
                }
                return fecha;
            }
            set
            {
                string fecha = "";
                if (CMediFechaUltimoAborto != null)
                {
                    fecha = this.CMediFechaUltimoAborto.Value.ToString("dd/MM/yyyy");
 //fecha = this.CMediFechaUltimoAborto.Value.ToString();

                }
                value = fecha;
            }

        }
        [Display(Name = "FUM")]
        public Nullable<System.DateTime> CMediFechaUltimaMenstruacion { get; set; }
        public string CMediFechaUltimaMenstruacionString
        {
            get
            {
                string fecha = "";
                if (CMediFechaUltimaMenstruacion != null)
                {
                    fecha = this.CMediFechaUltimaMenstruacion.Value.ToString("dd/MM/yyyy");
  //fecha = this.CMediFechaUltimaMenstruacion.Value.ToString();
                }
                return fecha;
            }
            set
            {
                string fecha = "";
                if (CMediFechaUltimaMenstruacion != null)
                {
                    fecha = this.CMediFechaUltimaMenstruacion.Value.ToString("dd/MM/yyyy");
   //fecha = this.CMediFechaUltimaMenstruacion.Value.ToString();
                }
                value = fecha;
            }

        }
        [Display(Name = "Menopausia")]
        public Nullable<int> CMediMenopausia { get; set; }
        [Display(Name = "# Gestación")]
        public Nullable<int> CMediGestacionVeces { get; set; }
        [Display(Name = "# Partos")]
        public Nullable<int> CMediPartosVeces { get; set; }
        [Display(Name = "# Abortos")]
        public Nullable<int> CMediAbortosVeces { get; set; }
        [Display(Name = "# Cesarias")]
        public Nullable<int> CMediCesariasVeces { get; set; }
        [Display(Name = "# Ectopicos")]
        public Nullable<int> CMediEctopico { get; set; }

        [Display(Name = "FUPAP")]
        public Nullable<System.DateTime> CMediFechaUltimoPapanicolau { get; set; }
        public string CMediFechaUltimoPapanicolauString
        {
            get
            {
                string fecha = "";
                if (CMediFechaUltimoPapanicolau != null)
                {
                    fecha = this.CMediFechaUltimoPapanicolau.Value.ToString("dd/MM/yyyy");
   //fecha = this.CMediFechaUltimoPapanicolau.Value.ToString();
                }
                return fecha;
            }
            set
            {
                string fecha = "";
                if (CMediFechaUltimoPapanicolau != null)
                {
                    fecha = this.CMediFechaUltimoPapanicolau.Value.ToString("dd/MM/yyyy");
//fecha = this.CMediFechaUltimoPapanicolau.Value.ToString();
                }
                value = fecha;
            }
        }

        //[Required(ErrorMessage = "Introduzca documento")]
        public string PaciDocumento { get; set; }
        [Display(Name = "Cédula o Pasaporte")]
        //[Required(ErrorMessage = "Seleccione tipo documento")]
        public int TDSecuencia { get; set; }
        [Display(Name = "Número Seguro Social")]
        public Nullable<int> PaciNumeroSeguroSocial { get; set; }
        [Display(Name = "Seguro")]
        public Nullable<int> IAsegSecuencia { get; set; }
        [Display(Name = "Plan")]
        public Nullable<int> IAPlanSecuencia { get; set; }
        [Display(Name = "Poliza")]
        public Nullable<int> PaciNumeroPoliza { get; set; }
        [Display(Name = "Es Menor")]
        public bool EsMenor { get; set; }
        [Display(Name = "Nombre")]
        //[Required(ErrorMessage = "Favor ingresar nombre")]
        public string PaciNombre { get; set; }
        [Display(Name = "Primer Apellido")]
        //[Required(ErrorMessage = "Favor ingresar primer apellido")]
        public string PaciApellido1 { get; set; }
        [Display(Name = "Segundo Apellido")]
        public string PaciApellido2 { get; set; }

        public string PaciApodo { get; set; }
        public string NombreCompleto
        {

            get
            {
                string compl = this.PaciNombre + " " + this.PaciApellido1 + " " + this.PaciApellido2;
                return compl;
            }


        }
        [Display(Name = "Fecha Nacimiento")]
        //[Required(ErrorMessage = "Favor ingresar fecha nacimiento")]
        public Nullable<System.DateTime> PaciFechaNacimiento { get; set; }
        public string PacifechaNacimientoString
        {
            get
            {
                string fecha = "";
                if (PaciFechaNacimiento != null)
                {
                    fecha = this.PaciFechaNacimiento.Value.ToString("dd/MM/yyyy");
 //fecha = this.PaciFechaNacimiento.Value.ToString();
                }
                return fecha;
            }
            set
            {
                string fecha = "";
                if (PaciFechaNacimiento != null)
                {
                    fecha = this.PaciFechaNacimiento.Value.ToString("dd/MM/yyyy");
 //fecha = this.PaciFechaNacimiento.Value.ToString();
                }
                value = fecha;
            }

        }




        [Display(Name = "Lugar Nacimiento")]
        public string PaciLugarNacimiento { get; set; }
        [Display(Name = "Edad")]
        public int? PaciEdad { get; set; }
        [Display(Name = "Correo Electrónico")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Favor ingresar un correo valido.")]
        public string PaciEmail { get; set; }
        [Display(Name = "Facebook")]
        public string PaciFacebook { get; set; }
        [Display(Name = "Dirección")]
        public string PaciDireccion { get; set; }
        [Display(Name = "Teléfono")]
        public string PaciTelefono { get; set; }
        [Display(Name = "Celular")]
        public string PaciCelular { get; set; }
        public Nullable<int> PaciCodigoPostal { get; set; }
        [Display(Name = "Estado Civil")]
        public string PaciEstadoCivil { get; set; }
        [Display(Name = "Profesión")]
        public string PaciProfesion { get; set; }
        [Display(Name = "Emergencia Nombre")]
        public string PaciNombreEmergencia { get; set; }
        [Display(Name = "Emergencia Apellido")]
        public string PaciApellidoEmergencia { get; set; }
        [Display(Name = "Emergencia Dirección")]
        public string PaciDireccionEmergencia { get; set; }
        [Display(Name = "Emergencia Teléfono")]
        public string PaciTelefonoEmergencia { get; set; }
        [Display(Name = "Foto")]
        public string PaciFotoPath { get; set; }
        public string PaciFotoNombre { get; set; }
        [Display(Name = "Genero")]
        //[Required(ErrorMessage = "Favor seleccionar genero")]
        public string PaciGenero { get; set; }
        public string FechaNacimientoString { get; set; }

        [Display(Name = "Nota Motivo Consulta")]
        [DataType(DataType.MultilineText)]
        public string MotiComentario { get; set; }

        [Display(Name = "Motivo de Consulta")]
        //[Required(ErrorMessage = "Es requerido motivo de consulta")]
        // [CannotBeEmptyAttribute]//esto es para validar lista
        public List<int> MConsSecuencia_fk { get; set; }

        [Display(Name = "Nota Evaluacion Física")]
        [DataType(DataType.MultilineText)]
        public string EFisiComentario { get; set; }
        [Display(Name = "Evaluacion Física")]
        //[Required(ErrorMessage="prueba")]
        //[CannotBeEmptyAttribute]//esto es para validar lista
        public List<int> EFisiSecuencia_fk { get; set; }

        [Display(Name = "Diagnostico")]
        public List<int> DiagSecuencia { get; set; }
        [Display(Name = "Nota Diagnostico")]
        [DataType(DataType.MultilineText)]
        public string DiagComentario { get; set; }

        [Display(Name = "Nota Tratamiento")]
        public string TratComentario { get; set; }
        [Display(Name = "Tratamiento")]
        public List<int> TratSecuencia_fk { get; set; }


        public string Familiar { get; set; }

        public bool EnfermedadActiva { get; set; }
        public Nullable<System.DateTime> FechaDeDiagnostico { get; set; }
        public string FechaDeDiagnosticoString
        {
            get
            {
                string fecha = "";
                if (FechaDeDiagnostico != null)
                {
                    fecha = this.FechaDeDiagnostico.Value.ToString("dd/MM/yyyy");
//fecha = this.FechaDeDiagnostico.Value.ToString();

                }
                return fecha;
            }
            set
            {
                string fecha = "";
                if (FechaDeDiagnostico != null)
                {
                    fecha = this.FechaDeDiagnostico.Value.ToString("dd/MM/yyyy");
                 //fecha = this.FechaDeDiagnostico.Value.ToString();

                }
                value = fecha;
            }

        }
        public bool EnfermedadInfecciosa { get; set; }


        public List<int> EnfeSecuenciaFamiliar { get; set; }
        public List<int> EnfeSecuenciaPersonal { get; set; }
        [Display(Name = "Nota Enfermedad")]
        [DataType(DataType.MultilineText)]
        public string EnfeComentarioFamiliar { get; set; }


        [Display(Name = "Nota Enfermedad")]
        [DataType(DataType.MultilineText)]
        public string EnfeComentarioPersonal { get; set; }
        /*No ulitizo estas variables por que a travez de ajax envio una lista
         * de recetas*/
           [Display(Name = "Secuencia")]
        public List<int> Consult_ReceSecuencia { get; set; }
        //public Nullable<int> CMHistSecuencia_fk { get; set; }
        [Display(Name = "Fecha")]
        public Nullable<System.DateTime> Consult_ReceFecha { get; set; }
        public string ReceFechaString
        {
            get
            {
                string fecha = "";
                if (Consult_ReceFecha != null)
                {
                    fecha = this.Consult_ReceFecha.Value.ToString("dd/MM/yyyy");
                    //fecha = this.Consult_ReceFecha.Value.ToString();

                }
                return fecha;
            }
            set
            {
                string fecha = "";
                if (Consult_ReceFecha != null)
                {
                    fecha = this.Consult_ReceFecha.Value.ToString("dd/MM/yyyy");
                    //fecha = this.Consult_ReceFecha.Value.ToString();

                }
                value = fecha;
            }
        }

        public string Consult_ReceInstruccionesFarmacia { get; set; }
        public string Consult_ReceInstruccionesAlPaciente { get; set; }
   /*End No ulitizo estas variables por que a travez de ajax envio una lista
         * de recetas*/
        [Display(Name = "Receta")]
        [DataType(DataType.MultilineText)]
        public string Consult_ReceComentario { get; set; }

               [Display(Name = "Codigo")]
        public string Consult_RecCodigo { get; set; }
    

        /*mEDICAMENTOS DE LA RECETA*/
        public List<int> Consult_MediSecuencia_fk { get; set; }

        /*ANALISIS DE LA RECETA*/
        public List<int> Consult_AClinSecuencia_fk { get; set; }

        /*IMAGENES DE LA RECETA*/
        public List<int> Consult_ImagSecuencia_fk { get; set; }

        /*End No ulitizo estas variables por que a travez de ajax envio una lista
         * de recetas*/


        public Nullable<int> UsuaSecuenciaCreacion { get; set; }
        public Nullable<System.DateTime> UsuaFechaCreacion { get; set; }
        public string UsuaFechaCreacionString
        {
            get
            {
                string fecha = "";
                if (UsuaFechaCreacion != null)
                {
                    //eSTO ME CONVIERTE A LA FECHA MES DIA ANIO
                    fecha = this.UsuaFechaCreacion.Value.ToString("dd/MM/yyyy");
                    //fecha = this.UsuaFechaCreacion.Value.ToString();

                }
                return fecha;
            }
            set
            {
                string fecha = "";
                if (UsuaFechaCreacion != null)
                {//eSTO ME CONVIERTE A LA FECHA MES DIA ANIO
                    fecha = this.UsuaFechaCreacion.Value.ToString("dd/MM/yyyy");
                    //fecha = this.UsuaFechaCreacion.Value.ToString();

                }
                value = fecha;
            }
        }
        public Nullable<int> UsuaSecuenciaModificacion { get; set; }
        public Nullable<System.DateTime> UsuaFechaModificacion { get; set; }

        public string UsuaFechaModificacionString
        {
            get
            {
                string fecha = "";
                if (UsuaFechaModificacion != null)
                {
                    fecha = this.UsuaFechaModificacion.Value.ToString("dd/MM/yyyy");
                    //fecha = this.UsuaFechaModificacion.Value.ToString();

                }
                return fecha;
            }

            set
            {
                string fecha = "";
                if (UsuaFechaModificacion != null)
                {
                    fecha = this.UsuaFechaModificacion.Value.ToString("dd/MM/yyyy");
                    //fecha = this.UsuaFechaModificacion.Value.ToString();

                }
                value = fecha;
            }
        }
         



        //public Usar_ConsultaMedicaHistorial usar_ConsultaMedicaHistorial { get; set; }
        //public Usar_ConsultaMedica usar_ConsultaMedica { get; set; }
        //public Usar_Paciente usar_Paciente { get; set; }    
        //public Usar_ConsultaMedicaHistorialMotivoConsulta usar_ObjMotivoConsulta { get; set; }
        //public Usar_ConsultaMedicaHistorialEvaluacionFisica usar_ObjEvaluacionFisica { get; set; }
        //public Usar_ConsultaMedicaHistorialDiagnostico usar_ObjDiagnostico { get; set; }    
        //public Usar_ConsultaMedicaHistorialTratamiento usar_ObjTratamiento { get; set; }
        //public Usar_ConsultaMedicaEnfermedaFamiliar usar_ConsultaMedicaEnfermedaFamiliar { get; set; }
        //public Usar_ConsultaMedicaEnfermedad usar_ConsultaMedicaEnfermedad { get; set; }

        //public Usar_RecetaComplementaria Usar_RecetaComplementaria { get; set; }

    }
}