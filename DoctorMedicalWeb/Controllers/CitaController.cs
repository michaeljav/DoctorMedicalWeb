using DoctorMedicalWeb.App_Data;
using DoctorMedicalWeb.Libreria;
using DoctorMedicalWeb.Models;
using DoctorMedicalWeb.ModelsComplementarios;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoctorMedicalWeb.Controllers
{
    public class CitaController : Controller
    {
        



        //
        // GET: /Cita/
        string controler = "Cita", vista = "Ini_Cita", PaginaAutorizada = Paginas.pag_Cita.ToString();
        public ActionResult Ini_Cita()
        {
            using (DoctorMedicalWebEntities db = new DoctorMedicalWebEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;

                //Si NO esta loguieado lo redireccionara al loguin
                if (HttpContext.Session["user"] == null)
                {
                    return RedirectToAction("Index", "PaginaPresentacion");
                }

                //lleno el combo de roles
                UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];

                //pARA EN LA VISTA PODER SABER A QUE  CONTROLADOR EL AJAX  LLAMARA
                ViewBag.ControlCsharp = controler;
                ViewBag.VistaCsharp = vista;


                //si no tiene  el listado formulario devuelvo al loguin
                List<Formulario> _sessformularioParaDesabilitar = new List<Formulario>();
                _sessformularioParaDesabilitar = (List<App_Data.Formulario>)Session["sessformularioParaDesabilitar"];

                if (_sessformularioParaDesabilitar.Count == 0)
                {
                    return RedirectToAction("Index", "DashBoard");
                }
                ViewBag.VBformularioParaDesabilitar = _sessformularioParaDesabilitar;

                //validar que si no tiene permiso a este formulario no entre
                List<vw_ListDeFormuriosbyRolyUser> formulariosPlanRoleUser = new List<vw_ListDeFormuriosbyRolyUser>();
                formulariosPlanRoleUser = (List<App_Data.vw_ListDeFormuriosbyRolyUser>)Session["FormulariosPermitidos"];
                //traera false o true;
                var EstaEsteFormulario = formulariosPlanRoleUser.Where(f => f.FormDescripcion == PaginaAutorizada).Any();
                if (formulariosPlanRoleUser.Count == 0)
                {
                    return RedirectToAction("Index", "DashBoard");
                }
                //si es diferente de true quiere decir que no tiene permiso
                else if (!EstaEsteFormulario)
                {
                    return RedirectToAction("Index", "DashBoard");
                }
                ViewBag.ListaFormulario = formulariosPlanRoleUser;

                //para que el div de accion solo aparezca si son 
                //vistas difrentes a home
                ViewBag.isHome = true;


                //Mandar lista de citas
                getCitas();

                List<ScheduleData> Appoint = new List<ScheduleData>();
                Appoint.Add(new ScheduleData { Id = 1, Subject = "Meeting1", StartTime = new DateTime(2016, 11, 18, 10, 00, 00), EndTime = new DateTime(2016, 11, 18, 11, 00, 00), Description = "", AllDay = false, Recurrence = false, RecurrenceRule = "" });
                Appoint.Add(new ScheduleData { Id = 2, Subject = "Meeting2", StartTime = new DateTime(2016, 11, 20, 10, 00, 00), EndTime = new DateTime(2016, 11, 20, 11, 00, 00), Description = "", AllDay = false, Recurrence = false, RecurrenceRule = "" });
                Appoint.Add(new ScheduleData { Id = 3, Subject = "Meeting3", StartTime = new DateTime(2016, 11, 22, 10, 00, 00), EndTime = new DateTime(2016, 11, 22, 11, 00, 00), Description = "", AllDay = false, Recurrence = false, RecurrenceRule = "" });
                ViewBag.dataSource = Appoint;




                //buscar listado de paciente
                List<Paciente> paciente = ((from pers in db.Pacientes
                                            where pers.DoctSecuencia_fk == usu.doctSecuencia
                                           && pers.EstaDesabilitado == false
                                            select pers).ToList());
                var usarPacientelist = new List<Usar_Paciente>();
                foreach (var item in paciente)
                {
                    var nuevoUsarPaci = new Usar_Paciente();
                    //asignar contenido a otro objeto
                    CopyClass.CopyObject(item, ref nuevoUsarPaci);
                    nuevoUsarPaci.ConsCodigo = usu.Consultorio.ConsCodigo;
                    nuevoUsarPaci.ConsDescripcion = usu.Consultorio.ConsDescripcion;
                    nuevoUsarPaci.clinRazonSocial = usu.Consultorio.clinRazonSocial;
                    usarPacientelist.Add(nuevoUsarPaci);
                }

                //bucar NombreConsultorio

                ViewBag.listPaciente = usarPacientelist;

                //fecha de hoy anio mes dia
                //para que no me permita crear cita antes de esta fecha actual
                string fecha = "";
                fecha = DateTime.Now.Year.ToString();
                fecha = fecha + "-" + DateTime.Now.Month.ToString();
                fecha = fecha + "-" + DateTime.Now.Day.ToString();
                ViewBag.fecHoy = fecha;

                    //var respuesta = new ResponseModel();
                    //respuesta = rangeHourWork();
                    //HorarioTrabajoDestalle hodi = (HorarioTrabajoDestalle)respuesta.dictionaryStringObjec["MinimaHoraComienzo"];
                    //HorarioTrabajoDestalle hodf = (HorarioTrabajoDestalle)respuesta.dictionaryStringObjec["MaximaHoraFinaliza"];
                    //int horaIn=00, horafi=24;
                    //if (hodi != null && hodf != null)
                    //{
                    //    if (hodi.hi != null && hodf.hf != null)
                    //    {
                    //        horaIn = hodi.hi.Hours;
                    //        horafi = hodf.hf.Hours;
                    //    }
                       
                    //}

                    //ViewBag.HSTART = horaIn;
                    //ViewBag.HEND = horafi;
              


                return View();
            }
        }


        [HttpPost]
        public JsonResult getRangeHourWork()
        {
            var respuesta = new ResponseModel();
            respuesta = rangeHourWork();
            respuesta.respuesta = true;
            return Json(respuesta);

        }
        private ResponseModel rangeHourWork()
        {
            var respuesta = new ResponseModel();
            Dictionary<string, object> dictionaryStringObjec = new Dictionary<string,object>();

            UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];
            using (var db = new DoctorMedicalWebEntities())
            {
                //de todos los dias la minima hora de comenar
               HorarioTrabajoDestalle minimaHoraComienzo = (from x in  db.HorarioTrabajoDestalles
                                                                where x.DoctSecuencia_fk == usu.doctSecuencia &&
                                                                           x.PaisSecuencia_fk == usu.Consultorio.PaisSecuencia_fk &&
                                                                           x.ClinSecuencia_fk == usu.Consultorio.clinSecuencia_fk &&
                                                                           x.ConsSecuencia_fk == usu.Consultorio.ConsSecuencia_fk                                                              
                                                                          orderby x.hi ascending
                                                            select x).FirstOrDefault();

               //de todos los dias la maxima  hora de finalizar
               HorarioTrabajoDestalle maximaHoraFinalizar = (from x in db.HorarioTrabajoDestalles
                                                            where x.DoctSecuencia_fk == usu.doctSecuencia &&
                                                                       x.PaisSecuencia_fk == usu.Consultorio.PaisSecuencia_fk &&
                                                                       x.ClinSecuencia_fk == usu.Consultorio.clinSecuencia_fk &&
                                                                       x.ConsSecuencia_fk == usu.Consultorio.ConsSecuencia_fk
                                                            orderby x.hf descending
                                                            select x).FirstOrDefault();


                //buscar partes en que se dividira  cada hora
               HorarioTrabajo DivisionesHora = (from x in db.HorarioTrabajoes
                                                             where x.DoctSecuencia_fk == usu.doctSecuencia &&
                                                                        x.PaisSecuencia_fk == usu.Consultorio.PaisSecuencia_fk &&
                                                                        x.ClinSecuencia_fk == usu.Consultorio.clinSecuencia_fk &&
                                                                        x.ConsSecuencia_fk == usu.Consultorio.ConsSecuencia_fk                                                      
                                                             select x).FirstOrDefault();




               int horaIn = 00, horafi = 24;
               if (minimaHoraComienzo != null && maximaHoraFinalizar != null)
               {
                   if (minimaHoraComienzo.hi != null && maximaHoraFinalizar.hf != null)
                   {
                       horaIn = minimaHoraComienzo.hi.Hours;
                       horafi = maximaHoraFinalizar.hf.Hours;
                   }

               }

                //si ya ha pasado una semana  de la omidificacioni o creacion de la configuracion
                // y no tiene el check de siempre utilizado, no se puede crear citas.
               dictionaryStringObjec.Add("IsExced7DaysConf", false);
               if( DivisionesHora.HTrabRepetirSiempre == false)
               {
                   //sumar 7 dias  la fecha de modificacion
                   var fecMas7Dias = DivisionesHora.HTrabFechaModificacion.Value.AddDays(7);
                   //si la fecha actual es mayuor que la fecha   modificada  mas 7 dias  quiere decir que no puede crear cita
                   if (Lib.GetLocalDateTime() >fecMas7Dias )
                   {

                       dictionaryStringObjec["IsExced7DaysConf"] = true;
                   }               
               }
              



               //ViewBag.HSTART = horaIn;
               //ViewBag.HEND = horafi;
               //dictionaryStringObjec.Add("MinimaHoraComienzo",minimaHoraComienzo);
               //dictionaryStringObjec.Add("MaximaHoraFinaliza", maximaHoraFinalizar);

               dictionaryStringObjec.Add("MinimaHoraComienzo", horaIn);
               dictionaryStringObjec.Add("MaximaHoraFinaliza", horafi);
               dictionaryStringObjec.Add("DivisionesHora", DivisionesHora.HTrabPartesHoraSchedule);

               respuesta.dictionaryStringObjec = dictionaryStringObjec;
                return respuesta;
            }
        }


        [HttpPost]
        public JsonResult getIsBefore(DateTime dateTime)
        {
            var respuesta = new ResponseModel();
            respuesta = getIsBeforeDateTime(dateTime);

            return Json(respuesta);

        }

        [HttpPost]
        public JsonResult getAppointmentPatientInfo(Usar_Cita usar_cita)
        {
            var respuesta = new ResponseModel();
            respuesta = AppointmentPatientInfo(usar_cita);

            return Json(respuesta);

        }

        /// <summary>
        /// busco los datos del paciente al cual pertenecce esta cieta
        /// para mostrarlos
        /// </summary>
        /// <param name="usar_cita"></param>
        /// <returns></returns>
        public ResponseModel AppointmentPatientInfo(Usar_Cita usar_cita)
        {
            var respuesta = new ResponseModel();
            using (DoctorMedicalWebEntities db = new DoctorMedicalWebEntities())
            {
                //lleno el combo de roles
                UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];

                //buscar listado de paciente
                Paciente paciente = ((from pers in db.Pacientes
                                      where pers.DoctSecuencia_fk == usu.doctSecuencia
                                      && pers.PaciSecuencia == usar_cita.PaciSecuencia_fk
                                     && pers.EstaDesabilitado == false
                                      select pers).SingleOrDefault());


                var nuevoUsarPaci = new Usar_Paciente();
                //asignar contenido a otro objeto
                CopyClass.CopyObject(paciente, ref nuevoUsarPaci);
                nuevoUsarPaci.ConsCodigo = usu.Consultorio.ConsCodigo;
                nuevoUsarPaci.ConsDescripcion = usu.Consultorio.ConsDescripcion;
                nuevoUsarPaci.clinRazonSocial = usu.Consultorio.clinRazonSocial;

                respuesta.obj = nuevoUsarPaci;
                respuesta.respuesta = true;
            }
            return respuesta;

        }

        /// <summary>
        /// valida que la cita que se ponga hoy sea mayor  a la fecha y hora de hoy
        /// es decir que no me ponga cita de una hora o fecha pasada
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public ResponseModel getIsBeforeDateTime(DateTime dateTime)
        {
            var respuesta = new ResponseModel();

            var ticliente = dateTime.Ticks;
            var _tiAhora = Lib.GetLocalDateTime();
            var tiAhora = _tiAhora.Ticks;//DateTime.Now.Ticks;
            //si la hora seleccionada es menor de la hora actual entonces no se pude crear una cita
            if (ticliente < tiAhora)
            {
                respuesta.respuesta = true;
            }
            return respuesta;

        }


        [HttpPost]
        public JsonResult getAppointments( )
        {
            var respuesta = new ResponseModel();
            respuesta.someCollection = getCitas();

            return Json(respuesta);

        }


        public List<Usar_Cita> getCitas()
        {
            List<Usar_Cita> citas = new List<Usar_Cita>
            {
                //new Usar_Cita {
                //    ProgramId= 1,
                //    PaciSecuencia_fk=10,
                //       ProgramName = "Turtle Walk",
                //       Comments = "Night out with turtles",
                //       ProgramStartTime = new DateTime(2016, 6, 2, 3, 0, 0),
                //       ProgramEndTime = new DateTime(2016, 6, 2, 4, 0, 0),
                //       IsAllDay = true
                //}
                //,
                //new Usar_Cita {
                //        ProgramId= 2,
                //          PaciSecuencia_fk=11,
                //       ProgramName = "Winter Sleepers",
                //       Comments = "Long sleep during winter season",
                //       ProgramStartTime = new DateTime(2016, 6, 3, 1, 0, 0),
                //       ProgramEndTime = new DateTime(2016, 6, 3, 2, 0, 0)
                //},
                //new Usar_Cita {
                //        ProgramId= 3,
                //          PaciSecuencia_fk=12,
                //       ProgramName = "Estivation",
                //       Comments = "Sleeping in hot season",
                //       ProgramStartTime = new DateTime(2016, 6, 4, 3, 0, 0),
                //       ProgramEndTime = new DateTime(2016, 6, 4, 4, 0, 0)
                //}
            };

            UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];

            using (var db = new DoctorMedicalWebEntities())
            {
                List<Cita> objelist = (from tf in db.Citas
                                       where tf.DoctSecuencia_fk == usu.doctSecuencia
                                       && tf.EstaDesabilitado == false
                                       //orderby tf.CitaSecuencia ascending
                                       select tf).ToList();

                var listObjToShow = new List<Usar_Cita>();
                //el calendario en el campo ProgramId el lo usa para el index
                //del array que crea cuando se le asigna el datasource al mismo.
                //asi que debo de usar otro campo para saber la secuencia.
                int secArraySchedule = 1;
                //crear objeto tipo formulario y asignar a la lista de usartipoformulario
                foreach (var item in objelist)
                {
                    Usar_Cita UsarObjNewToShow = new Usar_Cita();
                  //  UsarObjNewToShow.ProgramId = secArraySchedule;
                    //asignar contenido a otro objeto
                    CopyClass.CopyObject(item, ref UsarObjNewToShow);
                    listObjToShow.Add(UsarObjNewToShow);
                    secArraySchedule++;
                }
                ViewBag.listCitas = listObjToShow;

                citas = listObjToShow;
            }

            return citas;
        }


        [HttpPost]
        public JsonResult Save(Usar_Cita usar_cita, IEnumerable<Usar_Cita> NewList, IEnumerable<Usar_Cita> EditList, IEnumerable<Usar_Cita> DeletedList)
        {

            var respuesta = new ResponseModel();

            //Si NO esta loguieado lo redireccionara al loguin
            if (HttpContext.Session["user"] == null)
            {
                respuesta.respuesta = false;
                respuesta.error = "Usted debe de loguiarse.";
                respuesta.redirect = Url.Action("Index", "PaginaPresentacion");
                return Json(respuesta);
            }

            if (usar_cita.PaciSecuencia_fk < 1 )
            {
                respuesta.respuesta = false;
                respuesta.error = "Usted debe de seleccionar paciente.";
                return Json(respuesta);
            }







            //// solo trabajo con una transaccion a la vez, es decir
            //// solo una creacion de cita a la vez,
         
            //if (NewList != null ) {
            //    if ( NewList.Count() > 1)
            //    {
            //        respuesta.respuesta = false;
            //        respuesta.error = "Usted debe de trabajar solo una cita la vez.";        
            //        return Json(respuesta);
            //    }
            //}
            //else if (NewList != null)
            //{
            //    if (NewList.Count() > 1)
            //    {
            //        respuesta.respuesta = false;
            //        respuesta.error = "Usted debe de trabajar solo una cita la vez.";
            //        return Json(respuesta);
            //    }
            //}



            //si no ha validado volver y mostrar mensajes de validacion
            //aqui no lo voy a usar la validacion del modelo
            //if (!ModelState.IsValid)
            //{
            //    respuesta.respuesta = false;
            //    //respuesta.redirect = Url.Content("~/Roles/Roles");
            //    respuesta.redirect = Url.Content("~/" + controler + "/" + vista + "");
            //    respuesta.error = "Llenar Campos requeridos ";
            //    return Json(respuesta);
            //}

            UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];


        







            using (var db = new DoctorMedicalWebEntities())
            {
                //buscar partes en que se dividira  cada hora
                HorarioTrabajo DivisionesHora = (from x in db.HorarioTrabajoes
                                                 where x.DoctSecuencia_fk == usu.doctSecuencia &&
                                                            x.PaisSecuencia_fk == usu.Consultorio.PaisSecuencia_fk &&
                                                            x.ClinSecuencia_fk == usu.Consultorio.clinSecuencia_fk &&
                                                            x.ConsSecuencia_fk == usu.Consultorio.ConsSecuencia_fk
                                                 select x).FirstOrDefault();           

                //si ya ha pasado una semana  de la omidificacioni o creacion de la configuracion
                // y no tiene el check de siempre utilizado, no se puede crear citas.
              
                if (DivisionesHora.HTrabRepetirSiempre == false)
                {
                    //sumar 7 dias  la fecha de modificacion
                    var fecMas7Dias = DivisionesHora.HTrabFechaModificacion.Value.AddDays(7);
                    //si la  modificada  mas 7 dias  es mayor a la fecha 
                    if (Lib.GetLocalDateTime() > fecMas7Dias)
                    {

                        respuesta.respuesta = false;
                        respuesta.error = "Su configuración de horario ya tiene una semana, y fue desactivada.";
                        return Json(respuesta);
                    }
                }
              








                using (var dbtrans = db.Database.BeginTransaction())
                {

                    try
                    {

                        #region Agregar nuevas citas
                        //add new 
                        if (NewList != null && NewList.Count() > 0)
                        {
                            int proximoItem = 0;
                            //buscando la proxima secuencia, el doctor en todas sus consultorios tiene los mismos roles 
                            proximoItem = ((from diag in db.Citas
                                            where diag.DoctSecuencia_fk == usu.doctSecuencia
                                            select (int?)diag.CitaSecuencia).Max() ?? 0) + 1;
                            foreach (var nCita in NewList)
                            {
                                //si la fecha es menor a la fecha actual no se puede guardar esta cita por lo tanto
                                //debo de  no guardar y romper 
                                if (getIsBeforeDateTime(nCita.ProgramStartTime).respuesta)
                                {

                                    dbtrans.Rollback();
                                    respuesta.respuesta = false;
                                    respuesta.error = "Favor cree la cita desde la fecha y hora actual  en adelante.";
                                    return Json(respuesta);
                                }

                                Cita cita = new Cita();

                                //asignar contenido a otro objeto
                                CopyClass.CopyObject(nCita, ref cita);

                                cita.DoctSecuencia_fk = usu.doctSecuencia;
                                cita.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                cita.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                cita.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                                cita.PaciSecuencia_fk = usar_cita.PaciSecuencia_fk;
                                cita.CitaCancelada = usar_cita.CitaCancelada;
                                cita.CitaCancelacionMotivo = usar_cita.CitaCancelacionMotivo;
                                cita.CitaFechaCreacion = Lib.GetLocalDateTime();
                                cita.UsuaSecuenciaCreacion = usu.usuario.UsuaSecuencia;


                                cita.CitaSecuencia = proximoItem;
                                cita.EstaDesabilitado = false;
                                db.Citas.Add(cita);

                                proximoItem++;
                            }
                       
                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Nuevo.ToString(), null);

                        }
                        #endregion

                        #region Editar las citas
                        //Editar citas
                        if (EditList != null && EditList.Count() > 0)
                        {


                            foreach (var ECitaOrig in EditList)
                            {
                                 //si la fecha es menor a la fecha actual no se puede guardar esta cita por lo tanto
                                //debo de  no guardar y romper 
                                if (getIsBeforeDateTime(ECitaOrig.ProgramStartTime).respuesta)
                                {
                                    dbtrans.Rollback();
                                    respuesta.respuesta = false;
                                    respuesta.error = "Favor modifique la cita desde la fecha y hora actual  en adelante.";
                                    return Json(respuesta);
                                }


                                //buscando la proxima secuencia, el doctor en todas sus consultorios tiene los mismos roles 
                                var citEditDest = ((from diag in db.Citas
                                                where diag.DoctSecuencia_fk == usu.doctSecuencia
                                                && diag.PaciSecuencia_fk == usar_cita.PaciSecuencia_fk
                                                && diag.CitaSecuencia == ECitaOrig.CitaSecuencia
                                                select diag)).SingleOrDefault();
                                citEditDest.ProgramName = ECitaOrig.ProgramName;
                                citEditDest.Comments = ECitaOrig.Comments;
                                citEditDest.ProgramStartTime = ECitaOrig.ProgramStartTime;
                                citEditDest.ProgramEndTime = ECitaOrig.ProgramEndTime;
                                citEditDest.CitaCancelada = ECitaOrig.CitaCancelada;
                                citEditDest.CitaCancelacionMotivo = ECitaOrig.CitaCancelacionMotivo;
                                citEditDest.UsuaFechaModificacion = Lib.GetLocalDateTime();
                                citEditDest.UsuaSecuenciaModificacion = usu.usuario.UsuaSecuencia;
                             
                            }
                        
                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Editar.ToString(), null);

                        }

                        #endregion

                        #region Borrar las citas
                        //Editar citas
                        if (DeletedList != null && DeletedList.Count() > 0)
                        {


                            foreach (var deleCitaOrig in DeletedList)
                            {
                                //si la fecha es menor a la fecha actual no se puede guardar esta cita por lo tanto
                                //debo de  no guardar y romper 
                                //if (getIsBeforeDateTime(deleCitaOrig.ProgramStartTime).respuesta)
                                //{
                                //    dbtrans.Rollback();
                                //    respuesta.respuesta = false;
                                //    respuesta.error = "Favor modifique la cita desde la fecha y hora actual  en adelante.";
                                //    return Json(respuesta);
                                //}


                                //buscando la proxima secuencia, el doctor en todas sus consultorios tiene los mismos roles 
                                var citEditDest = ((from diag in db.Citas
                                                    where diag.DoctSecuencia_fk == usu.doctSecuencia
                                                    && diag.PaciSecuencia_fk == usar_cita.PaciSecuencia_fk
                                                    && diag.CitaSecuencia == deleCitaOrig.CitaSecuencia
                                                    select diag)).SingleOrDefault();

                                citEditDest.EstaDesabilitado = true;
                              

                            }
                       
                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Borrar.ToString(), null);

                        }

                        respuesta.respuesta = true;
                      
                            db.SaveChanges();
                            dbtrans.Commit();
                     

                        #endregion
                    }
                    catch (DbEntityValidationException e)
                    {
                        dbtrans.Rollback();
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        throw;
                    }
                }

            }





            return Json(respuesta);
        }



        [HttpPost]
        public JsonResult isdateTimeIntoRange(int dia, DateTime horainicio, DateTime horafinal)
        {
            var respuesta = new ResponseModel();
            respuesta = _isdateTimeIntoRange(dia, horainicio, horafinal);

            return Json(respuesta);

        }


        public ResponseModel _isdateTimeIntoRange(int dia,DateTime horainicio,DateTime horafinal)
        {
            var respuesta = new ResponseModel();

            UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];
            Dictionary<string, object> dictionaryStringObjec = new Dictionary<string, object>();
            using (var db = new DoctorMedicalWebEntities())
            {


                //Dia hora de comenar
                HorarioTrabajoDestalle HoraComienzo = (from x in db.HorarioTrabajoDestalles
                                                             where x.DoctSecuencia_fk == usu.doctSecuencia &&
                                                                        x.PaisSecuencia_fk == usu.Consultorio.PaisSecuencia_fk &&
                                                                        x.ClinSecuencia_fk == usu.Consultorio.clinSecuencia_fk &&
                                                                        x.ConsSecuencia_fk == usu.Consultorio.ConsSecuencia_fk &&
                                                                        x.DSemaSecuencia_Fk == dia
                                                             orderby x.hi ascending
                                                             select x).FirstOrDefault();



                //Dia hora de finalizar
                HorarioTrabajoDestalle HoraFinalizar = (from x in db.HorarioTrabajoDestalles
                                                              where x.DoctSecuencia_fk == usu.doctSecuencia &&
                                                                         x.PaisSecuencia_fk == usu.Consultorio.PaisSecuencia_fk &&
                                                                         x.ClinSecuencia_fk == usu.Consultorio.clinSecuencia_fk &&
                                                                         x.ConsSecuencia_fk == usu.Consultorio.ConsSecuencia_fk &&
                                                                         x.DSemaSecuencia_Fk == dia
                                                              orderby x.hf descending
                                                              select x).FirstOrDefault();



                //buscar  si esta habilitado el dia
                HorarioTrabajo habilitadoDia = (from x in db.HorarioTrabajoes
                                                 where x.DoctSecuencia_fk == usu.doctSecuencia &&
                                                            x.PaisSecuencia_fk == usu.Consultorio.PaisSecuencia_fk &&
                                                            x.ClinSecuencia_fk == usu.Consultorio.clinSecuencia_fk &&
                                                            x.ConsSecuencia_fk == usu.Consultorio.ConsSecuencia_fk &&
                                                            x.DSemaSecuencia_Fk == dia
                                                 select x).FirstOrDefault();



                if (habilitadoDia != null )
                {

                    dictionaryStringObjec.Add("isHabilitadoDia", habilitadoDia.HTrabDiaHabilitado);
                }

                if (HoraComienzo != null && HoraFinalizar != null)
                {
                    dictionaryStringObjec.Add("TieneHoraConf", true);
                    if ((horainicio.TimeOfDay >= HoraComienzo.hi) && (horafinal.TimeOfDay <= HoraFinalizar.hf))
                    {

                   
                        dictionaryStringObjec.Add("IsIntoRange", true);
                    }
                    else
                    {
                        dictionaryStringObjec.Add("HoraComienzo", HoraComienzo.hi.ToString());
                        dictionaryStringObjec.Add("HoraFinalizar", HoraComienzo.hf.ToString());
                        dictionaryStringObjec.Add("IsIntoRange", false);
                    }
                }
                else
                {
                    dictionaryStringObjec.Add("TieneHoraConf", false);

                }


             
            }

            respuesta.dictionaryStringObjec = dictionaryStringObjec;
            return respuesta;
        }




    }

}
