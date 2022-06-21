//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoctorMedicalWeb.App_Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Clinica
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Clinica()
        {
            this.Consultorios = new HashSet<Consultorio>();
            this.HorarioTrabajoes = new HashSet<HorarioTrabajo>();
        }
    
        public int clinSecuencia { get; set; }
        public int PaisSecuencia_fk { get; set; }
        public string clinRazonSocial { get; set; }
        public string clinEstablecimiento { get; set; }
        public string clinOrganismoSuperior { get; set; }
        public string clinDireccion { get; set; }
        public string clinTelefono { get; set; }
        public string clinTelefono2 { get; set; }
        public Nullable<int> clinRNC { get; set; }
        public Nullable<int> clinFax { get; set; }
        public string clinPaginaWeb { get; set; }
        public string clinEmail { get; set; }
        public string clinFotoName { get; set; }
        public string clinFotoPath { get; set; }
        public string clinCuerpoCartaCabecera { get; set; }
        public string clinCuerpoCartaPie { get; set; }
        public Nullable<decimal> clinLatitud { get; set; }
        public Nullable<decimal> clinLongitud { get; set; }
        public Nullable<bool> clinEstaBorrado { get; set; }
        public Nullable<int> usuaSecuenciaCreacion { get; set; }
        public Nullable<int> usuaSecuenciaModificacion { get; set; }
        public Nullable<System.DateTime> clinFechaCreacion { get; set; }
        public Nullable<System.DateTime> clinFechaModificacion { get; set; }
        public bool EstaDesabilitado { get; set; }
    
        public virtual Pai Pai { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Consultorio> Consultorios { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HorarioTrabajo> HorarioTrabajoes { get; set; }
    }
}