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
    
    public partial class RecetaImagene
    {
        public int DoctSecuencia_fk { get; set; }
        public int PaisSecuencia_fk { get; set; }
        public int ClinSecuencia_fk { get; set; }
        public int ConsSecuencia_fk { get; set; }
        public int PaciSecuencia_fk { get; set; }
        public int CMHistSecuencia_fk { get; set; }
        public int ReceSecuencia_fk { get; set; }
        public int ImagSecuencia_fk { get; set; }
        public int RImagSecuencia { get; set; }
        public bool EstaDesabilitado { get; set; }
    
        public virtual Imagene Imagene { get; set; }
        public virtual Receta Receta { get; set; }
    }
}