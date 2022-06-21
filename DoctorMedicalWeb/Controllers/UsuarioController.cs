using DoctorMedicalWeb.App_Data;
using DoctorMedicalWeb.Libreria;
using DoctorMedicalWeb.Models;
using DoctorMedicalWeb.ModelsComplementarios;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoctorMedicalWeb.Controllers
{
    public class UsuarioController : Controller
    {


        string controler = "Usuario", vista = "Ini_Usuario", PaginaAutorizada = Paginas.pag_UsuarioDoctor.ToString();

        //
        // GET: /Usuario/

        public ActionResult Ini_Usuario()
        {
            using (DoctorMedicalWebEntities db = new DoctorMedicalWebEntities())
            {
                //pARA EN LA VISTA PODER SABER A QUE  CONTROLADOR EL AJAX  LLAMARA
                ViewBag.ControlCsharp = controler;
                ViewBag.VistaCsharp = vista;

                db.Configuration.ProxyCreationEnabled = false;

                //Si NO esta loguieado lo redireccionara al loguin
                if (HttpContext.Session["user"] == null)
                {
                    TempData["NoTienePermisoParaPagina"] = "Usted no esta loguiado!";
                    return RedirectToAction("Index", "PaginaPresentacion");
                }
                //lleno el combo de Consultorio

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
                //Paginas.pag_Usuario.ToString() trael el valor de un enum  con las lista de paginas del sistema
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
                ViewBag.isHome = false;

                //Enlistar los 5 ultimos y lleno
                //el   ViewBag.ultimosCinco
                var a = UltimosCincoRegistros();

                Usar_Usuario usarUsuario = new Usar_Usuario();


                var obList = ((from Rols in db.Pais
                                  select Rols).ToList());
                ViewBag.paiBag = new SelectList(obList, "PaisSecuencia", "PaisNombre");

                var esobList = ((from Rols in db.Especialidades
                                  select Rols).ToList());
                ViewBag.especiBag = new SelectList(esobList, "EspeSecuencia", "EspeNombre");

                //lleno el combo de tipo Documento
                IEnumerable<TipoDocumento> tipoDocumento = db.TipoDocumentoes.ToList();
                ViewBag.tipoDocumentos = new SelectList(tipoDocumento, "TDSecuencia", "TDDocumento");

                //si  esta lleno es por que esta editando
                if (Session["Usar_Usuario"] != null)
                {
                    usarUsuario = (Usar_Usuario)Session["Usar_Usuario"];
                    //para llenar el dropdownlist y select el checklist

                    ViewBag.FillDropDownListCheckJson = usarUsuario.ConsSecuencia;

                    //ViewData["FillDropDownListCheckJson"] = usarUsuario.ConsSecuencia;
                    //limpiar sesion
                    Session["Usar_Usuario"] = null;
                    return View(usarUsuario);
                }

                return View(usarUsuario);
            }

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



                    //buscar los ultimos  cinco registros  de usuarios del doctor loguiado
                    List<Usuario> UsuarioListaUltimosCinco = (from tf in db.Usuarios
                                                              where tf.UsuaSecuencia == usu.usuario.UsuaSecuencia
                                                                       && tf.EstaDesabilitado == false
                                                              orderby tf.UsuaSecuencia descending
                                                              select tf).Take(5).ToList();

                    var listUsar_usuario = new List<Usar_Usuario>();
                    //crear objeto role y asignar a la lista de usaroles
                    foreach (var item in UsuarioListaUltimosCinco)
                    {
                        Usar_Usuario usuari = new Usar_Usuario();
                        //asignar contenido a otro objeto
                        CopyClass.CopyObject(item, ref usuari);
                        listUsar_usuario.Add(usuari);
                    }
                    //Utilizado para insertar el listado al grid
                    ViewBag.ultimosCinco = listUsar_usuario;


                    //ienumerable lista
                    respuesta.someCollection = listUsar_usuario;


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
        public JsonResult Save(Usar_Usuario usar_Usuario)
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

            //usuario loguieado
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

                        Usuario usuario = new Usuario();
                        //asignar contenido a otro objeto
                        CopyClass.CopyObject(usar_Usuario, ref usuario);

                        //vw_UsuarioDoctor usuarioDoctor = null;
                        //add new 

                        if (string.IsNullOrEmpty(usuario.UsuaSecuencia.ToString()) || usuario.UsuaSecuencia == 0)
                        {

                            ////verificar si existe este usuario
                            //usuarioDoctor = db.vw_UsuarioDoctor.Where(d => d.UsuaSecuencia == usu.UsuaSecuencia).FirstOrDefault();

                            //si existe el rol a insertar del doctor no se introduce
                            var existeUsuar = (from usuari in db.Usuarios
                                               where usuari.UsuaEmail == usar_Usuario.UsuaEmail
                                               select usuari).SingleOrDefault();
                            if (existeUsuar != null)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "Ya existe este Usuario";
                                respuesta.redirect = Url.Action(vista, controler);
                                return Json(respuesta);
                            }







                            usuario.RoleSecuencia_fk = 2;//doctor
                            usuario.PaisSecuencia = usar_Usuario.PaisSecuencia;
                            usuario.PlanSecuencia_fk = 4;//profesional;
                            usuario.EstaDesabilitado = false;
                            usuario.UsuaSecuenciaCreacion = usu.usuario.UsuaSecuencia;
                            usuario.UsuaFechaCreacion = Lib.GetLocalDateTime();

                            //buscando la proxima secuencia, el doctor en todas sus consultorios tiene los mismos roles 
                            proximoItem = ((from usuarioMax in db.Usuarios
                                            select (int?)usuarioMax.UsuaSecuencia).Max() ?? 0) + 1;
                            usuario.UsuaSecuencia = proximoItem;

                          
                                if (usuario.UsuaClave.Length < 6)
                                {
                                    respuesta.respuesta = false;
                                    respuesta.error = "La Contraseña tiene que tener mas de 5 digitos y mayusculas";
                                    var a = ModelState.IsValid;
                                    return Json(respuesta);
                                }
                                if (!usuario.UsuaClave.Any(char.IsUpper) ||
                                    !usuario.UsuaClave.Any(char.IsLower) ||
                                    !usuario.UsuaClave.Any(char.IsDigit))
                                {
                                    respuesta.respuesta = false;
                                    respuesta.error = "La Contraseña tiene que tener mas de 5 digitos, mayusculas y números";
                                    var a = ModelState.IsValid;
                                    return Json(respuesta);
                                }
                            
                            
                            db.Usuarios.Add(usuario);

                            //creo doctor
                            Doctor doc = new Doctor();
                            doc.DoctSecuencia = ((from usuarioMax in db.Doctors
                                                  select (int?)usuarioMax.DoctSecuencia).Max() ?? 0) + 1;
                            doc.EstaDesabilitado = false;
                            doc.UsuSecuencia = usuario.UsuaSecuencia;
                            doc.EspeSecuencia = usar_Usuario.EspeSecuencia_fk;
                            doc.PaisSecuencia = usar_Usuario.PaisSecuencia;
                            doc.DoctNombre = usar_Usuario.UsuaNombre;
                            doc.DoctApellido = usar_Usuario.UsuaApellido;
                            doc.DoctDocumento = usar_Usuario.DoctDocumento;
                            doc.TDSecuencia = usar_Usuario.TDSecuencia;
                            doc.DoctFechaCreacion = Lib.GetLocalDateTime();

                            db.Doctors.Add(doc);


                            //creo clinica
                            Clinica cli = new Clinica();
                            cli.clinSecuencia = ((from usuarioMax in db.Clinicas
                                                  select (int?)usuarioMax.clinSecuencia).Max() ?? 0) + 1;
                            cli.PaisSecuencia_fk = usar_Usuario.PaisSecuencia;
                            cli.clinRazonSocial = usar_Usuario.clinRazonSocial;
                            cli.EstaDesabilitado = false;
                            db.Clinicas.Add(cli);

                            //crear consultorio
                            Consultorio con = new Consultorio();
                            con.DoctSecuencia_fk = doc.DoctSecuencia;
                            con.EstaDesabilitado = false;
                            con.ConsSecuencia = ((from usuarioMax in db.Consultorios
                                                  select (int?)usuarioMax.ConsSecuencia).Max() ?? 0) + 1;
                            con.ClinSecuencia_fk = cli.clinSecuencia;
                            con.PaisSecuencia_fk = usar_Usuario.PaisSecuencia;
                            con.ConsCodigo = usar_Usuario.ConsCodigo;
                            db.Consultorios.Add(con);

                            //asignar usuario a consultorio
                            UsuarioConsultorio usuconsultio = new UsuarioConsultorio();
                            usuconsultio.UsuaSecuencia_fk = usuario.UsuaSecuencia;
                            usuconsultio.EstaDesabilitado = false;
                            usuconsultio.ConsSecuencia_fk = con.ConsSecuencia;
                            usuconsultio.ConsCodigo = con.ConsCodigo;
                            usuconsultio.ConsDescripcion = con.ConsDescripcion;
                            usuconsultio.PaisSecuencia_fk = usar_Usuario.PaisSecuencia;
                            usuconsultio.clinSecuencia_fk = cli.clinSecuencia;
                            usuconsultio.clinRazonSocial = cli.clinRazonSocial;
                            usuconsultio.UConsSecuencia = ((from usuarioMax in db.UsuarioConsultorios
                                                            select (int?)usuarioMax.UConsSecuencia).Max() ?? 0) + 1;
                            usuconsultio.UsuaSecuenciaCreacion = usu.usuario.UsuaSecuencia;
                            usuconsultio.UConsFechaCreacion = Lib.GetLocalDateTime();

                            db.UsuarioConsultorios.Add(usuconsultio);
                            

                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Nuevo.ToString(), null);
                        }
                        //Editando 
                        else
                        {
                        

                            //creo un inner join para poder validar que el usuario del personal que se buscara para editar
                            // pertenezca a el doctor loguiado 
                            //var EditarObjeValido = (from Perso in db.Usuarios
                            //                        where Perso.UsuaSecuencia == usar_Usuario.UsuaSecuencia                                                  
                            //                        select Perso).SingleOrDefault();

                            //if (EditarObjeValido == null)
                            //{

                            //    respuesta.respuesta = false;
                            //    respuesta.error = " este usuario a editar, no existe";
                            //    respuesta.redirect = Url.Action(vista, controler);
                            //    return Json(respuesta);
                            //}

                            //var EditarObje = (from usuari in db.Usuarios
                            //                  where usuari.UsuaSecuencia == usar_Usuario.UsuaSecuencia
                            //                  select usuari).SingleOrDefault();


                            //EditarObje.UsuaClave = usar_Usuario.UsuaClave;
                            //EditarObje.RoleSecuencia_fk = usar_Usuario.RoleSecuencia_fk;
                            //EditarObje.UsuaSecuenciaModificacion = usu.usuario.UsuaSecuencia;
                            //EditarObje.UsuaFechaModificacion = Lib.GetLocalDateTime();

                            ////BORRAR TODOS  FORMULARIOS ASIGNADOS A UN PERSONAL
                            //List<UsuarioConsultorio> lpersonConsulBorrar = new List<UsuarioConsultorio>();
                            //lpersonConsulBorrar = (from pcon in db.UsuarioConsultorios
                            //                       where pcon.UsuaSecuencia_fk == usar_Usuario.UsuaSecuencia

                            //                       select pcon).ToList();
                            //if (lpersonConsulBorrar.Count > 0)
                            //{
                            //    foreach (var item in lpersonConsulBorrar)
                            //    {
                            //        //borrar actuales consultorios
                            //        db.UsuarioConsultorios.Remove(item);
                            //    }

                            //    ////ingresar nuevos consultorios
                            //    ////guardar Personal Consultorios a quien puede insertar
                            //    int secConsul = 0;
                            //    //guardar a los consultorios que tiene acceso                          
                            //    foreach (int item in usar_Usuario.ConsSecuencia)
                            //    {
                            //        secConsul++;
                            //        UsuarioConsultorio usuarioconsul = new UsuarioConsultorio();
                            //        usuarioconsul.UsuaSecuencia_fk = EditarObje.UsuaSecuencia;
                            //        usuarioconsul.PaisSecuencia_fk = usu.usuario.PaisSecuencia;
                            //        usuarioconsul.ConsSecuencia_fk = item;
                            //        usuarioconsul.UConsSecuencia = secConsul;
                            //        //buscar consultorio del doctor del cual pertenece este personal
                            //        vw_ConsultorioDoctor consu = db.vw_ConsultorioDoctor.Where(c =>
                            //                                            c.DoctSecuencia_fk == usu.doctSecuencia
                            //                                            && c.PaisSecuencia_fk == usu.Consultorio.PaisSecuencia_fk
                            //                                            && c.ClinSecuencia_fk == usu.Consultorio.clinSecuencia_fk
                            //                                            && c.ConsSecuencia == item).SingleOrDefault();
                            //        usuarioconsul.ConsCodigo = consu.ConsCodigo;
                            //        usuarioconsul.ConsDescripcion = consu.ConsDescripcion;
                            //        usuarioconsul.clinSecuencia_fk = consu.ClinSecuencia_fk;
                            //        usuarioconsul.clinRazonSocial = consu.clinRazonSocial;
                            //        usuarioconsul.UConsFechaModificacion = Lib.GetLocalDateTime();
                            //        usuarioconsul.UsuaSecuenciaModificacion = usu.usuario.UsuaSecuencia;
                            //        db.UsuarioConsultorios.Add(usuarioconsul);
                            //    }



                            //    new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Editar.ToString(), null);
                            //}
                        }

                        db.SaveChanges();
                        dbtrans.Commit();

                        respuesta.respuesta = true;
                        respuesta.redirect = Url.Content("~/" + controler + "/" + vista + "");

                        //Enlistar los 5 ultimos Registros                
                        respuesta.someCollection = UltimosCincoRegistros().someCollection;
                        return Json(respuesta);
                    }
                    catch (DbEntityValidationException ex)
                    {

                        foreach (var validationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                Trace.TraceInformation("Property: {0} Error: {1}",
                                                        validationError.PropertyName,
                                                        validationError.ErrorMessage);
                            }
                        }

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

        }//end method save
          [HttpPost]
        //este metodo viene desde el registro que se selecciono del grid
        public JsonResult Editar(Usar_Usuario usar_Usuario)
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

            //usuario loguieado
            UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];
            using (var db = new DoctorMedicalWebEntities())
            {
                if (usar_Usuario != null)
                {
                    //usuario que  se selecciono en el grid
                    Usuario UsuarioObje = new Usuario();




                    UsuarioObje = (from objUsua in db.Usuarios
                                   where objUsua.UsuaSecuencia == usar_Usuario.UsuaSecuencia
                                            && objUsua.EstaDesabilitado == false
                                   select objUsua).SingleOrDefault();

                    if (UsuarioObje != null)
                    {
                        //objeto para cargarlo y enviarlo a la vista
                        var usarUsuario = new Usar_Usuario();
                        //asignar contenido a otro objeto
                        CopyClass.CopyObject(UsuarioObje, ref usarUsuario);
                        usarUsuario.UsuaClaveConfirmacion = UsuarioObje.UsuaClave;

                        //llenno la sesion con el usuario a editar
                        //para abrirlo en  el metodo principal del controler
                        Session["Usar_Usuario"] = usarUsuario;
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
        //Metodo para borrar usuario Persona
          [HttpPost]
        public JsonResult Borrar(Usar_Usuario usar_Usuario)
        {
            var respuesta = new ResponseModel();
            //Valiod que este loguieado
            UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];
            //Si NO esta loguieado lo redireccionara al loguin
            if (usu == null)
            {
                TempData["NoTienePermisoParaPagina"] = "Usted no esta loguiado!";
                respuesta.respuesta = false;
                respuesta.redirect = Url.Action(vista, controler);
                return Json(respuesta);
            }

            using (var db = new DoctorMedicalWebEntities())
            {

                //si secuenca esta vacio devolver mensaje
                if (string.IsNullOrEmpty(usar_Usuario.UsuaSecuencia.ToString()) || usar_Usuario.UsuaSecuencia < 1)
                {
                    respuesta.respuesta = false;
                    respuesta.error = "Favor seleccionar una  Usuario en el listado";
                    respuesta.redirect = Url.Action(vista, controler);
                    return Json(respuesta);

                }
                using (var dbtrans = db.Database.BeginTransaction())
                {
                    try
                    {
                        //borro registro
                        if (usar_Usuario != null)
                        {


                            var EditarObjeValido = (from usuari in db.Usuarios
                                                    where usuari.UsuaSecuencia == usar_Usuario.UsuaSecuencia
                                                    select usuari).SingleOrDefault();

                            if (EditarObjeValido == null)
                            {

                                respuesta.respuesta = false;
                                respuesta.error = " este usuario, no existe";
                                respuesta.redirect = Url.Action(vista, controler);
                                return Json(respuesta);
                            }


                            Usuario borrar = (from usuariopborr in db.Usuarios
                                              where usuariopborr.UsuaSecuencia == usar_Usuario.UsuaSecuencia
                                              select usuariopborr).SingleOrDefault();

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
                        respuesta.error = "Posiblemente usted tiene otro registro que depende de éste o No Existe este Formulario " + ex.GetBaseException().Message.ToString();
                        //respuesta.redirect = Url.Action("Ini_TipoFormulario", "TipoFormulario");
                        respuesta.redirect = Url.Action(vista, controler);
                        new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Error.ToString(), ex.GetBaseException().Message);
                        return Json(respuesta);
                    }
                }
            }
        }//End Borrar Metodo

        public ActionResult UsuarioDoctorlista()
        {
            //pARA EN LA VISTA PODER SABER A QUE  CONTROLADOR EL AJAX  LLAMARA
            ViewBag.ControlCsharp = controler;
            ViewBag.VistaCsharp = vista;

            //Valiod que este loguieado
            UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];
            //Si NO esta loguieado lo redireccionara al loguin
            if (usu == null)
            {
                TempData["NoTienePermisoParaPagina"] = "Usted no esta loguiado!";
                return RedirectToAction(vista, controler);
            }

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

                //para que el div de accion solo aparezca si son 
                //no son home y como no quiero que aparezca el div lo en esta vista de lista
                //lo pongo true como para indicar que esta pagina es home, para que no me aparezca el 
                //div de accion
                ViewBag.isHome = true;


                List<Usuario> listUsuario = (from vw_UsuariosPersonalDelDoctorob in db.Usuarios
                                             where  vw_UsuariosPersonalDelDoctorob.EstaDesabilitado==false
                                             select vw_UsuariosPersonalDelDoctorob).ToList();
                var listUsar_usuario = new List<Usar_Usuario>();

                ////crear objeto tipo formulario y asignar a la lista de usartipoformulario
                foreach (var item in listUsuario)
                {
                    Usar_Usuario usar_usuario = new Usar_Usuario();
                    //asignar contenido a otro objeto
                    CopyClass.CopyObject(item, ref usar_usuario);
                    listUsar_usuario.Add(usar_usuario);
                }
                ViewBag.datasource = listUsuario;
                return View();
            }
        }

    }
}
