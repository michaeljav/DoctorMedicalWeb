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
    
    public partial class PlanFacturaDetalle
    {
        public int PFDetaSecuencia { get; set; }
        public int UsuSecuencia { get; set; }
        public int PFactSecuencia { get; set; }
        public int PlanSecuencia_fk { get; set; }
        public bool EstaDesabilitado { get; set; }
    
        public virtual Plane Plane { get; set; }
        public virtual PlanFactura PlanFactura { get; set; }
    }
}