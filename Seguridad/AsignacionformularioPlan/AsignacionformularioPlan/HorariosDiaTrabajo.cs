//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AsignacionformularioPlan
{
    using System;
    using System.Collections.Generic;
    
    public partial class HorariosDiaTrabajo
    {
        public int DoctSecuencia_fk { get; set; }
        public int PaisSecuencia_fk { get; set; }
        public int ClinSecuencia_fk { get; set; }
        public int ConsSecuencia_fk { get; set; }
        public int HDTrabSecuencia { get; set; }
        public System.DateTime Fecha { get; set; }
        public System.TimeSpan Tiempo { get; set; }
        public string Descripcion { get; set; }
        public bool Aplicado { get; set; }
        public System.DateTime HDTrabFechaModificacion { get; set; }
        public int UsuaSecuenciaCreacion { get; set; }
        public System.DateTime HDTrabFechaCreacion { get; set; }
        public int UsuaSecuenciaModificacion { get; set; }
        public bool EstaDesabilitado { get; set; }
    
        public virtual Pai Pai { get; set; }
    }
}