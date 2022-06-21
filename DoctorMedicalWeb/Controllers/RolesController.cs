using DoctorMedicalWeb.App_Data;
using DoctorMedicalWeb.Libreria;
using DoctorMedicalWeb.Models;
using DoctorMedicalWeb.ModelsComplementarios;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoctorMedicalWeb.Controllers
{
    public class RolesController : Controller
    {
        //DoctorMedicalWebEntities db = new DoctorMedicalWebEntities();
        //DbContextTransaction dbtrans = null;

        //public string ValidarUsuario(){

        //}
        //
        // GET: /Roles/

        string controler = "Roles", vista = "Roles", PaginaAutorizada = Paginas.pag_Roles.ToString();
        public ActionResult Roles()
        {
            //Si NO esta loguieado lo redireccionara al loguin
            if (HttpContext.Session["user"] == null)
            {
                return RedirectToAction("Index", "PaginaPresentacion");
            }

            //lleno el combo de roles
            UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];
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

            Usar_Role usarrole = new Usar_Role();
            //si  esta lleno es por que esta editando
            if (Session["Usar_Role"] != null)
            {
                usarrole = (Usar_Role)Session["Usar_Role"];
                //limpiar sesion
                Session["Usar_Role"] = null;
                return View(usarrole);
            }

            return View(usarrole);
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
                    List<Role> RolesListaUltimosCinco = (from tf in db.Roles
                                                         
                                                         orderby tf.RoleSecuencia descending
                                                         select tf).Take(5).ToList();

                    var listUsar_role = new List<Usar_Role>();
                    //crear objeto role y asignar a la lista de usaroles
                    foreach (var item in RolesListaUltimosCinco)
                    {
                        Usar_Role formulario = new Usar_Role();
                        //asignar contenido a otro objeto
                        CopyClass.CopyObject(item, ref formulario);
                        listUsar_role.Add(formulario);
                    }
                    //Utilizado para insertar el listado al grid
                    ViewBag.ultimosCinco = listUsar_role;


                    //ienumerable lista
                    respuesta.someCollection = listUsar_role;


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
        public JsonResult Save(Usar_Role usar_Role)
        {
            var respuesta = new ResponseModel();
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
                UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];


                using (var dbtrans = db.Database.BeginTransaction())
                {

                    try
                    {

                        Role role = new Role();
                        //asignar contenido a otro objeto
                        CopyClass.CopyObject(usar_Role, ref role);

                        vw_UsuarioDoctor usuarioDoctor = null;
                        //add new 
                
                            if (string.IsNullOrEmpty(role.RoleSecuencia.ToString()) || role.RoleSecuencia == 0)
                        {
                            //Si NO esta loguieado lo redireccionara al loguin
                            if (HttpContext.Session["user"] == null)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "Usted debe de loguiarse.";
                                respuesta.redirect = Url.Action("Index", "PaginaPresentacion");
                                return Json(respuesta);
                            }


                            usuarioDoctor = db.vw_UsuarioDoctor.Where(d => d.UsuaSecuencia == usu.usuario.UsuaSecuencia).FirstOrDefault();

                            //si existe el role del doctor no se introduce
                            var existeRole = (from roll in db.Roles
                                              where roll.RoleDescripcion == usar_Role.RoleDescripcion
                                                 
                                              select roll).SingleOrDefault();
                            if (existeRole != null)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "Ya existe este role";
                                //respuesta.redirect = Url.Action("Roles", "Roles");
                                respuesta.redirect = Url.Action(vista, controler);
                                return Json(respuesta);
                            }


                          
                            role.UsuaSecuencia = usu.usuario.UsuaSecuencia;
                            role.RoleFechaCreacion = Lib.GetLocalDateTime();
                            role.RoleFechaModificacion = Lib.GetLocalDateTime();

                            //buscando la proxima secuencia, el doctor en todas sus consultorios tiene los mismos roles 
                            proximoItem = ((from rollMax in db.Roles
                                            
                                            select (int?)rollMax.RoleSecuencia).Max() ?? 0) + 1;
                            role.RoleSecuencia = proximoItem;
                            db.Roles.Add(role);
                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Nuevo.ToString(), null);
                        }
                        //Editando 
                        else
                        {

                            var roleEditando = db.Roles.Where(ro => 
                                             ro.RoleSecuencia == usar_Role.RoleSecuencia).SingleOrDefault();
                            roleEditando.RoleDescripcion = role.RoleDescripcion;
                            //Si NO esta loguieado lo redireccionara al loguin
                            if (HttpContext.Session["user"] == null)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "Usted debe de loguiarse.";
                                respuesta.redirect = Url.Action("Index", "PaginaPresentacion");
                                return Json(respuesta);
                            }


                            usuarioDoctor = db.vw_UsuarioDoctor.Where(d => d.UsuaSecuencia == usu.usuario.UsuaSecuencia).FirstOrDefault();
                            roleEditando.UsuaSecuenciaModificacion = usuarioDoctor.UsuaSecuencia;
                            roleEditando.RoleFechaModificacion = Lib.GetLocalDateTime();
                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Editar.ToString(), null);
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
                            //if(dbtrans.UnderlyingTransaction.Rollback)
                            //dbtrans.UnderlyingTransaction.Rollback();
                            //dbtrans.UnderlyingTransaction.Rollback();
                            //dbtrans.UnderlyingTransaction.Dispose();
                            dbtrans.Rollback();
                            dbtrans.Dispose();
                        }

                        respuesta.respuesta = false;
                        //                respuesta.redirect = Url.Content("~/Roles/Roles");
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
        public JsonResult Editar(Usar_Role usar_Role)
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

            //lleno el combo de roles
            UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];

            using (var db = new DoctorMedicalWebEntities())
            {
                if (usar_Role != null)
                {

                    Role roll = (from rol in db.Roles
                                 where rol.RoleSecuencia == usar_Role.RoleSecuencia
                                                   

                                 select rol).SingleOrDefault();

                    if (roll != null)
                    {
                        Usar_Role usarRole = new Usar_Role();
                        //asignar contenido a otro objeto
                        CopyClass.CopyObject(roll, ref usarRole);
                        //llenno la sesion con el role a editar
                        //para abrirlo en  el metodo principal del controler
                        Session["Usar_Role"] = usarRole;
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
          [HttpPost]
        public JsonResult Borrar(Usar_Role usar_Role)
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

            //lleno el combo de roles
            UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];

            //si el usuario loguiado es un rol doctor  u otro rol como secretaria  no puedo borrar el rol doctor
            if (usu.usuario.RoleSecuencia_fk >= 1)
            {
                //No puede borrar el rol doctor
                if (usar_Role.RoleSecuencia == 1)
                {
                    respuesta.respuesta = false;
                    respuesta.error = "No puede borrar el rol doctor ";
                    return Json(respuesta);

                }

            }
            


            using (var db = new DoctorMedicalWebEntities())
            {

                //si secuenca esta vacio devolver mensaje
                if (usar_Role.RoleSecuencia < 1 || usar_Role.RoleSecuencia == null)
                {
                    respuesta.respuesta = false;
                    respuesta.error = "Favor seleccionar un role en el listado";
                    respuesta.redirect = Url.Action(vista, controler);
                    return Json(respuesta);

                }
                using (var dbtrans = db.Database.BeginTransaction())
                {
                    try
                    {
                        //borro registro
                        if (usar_Role != null)
                        {

                            Role role = new Role();
                            //asignar contenido a otro objeto
                            CopyClass.CopyObject(usar_Role, ref role);


                            Role borrar = (from rol in db.Roles
                                           where rol.RoleDescripcion == usar_Role.RoleDescripcion
                                                             
                                           select rol).SingleOrDefault();

                            if (borrar != null)
                            {
                                //borrar.EstaDesabilitado = true;
                                db.Roles.Remove(borrar);
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

        public ActionResult Rolelista()
        {
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

                List<Role> Role = db.Roles.ToList();
                var listRole = new List<Usar_Role>();
                //crear objeto tipo formulario y asignar a la lista de usartipoformulario
                foreach (var item in Role)
                {
                    Usar_Role role = new Usar_Role();
                    //asignar contenido a otro objeto
                    CopyClass.CopyObject(item, ref role);
                    listRole.Add(role);
                }
                ViewBag.datasource = listRole;
                return View();
            }
        }
    }
}
