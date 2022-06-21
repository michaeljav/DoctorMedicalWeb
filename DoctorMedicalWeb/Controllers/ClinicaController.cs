using DoctorMedicalWeb.App_Data;
using DoctorMedicalWeb.Libreria;
using DoctorMedicalWeb.Models;
using DoctorMedicalWeb.ModelsComplementarios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoctorMedicalWeb.Controllers
{
    public class ClinicaController : Controller
    {
        //
        // GET: /Clinica/
        string controler = "Clinica", vista = "Ini_Clinica", PaginaAutorizada = Paginas.pag_Clinica.ToString();
        public ActionResult Ini_Clinica()
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

                Usar_Clinica usarClinica = new Usar_Clinica();
                //si  esta lleno es por que esta editando
                if (Session["Usar_Clinica"] != null)
                {
                    usarClinica = (Usar_Clinica)Session["Usar_Clinica"];

                    //limpiar sesion
                    Session["Usar_Clinica"] = null;
                    return View(usarClinica);
                }

                return View(usarClinica);
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
                    var objListaUltimosCinco = (from tf in db.Clinicas

                                                orderby tf.clinSecuencia descending
                                                select tf).Take(5).ToList();

                    var listClinics = new List<Usar_Clinica>();
                    //crear objeto role y asignar a la lista de usaroles
                    foreach (var item in objListaUltimosCinco)
                    {
                        var ObjNuevo = new Usar_Clinica();
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
        public JsonResult Borrar(Usar_Clinica usar_clinic)
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
                if (string.IsNullOrEmpty(usar_clinic.clinSecuencia.ToString()) || usar_clinic.clinSecuencia < 1)
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
                        if (usar_clinic != null)
                        {

                            //Paciente objetoNuevo = new Paciente();
                            ////asignar contenido a otro objeto
                            //CopyClass.CopyObject(usar_pacient, ref objetoNuevo);


                            var borrar = (from objquery in db.Clinicas
                                          where objquery.clinSecuencia == usar_clinic.clinSecuencia
                                          && objquery.clinRazonSocial == usar_clinic.clinRazonSocial
                                          select objquery).SingleOrDefault();

                            if (borrar != null)
                            {
                                db.Clinicas.Remove(borrar);
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
        public JsonResult Editar(Usar_Clinica usar_clinic)
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
                if (usar_clinic != null)
                {

                    var ObjetoEditar = (from ObjQuery in db.Clinicas
                                        where ObjQuery.clinSecuencia == usar_clinic.clinSecuencia
                                        select ObjQuery).SingleOrDefault();

                    if (ObjetoEditar != null)
                    {
                        var objNuevo = new Usar_Clinica();
                        //asignar contenido a otro objeto
                        CopyClass.CopyObject(ObjetoEditar, ref objNuevo);
                        //llenno la sesion con el role a editar
                        //para abrirlo en  el metodo principal del controler
                        Session["Usar_Clinica"] = objNuevo;

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
          [HttpPost]
        public JsonResult Save(Usar_Clinica usar_clinic, HttpPostedFileBase file)
        {
            var respuesta = new ResponseModel();
            var fileName = "";
            var path = "";
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
                        if (string.IsNullOrEmpty(usar_clinic.clinSecuencia.ToString()) || usar_clinic.clinSecuencia == 0)
                        {
                            var objNuevo = new Clinica();
                            //asignar contenido a otro objeto
                            CopyClass.CopyObject(usar_clinic, ref objNuevo);



                            //si existe el paciente del doctor no se introduce
                            var existe = (from objbuscar in db.Clinicas
                                          where objbuscar.clinRazonSocial == usar_clinic.clinRazonSocial
                                          select objbuscar).SingleOrDefault();
                            if (existe != null)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "Ya existe esta clinica";
                                respuesta.redirect = Url.Action(vista, controler);
                                return Json(respuesta);
                            }




                            objNuevo.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                            objNuevo.clinEstaBorrado = false;

                            objNuevo.usuaSecuenciaCreacion = usu.usuario.UsuaSecuencia;
                            objNuevo.clinFechaCreacion = Lib.GetLocalDateTime();

                            //buscando la proxima secuencia, el doctor en todas sus consultorios tiene los mismos roles 
                            proximoItem = ((from objBusc in db.Clinicas
                                            select (int?)objBusc.clinSecuencia).Max() ?? 0) + 1;
                            objNuevo.clinSecuencia = proximoItem;
                            
                            // Verify that the user selected a file
                            if (file != null && file.ContentLength > 0)
                            {
                                //get extension
                                string ext = Path.GetExtension(file.FileName);
                                // extract only the fielname
                                // Path.GetFileName(file.FileName);
                                fileName = "Clinica";
                                //fileName = Path.GetFileNameWithoutExtension(fileName).ToString()+ usu.usuario.UsuaSecuencia.ToString();
                                fileName = fileName + proximoItem.ToString();
                                fileName = fileName + ext;
                                // store the file inside ~/App_Data/uploads folder
                                path = Path.Combine(Server.MapPath("~/Content/ImagenesUploads"), fileName);


                                //extraigo el directorio
                                //bool folderExists = Directory.Exists(Path.GetDirectoryName(path));
                                //sino existe  el directorio  lo creo
                                if (Directory.Exists(Path.GetDirectoryName(path)) == false)
                                {

                                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                                }


                                string imagenPath = "~/Content/ImagenesUploads/" + fileName;


                                objNuevo.clinFotoName = fileName;
                                objNuevo.clinFotoPath = imagenPath;
                            }

                            db.Clinicas.Add(objNuevo);
                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Nuevo.ToString(), null);
                        }
                        //Editando 
                        else
                        {

                            //busco el objeto que se editara
                            //lo busco por la secuencia para que pueda editar ese mismo sin tener que borrarlo
                            //si lo busco por el nombre no lo  editar ya que no lo encontrara.
                            var objetoEditar = db.Clinicas.Where(ro => ro.clinSecuencia == usar_clinic.clinSecuencia).SingleOrDefault();

                            if (objetoEditar == null)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "No existe esta  clinica";
                                respuesta.redirect = Url.Action(vista, controler);
                                return Json(respuesta);
                            }

                            //asigno al objeto que se editara los nuevos datos introducidos a editar
                            CopyClass.CopyObject(usar_clinic, ref objetoEditar);

                            objetoEditar.clinEstaBorrado = false;
                            objetoEditar.usuaSecuenciaModificacion = usu.usuario.UsuaSecuencia;
                            objetoEditar.clinFechaModificacion = Lib.GetLocalDateTime();

                            // Verify that the user selected a file
                            if (file != null && file.ContentLength > 0)
                            {
                                //get extension
                                string ext = Path.GetExtension(file.FileName);
                                // extract only the fielname
                                // Path.GetFileName(file.FileName);
                                fileName = "Clinica";
                                //fileName = Path.GetFileNameWithoutExtension(fileName).ToString()+ usu.usuario.UsuaSecuencia.ToString();
                                fileName = fileName + objetoEditar.clinSecuencia.ToString();
                                fileName = fileName + ext;
                                // store the file inside ~/App_Data/uploads folder
                                path = Path.Combine(Server.MapPath("~/Content/ImagenesUploads"), fileName);


                                //extraigo el directorio
                                //bool folderExists = Directory.Exists(Path.GetDirectoryName(path));
                                //sino existe  el directorio  lo creo
                                if (Directory.Exists(Path.GetDirectoryName(path)) == false)
                                {

                                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                                }


                                string imagenPath = "~/Content/ImagenesUploads/" + fileName;


                                objetoEditar.clinFotoName = fileName;
                                objetoEditar.clinFotoPath = imagenPath;
                            }


                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Editar.ToString(), null);
                        }

                        int returnsave = db.SaveChanges();
                        dbtrans.Commit();


                        // Verify that the user selected a file
                        if (file != null && file.ContentLength > 0)
                        {
                            //si existe el archivo lo borro para crearlo nuevamente
                            try
                            {
                                if (System.IO.File.Exists(path))
                                {

                                    System.IO.File.Delete(path);
                                }
                            }
                            catch (Exception ex)
                            {
                                //throw ex;
                                respuesta.respuesta = false;
                                respuesta.redirect = Url.Content("~/Doctor/Index");
                                respuesta.error = "Ocurrio Un inconveniente, Favor volver a tratar. " + ex.Message + "---" + ex.GetBaseException().ToString();
                                new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Error.ToString(), ex.GetBaseException().Message);
                                return Json(respuesta, JsonRequestBehavior.AllowGet);

                            }

                            //guardo la imagen el el servidor
                            //la pongo aqui , para que si hay error  al guardar que no se logre guardar la foto tampoco
                            file.SaveAs(path);
                        }


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

        public ActionResult Clinicalista()
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
                var objetoEnlistar = (from tf in db.Clinicas

                                      orderby tf.clinRazonSocial
                                      select tf).ToList();

                var listObjetoNuevo = new List<Usar_Clinica>();
                //crear objeto tipo formulario y asignar a la lista de usartipoformulario
                foreach (var item in objetoEnlistar)
                {
                    var objNuevoParaLista = new Usar_Clinica();
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
