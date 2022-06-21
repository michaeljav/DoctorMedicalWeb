/*******Permisos en la palicacion******/
select * from [sdoc].[Usuario]

select * from [sdoc].[UsuarioRole]

select * from [sdoc].[Role]

select * from [sdoc].[RolFormularios]

select * from [sdoc].[Formularios]

select * from [sdoc].[FormulariosAccion]

select * from [sdoc].[Acciones]

/******Campos para visualizar en pantalla de consulta por especialidad****/
select * from [sdoc].[Usuario]

select * from [sdoc].[Doctor]

select * from [sdoc].[Especialidades]

select * from [sdoc].[EspecialidadFormularioCampos]

select * from [sdoc].[FormularioCampos]

/*******Seguridad planes*****/

select * from [sdoc].[Usuario]

select * from [sdoc].[Planes]  --HECHO

select * from [sdoc].[PlanFormularios]




proceso

Crear usuario: 
----campos claves
*Pais,Nombre, apellido,especialidad que se tambien guardaran tambien en la tabla de 
doctor
*Plan elegido
*Rol por defecto que es administrador sistema 1

