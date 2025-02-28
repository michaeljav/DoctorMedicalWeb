﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DoctorMedicalWebEntities : DbContext
    {
        public DoctorMedicalWebEntities()
            : base("name=DoctorMedicalWebEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Alergia> Alergias { get; set; }
        public virtual DbSet<AnalisisClinico> AnalisisClinicoes { get; set; }
        public virtual DbSet<ArchivosPaciente> ArchivosPacientes { get; set; }
        public virtual DbSet<Auditoria> Auditorias { get; set; }
        public virtual DbSet<CategoriaPersonal> CategoriaPersonals { get; set; }
        public virtual DbSet<Cirugia> Cirugias { get; set; }
        public virtual DbSet<Cita> Citas { get; set; }
        public virtual DbSet<Clinica> Clinicas { get; set; }
        public virtual DbSet<ConsultaMedica> ConsultaMedicas { get; set; }
        public virtual DbSet<ConsultaMedicaCirugia> ConsultaMedicaCirugias { get; set; }
        public virtual DbSet<ConsultaMedicaEmbarazo> ConsultaMedicaEmbarazoes { get; set; }
        public virtual DbSet<ConsultaMedicaEnfermeda> ConsultaMedicaEnfermedas { get; set; }
        public virtual DbSet<ConsultaMedicaEnfermedaFamiliar> ConsultaMedicaEnfermedaFamiliars { get; set; }
        public virtual DbSet<ConsultaMedicaHistorial> ConsultaMedicaHistorials { get; set; }
        public virtual DbSet<ConsultaMedicaHistorialDiagnostico> ConsultaMedicaHistorialDiagnosticoes { get; set; }
        public virtual DbSet<ConsultaMedicaHistorialEnfermeda> ConsultaMedicaHistorialEnfermedas { get; set; }
        public virtual DbSet<ConsultaMedicaHistorialEvaluacionFisica> ConsultaMedicaHistorialEvaluacionFisicas { get; set; }
        public virtual DbSet<ConsultaMedicaHistorialMotivoConsulta> ConsultaMedicaHistorialMotivoConsultas { get; set; }
        public virtual DbSet<ConsultaMedicaHistorialTratamiento> ConsultaMedicaHistorialTratamientoes { get; set; }
        public virtual DbSet<ConsultaMedicalHistorialAlergia> ConsultaMedicalHistorialAlergias { get; set; }
        public virtual DbSet<Consultorio> Consultorios { get; set; }
        public virtual DbSet<Diagnostico> Diagnosticoes { get; set; }
        public virtual DbSet<Doctor> Doctors { get; set; }
        public virtual DbSet<Enfermedad> Enfermedads { get; set; }
        public virtual DbSet<Especialidade> Especialidades { get; set; }
        public virtual DbSet<EspecialidadFormularioCampos> EspecialidadFormularioCampos { get; set; }
        public virtual DbSet<EvaluacionFisica> EvaluacionFisicas { get; set; }
        public virtual DbSet<Factura> Facturas { get; set; }
        public virtual DbSet<FacturaDetalle> FacturaDetalles { get; set; }
        public virtual DbSet<FacturaEstado> FacturaEstadoes { get; set; }
        public virtual DbSet<FormularioCampos> FormularioCampos { get; set; }
        public virtual DbSet<Formulario> Formularios { get; set; }
        public virtual DbSet<FormulariosAccion> FormulariosAccions { get; set; }
        public virtual DbSet<GrupoSanguineo> GrupoSanguineos { get; set; }
        public virtual DbSet<HorariosDiaTrabajo> HorariosDiaTrabajoes { get; set; }
        public virtual DbSet<ImagenDoctor> ImagenDoctors { get; set; }
        public virtual DbSet<Imagene> Imagenes { get; set; }
        public virtual DbSet<InstitucionAseguradoraPlane> InstitucionAseguradoraPlanes { get; set; }
        public virtual DbSet<InstitucionesAseguradora> InstitucionesAseguradoras { get; set; }
        public virtual DbSet<LoginProvider> LoginProviders { get; set; }
        public virtual DbSet<Medicamento> Medicamentoes { get; set; }
        public virtual DbSet<Moneda> Monedas { get; set; }
        public virtual DbSet<MotivoConsulta> MotivoConsultas { get; set; }
        public virtual DbSet<Paciente> Pacientes { get; set; }
        public virtual DbSet<PacienteFoto> PacienteFotoes { get; set; }
        public virtual DbSet<Pai> Pais { get; set; }
        public virtual DbSet<Personal> Personals { get; set; }
        public virtual DbSet<Plane> Planes { get; set; }
        public virtual DbSet<PlanFactura> PlanFacturas { get; set; }
        public virtual DbSet<PlanFacturaDetalle> PlanFacturaDetalles { get; set; }
        public virtual DbSet<PlanFormulario> PlanFormularios { get; set; }
        public virtual DbSet<Procedimiento> Procedimientos { get; set; }
        public virtual DbSet<RecetaAnalisisClinico> RecetaAnalisisClinicoes { get; set; }
        public virtual DbSet<RecetaImagene> RecetaImagenes { get; set; }
        public virtual DbSet<RecetaMedicamento> RecetaMedicamentos { get; set; }
        public virtual DbSet<RecetaPacienteVacuna> RecetaPacienteVacunas { get; set; }
        public virtual DbSet<Receta> Recetas { get; set; }
        public virtual DbSet<Representante> Representantes { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RolFormulario> RolFormularios { get; set; }
        public virtual DbSet<TipoArchivoPaciente> TipoArchivoPacientes { get; set; }
        public virtual DbSet<TipoDocumento> TipoDocumentoes { get; set; }
        public virtual DbSet<TipoFormulario> TipoFormularios { get; set; }
        public virtual DbSet<TipoIndicacion> TipoIndicacions { get; set; }
        public virtual DbSet<TipoPago> TipoPagoes { get; set; }
        public virtual DbSet<Tratamiento> Tratamientoes { get; set; }
        public virtual DbSet<UsoMedicamento> UsoMedicamentos { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<UsuarioConsultorio> UsuarioConsultorios { get; set; }
        public virtual DbSet<Vacuna> Vacunas { get; set; }
        public virtual DbSet<Accione> Acciones { get; set; }
        public virtual DbSet<EstadoAgenda> EstadoAgendas { get; set; }
        public virtual DbSet<PediatraParto> PediatraPartoes { get; set; }
        public virtual DbSet<PEDIATRIAeMBARAZO> PEDIATRIAeMBARAZOes { get; set; }
        public virtual DbSet<PlanFormularios_copia> PlanFormularios_copia { get; set; }
        public virtual DbSet<TipoCompromiso> TipoCompromisoes { get; set; }
        public virtual DbSet<vw_ConsultorioDoctor> vw_ConsultorioDoctor { get; set; }
        public virtual DbSet<vw_ListDeFormuriosbyRolyUser> vw_ListDeFormuriosbyRolyUser { get; set; }
        public virtual DbSet<vw_planFormulario> vw_planFormulario { get; set; }
        public virtual DbSet<vw_UsuarioConsultorios> vw_UsuarioConsultorios { get; set; }
        public virtual DbSet<vw_UsuarioDoctor> vw_UsuarioDoctor { get; set; }
        public virtual DbSet<vw_UsuariosPersonalDelDoctor> vw_UsuariosPersonalDelDoctor { get; set; }
    }
}
