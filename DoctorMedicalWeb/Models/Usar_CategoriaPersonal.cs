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
    using DoctorMedicalWeb.App_Data;
using System;
using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    
    public partial class Usar_CategoriaPersonal
    {

        public int? DoctSecuencia_fk { get; set; }
        public int? CPersSecuencia { get; set; }
        [DisplayName("Categoria Personal")]
        [Required(ErrorMessage = "Favor introducir categoria personal")]
        public string CPersNombre { get; set; }
        public Nullable<int> UsuaSecuencia { get; set; }
        public Nullable<System.DateTime> CPersFechaCreacion { get; set; }
        public Nullable<int> UsuaSecuenciaModificacion { get; set; }
        public Nullable<System.DateTime> CPersFechaModificacion { get; set; }
		    public bool EstaDesabilitado { get; set; }
      
    }
}