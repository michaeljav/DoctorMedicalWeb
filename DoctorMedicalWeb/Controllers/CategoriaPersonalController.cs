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
    public class CategoriaPersonalController : Controller
    {
        string controler = "CategoriaPersonal", vista = "categoriaPersonal",  PaginaAutorizada =  Paginas.pag_CategoriaPersonal.ToString();
   
        //
        // GET: /CategoriaPersonal/
  
        public ActionResult categoriaPersonal()
        {
            //Si NO esta loguieado lo redireccionara al loguin
            if (HttpContext.Session["user"] == null)
            {
                return RedirectToAction("Index", "PaginaPresentacion");
            }
                   UsuarioLoguiado usu  = (UsuarioLoguiado)HttpContext.Session["user"];
            //si es personal que esta creando un usuario, no permitir proseguir.
            if (usu.persSecuencia > 0)
            {
                TempData["NoTienePermisoParaPagina"] = "Usted no tiene Permiso!";
                //respuesta.respuesta = false;
                //respuesta.error = "Usted No tiene permiso para crear usuario. Favor solicitarle al doctor.";
                //return Json(respuesta);
                return RedirectToAction("Index", "DashBoard");
            }


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

            Usar_CategoriaPersonal usarCategoriaPersonal = new Usar_CategoriaPersonal();

            //si  esta lleno es por que esta editando
            if (Session["Usar_CategoriaPersonal"] != null)
            {
                usarCategoriaPersonal = (Usar_CategoriaPersonal)Session["Usar_CategoriaPersonal"];
                //limpiar sesion
                Session["Usar_CategoriaPersonal"] = null;
                return View(usarCategoriaPersonal);
            }


            




            return View(usarCategoriaPersonal);
        }//End Metodo CategoriaPersonal
          [HttpPost]
        public JsonResult Editar(Usar_CategoriaPersonal usar_CategoriaPersonal)
        {        //string controler = "CategoriaPersonal", vista = "categoriaPersonal";
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
                if (usar_CategoriaPersonal != null)
                {
                    CategoriaPersonal catePersonal = new CategoriaPersonal();

                     catePersonal = (from cPersonal in db.CategoriaPersonals
                                                      where cPersonal.CPersSecuencia == usar_CategoriaPersonal.CPersSecuencia
                                                                         && cPersonal.DoctSecuencia_fk == usu.doctSecuencia
                                                                         && cPersonal.EstaDesabilitado == false
                                                      select cPersonal).SingleOrDefault();

                    if (catePersonal != null)
                    {
                        Usar_CategoriaPersonal usarCategoriaPersonal = new Usar_CategoriaPersonal();
                        //asignar contenido a otro objeto
                        CopyClass.CopyObject(catePersonal, ref usarCategoriaPersonal);
                        //llenno la sesion con el role a editar
                        //para abrirlo en  el metodo principal del controler
                        Session["Usar_CategoriaPersonal"] = usarCategoriaPersonal;
                        //existe este formulario
                        respuesta.respuesta = true;
                        respuesta.redirect = Url.Action(vista, controler);

                    }

                }
                else
                {
                    //No existe este formulario
                    respuesta.respuesta = false;
                    respuesta.error = "Este formulario no existe";

                }
            }


            return Json(respuesta);
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

                    vw_UsuarioDoctor docSecuencia = db.vw_UsuarioDoctor.Where(d => d.UsuaSecuencia == usu.usuario.UsuaSecuencia).FirstOrDefault();

                    //buscar los ultimos  cinco registros  de roles del doctor loguiado
                    List<CategoriaPersonal> CategoriaPersonalListaUltimosCinco = (from tf in db.CategoriaPersonals
                                                                                  where tf.DoctSecuencia_fk == docSecuencia.DoctSecuencia
                                                                                  && tf.EstaDesabilitado == false
                                                                                  orderby tf.CPersSecuencia descending
                                                                                  select tf).Take(5).ToList();

                    var listUsar_CategoriaPersonal = new List<Usar_CategoriaPersonal>();
                    //crear objeto role y asignar a la lista de usaroles
                    foreach (var item in CategoriaPersonalListaUltimosCinco)
                    {
                        Usar_CategoriaPersonal formulario = new Usar_CategoriaPersonal();
                        //asignar contenido a otro objeto
                        CopyClass.CopyObject(item, ref formulario);
                        listUsar_CategoriaPersonal.Add(formulario);
                    }
                    //Utilizado para insertar el listado al grid
                    ViewBag.ultimosCinco = listUsar_CategoriaPersonal;

                    //ienumerable lista
                    respuesta.someCollection = listUsar_CategoriaPersonal;

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
        }//End Cincoultimosresgistros
          [HttpPost]
        public JsonResult Save(Usar_CategoriaPersonal usar_CategoriaPersonal)
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
                // string controler = "CategoriaPersonal", vista = "categoriaPersonal";

                //si no ha validado volver y mostrar mensajes de validacion
                if (!ModelState.IsValid)
                {
                    respuesta.respuesta = false;
                    respuesta.redirect = Url.Content("~/" + controler + "/" + vista + "");
                    respuesta.error = "Llenar Campos requeridos ";
                    return Json(respuesta);
                }
                using (var dbtrans = db.Database.BeginTransaction())
                {
                    try
                    {
                        CategoriaPersonal categoriapersonal = new CategoriaPersonal();
                        //asignar contenido a otro objeto
                        CopyClass.CopyObject(usar_CategoriaPersonal, ref categoriapersonal);

                        vw_UsuarioDoctor usuarioDoctor = null;
                        //add new 
                 
                            if (string.IsNullOrEmpty(categoriapersonal.CPersSecuencia.ToString()) || categoriapersonal.CPersSecuencia == 0)
                        {
                           
                            usuarioDoctor = db.vw_UsuarioDoctor.Where(d => d.UsuaSecuencia == usu.usuario.UsuaSecuencia).FirstOrDefault();

                            //si existe el role del doctor no se introduce
                            var existeCategoriaPersonal = (from cpersonal in db.CategoriaPersonals
                                                           where cpersonal.CPersNombre == usar_CategoriaPersonal.CPersNombre
                                                               && cpersonal.DoctSecuencia_fk == usu.doctSecuencia
                                                               && cpersonal.EstaDesabilitado==false
                                                           select cpersonal).SingleOrDefault();
                            if (existeCategoriaPersonal != null)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "Ya existe esta Categoria Personal";
                                respuesta.redirect = Url.Action(vista, controler);
                                return Json(respuesta);
                            }

                         
                            categoriapersonal.DoctSecuencia_fk = usuarioDoctor.DoctSecuencia;
                            categoriapersonal.UsuaSecuencia = usu.usuario.UsuaSecuencia;
                            categoriapersonal.CPersFechaCreacion = Lib.GetLocalDateTime();
                            categoriapersonal.CPersFechaModificacion = Lib.GetLocalDateTime();

                            //buscando la proxima secuencia, el doctor en todas sus consultorios tiene los mismos roles 
                            proximoItem = ((from CPersonalMax in db.CategoriaPersonals
                                            where CPersonalMax.DoctSecuencia_fk == usu.doctSecuencia
                                            select (int?)CPersonalMax.CPersSecuencia).Max() ?? 0) + 1;
                            categoriapersonal.CPersSecuencia = proximoItem;
                            
                            db.CategoriaPersonals.Add(categoriapersonal);
                            //Auditoria Usuario Loguiado

                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Nuevo.ToString(), null);
                        }
                        //Editando 
                        else
                        {

                            var CategoriaPersonalEditando = db.CategoriaPersonals.Where(ro => ro.DoctSecuencia_fk == usu.doctSecuencia
                                            && ro.CPersSecuencia == usar_CategoriaPersonal.CPersSecuencia
                                            && ro.EstaDesabilitado== false
                                            ).SingleOrDefault();
                            CategoriaPersonalEditando.CPersNombre = categoriapersonal.CPersNombre;
                            //Si NO esta loguieado lo redireccionara al loguin
                            if (HttpContext.Session["user"] == null)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "Usted debe de loguiarse.";
                                respuesta.redirect = Url.Action("Index", "PaginaPresentacion");
                                return Json(respuesta);
                            }
                          
                            usuarioDoctor = db.vw_UsuarioDoctor.Where(d => d.UsuaSecuencia == usu.usuario.UsuaSecuencia).FirstOrDefault();
                            CategoriaPersonalEditando.UsuaSecuenciaModificacion = usuarioDoctor.UsuaSecuencia;
                            CategoriaPersonalEditando.CPersFechaModificacion = Lib.GetLocalDateTime();
                            //Auditoria Usuario Loguiado
                            Lib lib = new Lib();
                            lib.IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Editar.ToString(), null);
                        }

                        db.SaveChanges();
                        dbtrans.Commit();

                        respuesta.respuesta = true;
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
                        new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Error.ToString(), ex.GetBaseException().Message.ToString());
                        return Json(respuesta, JsonRequestBehavior.AllowGet);
                        
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
        public JsonResult Borrar(Usar_CategoriaPersonal usar_CategoriaPersonal)
        {
            //string controler = "Roles", vista = "Roles";
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
                if (usar_CategoriaPersonal.CPersSecuencia < 1 || usar_CategoriaPersonal.CPersSecuencia == null)
                {
                    respuesta.respuesta = false;
                    respuesta.error = "Favor seleccionar una Categoria Personal en el listado";
                    respuesta.redirect = Url.Action(vista, controler);
                    return Json(respuesta);

                }
                using (var dbtrans = db.Database.BeginTransaction())
                {
                    try
                    {
                        //borro registro
                        if (usar_CategoriaPersonal != null)
                        {

                            CategoriaPersonal CategoriaPersonal = new CategoriaPersonal();
                            //asignar contenido a otro objeto
                            CopyClass.CopyObject(usar_CategoriaPersonal, ref CategoriaPersonal);


                            CategoriaPersonal borrar = (from cPersonal in db.CategoriaPersonals
                                           where cPersonal.CPersNombre == usar_CategoriaPersonal.CPersNombre
                                                              && cPersonal.DoctSecuencia_fk == usu.doctSecuencia
                                                             
                                           select cPersonal).SingleOrDefault();

                            if (borrar != null)
                            {
                                //borrar.EstaDesabilitado = true;
                                db.CategoriaPersonals.Remove(borrar);
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
                        respuesta.error = "Posiblemente usted tiene otro registro que depende de éste o No Existe este Formulario "+ex.Message.ToString();
                        //respuesta.redirect = Url.Action("Ini_TipoFormulario", "TipoFormulario");
                        respuesta.redirect = Url.Action(vista, controler);
                        new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Error.ToString(), ex.GetBaseException().Message);
                        return Json(respuesta);
                    }
                }



            }
        }//End Borrar Metodo

        public ActionResult CategoriaPersonallista()
        {
            //Si NO esta loguieado lo redireccionara al loguin
            if (HttpContext.Session["user"] == null)
            {
                return RedirectToAction("Index", "PaginaPresentacion");
            }
         
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
                    //ViewBag.Redirect = Url.Action(vista, controler); 
                    TempData["NoTienePermisoParaPagina"] = "Usted no tiene permiso para este formulario!";
                    //ViewBag.FormularioNOAsignadoAUsuario = true;
                    //return RedirectToAction("Index", "DashBoard"); 
                    return RedirectToAction(vista, controler);
                }
                ViewBag.ListaFormulario = formulariosPlanRoleUser;



                //es obligatorio por que  la llamada necesita un proxy
                db.Configuration.ProxyCreationEnabled = false;

                //Si NO esta loguieado lo redireccionara al loguin
                if (HttpContext.Session["user"] == null)
                {
                    return RedirectToAction("Index", "PaginaPresentacion");
                }

                //para que el div de accion solo aparezca si son 
                //no son home y como no quiero que aparezca el div lo en esta vista de lista
                //lo pongo true como para indicar que esta pagina es home, para que no me aparezca el 
                //div de accion
                ViewBag.isHome = true;

                List<CategoriaPersonal> CategoriaPersonal = (from c in db.CategoriaPersonals
                                                             where c.EstaDesabilitado == false &&
                                                             c.DoctSecuencia_fk == usu.doctSecuencia
                                                             select c).ToList();
                var listCategoriaPersonal = new List<Usar_CategoriaPersonal>();
                //crear objeto tipo formulario y asignar a la lista de usartipoformulario
                foreach (var item in CategoriaPersonal)
                {
                    Usar_CategoriaPersonal catPersonal = new Usar_CategoriaPersonal();
                    //asignar contenido a otro objeto
                    CopyClass.CopyObject(item, ref catPersonal);
                    listCategoriaPersonal.Add(catPersonal);
                }
                ViewBag.datasource = listCategoriaPersonal;
                return View();
            }
        }


    }
}
