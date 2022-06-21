using DoctorMedicalWeb.App_Data;
using DoctorMedicalWeb.Libreria;
using DoctorMedicalWeb.Models;
using DoctorMedicalWeb.ModelsComplementarios;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;


namespace DoctorMedicalWeb.Controllers
{
      //[OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
  
    public class DoctorController : Controller
    {
       

        DoctorMedicalWebEntities db = new DoctorMedicalWebEntities();
        DbContextTransaction dbtrans = null;
        //guardar auditoria
        Libreria.Lib lib = new Libreria.Lib();
        //
        // GET: /Doctor/
        //App_Data.vw_UsuarioConsultorios u;
      
         public ActionResult Index()
        {

            //Si NO esta loguieado lo redireccionara al loguin
            if (HttpContext.Session["user"] == null)
            {
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

            IEnumerable<Especialidade> especialida = db.Especialidades.ToList();

            ViewBag.Especialidad = new SelectList(especialida, "EspeSecuencia", "EspeNombre");

            IEnumerable<TipoDocumento> tipoDocumento = db.TipoDocumentoes.ToList();
            ViewBag.tipoDocumentos = new SelectList(tipoDocumento, "TDSecuencia", "TDDocumento");

            //ViewBag.NombreCompleto = u.UsuaNombre + " " + u.UsuaApellido;

            //busco el doctor loguiado por medio del usuario en cuestion
            //un usuario tiene un solo doctor
            var doctorActual = db.Doctors.Where(d => d.DoctSecuencia == usu.doctSecuencia).FirstOrDefault();

            //para que el div de accion solo aparezca si son 
            //vistas difrentes a home
            ViewBag.isHome = false;

            Usar_Doctor usar_Doctor = new Usar_Doctor();
            //asignar contenido a otro objeto
            CopyClass.CopyObject(doctorActual, ref  usar_Doctor);
            if (usar_Doctor.DoctFotoPath == null)
            usar_Doctor.DoctFotoPath = "";
            usar_Doctor.UsuaClave = usu.usuario.UsuaClave;
            usar_Doctor.UsuaClaveConfirmacion = usu.usuario.UsuaClave;
            usar_Doctor.UsuaEmail = db.Usuarios.Where(u => u.UsuaSecuencia == usu.usuario.UsuaSecuencia).Select(t => t.UsuaEmail).SingleOrDefault();




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
            var EstaEsteFormulario = formulariosPlanRoleUser.Where(f => f.FormDescripcion == "pag_Doctor").Any();
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



            //Usar_Role usarrole = new Usar_Role();
            ////si  esta lleno es por que esta editando
            //if (Session["Usar_Role"] != null)
            //{
            //    usarrole = (Usar_Role)Session["Usar_Role"];
            //    //limpiar sesion
            //    Session["Usar_Role"] = null;
            //    return View(usarrole);
            //}

            return View(usar_Doctor);
        }

        [HttpPost]
        public JsonResult SaveDoctor(Usar_Doctor usar_doctor, HttpPostedFileBase file)
        {
            var fileName = "";
            var path = "";
            var respuesta = new ResponseModel();
            if (HttpContext.Session["user"] == null)
            {
                respuesta.respuesta = false;
                respuesta.redirect = Url.Content("~/PaginaPresentacion/Index");
                
                
                return Json(respuesta);
                
            }

            UsuarioLoguiado usu = ((UsuarioLoguiado)HttpContext.Session["user"]);


            // usar_doctor=null;
            //if (usar_doctor==null)
            //{

            //    respuesta.respuesta = false;

            //    respuesta.error = "Llenar Campos requeridos ";

            //    return Json(respuesta);
            //}







            //si no ha validado volver y mostrar mensajes de validacion

            if (!ModelState.IsValid)
            {

                respuesta.respuesta = false;
                respuesta.redirect = Url.Content("~/Doctor/Index");
                respuesta.error = "Llenar Campos requeridos ";
                var a = ModelState.IsValid;
                return Json(respuesta);
            }

            try
            {
                //busco el doctor a editar
                Doctor doctor = db.Doctors.Where(d => d.DoctSecuencia == usu.doctSecuencia).SingleOrDefault();

                //asignar contenido a otro objeto
                //   CopyClass.CopyObject(usar_doctor, ref doctor );
                //solo se puden modificar estos campos al doctor
                doctor.DoctFechaModificacion = Lib.GetLocalDateTime();
                doctor.UsuaSecuenciaModificacion = usu.usuario.UsuaSecuencia;
                doctor.DoctNombre = usar_doctor.DoctNombre;
                doctor.DoctApellido = usar_doctor.DoctApellido;
                doctor.DoctCUPRE = usar_doctor.DoctCUPRE;
                doctor.DoctGenero = usar_doctor.DoctGenero;
                doctor.DoctFechaNacimiento = usar_doctor.DoctFechaNacimiento;
                doctor.DoctDireccion = usar_doctor.DoctDireccion;
                doctor.DoctCelular = usar_doctor.DoctCelular;
                doctor.DoctTelefono = usar_doctor.DoctTelefono;
                doctor.DoctDocumento = usar_doctor.DoctDocumento;
                doctor.TDSecuencia = usar_doctor.TDSecuencia;

                
                using (dbtrans = db.Database.BeginTransaction())
                {

                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {
                        //get extension
                        string ext = Path.GetExtension(file.FileName);
                        // extract only the fielname
                        // Path.GetFileName(file.FileName);
                        fileName = "Doctor";
                        //fileName = Path.GetFileNameWithoutExtension(fileName).ToString()+ usu.usuario.UsuaSecuencia.ToString();
                        fileName = fileName + usu.usuario.UsuaSecuencia.ToString();
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
                        //string imagenPath =Server.MapPath("~/Content/ImagenesUploads/") + fileName;
                    

                        doctor.DoctFotoNombre = fileName;
                        doctor.DoctFotoPath = imagenPath;
                    }

                    Usuario usuarPas = db.Usuarios.Where(u => u.UsuaSecuencia == usu.usuario.UsuaSecuencia).SingleOrDefault();
                    if (!string.IsNullOrEmpty(usar_doctor.UsuaClave))
                    {
                        if (usar_doctor.UsuaClave.Length < 6)
                        {
                            respuesta.respuesta = false;                         
                            respuesta.error = "La Contraseña tiene que tener mas de 5 digitos y mayusculas";
                            var a = ModelState.IsValid;
                            return Json(respuesta);
                        }
                        if (!usar_doctor.UsuaClave.Any(char.IsUpper) ||
                            !usar_doctor.UsuaClave.Any(char.IsLower) ||
                            !usar_doctor.UsuaClave.Any(char.IsDigit))
                        {
                            respuesta.respuesta = false;
                            respuesta.error = "La Contraseña tiene que tener mas de 5 digitos, mayusculas y números";
                            var a = ModelState.IsValid;
                            return Json(respuesta);
                        }
                        usuarPas.UsuaClave = usar_doctor.UsuaClave;
                    }


                  
                    db.SaveChanges();
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
                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), Paginas.pag_Doctor.ToString(), Accion.Error.ToString(), ex.GetBaseException().Message);
                            return Json(respuesta, JsonRequestBehavior.AllowGet);

                        }

                      
                        //guardo la imagen el el servidor
                        //la pongo aqui , para que si hay error  al guardar que no se logre guardar la foto tampoco
                        file.SaveAs(path);
                    }

                    respuesta.respuesta = true;
                    // respuesta.redirect = "/Doctor/Index"; 
                    respuesta.redirect = Url.Content("~/Doctor/Index");

                    new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), Paginas.pag_Doctor.ToString(), Accion.Editar.ToString(), null);

                }
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
                respuesta.redirect = Url.Content("~/Doctor/Index");
                respuesta.error = "Ocurrio Un inconveniente, Favor volver a tratar. " + ex.Message + "---" + ex.GetBaseException().ToString();
                new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), Paginas.pag_Doctor.ToString(), Accion.Error.ToString(), ex.GetBaseException().Message);
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
}
