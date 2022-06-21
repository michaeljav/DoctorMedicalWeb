using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DoctorMedicalWeb.Models;
using System.Web.Mvc;


using System.Data.Entity;

using System.Transactions;
using DoctorMedicalWeb.App_Data;
using DoctorMedicalWeb.ModelsComplementarios;




namespace DoctorMedicalWeb.Libreria
{


    public enum Paginas
    {
        pag_Consulta,
        pag_Paciente,
        pag_Personal,
        pag_Cita,
        pag_Agenda,
        pag_FacturacionMenu,
        pag_Facturacion,
        pag_Caja,
        pag_MantenimientoMenu,
        pag_Diagnosticos,
        pag_TipoDiagnosticos,
        pag_Procedimientos,
        Mantenimiento_pag_Consultorio,
        pag_ConfiguracionMenu,
        Configuracion_pag_Consultorio,
        pag_Doctor,
        Ini_TipoFormulario,
        PruebaAngular,
        pag_HorarioTrabajo,
        pag_InstitucionesAseguradoras,
        pag_SeguridadMenu,
        pag_Auditoria,
        pag_UsuarioPersonal,
        pag_Acciones,
        pag_Roles,
        pag_Receta,
        pag_ContabilidadMenu,
        pag_607,
        pag_BackupExcelConsulta,
        pag_CitaPacienteEnLinea,
        pag_DigitalizacionConsultas,
        pag_CategoriaPersonal,
        Rolelista,
        TipoFormulariolista,
        Pag_CategoriaPersonalLista,
        pag_RolePages,
        pag_MotivoConsulta,
        pag_Tratamiento,
        pag_EvaluacionFisica,
        pag_Clinica,
        pag_Consultorio,
        pag_UsuarioDoctor,
        pag_Alergia,
        pag_Medicamentos,
        pag_AnalisisClinico,
        pag_Imagenes,
        pag_Enfermedad
    }

    public enum Maintenance
    {
        MotivoConsulta,
        EvaluacionFisica,
        Diagnostico,
        Tratamiento,
        Enfermedad,
        Medicamento,
        Imagenes,
        AnalisisClinico



    }
    public enum Accion
    {
        Iniciar_Sesion,
        Finalizar_Session,
        Nuevo,
        Editar,
        Borrar,
        Imprimir,
        Error


    }

    public class Lib
    {

