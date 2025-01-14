﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoctorMedicalWeb.Models
{
    public class Usar_vw_receta
    {
    
        public int DoctSecuencia_fk { get; set; }
        public int ClinSecuencia_fk { get; set; }
        public int ConsSecuencia_fk { get; set; }
        public int PaciSecuencia_fk { get; set; }
        public int ReceSecuencia { get; set; }
        public int CMHistSecuencia_fk { get; set; }
        public System.DateTime ReceFecha { get; set; }
        public string ReceFechaString
        {
            get
            {
                string fecha = "";
                if (ReceFecha != null)
                {
                    fecha = this.ReceFecha.ToString("dd'/'MM'/'yyyy HH:mm:ss");

                }
                return fecha;
            }
            set
            {
                string fecha = "";
                if (ReceFecha != null)
                {
                    fecha = this.ReceFecha.ToString("dd'/'MM'/'yyyy HH:mm:ss");

                }
                value = fecha;
            }
        }
        public string ReceComentario { get; set; }
        public Nullable<System.DateTime> UsuaFechaCreacion { get; set; }
        public string DoctCUPRE { get; set; }
        public string DoctNombre { get; set; }
        public string DoctApellido { get; set; }
        public Nullable<System.DateTime> DoctFechaNacimiento { get; set; }
        public string DoctTelefono { get; set; }
        public string DoctCelular { get; set; }
        public string clinRazonSocial { get; set; }
        public string clinDireccion { get; set; }
        public string clinTelefono { get; set; }
        public string clinTelefono2 { get; set; }
        public Nullable<int> clinRNC { get; set; }
        public Nullable<int> clinFax { get; set; }
        public string clinPaginaWeb { get; set; }
        public string clinEmail { get; set; }
        public string clinFotoName { get; set; }
        public string clinFotoPath { get; set; }
        public int ConsSecuencia { get; set; }
        public string ConsCodigo { get; set; }
        public string ConsDescripcion { get; set; }
        public string ConsDireccion { get; set; }
        public string ConsTelefono { get; set; }
        public string ConsExtencion { get; set; }
        public string ConsTelefono2 { get; set; }
        public string ConsExtension2 { get; set; }
        public string PaciDocumento { get; set; }
        public int TDSecuencia { get; set; }
        public Nullable<int> PaciNumeroSeguroSocial { get; set; }
        public Nullable<int> IAsegSecuencia { get; set; }
        public Nullable<int> IAPlanSecuencia { get; set; }
        public Nullable<int> PaciNumeroPoliza { get; set; }
        public bool EsMenor { get; set; }
        public string PaciNombre { get; set; }
        public string PaciApellido1 { get; set; }
        public string PaciApellido2 { get; set; }
        public string PaciApodo { get; set; }
        public Nullable<System.DateTime> PaciFechaNacimiento { get; set; }
        public string PaciLugarNacimiento { get; set; }
        public Nullable<int> PaciEdad { get; set; }
        public string PaciEmail { get; set; }
        public string PaciFacebook { get; set; }
        public string PaciDireccion { get; set; }
        public string PaciTelefono { get; set; }
        public string PaciCelular { get; set; }
        public Nullable<int> PaciCodigoPostal { get; set; }
        public string PaciEstadoCivil { get; set; }
        public string PaciProfesion { get; set; }
        public string PaciNombreEmergencia { get; set; }
        public string PaciApellidoEmergencia { get; set; }
        public string PaciDireccionEmergencia { get; set; }
        public string PaciTelefonoEmergencia { get; set; }
        public string PaciFotoPath { get; set; }
        public string PaciFotoNombre { get; set; }
        public string PaciGenero { get; set; }
    }
}