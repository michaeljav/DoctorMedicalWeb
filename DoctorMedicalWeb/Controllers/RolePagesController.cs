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
    public class RolePagesController : Controller
    {
        string controler = "RolePages", vista = "Ini_RolePages", PaginaAutorizada = Paginas.pag_RolePages.ToString();

        //
        // GET: /RolePages/

        public ActionResult Ini_RolePages()
        {



            using (DoctorMedicalWebEntities db = new DoctorMedicalWebEntities())
            {
                bool proxyCreation = db.Configuration.ProxyCreationEnabled;

                try
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    //Si NO esta loguieado lo redireccionara al loguin
                    if (HttpContext.Session["user"] == null)
                    {
                        TempData["NoTienePermisoParaPagina"] = "Usted no esta loguiado!";
                        return RedirectToAction("Index", "PaginaPresentacion");
                    }


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

                    List<Role> rolesList = new List<Role>();
                    //si soy admin rol 1 que me muestre todos los roles
                    if (usu.usuario.RoleSecuencia_fk == 1)
                    {
                        rolesList = ((from Rols in db.Roles
                                      select Rols).ToList());
                    }
                    else
                    {
                        rolesList = ((from Rols in db.Roles
                                      where Rols.RoleSecuencia != 1//Diferente a admin
                              && Rols.RoleSecuencia != 2//diferente a doctor 
                                      select Rols).ToList());
                    }
                    ViewBag.RolesLista = new SelectList(rolesList, "RoleSecuencia", "RoleDescripcion");

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
                        TempData["NoTienePermisoParaPagina"] = "Usted no tiene permiso para este formulario!";
                        return RedirectToAction("Index", "DashBoard");
                    }
                    ViewBag.ListaFormulario = formulariosPlanRoleUser;

                    //para que el div de accion solo aparezca si son 
                    //vistas difrentes a home
                    ViewBag.isHome = true;

                    //Formularios asignados a este rol
                    //var a = BuscarFormulariosAsignadosARol();

                    Usar_RolFormulario usarCompleRoleFomularios = new Usar_RolFormulario();

                    //si  esta lleno es por que esta editando
                    /*      if (Session["Usar_Personal"] != null)
                     {
                         usarCompleRoleFomularios = (UsarCompleRoleFomularios)Session["Usar_Personal"];
                         //para llenar el dropdownlist y select el checklist
                         ViewBag.FillDropDownListCheckJson = usarCompleRoleFomularios.ConsSecuencia;
                         ViewData["FillDropDownListCheckJson"] = usarCompleRoleFomularios.ConsSecuencia;
                         //limpiar sesion
                         Session["Usar_Personal"] = null;
                         return View(usarCompleRoleFomularios);
                     }*/
                    try
                    {
                        //if (usarCompleRoleFomularios.RoleSecuencia_fk == 0)
                        //    usarCompleRoleFomularios = null;
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    return View(usarCompleRoleFomularios);


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
        }

        //buscar los formularios asignados a  un rol
        public List<Usar_vw_planFormularioModificado> BuscarFormulariosAsignadosARol(Usar_RolFormulario rol)
        {
            List<Usar_vw_planFormularioModificado> Objvw_PlanFormularioLIst = new List<Usar_vw_planFormularioModificado>();
            using (var db = new DoctorMedicalWebEntities())
            {
                bool proxyCreation = db.Configuration.ProxyCreationEnabled;

                try
                {
                    db.Configuration.ProxyCreationEnabled = false;


                    //usuario loguieado
                    UsuarioLoguiado consultorioUsuarioSeleccionado = (UsuarioLoguiado)HttpContext.Session["user"];

                    //busco listado de formularios del plan del usuario loguiado
                    List<vw_planFormulario> PlanFormulariosList = ((from Formul in db.vw_planFormulario
                                                                    where Formul.PlanSecuencia == consultorioUsuarioSeleccionado.usuario.PlanSecuencia_fk
                                                                    select Formul).ToList());

                    //asigno el listado a el objeto listado
                    foreach (vw_planFormulario pForm in PlanFormulariosList)
                    {
                        Usar_vw_planFormularioModificado pf = new Usar_vw_planFormularioModificado();
                        //asignar contenido a otro objeto
                        CopyClass.CopyObject(pForm, ref pf);
                        Objvw_PlanFormularioLIst.Add(pf);

                    }

                    //Busco los formularios que les pertenencen a este rol
                    List<RolFormulario> rolFormulario = ((from rFormul in db.RolFormularios
                                                          where
                                                          rFormul.DoctSecuencia_fk == consultorioUsuarioSeleccionado.doctSecuencia
                                                          &&
                                                          rFormul.RoleSecuencia_fk == rol.RoleSecuencia_fk
                                                          select rFormul).ToList());

                    //recorro cada uno de los roles y en la lista Objvw_PlanFormularioLIst para poner lo en true 
                    foreach (RolFormulario item in rolFormulario)
                    {

                        //busco este formulario  item
                        Usar_vw_planFormularioModificado pcr = Objvw_PlanFormularioLIst.Where(p => p.FormSecuencia_fk == item.FormSecuencia_fk).SingleOrDefault();
                        //si existe  en el listado de Objvw_PlanFormularioLIst entonces lo pongo true
                        //por que quiere decir que en la tabla RolFormularios lo habian asigando el formulario a este rol
                        if (pcr != null)
                            Objvw_PlanFormularioLIst.Where(p => p.FormSecuencia_fk == item.FormSecuencia_fk).SingleOrDefault().seleccionado = true;

                    }

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

            return Objvw_PlanFormularioLIst;
        }

        [HttpPost]
        public JsonResult BuscarFormulariosDeRol(Usar_RolFormulario usarCompleRoleFormularios)
        {
            ResponseModel respuesta = new ResponseModel();
            //if (usarCompleRoleFormularios.RoleSecuencia_fk < 1)
            //{
            //    respuesta.respuesta = false;
            //    respuesta.error = "Debe seleccionar un rol";
            //    return Json(respuesta);
            //}

            if (!ModelState.IsValid)
            {
                respuesta.respuesta = false;
                ////respuesta.redirect = Url.Content("~/Roles/Roles");
                //respuesta.redirect = Url.Content("~/" + controler + "/" + vista + "");
                //respuesta.error = "Llenar Campos requeridos ";
                return Json(respuesta);
            }


            using (DoctorMedicalWebEntities db = new DoctorMedicalWebEntities())
            {
                bool proxyCreation = db.Configuration.ProxyCreationEnabled;

                try
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    //Si NO esta loguieado lo redireccionara al loguin
                    if (HttpContext.Session["user"] == null)
                    {
                        TempData["NoTienePermisoParaPagina"] = "Usted no esta loguiado!";
                        respuesta.respuesta = false;
                        respuesta.error = "Se ha cerrado la sesion.";
                        respuesta.redirect = Url.Action("Index", "PaginaPresentacion");
                        return Json(respuesta);
                    }

                    ViewBag.planFormulariosLIsta = BuscarFormulariosAsignadosARol(usarCompleRoleFormularios);
                    respuesta.someCollection = ViewBag.planFormulariosLIsta;
                    respuesta.respuesta = true;

                    return Json(respuesta);


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
        }

        [HttpPost]
        public JsonResult Save(Usar_RolFormulario FieldFormsvie, List<Usar_vw_planFormularioModificado> grid)
        {
            ResponseModel respuesta = new ResponseModel();
            //List<string> fielsError = new List<string>();
            ////Si NO esta loguieado lo redireccionara al loguin
            if (HttpContext.Session["user"] == null)
            {
                respuesta.respuesta = false;
                respuesta.error = "Usted debe de loguiarse.";
                respuesta.redirect = Url.Action("Index", "PaginaPresentacion");
                return Json(respuesta);
            }
            //si no ha validado volver y mostrar mensajes de validacion
            if (!ModelState.IsValid)
            {
                respuesta.respuesta = false;
                respuesta.error = "Seleccionar Campo requerido ";
                return Json(respuesta);
            }
            UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];

            //validar que  estos formularios el doctor no los logre quitar
            // 22 seguridadmenu
            //38 Asignar Pagina a Pefil
            //26 perfil
            //sino existe eston sformularios le digo que debe de seleccionarlo  role 1 = doctor
            if (FieldFormsvie.RoleSecuencia_fk == 1)
            {
                //si falta uno de estos formularios  No se procedera a guardar.
                if (grid == null)
                {
                    respuesta.respuesta = false;
                    respuesta.error = "Seleccionar estos formulario:  22 seguridadmenu, 38 Asignar Pagina a Pefil, 26 perfil ";
                    return Json(respuesta);

                }
                bool Existe22 = false, Existe38 = false, Existe26 = false;
                foreach (var item in grid)
                {
                    if (item.FormSecuencia_fk == 22)
                    {
                        Existe22 = true;
                    }
                    if (item.FormSecuencia_fk == 38)
                    {
                        Existe38 = true;
                    }
                    if (item.FormSecuencia_fk == 26)
                    {
                        Existe26 = true;
                    }
                }
                //si falta uno de estos formularios  No se procedera a guardar.
                if (Existe22 == false || Existe26 == false || Existe38 == false)
                {
                    respuesta.respuesta = false;
                    respuesta.error = "Seleccionar estos formulario:  22 seguridadmenu, 38 Asignar Pagina a Pefil, 26 perfil ";
                    return Json(respuesta);

                }

            }


            using (var db = new DoctorMedicalWebEntities())
            {
                //int proximoItem = 0;

                using (var dbtrans = db.Database.BeginTransaction())
                {
                    try
                    {
                        RolFormulario rolformulario = new RolFormulario();

                        //Busco busco el rol 
                        var RolAInsertar = db.Roles.Where(ro =>
                                         ro.RoleSecuencia == FieldFormsvie.RoleSecuencia_fk
                                        ).SingleOrDefault();

                        if (RolAInsertar == null)
                        {
                            respuesta.respuesta = false;
                            respuesta.error = "No existe este rol ";
                            return Json(respuesta);
                        }
                        if (grid == null)
                        {
                            grid = new List<Usar_vw_planFormularioModificado>();
                        }
                        //BORRAR TODOS  FORMULARIOS ASIGNADOS A este rol
                        List<RolFormulario> rolFormularioBorrar = new List<RolFormulario>();
                        rolFormularioBorrar = (from rform in db.RolFormularios
                                               where rform.DoctSecuencia_fk == usu.doctSecuencia
                                                 && rform.RoleSecuencia_fk == FieldFormsvie.RoleSecuencia_fk
                                               select rform).ToList();


                        foreach (var item in rolFormularioBorrar)
                        {
                            //borrar actuales formulraios asignados a este rol
                            db.RolFormularios.Remove(item);
                        }

                        //ingresar nuevos Formularios perteneciente a este rol

                        int sec = 0;
                        foreach (Usar_vw_planFormularioModificado item in grid)
                        {
                            ++sec;
                            RolFormulario rolFormular = new RolFormulario();
                            rolFormular.DoctSecuencia_fk = usu.doctSecuencia;
                            rolFormular.PaisSecuencia_fk = usu.usuario.PaisSecuencia;
                            rolFormular.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                            rolFormular.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                            rolFormular.RFSecuencia = sec;
                            rolFormular.RoleSecuencia_fk = (int)FieldFormsvie.RoleSecuencia_fk;
                            rolFormular.FormSecuencia_fk = item.FormSecuencia_fk;
                            rolFormular.PlanSecuencia_fk = usu.usuario.PlanSecuencia_fk;
                            db.RolFormularios.Add(rolFormular);
                        }






                        db.SaveChanges();
                        dbtrans.Commit();

                        respuesta.respuesta = true;
                        //Reenvio a la vista principal
                        respuesta.redirect = Url.Content("~/" + controler + "/" + vista + "");

                        //Enlistar los 5 ultimos Registros                
                        respuesta.someCollection = BuscarFormulariosAsignadosARol(FieldFormsvie);
                        new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Editar.ToString(), null);
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

            //respuesta.respuesta = true;

            //return Json(respuesta);

        }//end method save
          [HttpPost]
        public JsonResult TestList(perso model)
        {
            //perso p = new perso();
            //p = model;
            ResponseModel respuesta = new ResponseModel();
            respuesta.respuesta = false;
            return Json(respuesta);
        }
    }

    #region Funciono
    public class perso
    {
        public List<RoleModel> Roles { get; set; }
        public string Name { get; set; }
    }
    public class RoleModel
    {
        public string RoleName { get; set; }
        public string Description { get; set; }
    }
    #endregion
}