        DoctorMedicalWebEntities db = new DoctorMedicalWebEntities();
        //Guardar Auditoria     
        /// <summary>
        /// Metodo para guardar Acciones
        /// </summary>
        /// <param name="PaisSecuencia">Pais</param>
        /// <param name="ClinSecuencia">Clinica</param>
        /// <param name="ConsSecuencia">Consultorio</param>
        /// <param name="DoctSecuencia">Doctor</param>
        /// <param name="PersSecuencia">Personal</param>
        /// <param name="TPersSecuencia">TipoPersonal</param>
        /// <param name="AudiFechaMaquina">FechaMaquinaAuditada</param>
        /// <param name="AudiValorOriginal">ValorOriginalXML</param>
        /// <param name="AudiValorNuevo">ValorNuevoXML</param>
        /// <param name="UsuaCodigo">CodigoUsuario</param>
        /// <param name="AudiIpMaquina">IpMaquina</param>
        /// <param name="PagiSecuencia">Pagina En que se realizo la accion</param>
        /// <param name="TablSecuencia">Tabla</param>
        /// <param name="Accisecuencia">Accion Realizada</param>
        public void IsertarAuditoria(int PaisSecuencia, int ClinSecuencia, int ConsSecuencia,
            int DoctSecuencia, int PersSecuencia, int? TPersSecuencia, DateTime AudiFechaMaquina,
            string AudiValorOriginal, string AudiValorNuevo, int UsuaCodigo, string AudiIpMaquina,
            string Pagina, int? TablSecuencia, string Accisecuencia)
        {

            DbContextTransaction dbtrans = null;
            DoctorMedicalWebEntities db = null;
            try
            {
                using (db = new DoctorMedicalWebEntities())
                {
                    using (dbtrans = db.Database.BeginTransaction())
                    {

                        Auditoria a = new Auditoria();

                        a.PaisSecuencia = PaisSecuencia;
                        a.ClinSecuencia = ClinSecuencia;
                        a.ConsSecuencia = ConsSecuencia;
                        a.DoctSecuencia = DoctSecuencia;
                        a.PersSecuencia = PersSecuencia;
                        a.TPersSecuencia = TPersSecuencia;
                        a.AudiFechaMaquina = AudiFechaMaquina;
                        a.AudiFechaServidor = Lib.GetLocalDateTime();
                        a.AudiValorOriginal = AudiValorOriginal;
                        a.AudiValorNuevo = AudiValorNuevo;
                        a.UsuaCodigo = UsuaCodigo;
                        a.AudiIpMaquina = AudiIpMaquina;
                        a.PagiSecuencia = Pagina;
                        a.TablSecuencia = TablSecuencia;
                        a.Accion = Accisecuencia;
                        
                        //buscando la proxima secuencia
                        int proximoItem = (db.Auditorias.Select(x => (int?)x.AudiSecuencia).Max() ?? 0) + 1;
                        a.AudiSecuencia = proximoItem;
                        //guardando auditoria
                        db.Auditorias.Add(a);
                        db.SaveChanges();
                        dbtrans.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                if (dbtrans != null)
                    dbtrans.Rollback();
                throw ex;
            }

        }

        //Guardar Auditoria     
        /// <summary>
        /// Metodo para guardar Acciones
        /// </summary>
        /// <param name="PaisSecuencia">Pais</param>
        /// <param name="ClinSecuencia">Clinica</param>
        /// <param name="ConsSecuencia">Consultorio</param>
        /// <param name="DoctSecuencia">Doctor</param>
        /// <param name="PersSecuencia">Personal</param>
        /// <param name="TPersSecuencia">TipoPersonal</param>
        /// <param name="AudiFechaMaquina">FechaMaquinaAuditada</param>
        /// <param name="AudiValorOriginal">ValorOriginalXML</param>
        /// <param name="AudiValorNuevo">ValorNuevoXML</param>
        /// <param name="UsuaCodigo">CodigoUsuario</param>
        /// <param name="AudiIpMaquina">IpMaquina</param>
        /// <param name="PagiSecuencia">Pagina En que se realizo la accion</param>
        /// <param name="TablSecuencia">Tabla</param>
        /// <param name="Accisecuencia">Accion Realizada</param>
        public void IsertarAuditoria(UsuarioLoguiado usu, string pagina, string AccionRealizada, string DescripcionError)
        {

            DbContextTransaction dbtrans = null;
            DoctorMedicalWebEntities db = null;
            try
            {
                using (db = new DoctorMedicalWebEntities())
                {
                    using (dbtrans = db.Database.BeginTransaction())
                    {

                        Auditoria a = new Auditoria();

                        a.PaisSecuencia = usu.usuario.PaisSecuencia;
                        a.ClinSecuencia = usu.Consultorio.clinSecuencia_fk;
                        a.ConsSecuencia = usu.Consultorio.ConsSecuencia_fk;
                        a.DoctSecuencia = usu.doctSecuencia;
                        a.PersSecuencia = usu.persSecuencia;
                        a.TPersSecuencia = null;
                        a.AudiFechaMaquina = Lib.GetLocalDateTime();
                        a.AudiFechaServidor = Lib.GetLocalDateTime();
                        a.AudiValorOriginal = null;
                        a.AudiValorNuevo = null;
                        a.UsuaCodigo = usu.usuario.UsuaSecuencia;
                        a.AudiIpMaquina = GetIPAddress();
                        a.PagiSecuencia = pagina;
                        a.TablSecuencia = null;
                        a.Accion = AccionRealizada;
                        a.DescripcionError = DescripcionError;
                        //buscando la proxima secuencia
                        int proximoItem = (db.Auditorias.Select(x => (int?)x.AudiSecuencia).Max() ?? 0) + 1;
                        a.AudiSecuencia = proximoItem;
                        //guardando auditoria
                        db.Auditorias.Add(a);
                        db.SaveChanges();
                        dbtrans.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                if (dbtrans != null)
                    dbtrans.Rollback();
                throw ex;
            }

        }


        public string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        /// <summary>
        /// Metodo para formaterar valor codigo
        /// </summary>
        /// <param name="Ceros"> cantidad de cifras con las que queremos el numero resultante, es decir si introduzco en el parametro 1 un  3 y en el segundo parametro
        ///  introduzco un 1 el resultado sera = 001</param>
        /// <param name="valorOrirignal">valor original a formatear en string ejemplo un "1"</param>
        /// <returns></returns>
        public static string FormatearCodigo(int Ceros, string _valorOririg)
        {
            int ceros = Ceros;
            string valorOriginal = _valorOririg;
            int tam = valorOriginal.Length;
            int faltantes = ceros - tam;
            string valorFormateado = "";
            for (int i = 1; i <= faltantes; i++)
            {
                valorFormateado += "0";
            }

            valorFormateado += valorOriginal;
            return valorFormateado;
        }

        /// <summary>
        /// Este metodo me da la fecha y hora de republica dominicana
        /// Despues trabajare para que sea modificado
        /// </summary>
        /// <returns></returns>
        public static DateTime GetLocalDateTime()
        {
            //Republica Dominicana Hora
          //  return GetLocalDateTime("Central Brazilian Standard Time");
            //Index	Name of Time Zone	        Time   
            //055	S.A. Western Standard Time	(GMT-04:00) Caracas, La Paz
            //https://msdn.microsoft.com/en-us/library/gg154758.aspx
            return GetLocalDateTime("SA Western Standard Time");
            //return GetLocalDateTime("India Standard Time");
        }

        public static TimeZoneInfo GetTimeZoneInfo()
        {
            //Republica Dominicana Hora
            //  return GetLocalDateTime("Central Brazilian Standard Time");
            //Index	Name of Time Zone	        Time   
            //055	S.A. Western Standard Time	(GMT-04:00) Caracas, La Paz
            //https://msdn.microsoft.com/en-us/library/gg154758.aspx
            return getTimeZone("SA Western Standard Time");
            //return GetLocalDateTime("India Standard Time");
        }

        private static TimeZoneInfo getTimeZone( string timeZoneId)
        {
            var timezone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return timezone;

        }
        private static DateTime GetLocalDateTime(string timeZoneId)
        {
            var timezone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var result = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);
            return result;
        }
        /// <summary>
        /// El schedule en su columna ProgramId lo que introduce en momentos
        /// de  crear una cita es  la maxima o siguiente cantidad de citas que contiene,
        /// el no se guia de el valor del ProgramID sino la cantidad de citas que tiene.
        /// </summary>
        /// <param name="listCitas"></param>
        /// <returns></returns>
        public static List<Usar_Cita> listCitasStatic(List<Usar_Cita> listCitas)
        {
            var listci = new List<Usar_Cita>();
            return listci;


        }
        /// <summary>
        /// Substract time
        /// </summary>
        /// <param name="now"></param>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static DateTime SubstractTime(DateTime now, int hours, int minutes, int seconds)
        {
            TimeSpan T1 = new TimeSpan(hours, minutes, seconds);
           
            return now.Subtract(T1);
        }
        /// <summary>
        /// Sum time
        /// </summary>
        /// <param name="now"></param>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static DateTime SumTime(DateTime now, int hours, int minutes, int seconds)
        {
            TimeSpan T1 = new TimeSpan(hours, minutes, seconds);
            return now.Add(T1);
        }

        /// <summary>
        /// Fecha NO PUEDE SER DIFERENTE A LA QUE ESTE EN EL CLIENTE
        /// </summary>
        /// <param name="dateTimeClient"></param>
        /// <returns></returns>
        public static ResponseModel IsRightDate(DateTime dateTimeClient)
        {
            var respuesta = new ResponseModel();
            Dictionary<string, object> dictionaryStringObjec = new Dictionary<string, object>();

            DateTime horaEntranteNuevaCliente = dateTimeClient;
           
            //var datetimeNowSever = DateTime.Now;
            var datetimeNowSever = Lib.GetLocalDateTime();
            //validar fecha
            if (dateTimeClient.Date != datetimeNowSever.Date)
            {
                dictionaryStringObjec.Add("isDateDiference", true);
                //respuesta.error = "DATE SERVER: "+datetimeNowSever.Date.ToShortDateString() + " DATE CLIENTE:" + dateTimeClient.Date.ToShortDateString() + "++Por favor, Arregle su fecha.  Fecha y hora servidor: " + Lib.GetLocalDateTime().ToString();
                respuesta.error =  "Por favor, Arregle su fecha.  Fecha y hora servidor: " + Lib.GetLocalDateTime().ToString();
            }
            else
            {
                dictionaryStringObjec.Add("isDateDiference", false);
            } 
            
            //time 15 before time currenc
            DateTime timeServer15Substra = Lib.SubstractTime(datetimeNowSever, 0, 15, 0);
            //time 15 after curren
            DateTime timeServer15Sum = Lib.SumTime(datetimeNowSever, 0, 15, 0);

            //validar tiempo
            //if ((dateTimeClient.TimeOfDay >= timeServer15Substra.TimeOfDay) && (dateTimeClient.TimeOfDay <= timeServer15Sum.TimeOfDay))
                if ((dateTimeClient >= timeServer15Substra) && (dateTimeClient <= timeServer15Sum))
            {

              dictionaryStringObjec.Add("isTimeIntoRange",  true);

            }
            else
            {
                dictionaryStringObjec.Add("isTimeIntoRange", false);
                //respuesta.error = "Hora Nueva Cliente " + horaEntranteNuevaCliente.ToString()+ " hora mas cliente " + timeServer15Sum.TimeOfDay.ToString() + " HORA MENOS SERVER: " + timeServer15Substra.TimeOfDay.ToString() + " hORA CLIENTE: " + dateTimeClient.TimeOfDay.ToString() + "Por favor, Arregle su hora.  Fecha y hora servidor: " + Lib.GetLocalDateTime().ToString();
                respuesta.error ="Por favor, Arregle su hora.  Fecha y hora servidor: " + Lib.GetLocalDateTime().ToString();
               
            }

            respuesta.dictionaryStringObjec = dictionaryStringObjec;     
            return (respuesta);
        }


    }



}