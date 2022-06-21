----ldoc = login  doc
----udoc = usuario doc
----sdoc = Squema doc

----USUARIO:udoc
----clave:123456

--Crear base datos
create database DoctorMedicalWeb
go
--Elegir base datos 
use DoctorMedicalWeb

go
--crear login con su clave
CREATE LOGIN ldoc WITH PASSWORD=N'123456', 
DEFAULT_DATABASE=[DoctorMedicalWeb],
DEFAULT_LANGUAGE=[us_english],
 CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
--crera esquema
create   schema sdoc 
go
--crear usuario y asignarlo a login con esquema por defecto
create user udoc for login ldoc with default_schema=  sdoc

--asignar al schema owner  el usuario
alter authorization on schema::sdoc to udoc
go
--esto es para tener todos los derechos
exec sp_addrolemember 'db_owner', 'udoc'
