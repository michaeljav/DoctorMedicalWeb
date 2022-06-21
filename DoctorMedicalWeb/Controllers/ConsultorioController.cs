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
    public class ConsultorioController : Controller
    {
        //
        // GET: /Consultorio/
        string controler = "Consultorio", vista = "Ini_Consultorio", PaginaAutorizada = Paginas.pag_Consultorio.ToString();
        public ActionResult Ini_Consultorio()
        {
            //Si NO esta loguieado lo redireccionara al loguin
            if (HttpContext.Session["user"] == null)
            {
                return RedirectToAction("Index", "PaginaPresentacion");
            }

            //pARA EN LA VISTA PODER SABER A QUE  CONTROLADOR EL AJAX  LLAMARA
            ViewBag.ControlCsharp = controler;
            ViewBag.VistaCsharp = vista;

            var usu = (UsuarioLoguiado)HttpContext.Session["user"];
            using (var db = new DoctorMedicalWebEntities())
            {
                //si es personal que esta creando un usuario, no permitir proseguir.
                if (usu.persSecuencia > 0)
                {
                    TempData["NoTienePermisoParaPagina"] = "Usted no tiene Permiso!";
                    //respuesta.respuesta = false;
                    //respuesta.error = "Usted No tiene permiso para crear usuario. Favor solicitarle al doctor.";
                    //return Json(respuesta);
                    return RedirectToAction("Index", "DashBoard");
                }

                //para que el div de accion solo aparezca si son 
                //vistas difrentes a home
                ViewBag.isHome = false;

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

                //Enlistar los 5 ultimos y lleno
                //el   ViewBag.ultimosCinco
                var a = UltimosCincoRegistros();

                var rolesList = ((from Rols in db.Clinicas
                                  select Rols).ToList());
                ViewBag.Consultorio = new SelectList(rolesList, "clinSecuencia", "clinRazonSocial");


                List<EstaDesabilitado> listEsta = new List<EstaDesabilitado> {
              //new EstaDesabilitado(){EstaDesabilitados ="Activo"},
              new EstaDesabilitado(){EstaDesabilitados ="Desactivo"},
          
            };
                ViewBag.estaDesactivo = new SelectList(listEsta, "EstaDesabilitados", "EstaDesabilitados");


                var usarConsultorio = new Usar_Consultorio();
                //si  esta lleno es por que esta editando
                if (Session["Usar_Consultorio"] != null)
                {
                    usarConsultorio = (Usar_Consultorio)Session["Usar_Consultorio"];

                    //limpiar sesion
                    Session["Usar_Consultorio"] = null;
                    return View(usarConsultorio);
                }

                return View(usarConsultorio);
            }

        }

        public ResponseModel UltimosCincoRegistros()
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
                    UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];

                    //     vw_UsuarioDoctor docSecuencia = db.vw_UsuarioDoctor.Where(d => d.UsuaSecuencia == usu.usuario.UsuaSecuencia).FirstOrDefault();

                    //buscar los ultimos  cinco registros  de roles del doctor loguiado
                    var objListaUltimosCinco = (from tf in db.Consultorios
                                                where tf.DoctSecuencia_fk == usu.doctSecuencia
                                                && tf.EstaDesabilitado == false
                                                orderby tf.ConsSecuencia descending
                                                select tf).Take(5).ToList();

                    var listClinics = new List<Usar_Consultorio>();
                    //crear objeto role y asignar a la lista de usaroles
                    foreach (var item in objListaUltimosCinco)
                    {
                        var ObjNuevo = new Usar_Consultorio();
                        //asignar contenido a otro objeto
                        CopyClass.CopyObject(item, ref ObjNuevo);
                        listClinics.Add(ObjNuevo);
                    }
                    //Utilizado para insertar el listado al grid
                    ViewBag.ultimosCinco = listClinics;


                    //ienumerable lista
                    respuesta.someCollection = listClinics;


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
          [HttpPost]
        public JsonResult Borrar(Usar_Consultorio usar_consultorio)
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

                //si secuenca esta vacio devolver mensaje
                if (string.IsNullOrEmpty(usar_consultorio.ConsSecuencia.ToString()) || usar_consultorio.ConsSecuencia < 1)
                {
                    respuesta.respuesta = false;
                    respuesta.error = "Favor seleccionar una clinica";
                    respuesta.redirect = Url.Action(vista, controler);
                    return Json(respuesta);

                }

                using (var dbtrans = db.Database.BeginTransaction())
                {
                    try
                    {
                        //borro registro
                        if (usar_consultorio != null)
                        {

                            //Paciente objetoNuevo = new Paciente();
                            ////asignar contenido a otro objeto
                            //CopyClass.CopyObject(usar_pacient, ref objetoNuevo);


                            var borrar = (from objquery in db.Consultorios
                                          where objquery.DoctSecuencia_fk == usu.doctSecuencia
                                          && objquery.ConsSecuencia == usar_consultorio.ConsSecuencia
                                          select objquery).SingleOrDefault();

                            if (borrar != null)
                            {

                                borrar.EstaDesabilitado = true;
                                db.SaveChanges();
                                dbtrans.Commit();
                                respuesta.someCollection = UltimosCincoRegistros().someCollection;
                                respuesta.respuesta = true;
                                respuesta.redirect = Url.Action(vista, controler);
                                new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Borrar.ToString(), null);
                            }
                            else
                            {
                                dbtrans.Rollback();
                                respuesta.someCollection = UltimosCincoRegistros().someCollection;
                                respuesta.respuesta = false;
                                respuesta.error = "No existe este registro.";
                                respuesta.redirect = Url.Action(vista, controler);
                            }
                        }


                        return Json(respuesta);
                    }
                    catch (Exception ex)
                    {
                        dbtrans.Rollback();
                        respuesta.someCollection = UltimosCincoRegistros().someCollection;
                        respuesta.respuesta = false;
                        respuesta.error = "Posiblemente usted tiene otro registro que depende de éste o No Existe este Formulario";
                        //respuesta.redirect = Url.Action("Ini_TipoFormulario", "TipoFormulario");
                        respuesta.redirect = Url.Action(vista, controler);
                        new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Error.ToString(), ex.GetBaseException().Message);
                        return Json(respuesta);
                    }
                }



            }
        }
          [HttpPost]
        public JsonResult Editar(Usar_Consultorio usar_consultorio)
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
                if (usar_consultorio != null)
                {

                    var ObjetoEditar = (from ObjQuery in db.Consultorios
                                        where ObjQuery.DoctSecuencia_fk == usu.doctSecuencia
                                        && ObjQuery.ConsSecuencia == usar_consultorio.ConsSecuencia
                                        && ObjQuery.EstaDesabilitado == false
                                        select ObjQuery).SingleOrDefault();

                    if (ObjetoEditar != null)
                    {
                        var objNuevo = new Usar_Consultorio();
                        //asignar contenido a otro objeto
                        CopyClass.CopyObject(ObjetoEditar, ref objNuevo);
                        //llenno la sesion con el role a editar
                        //para abrirlo en  el metodo principal del controler
                        Session["Usar_Consultorio"] = objNuevo;

                        //existe este formulario
                        respuesta.respuesta = true;
                        respuesta.redirect = Url.Action(vista, controler);

                    }

                }
                else
                {
                    //No existe este formulario
                    respuesta.respuesta = false;
                    respuesta.error = "Este consultorio no existe";

                }
            }
            return Json(respuesta);
        }
          [HttpPost]
        public JsonResult Save(Usar_Consultorio usar_consultorio)
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
                int proximoItem = 0;

                //si no ha validado volver y mostrar mensajes de validacion
                if (!ModelState.IsValid)
                {
                    respuesta.respuesta = false;
                    //respuesta.redirect = Url.Content("~/Roles/Roles");
                    respuesta.redirect = Url.Content("~/" + controler + "/" + vista + "");
                    respuesta.error = "Llenar Campos requeridos ";
                    return Json(respuesta);
                }


                if (usu.persSecuencia > 0)
                {
                    respuesta.respuesta = false;
                    respuesta.error = "Usted es no es doctor.";
                    return Json(respuesta);
                }

                using (var dbtrans = db.Database.BeginTransaction())
                {

                    try
                    {


                        //add new 
                        if (string.IsNullOrEmpty(usar_consultorio.ConsSecuencia.ToString()) || usar_consultorio.ConsSecuencia == 0)
                        {
                            var objNuevo = new Consultorio();
                            //asignar contenido a otro objeto
                            CopyClass.CopyObject(usar_consultorio, ref objNuevo);



                            //si existe el paciente del doctor no se introduce
                            var existe = (from objbuscar in db.Consultorios
                                          where objbuscar.DoctSecuencia_fk == usu.doctSecuencia &&
                                             objbuscar.ClinSecuencia_fk == usar_consultorio.ClinSecuencia_fk &&
                                             objbuscar.EstaDesabilitado == false &&
                                          objbuscar.ConsCodigo == usar_consultorio.ConsCodigo
                                          select objbuscar).SingleOrDefault();
                            if (existe != null)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "Ya existe este consultorio";
                                respuesta.redirect = Url.Action(vista, controler);
                                return Json(respuesta);
                            }


                            
                            objNuevo.DoctSecuencia_fk = usu.doctSecuencia;
                            objNuevo.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                            objNuevo.UsuaSecuenciaCreacion = usu.usuario.UsuaSecuencia;
                            objNuevo.ConsFechaCreacion = Lib.GetLocalDateTime();

                            //buscando la proxima secuencia, el doctor en todas sus consultorios tiene los mismos roles 
                            proximoItem = ((from objBusc in db.Consultorios
                                            where objBusc.DoctSecuencia_fk == usu.doctSecuencia
                                            select (int?)objBusc.ConsSecuencia).Max() ?? 0) + 1;
                            objNuevo.ConsSecuencia = proximoItem;

                            db.Consultorios.Add(objNuevo);

                            //guardo en la tabla usuarioconsultorio  este nuevo consultorio que pertenece a este usuario,
                            //para que cuando este asignando consultorios a los usuarios de personal, el sistema pueda mostrar
                            //los  consultorios ya que homologue los consultorios de doctores atravez de  el codigo de usuario en la tabla usuarioconsultorio
                            // y asi tambien tener el nombre de la clinica y consultorio junto en un campo.
                            //Nota: Con buscar el usuario, sé cuantos consultorios tiene ese usuario, para cuando se vaya a loguiar
                            //solo tener que buscar el usuario y asi ver si tiene mas de un consultorio asignado.

                            UsuarioConsultorio uc = new UsuarioConsultorio();
                            uc.UsuaSecuencia_fk = usu.usuario.UsuaSecuencia;

                            uc.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                            uc.clinSecuencia_fk = objNuevo.ClinSecuencia_fk;
                            uc.ConsSecuencia_fk = objNuevo.ConsSecuencia;
                            //buscar secuencia en la tabla
                            uc.UConsSecuencia = ((from objBusc in db.UsuarioConsultorios
                                                  where objBusc.UsuaSecuencia_fk == usu.usuario.UsuaSecuencia
                                                  select (int?)objBusc.UConsSecuencia).Max() ?? 0) + 1;
                            //buscar nombre de clinica
                            string nombreClinica = (from objbuscar in db.Clinicas
                                                    where objbuscar.clinSecuencia == objNuevo.ClinSecuencia_fk &&
                                                           objbuscar.PaisSecuencia_fk == usu.Consultorio.PaisSecuencia
                                                    select objbuscar.clinRazonSocial).SingleOrDefault();
                            if (string.IsNullOrEmpty(nombreClinica))
                            {
                                respuesta.respuesta = false;
                                respuesta.error = " No se pudo conseguir el nombre de la clinica selecionada ";
                                respuesta.redirect = Url.Action(vista, controler);
                                return Json(respuesta);
                            }
                            uc.clinRazonSocial = nombreClinica;
                            uc.ConsCodigo = objNuevo.ConsCodigo;
                            uc.ConsDescripcion = objNuevo.ConsDescripcion;
                            uc.UsuaSecuenciaCreacion = usu.usuario.UsuaSecuencia;
                            uc.UConsFechaCreacion = Lib.GetLocalDateTime();

                            db.UsuarioConsultorios.Add(uc);


                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Nuevo.ToString(), null);
                        }
                        //Editando 
                        else
                        {

                            //busco el objeto que se editara
                            //lo busco por la secuencia para que pueda editar ese mismo sin tener que borrarlo
                            //si lo busco por el nombre no lo  editar ya que no lo encontrara.
                            var objetoEditar = db.Consultorios.Where(ro => ro.DoctSecuencia_fk == usu.doctSecuencia && ro.ConsSecuencia == usar_consultorio.ConsSecuencia).SingleOrDefault();

                            if (objetoEditar == null)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "No existe este consultorio";
                                respuesta.redirect = Url.Action(vista, controler);
                                return Json(respuesta);
                            }
                            ////si hay un cilinica consultorio no se puede agregar otro igual
                            //var existeeste = db.Consultorios.Where(ro => ro.DoctSecuencia_fk == usu.doctSecuencia
                            //     && ro.ClinSecuencia_fk == usar_consultorio.ClinSecuencia_fk
                            //    && ro.ConsSecuencia == usar_consultorio.ConsSecuencia).SingleOrDefault();

                            //if (existeeste != null)
                            //{
                            //    respuesta.respuesta = false;
                            //    respuesta.error = "ya existe para esta clinica este consultorio";
                            //    respuesta.redirect = Url.Action(vista, controler);
                            //    return Json(respuesta);
                            //}
                            //asigno al objeto que se editara los nuevos datos introducidos a editar
                            CopyClass.CopyObject(usar_consultorio, ref objetoEditar);
                            objetoEditar.DoctSecuencia_fk = usu.doctSecuencia;
                            objetoEditar.UsuaSecuenciaModificacion = usu.usuario.UsuaSecuencia;
                            objetoEditar.ConsFechaModificacion = Lib.GetLocalDateTime();





                            //guardo en la tabla usuarioconsultorio  este nuevo consultorio que pertenece a este usuario,
                            //para que cuando este asignando consultorios a los usuarios de personal, el sistema pueda mostrar
                            //los  consultorios ya que homologue los consultorios de doctores atravez de  el codigo de usuario en la tabla usuarioconsultorio
                            // y asi tambien tener el nombre de la clinica y consultorio junto en un campo.
                            //Nota: Con buscar el usuario, sé cuantos consultorios tiene ese usuario, para cuando se vaya a loguiar
                            //solo tener que buscar el usuario y asi ver si tiene mas de un consultorio asignado.

                            //busco el consultorio de este usuario Doctor para modificarlo
                            UsuarioConsultorio  uc = db.UsuarioConsultorios.Where(ro =>
                                                                    ro.UsuaSecuencia_fk == usu.usuario.UsuaSecuencia &&                                                                                                                    
                                                                    ro.ConsSecuencia_fk == usar_consultorio.ConsSecuencia).SingleOrDefault();

                            if (uc != null) {
                                uc.clinSecuencia_fk = usar_consultorio.ClinSecuencia_fk;
                    
                        
                            //buscar nombre de clinica
                            string nombreClinica = (from objbuscar in db.Clinicas
                                                    where objbuscar.clinSecuencia == objetoEditar.ClinSecuencia_fk &&
                                                           objbuscar.PaisSecuencia_fk == usu.Consultorio.PaisSecuencia
                                                    select objbuscar.clinRazonSocial).SingleOrDefault();
                            if (string.IsNullOrEmpty(nombreClinica))
                            {
                                respuesta.respuesta = false;
                                respuesta.error = " No se pudo conseguir el nombre de la clinica selecionada ";
                                respuesta.redirect = Url.Action(vista, controler);
                                return Json(respuesta);
                            }
                            uc.EstaDesabilitado = usar_consultorio.EstaDesabilitado;
                            uc.clinRazonSocial = nombreClinica;
                            uc.ConsCodigo = usar_consultorio.ConsCodigo;
                            uc.ConsDescripcion = usar_consultorio.ConsDescripcion;
                            uc.UsuaSecuenciaModificacion = usu.usuario.UsuaSecuencia;
                            uc.UConsFechaModificacion = Lib.GetLocalDateTime();
                            }
                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Editar.ToString(), null);
                        }

                        int returnsave = db.SaveChanges();
                        dbtrans.Commit();

                        respuesta.respuesta = true;
                        respuesta.returnSaveChange = returnsave;
                        respuesta.redirect = Url.Content("~/" + controler + "/" + vista + "");

                        //Enlistar los 5 ultimos Registros                
                        respuesta.someCollection = UltimosCincoRegistros().someCollection;


                        return Json(respuesta);
                    }
                    catch (Exception ex)
                    {

                        if (dbtrans != null)
                        {

                            dbtrans.Rollback();
                            dbtrans.Dispose();
                        }

                        respuesta.respuesta = false;
                        respuesta.redirect = Url.Content("~/" + controler + "/" + vista + "");
                        respuesta.error = "Ocurrio Un inconveniente, Favor volver a tratar. " + ex.Message + "---" + ex.GetBaseException().ToString();
                        new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Error.ToString(), ex.GetBaseException().Message);
                        return Json(respuesta, JsonRequestBehavior.AllowGet);
                        // return Json(new { Success = false, Message = "Ocurrio Un inconveniente, Favor volver a tratar. " + ex.Message + "---" + ex.GetBaseException().ToString() });
                    }
                    finally
                    {
                        if (dbtrans != null)
                        {
                            dbtrans.Dispose();
                        }
                    }
                }
            }

        }//end method save

        public ActionResult Consultoriolista()
        {   //pARA EN LA VISTA PODER SABER A QUE  CONTROLADOR EL AJAX  LLAMARA
            ViewBag.ControlCsharp = controler;
            ViewBag.VistaCsharp = vista;

            //Si NO esta loguieado lo redireccionara al loguin
            if (HttpContext.Session["user"] == null)
            {
                return RedirectToAction("Index", "PaginaPresentacion");
            }

            //lleno el combo de roles
            UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];

            using (var db = new DoctorMedicalWebEntities())
            {
                //si no tiene  el listado formulario devuelvo al loguin
                List<Formulario> _sessformularioParaDesabilitar = new List<Formulario>();
                _sessformularioParaDesabilitar = (List<App_Data.Formulario>)Session["sessformularioParaDesabilitar"];

                if (_sessformularioParaDesabilitar == null || _sessformularioParaDesabilitar.Count == 0)
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

                //es obligatorio por que  la llamada necesita un proxy
                db.Configuration.ProxyCreationEnabled = false;



                //para que el div de accion solo aparezca si son 
                //no son home y como no quiero que aparezca el div lo en esta vista de lista
                //lo pongo true como para indicar que esta pagina es home, para que no me aparezca el 
                //div de accion
                ViewBag.isHome = true;
                var objetoEnlistar = (from tf in db.Consultorios
                                      where tf.DoctSecuencia_fk == usu.doctSecuencia
                                      && tf.EstaDesabilitado== false
                                      orderby tf.ConsCodigo
                                      select tf).ToList();

                var listObjetoNuevo = new List<Usar_Consultorio>();
                //crear objeto tipo formulario y asignar a la lista de usartipoformulario
                foreach (var item in objetoEnlistar)
                {
                    var objNuevoParaLista = new Usar_Consultorio();
                    //asignar contenido a otro objeto
                    CopyClass.CopyObject(item, ref objNuevoParaLista);
                    listObjetoNuevo.Add(objNuevoParaLista);
                }
                ViewBag.datasource = listObjetoNuevo;
                return View();
            }
        }

    }
}
