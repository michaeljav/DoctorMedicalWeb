select *  from [dbo].[ConsultaMedicaHistorial]
select *  from [dbo].[ConsultaMedicaEmbarazo]
select *  from [dbo].[ConsultaMedicaHistorialMotivoConsulta]
select *   from [dbo].[ConsultaMedicaHistorialEvaluacionFisica]
select *  from [dbo].[ConsultaMedicaHistorialDiagnostico]
select *  from [dbo].[ConsultaMedicaHistorialTratamiento]

--consulta
select *  from [dbo].[ConsultaMedica]
select *   from [dbo].[ConsultaMedicaEnfermeda]
select *  from [dbo].[ConsultaMedicaEnfermedaFamiliar]


--Receta
select *    from [dbo].[Recetas]                      -- order by substring(RecCodigo,CHARINDEX('-',RecCodigo),LEN(RecCodigo)) desc
select *    from [dbo].[RecetaMedicamentos] 
select *    from [dbo].[RecetaAnalisisClinico] 
select *     from [dbo].[RecetaImagenes]  

select *    from [dbo].[Recetas]       where DoctSecuencia_fk = 3                -- order by substring(RecCodigo,CHARINDEX('-',RecCodigo),LEN(RecCodigo)) desc
select *    from [dbo].[RecetaMedicamentos] where DoctSecuencia_fk = 3
select *    from [dbo].[RecetaAnalisisClinico] where DoctSecuencia_fk = 3
select *     from [dbo].[RecetaImagenes]  where DoctSecuencia_fk = 3


--paciente
select * from [dbo].paciente
go
 select * from dbo.InstitucionesAseguradoras
 go
  select * from dbo.InstitucionAseguradoraPlanes
  go
   select * from dbo.Doctor
  go
  select * from dbo.Personal
  go
 select * from dbo.Usuario
 

 select * from dbo.AnalisisClinico
 select * from dbo.Diagnostico
 select * from dbo.Enfermedad
 select * from dbo.EvaluacionFisica
 select * from dbo.Imagenes
 select * from dbo.Medicamento
 select * from dbo.MotivoConsulta
 select * from dbo.Tratamiento
 select * from dbo.[Auditoria]

 --Borrar Mantenimientos
 --select * from dbo.AnalisisClinico
 --select * from dbo.Diagnostico
 --select * from dbo.Enfermedad
 --select * from dbo.EvaluacionFisica
 --select * from dbo.Imagenes
 --select * from dbo.Medicamento
 --select * from dbo.MotivoConsulta
 --select * from dbo.Tratamiento

 -- select * from dbo.[Auditoria]

    --select 'delete from dbo.'+TABLE_NAME ,* from INFORMATION_SCHEMA.TABLES where TABLE_TYPE='BASE TABLE' order by table_name



--BORRAR
/*


delete from [dbo].[ConsultaMedicaHistorialTratamiento]
delete from [dbo].[ConsultaMedicaHistorialDiagnostico]
delete  from [dbo].[ConsultaMedicaHistorialEvaluacionFisica]
delete from [dbo].[ConsultaMedicaHistorialMotivoConsulta]
delete from [dbo].[ConsultaMedicaEmbarazo]
delete from [dbo].[ConsultaMedicaHistorial]

delete  from [dbo].[ConsultaMedicaEnfermeda]
delete from [dbo].[ConsultaMedicaEnfermedaFamiliar]
 delete from [dbo].[ConsultaMedica]

 
delete from [dbo].[RecetaImagenes] 
delete from [dbo].[RecetaAnalisisClinico] 
 delete from [dbo].[RecetaMedicamentos]
 delete from [dbo].[Recetas]

 --Paciente
 delete from dbo.Paciente 

 --Datos del doctor
 delete from dbo.UsuarioConsultorio
 go
 delete from dbo.Consultorio
 go
 delete from dbo.Clinica
 go
 delete from dbo.InstitucionesAseguradoras
 go
  delete from dbo.InstitucionAseguradoraPlanes
  go
 delete from dbo.Doctor
  go
  delete from dbo.Personal
  go
 delete from dbo.Usuario



 --Borrar Mantenimientos
 delete from dbo.AnalisisClinico     
 delete from dbo.Diagnostico
 delete from dbo.Enfermedad
 delete from dbo.EvaluacionFisica
 delete from dbo.Imagenes
 delete from dbo.Medicamento
 delete from dbo.MotivoConsulta
 delete from dbo.Tratamiento


delete from [dbo].[Auditoria]

 */




--select * from INFORMATION_SCHEMA.COLUMNS 
--where TABLE_NAME = 'ConsultaMedica' and DATA_TYPE='datetime'