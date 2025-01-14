//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoctorMedicalWeb.Models
{
  
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public  class Usar_ConsultaMedica 
    {
	  
         public int? DoctSecuencia_fk { get; set; }
        public int? PaisSecuencia_fk { get; set; }
        public int? clinSecuencia_fk { get; set; }
        public int? ConsSecuencia_fk { get; set; }
        public int? PaciSecuencia_fk { get; set; }
        public int? CMediSecuencia { get; set; }

        public Nullable<System.DateTime> CMediFecha { get; set; }
        public Nullable<System.TimeSpan> CMediHora { get; set; }
            [Display(Name = "Unidad Estatura")]
        public string CMediUnidadesMedidaTalla { get; set; }
            public string EnfeComentario { get; set; }
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
            public Nullable<System.DateTime> CMedEmbarazadaFecha { get; set; }
            public Nullable<int> CMedEmbarazadaSemanas { get; set; }
            public Nullable<int> CMedEmbarazadaDias { get; set; }
            public Nullable<int> CMedEmbarazadaMeses { get; set; }
            public Nullable<int> CMedEmbarazadaMesActualDias { get; set; }
            public Nullable<System.DateTime> CMedEmbarazadaFechaProbableParto { get; set; }
        
        public string CMediAntecedentePadre { get; set; }
        public string CMediAntecedenteMadre { get; set; }
        public string CMediAntecedenteHermanos { get; set; }
        public string CMediAntecedenteOtros { get; set; }

            [Display(Name = "Menarqu�a")]
        public string CMediMenarquia { get; set; }
            [Display(Name = "Patr�n Menstrual")]
        public string CMediPatronMenstrual { get; set; }
            [Display(Name = "Duraci�n Menstrual")]
        public string CMediMensutracionDuracion { get; set; }
            [Display(Name = "Dismenorrea")]
        public bool CMediDismenorrea { get; set; }
            [Display(Name = "Primer Coito")]
        public string CMediPrimerCoito { get; set; }
       
            [Display(Name = "Dispareunia")]
        public bool CMediDispareunia { get; set; }
            [Display(Name = "Vida Sex. Activa")]
        public bool CMediVidaSexualActiva { get; set; }
      [Display(Name = "# Parejas")]
        public Nullable<int> CMediNumeroParejasSexual { get; set; }
          [Display(Name = "FUP")] 
        public Nullable<System.DateTime> CMediFechaUltimoParto { get; set; }
          [Display(Name = "FUA")] 
        public Nullable<System.DateTime> CMediFechaUltimoAborto { get; set; }
          [Display(Name = "FUM")] 
        public Nullable<System.DateTime> CMediFechaUltimaMenstruacion { get; set; }
          [Display(Name = "Menopausia")] 
        public string CMediMenopausia { get; set; }
          [Display(Name = "# Gestaci�n")] 
        public Nullable<int> CMediGestacionVeces { get; set; }
          [Display(Name = "# Partos")] 
        public Nullable<int> CMediPartosVeces { get; set; }
          [Display(Name = "# Abortos")] 
        public Nullable<int> CMediAbortosVeces { get; set; }
          [Display(Name = "# Cesarias")] 
        public Nullable<int> CMediCesariasVeces { get; set; }
          [Display(Name = "# Ectopicos")] 
        public Nullable<int> CMediEctopico { get; set; }

        public string CMediTensionArterial { get; set; }
        public string CMediFrecuenciaCardiaca { get; set; }
        public string CMediTiroides { get; set; }
        public string CMediPulmones { get; set; }
        public string CMediCorazon { get; set; }
        public string CMediMamas { get; set; }
        public string CMediAbdomen { get; set; }
        public string CMediGenitalesExternos { get; set; }
        public string CMediTactoVaginal { get; set; }
        public string CMediExtremidadesInferiores { get; set; }

      
          [Display(Name = "FUPAP")] 
        public Nullable<System.DateTime> CMediFechaUltimoPapanicolau { get; set; }

        public Nullable<int> UsuaSecuenciaCreacion { get; set; }
        public Nullable<System.DateTime> UsuaFechaCreacion { get; set; }
        public Nullable<int> UsuaSecuenciaModificacion { get; set; }
        public Nullable<System.DateTime> UsuaFechaModificacion { get; set; }
        public bool EstaDesabilitado { get; set; }
    }
}
