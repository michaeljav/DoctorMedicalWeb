using DoctorMedicalWeb.App_Data;
using DoctorMedicalWeb.Libreria;
using DoctorMedicalWeb.Models;
using DoctorMedicalWeb.ModelsComplementarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoctorMedicalWeb.Controllers
{
    public class HorarioTrabajoController : Controller
    {
        //
        // GET: /HorarioTrabajo/

        string controler = "HorarioTrabajo", vista = "Ini_HorarioTrabajo", PaginaAutorizada = Paginas.pag_HorarioTrabajo.ToString();
        public ActionResult Ini_HorarioTrabajo()
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



                //Partes en que se dividira una hora en el calendario
                List<numeros> listEsta = new List<numeros>();
                                
                for (int i = 2; i <=10; i++)
                {
                    numeros n = new numeros();
                    n.numero = i;
                    listEsta.Add(n);
                }
                ViewBag.partesHoraCalendario = new SelectList(listEsta, "numero", "numero");


                return View();
            }

        }

        [HttpPost]
        public JsonResult Save(IEnumerable<Usar_HorarioTrabajo> HoraTra, IEnumerable<Usar_HorarioTrabajoDestalle> HoraDetaSun, IEnumerable<Usar_HorarioTrabajoDestalle> HoraDetaMon, IEnumerable<Usar_HorarioTrabajoDestalle> HoraDetaTue, IEnumerable<Usar_HorarioTrabajoDestalle> HoraDetaWed, IEnumerable<Usar_HorarioTrabajoDestalle> HoraDetaThu, IEnumerable<Usar_HorarioTrabajoDestalle> HoraDetaFri, IEnumerable<Usar_HorarioTrabajoDestalle> HoraDetaSat)
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

            if (HoraDetaSun == null &&
               HoraDetaMon == null &&
               HoraDetaTue == null &&
              HoraDetaWed == null &&
              HoraDetaThu == null &&
               HoraDetaFri == null &&
               HoraDetaSat == null)
            {

                respuesta.error = "Por favor ingrese por lo menos un rango de hora en un dia";
                respuesta.respuesta = false;
                return Json(respuesta);
            }



            UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];

            using (var db = new DoctorMedicalWebEntities())
            {
                using (var dbtrans = db.Database.BeginTransaction())
                {

                    try
                    {

                        //busco si el doctor tiene ya un horario en este consultorio, sino entonces creo uno
                        var horarioTrabajo = ((from diag in db.HorarioTrabajoes
                                               where diag.DoctSecuencia_fk == usu.doctSecuencia
                                                && diag.PaisSecuencia_fk == usu.Consultorio.PaisSecuencia
                                               && diag.ClinSecuencia_fk == usu.Consultorio.clinSecuencia_fk
                                               && diag.ConsSecuencia_fk == usu.Consultorio.ConsSecuencia_fk
                                               select diag)).ToList();

                        #region Edito los buscado
                        //if existe  entonces edito
                        if (horarioTrabajo.Count() > 0)
                        {
                            foreach (var item in horarioTrabajo)
                            {
                                //busco en la lista  de dias  habilitados o desabilitados  que vino de la vista
                                //para ver como esta  para  ponerlo  a este registro que se esta editando

                                Usar_HorarioTrabajo guardarEdicion = HoraTra.Where(x =>
                                                                                   //x.DoctSecuencia_fk == usu.doctSecuencia &&
                                                                                   //x.ClinSecuencia_fk == usu.Consultorio.clinSecuencia_fk &&
                                                                                   //x.ConsSecuencia_fk == usu.Consultorio.ConsSecuencia_fk &&
                                                                                   x.DSemaSecuencia_Fk == item.DSemaSecuencia_Fk).SingleOrDefault();

                                //editar maestro
                                item.DoctSecuencia_fk = usu.doctSecuencia;
                                item.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                                item.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                item.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                item.HTrabRepetirSiempre = guardarEdicion.HTrabRepetirSiempre;
                                item.HTrabDiaHabilitado = guardarEdicion.HTrabDiaHabilitado;
                                item.HTrabFechaModificacion = Lib.GetLocalDateTime();
                                item.UsuaSecuenciaModificacion = usu.usuario.UsuaSecuencia;
                                item.HTrabPartesHoraSchedule = guardarEdicion.HTrabPartesHoraSchedule;

                             

                            }

                            //Borro cada Dia y vuelvo a insertar
                            borrarDetalle(db, usu);

                            //inserto el horario de domingo
                            insertarHorarioDetalle(db, HoraDetaSun, usu, 1);
                            //inserto el horario de lunes
                            insertarHorarioDetalle(db, HoraDetaMon, usu, 2);
                            //inserto el horario de martes
                            insertarHorarioDetalle(db, HoraDetaTue, usu, 3);
                            //inserto el horario de miercoles
                            insertarHorarioDetalle(db, HoraDetaWed, usu, 4);
                            //inserto el horario de jueves
                            insertarHorarioDetalle(db, HoraDetaThu, usu, 5);
                            //inserto el horario de viernes
                            insertarHorarioDetalle(db, HoraDetaFri, usu, 6);
                            //inserto el horario de sabado
                            insertarHorarioDetalle(db, HoraDetaSat, usu, 7);




                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Editar.ToString(), null);

                        }
                        #endregion

                        //sino existe
                        #region Insertar  maestro y detalle
                        else
                        {
                            int proximoItem = 0;

                            proximoItem = ((from diag in db.HorarioTrabajoes
                                            select (int?)diag.HTrabSecuencia).Max() ?? 0) + 1;

                            //int secuencia = 1;
                            //inserto los seite dias de la semana
                            foreach (var DiasHabilitados in HoraTra)
                            {

                                HorarioTrabajo horarioTrab = new HorarioTrabajo();
                                horarioTrab.DoctSecuencia_fk = usu.doctSecuencia;
                                horarioTrab.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                                horarioTrab.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                horarioTrab.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                horarioTrab.HTrabSecuencia = proximoItem;
                                horarioTrab.DSemaSecuencia_Fk = DiasHabilitados.DSemaSecuencia_Fk;
                                horarioTrab.HTrabRepetirSiempre = DiasHabilitados.HTrabRepetirSiempre;
                                horarioTrab.HTrabDiaHabilitado = DiasHabilitados.HTrabDiaHabilitado;
                                horarioTrab.HTrabFechaCreacion = Lib.GetLocalDateTime();
                                horarioTrab.UsuaSecuenciaCreacion = usu.usuario.UsuaSecuencia;
                                horarioTrab.HTrabFechaModificacion = Lib.GetLocalDateTime();
                                horarioTrab.UsuaSecuenciaModificacion = usu.usuario.UsuaSecuencia;
                                horarioTrab.HTrabEstaDesabilitado = false;
                                horarioTrab.HTrabPartesHoraSchedule = DiasHabilitados.HTrabPartesHoraSchedule;
                                db.HorarioTrabajoes.Add(horarioTrab);
                                proximoItem++;
                                //secuencia++;
                            }



                            ////inserto el horario de domingo
                            insertarHorarioDetalle(db, HoraDetaSun, usu, 1);
                            //inserto el horario de lunes
                            insertarHorarioDetalle(db, HoraDetaMon, usu, 2);
                            //inserto el horario de martes
                            insertarHorarioDetalle(db, HoraDetaTue, usu, 3);
                            //inserto el horario de miercoles
                            insertarHorarioDetalle(db, HoraDetaWed, usu, 4);
                            //inserto el horario de jueves
                            insertarHorarioDetalle(db, HoraDetaThu, usu, 5);
                            //inserto el horario de viernes
                            insertarHorarioDetalle(db, HoraDetaFri, usu, 6);
                            //inserto el horario de sabado
                            insertarHorarioDetalle(db, HoraDetaSat, usu, 7);

                        }


                        #endregion

                        respuesta.respuesta = true;



                        db.SaveChanges();
                        dbtrans.Commit();

                        var buscarHoriariosGuardados = ListaHorariosDeTrabajo(usu);
                        respuesta.dictionaryStringListObjecIenume = buscarHoriariosGuardados.dictionaryStringListObjecIenume;
                        respuesta.someCollection = buscarHoriariosGuardados.someCollection;

                    }
                    catch (Exception)
                    {
                        dbtrans.Rollback();
                        throw;
                    }
                }

            }





            return Json(respuesta);
        }


        /// <summary>
        /// borro los detalles de hoarior 
        /// </summary>
        public void borrarDetalle(DoctorMedicalWebEntities db, UsuarioLoguiado usu)
        {


            List<HorarioTrabajoDestalle> guardarEdicion = db.HorarioTrabajoDestalles.Where(x =>
                                                                        x.DoctSecuencia_fk == usu.doctSecuencia &&
                                                                          x.PaisSecuencia_fk == usu.Consultorio.PaisSecuencia_fk &&
                                                                        x.ClinSecuencia_fk == usu.Consultorio.clinSecuencia_fk &&
                                                                        x.ConsSecuencia_fk == usu.Consultorio.ConsSecuencia_fk
                                                                       ).ToList();
            foreach (HorarioTrabajoDestalle deleCitaOrig in guardarEdicion)
            {
                db.HorarioTrabajoDestalles.Remove(deleCitaOrig);
            }
        }



        /// <summary>
        /// Insertar  las horas del dia 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="horaDetall"></param>
        /// <param name="usu"></param>
        /// <param name="diaSemana"></param>
        public void insertarHorarioDetalle(DoctorMedicalWebEntities db, IEnumerable<Usar_HorarioTrabajoDestalle> horaDetall, UsuarioLoguiado usu, int diaSemana)
        {
            if (horaDetall == null) { return; }

            foreach (var item in horaDetall)
            {
                HorarioTrabajoDestalle htradetalle = new HorarioTrabajoDestalle();

                htradetalle.DoctSecuencia_fk = usu.doctSecuencia;
                htradetalle.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                htradetalle.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                htradetalle.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                htradetalle.DSemaSecuencia_Fk = diaSemana;
                DateTime timeValueini = Convert.ToDateTime(item.hi);
                DateTime timeValuefi = Convert.ToDateTime(item.hf);
                htradetalle.hi = timeValueini.TimeOfDay;
                htradetalle.hf = timeValuefi.TimeOfDay;
                htradetalle.ht = item.ht;
                db.HorarioTrabajoDestalles.Add(htradetalle);


            }


        }

        [HttpPost]
        public JsonResult BuscarListaDehorarios()
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
            UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];

            using (var db = new DoctorMedicalWebEntities())
            {
                     
                using (var dbtrans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var buscarHoriariosGuardados = ListaHorariosDeTrabajo(usu);
                        respuesta.dictionaryStringListObjecIenume = buscarHoriariosGuardados.dictionaryStringListObjecIenume;
                        respuesta.someCollection = buscarHoriariosGuardados.someCollection;
                        respuesta.respuesta = true;

                        return Json(respuesta);
                    }
                    catch (Exception ex)
                    {
                        dbtrans.Rollback();                      
                        respuesta.respuesta = false;                      
                        //respuesta.redirect = Url.Action("Ini_TipoFormulario", "TipoFormulario");
                        respuesta.redirect = Url.Action(vista, controler);
                        new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Error.ToString(), ex.GetBaseException().Message);
                        return Json(respuesta);
                    }
                }



            }
        }


        /// <summary>
        /// Buscar lista de horarios 
        /// </summary>
        /// <returns></returns>
        public ResponseModel ListaHorariosDeTrabajo(UsuarioLoguiado usu)
        {
            var respuesta = new ResponseModel();
            using (var db = new DoctorMedicalWebEntities())
            {
                bool proxyCreation = db.Configuration.ProxyCreationEnabled;
                //solucion del problema con la referencias circular circular reference
                /*The Circular Reference error during serializing the object of the Entity data Model can be resolved in two ways.
                    By using the ViewModel.
                    Setting the ProxyCreationEnabled as false.         db.Configuration.ProxyCreationEnabled = false;
                 * https://www.syncfusion.com/forums/119515/a-circular-reference-was-detected-while-serializing-an-object-of-type
                 * https://juristr.com/blog/2011/08/javascriptserializer-circular-reference/
                 */
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    //UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];

                    //     vw_UsuarioDoctor docSecuencia = db.vw_UsuarioDoctor.Where(d => d.UsuaSecuencia == usu.usuario.UsuaSecuencia).FirstOrDefault();



                    //busco si el doctor tiene ya un horario en este consultorio, sino entonces creo uno
                    var horarioTrabajo = ((from diag in db.HorarioTrabajoes
                                           where diag.DoctSecuencia_fk == usu.doctSecuencia
                                           && diag.PaisSecuencia_fk == usu.Consultorio.PaisSecuencia
                                           && diag.ClinSecuencia_fk == usu.Consultorio.clinSecuencia_fk
                                           && diag.ConsSecuencia_fk == usu.Consultorio.ConsSecuencia_fk
                                           select diag)).ToList();
                    //Maestro
                    var listaHorarioTrab = new List<Usar_HorarioTrabajo>();

                    //crear objeto role y asignar a la lista de usaroles
                    foreach (var item in horarioTrabajo)
                    {
                        var ObjNuevo = new Usar_HorarioTrabajo();
                        //asignar contenido a otro objeto
                        CopyClass.CopyObject(item, ref ObjNuevo);
                        listaHorarioTrab.Add(ObjNuevo);


                    }
                    //inserto master                  
                    respuesta.someCollection = listaHorarioTrab;

                    //Detalle cada dia
                    Dictionary<string, List<Usar_HorarioTrabajoDestalle>> dictionaryStringListObjecIenume = new Dictionary<string, List<Usar_HorarioTrabajoDestalle>>();

                    //busco todos los horarios de este doctor en este consultorio

                    var horarioTrabajoDetall = ((from diag in db.HorarioTrabajoDestalles
                                                 where diag.DoctSecuencia_fk == usu.doctSecuencia
                                                 && diag.PaisSecuencia_fk == usu.Consultorio.PaisSecuencia
                                                 && diag.ClinSecuencia_fk == usu.Consultorio.clinSecuencia_fk
                                                 && diag.ConsSecuencia_fk == usu.Consultorio.ConsSecuencia_fk
                                                 select diag)).ToList();


                    //inserto el detalle de cada dia de este horario de ttrabajo en una lista para poderla asignar
                    //a cada lisbox de cada dia
                    dictionaryStringListObjecIenume.Add("sundayL", llenarDetalleHorario(horarioTrabajoDetall.Where(x => x.DSemaSecuencia_Fk == 1).ToList()));
                    dictionaryStringListObjecIenume.Add("mondayL", llenarDetalleHorario(horarioTrabajoDetall.Where(x => x.DSemaSecuencia_Fk == 2).ToList()));
                    dictionaryStringListObjecIenume.Add("tuesdayL", llenarDetalleHorario(horarioTrabajoDetall.Where(x => x.DSemaSecuencia_Fk == 3).ToList()));
                    dictionaryStringListObjecIenume.Add("wednesdayL", llenarDetalleHorario(horarioTrabajoDetall.Where(x => x.DSemaSecuencia_Fk == 4).ToList()));
                    dictionaryStringListObjecIenume.Add("thursdayL", llenarDetalleHorario(horarioTrabajoDetall.Where(x => x.DSemaSecuencia_Fk == 5).ToList()));
                    dictionaryStringListObjecIenume.Add("fridayL", llenarDetalleHorario(horarioTrabajoDetall.Where(x => x.DSemaSecuencia_Fk == 6).ToList()));
                    dictionaryStringListObjecIenume.Add("saturdayL", llenarDetalleHorario(horarioTrabajoDetall.Where(x => x.DSemaSecuencia_Fk == 7).ToList()));

                    respuesta.dictionaryStringListObjecIenume = dictionaryStringListObjecIenume;


                    //Utilizado para insertar el listado al grid
                    //ViewBag.listHorarios = listaHorarioTrab;





                }
                catch (Exception ex)
                {

                    throw ex;
                }
                finally
                {
                    db.Configuration.ProxyCreationEnabled = proxyCreation;
                }
            }

            return respuesta;
        }

        private List<Usar_HorarioTrabajoDestalle> llenarDetalleHorario(List<HorarioTrabajoDestalle> lis)
        {
            List<Usar_HorarioTrabajoDestalle> nue = new List<Usar_HorarioTrabajoDestalle>();
            
            foreach (var item in lis)
            {
                Usar_HorarioTrabajoDestalle objn = new Usar_HorarioTrabajoDestalle();
                objn.DoctSecuencia_fk = item.DoctSecuencia_fk;
                objn.PaisSecuencia_fk = item.PaisSecuencia_fk;
                objn.ClinSecuencia_fk = item.ClinSecuencia_fk;
                objn.ConsSecuencia_fk = item.ConsSecuencia_fk;
                objn.DSemaSecuencia_Fk = item.DSemaSecuencia_Fk;
                objn.hi = item.hi.ToString();
                objn.hf = item.hf.ToString();
                objn.ht = item.ht;
                nue.Add(objn);
            }


            return nue;
        }




    }
  public   class numeros
    {
        public int numero { get; set; }
    }
    
}
