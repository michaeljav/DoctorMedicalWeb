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
    public class DiagnosticoController : Controller
    {
        //
        // GET: /Diagnostico/
        string controler = "Diagnostico", vista = "Ini_Diagnostico",  PaginaAutorizada =  Paginas.pag_Diagnosticos.ToString();
        public ActionResult Ini_Diagnostico()
        {
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
            ViewBag.isHome = false;

            //Enlistar los 5 ultimos y lleno
            //el   ViewBag.ultimosCinco
            var a = UltimosCincoRegistros();
            

            Usar_Diagnostico usarDiagnostico = new Usar_Diagnostico();
            //si  esta lleno es por que esta editando
            if (Session["Usar_Diagnostico"] != null)
            {
                usarDiagnostico = (Usar_Diagnostico)Session["Usar_Diagnostico"];
                //limpiar sesion
                Session["Usar_Diagnostico"] = null;            

                return View(usarDiagnostico);
            }

            return View(usarDiagnostico);
        }

        //buscar los ultimos 5 registros
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
                    List<Diagnostico> DiagnostiListaUltimosCinco = (from tf in db.Diagnosticoes
                                                                                      where tf.DoctSecuencia_fk == usu.doctSecuencia
                                                                                      && tf.EstaDesabilitado == false
                                                                                      orderby tf.DiagSecuencia descending
                                                                                      select tf).Take(5).ToList();

                    var listUsar_Diagnostico = new List<Usar_Diagnostico>();
                    //crear objeto role y asignar a la lista de usaroles
                    foreach (var item in DiagnostiListaUltimosCinco)
                    {
                        Usar_Diagnostico diag = new Usar_Diagnostico();
                        //asignar contenido a otro objeto
                        CopyClass.CopyObject(item, ref diag);
                        listUsar_Diagnostico.Add(diag);
                    }
                    //Utilizado para insertar el listado al grid
                    ViewBag.ultimosCinco = listUsar_Diagnostico;


                    //ienumerable lista
                    respuesta.someCollection = listUsar_Diagnostico;


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
        public JsonResult Save(Usar_Diagnostico usar_Diagnostico)
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



                using (var dbtrans = db.Database.BeginTransaction())
                {

                    try
                    {

                       
                        //add new 
                        if (string.IsNullOrEmpty(usar_Diagnostico.DiagSecuencia.ToString()) || usar_Diagnostico.DiagSecuencia == 0)
                        { 
                            Diagnostico diagnost = new Diagnostico();

                            //asignar contenido a otro objeto
                            CopyClass.CopyObject(usar_Diagnostico, ref diagnost);
                            
                            //si existe la insituciona aseguradora del doctor no se introduce
                            var existe = (from dgtico in db.Diagnosticoes
                                          where dgtico.DiagNombre == usar_Diagnostico.DiagNombre
                                              && dgtico.DoctSecuencia_fk == usu.doctSecuencia
                                          select dgtico).SingleOrDefault();
                            if (existe != null)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "Ya existe este diagnostico";
                                respuesta.redirect = Url.Action(vista, controler);
                                return Json(respuesta);
                            }


                            diagnost.DoctSecuencia_fk = usu.doctSecuencia;
                            diagnost.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                            diagnost.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                            diagnost.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;

                            //buscando la proxima secuencia, el doctor en todas sus consultorios tiene los mismos roles 
                            proximoItem = ((from diag in db.Diagnosticoes
                                            where diag.DoctSecuencia_fk == usu.doctSecuencia
                                            select (int?)diag.DiagSecuencia).Max() ?? 0) + 1;
                            diagnost.DiagSecuencia = proximoItem;
                            db.Diagnosticoes.Add(diagnost);

                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Nuevo.ToString(), null);
                        }
                        //Editando 
                        else
                        {

                            //busco el objeto que se editara
                            //lo busco por la secuencia para que pueda editar ese mismo sin tener que borrarlo
                            //si lo busco por el nombre no lo  editar ya que no lo encontrara.
                            var objEditando = db.Diagnosticoes  .Where(ro => ro.DoctSecuencia_fk == usu.doctSecuencia
                                            && ro.DiagSecuencia == usar_Diagnostico.DiagSecuencia).SingleOrDefault();

                            if (objEditando == null)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "No existe Diagnostico";
                                //respuesta.redirect = Url.Action(vista, controler);
                                return Json(respuesta);
                            }
                            usar_Diagnostico.DoctSecuencia_fk = usu.doctSecuencia;
                            usar_Diagnostico.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                            usar_Diagnostico.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                            usar_Diagnostico.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;

                            //asigno al objeto que se editara los nuevos datos introducidos a editar
                            CopyClass.CopyObject(usar_Diagnostico, ref objEditando);

                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Editar.ToString(), null);
                        }

                        db.SaveChanges();
                        dbtrans.Commit();

                        respuesta.respuesta = true;
                        respuesta.redirect = Url.Content("~/" + controler + "/" + vista + "");

                        //Enlistar los 5 ultimos Registros                
                        respuesta.someCollection = UltimosCincoRegistros().someCollection;
                        //Para limpiar  la lista de
                        respuesta.ObjectGridList = new List<Usar_InstitucionAseguradoraPlanes>();
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
          [HttpPost]
        public JsonResult Borrar(Usar_Diagnostico usar_Diagnostico)
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
                if (usar_Diagnostico.DiagSecuencia < 1 || usar_Diagnostico.DiagSecuencia == null)
                {
                    respuesta.respuesta = false;
                    respuesta.error = "Favor seleccionar una diagnostico";
                    respuesta.redirect = Url.Action(vista, controler);
                    return Json(respuesta);

                }
                using (var dbtrans = db.Database.BeginTransaction())
                {
                    try
                    {
                        //borro registro
                        if (usar_Diagnostico != null)
                        {

                            Diagnostico diagno = new Diagnostico();
                            //asignar contenido a otro objeto
                            CopyClass.CopyObject(usar_Diagnostico, ref diagno);


                            Diagnostico borrar = (from InstiSeg in db.Diagnosticoes
                                                               where InstiSeg.DoctSecuencia_fk == usu.doctSecuencia
                                                                  && InstiSeg.DiagSecuencia == usar_Diagnostico.DiagSecuencia
                                                               select InstiSeg).SingleOrDefault();

                            if (borrar != null)
                            {


                                db.Diagnosticoes.Remove(borrar);
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
        public JsonResult Editar(Usar_Diagnostico usar_Diagnostico)
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
                if (usar_Diagnostico != null)
                {

                    Diagnostico objEdit = (from IA in db.Diagnosticoes
                                                           where IA.DoctSecuencia_fk == usu.doctSecuencia
                                                           && IA.DiagSecuencia == usar_Diagnostico.DiagSecuencia
                                                           && IA.EstaDesabilitado == false
                                                           select IA).SingleOrDefault();

                    if (objEdit != null)
                    {
                        Usar_Diagnostico objNuevo = new Usar_Diagnostico();
                        //asignar contenido a otro objeto
                        CopyClass.CopyObject(objEdit, ref objNuevo);
                        //llenno la sesion con el role a editar
                        //para abrirlo en  el metodo principal del controler
                        Session["Usar_Diagnostico"] = objNuevo;
                       
                        //existe este formulario
                        respuesta.respuesta = true;
                        respuesta.redirect = Url.Action(vista, controler);

                    }

                }
                else
                {
                    //No existe este formulario
                    respuesta.respuesta = false;
                    respuesta.error = "Esta Insitucion Aseguradora no existe";

                }
            }
            return Json(respuesta);
        }
        
        public ActionResult Diagnosticolista()
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
                List<Diagnostico> objelist = (from tf in db.Diagnosticoes
                                              where tf.DoctSecuencia_fk == usu.doctSecuencia
                                              && tf.EstaDesabilitado == false
                                              orderby tf.DiagNombre
                                              select tf).ToList();

                var listObjToShow = new List<Usar_Diagnostico>();
                //crear objeto tipo formulario y asignar a la lista de usartipoformulario
                foreach (var item in objelist)
                {
                    Usar_Diagnostico UsarObjNewToShow = new Usar_Diagnostico();
                    //asignar contenido a otro objeto
                    CopyClass.CopyObject(item, ref UsarObjNewToShow);
                    listObjToShow.Add(UsarObjNewToShow);
                }
                ViewBag.datasource = listObjToShow;
                return View();
            }
        }


    }
}
