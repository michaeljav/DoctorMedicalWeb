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
    public class UsuarioPersonalController : Controller
    {
        string controler = "UsuarioPersonal", vista = "Ini_UsuarioPersonal", PaginaAutorizada = Paginas.pag_UsuarioPersonal.ToString();

        //
        // GET: /Usuario/

        public ActionResult Ini_UsuarioPersonal()
        {
            using (DoctorMedicalWebEntities db = new DoctorMedicalWebEntities())
            {

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

                List<Role> rolesList = ((from Rols in db.Roles
                                         where 
                                        Rols.RoleSecuencia != 1//Diferente a admin
                                         && Rols.RoleSecuencia != 2//diferente a doctor 
                                         select Rols).ToList());
                ViewBag.RolesLista = new SelectList(rolesList, "RoleSecuencia", "RoleDescripcion");



             //Lista de consultorios del doctor
                List<vw_UsuarioConsultorios> CLinicaConsoltoriosDoctUsuar = ((from cantConsultorio in db.vw_UsuarioConsultorios
                                                                              where cantConsultorio.UsuaSecuencia == usu.usuario.UsuaSecuencia
                                                                              select cantConsultorio).ToList());
                ViewBag.ConsultoriosListaCheck = CLinicaConsoltoriosDoctUsuar;

                //List<vw_UsuarioConsultorios> CLinicaConsoltoriosDoctUsuar = ((from cantConsultorio in db.vw_UsuarioConsultorios
                //                                                              where cantConsultorio.UsuaSecuencia == usu.usuario.UsuaSecuencia
                //                                                              select cantConsultorio).ToList());
                //ViewBag.ConsultoriosListaCheck = CLinicaConsoltoriosDoctUsuar;

                //buscar listado de personal
                List<Personal> personal = ((from pers in db.Personals
                                            where pers.DoctSecuencia_fk == usu.doctSecuencia
                                            && pers.EstaDesabilitado ==false
                                            select pers).ToList());


                ViewBag.listPersonal = personal;


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

                Usar_UsuarioPersonal usarUsuario = new Usar_UsuarioPersonal();
                //Esta propiedades con espacio es para que en chrome no me auto llene estos campos ya que los tiene guardados en la cache
                usarUsuario.UsuaEmail = " ";
              
                //si  esta lleno es por que esta editando
                if (Session["Usar_UsuarioPersonal"] != null)
                {
                    usarUsuario = (Usar_UsuarioPersonal)Session["Usar_UsuarioPersonal"];
                    //para llenar el dropdownlist y select el checklist
                    
                    ViewBag.FillDropDownListCheckJson = usarUsuario.ConsSecuencia;

                    //ViewData["FillDropDownListCheckJson"] = usarUsuario.ConsSecuencia;
                    //limpiar sesion
                    Session["Usar_UsuarioPersonal"] = null;
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

                    vw_UsuarioDoctor docSecuencia = db.vw_UsuarioDoctor.Where(d => d.UsuaSecuencia == usu.usuario.UsuaSecuencia).FirstOrDefault();

                    //buscar los ultimos  cinco registros  de usuarios del doctor loguiado
                    List<vw_UsuariosPersonalDelDoctor> UsuarioListaUltimosCinco = (from tf in db.vw_UsuariosPersonalDelDoctor
                                                                                   where tf.DoctSecuencia_fk == docSecuencia.DoctSecuencia
                                                                                   orderby tf.UsuaSecuencia descending
                                                                                   select tf).Take(5).ToList();

                    var listUsar_usuario = new List<Usar_UsuarioPersonal>();
                    //crear objeto role y asignar a la lista de usaroles
                    foreach (var item in UsuarioListaUltimosCinco)
                    {
                        Usar_UsuarioPersonal usuari = new Usar_UsuarioPersonal();
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
        public JsonResult Save(Usar_UsuarioPersonal usar_UsuarioPersonal)
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
                        CopyClass.CopyObject(usar_UsuarioPersonal, ref usuario);

                        //vw_UsuarioDoctor usuarioDoctor = null;
                        //add new 

                        if (string.IsNullOrEmpty(usuario.UsuaSecuencia.ToString()) || usuario.UsuaSecuencia == 0)
                        {

                            ////verificar si existe este usuario
                            //usuarioDoctor = db.vw_UsuarioDoctor.Where(d => d.UsuaSecuencia == usu.UsuaSecuencia).FirstOrDefault();

                            //si existe el rol a insertar del doctor no se introduce
                            var existeUsuar = (from usuari in db.Usuarios
                                               where usuari.UsuaEmail == usar_UsuarioPersonal.UsuaEmail
                                               select usuari).SingleOrDefault();
                            if (existeUsuar != null)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "Ya existe este Usuario";
                                respuesta.redirect = Url.Action(vista, controler);
                                return Json(respuesta);
                            }

                            //verificar que ese personal no haya sido seleccionado con otro usuario

                            Personal person = db.Personals.Where(p => p.DoctSecuencia_fk == usu.doctSecuencia && p.PersSecuencia == usar_UsuarioPersonal.PersSecuencia).SingleOrDefault();

                            if (person.UsuaSecuencia != null)
                            {

                                respuesta.respuesta = false;
                                respuesta.error = "Ya este personal tiene asignado un usuario";
                                respuesta.redirect = Url.Action(vista, controler);
                                return Json(respuesta);
                            }




                            usuario.UsuaNombre = person.PersNombre;
                            usuario.UsuaApellido = person.PersApellido;
                            usuario.UsuaFechaNacimiento = (DateTime)person.PersFechaNacimiento;
                            usuario.UsuaGenero = person.PersGenero;
                            usuario.RoleSecuencia_fk = usar_UsuarioPersonal.RoleSecuencia_fk;
                            usuario.PaisSecuencia = usu.usuario.PaisSecuencia;
                            usuario.PlanSecuencia_fk = usu.usuario.PlanSecuencia_fk;
                            usuario.EstaDesabilitado = false;
                            usuario.UsuaSecuenciaCreacion = usu.usuario.UsuaSecuencia;
                            usuario.UsuaFechaCreacion = Lib.GetLocalDateTime();
                            usuario.UsuaSecuenciaModificacion = usu.usuario.UsuaSecuencia;
                            //insertar clave
                            
                                if (usar_UsuarioPersonal.UsuaClave.Length < 6)
                                {
                                    respuesta.respuesta = false;
                                    respuesta.error = "La Contraseña tiene que tener mas de 5 digitos y mayusculas";
                                    var a = ModelState.IsValid;
                                    return Json(respuesta);
                                }
                                if (!usar_UsuarioPersonal.UsuaClave.Any(char.IsUpper) ||
                                    !usar_UsuarioPersonal.UsuaClave.Any(char.IsLower) ||
                                    !usar_UsuarioPersonal.UsuaClave.Any(char.IsDigit))
                                {
                                    respuesta.respuesta = false;
                                    respuesta.error = "La Contraseña tiene que tener mas de 5 digitos, mayusculas y números";
                                    var a = ModelState.IsValid;
                                    return Json(respuesta);
                                }

                                //SI TIENE UPDATE EL EMAIL NO  SE LOGUIEA
                                if (usar_UsuarioPersonal.UsuaEmail.Any(char.IsUpper))
                                {
                                    respuesta.respuesta = false;
                                    respuesta.error = "El usuario debe de ser minúsculo.";
                                
                                    return Json(respuesta);
                                }



                                usuario.UsuaClave = usar_UsuarioPersonal.UsuaClave;
                            


                            //buscando la proxima secuencia, el doctor en todas sus consultorios tiene los mismos roles 
                            proximoItem = ((from usuarioMax in db.Usuarios
                                            select (int?)usuarioMax.UsuaSecuencia).Max() ?? 0) + 1;
                            usuario.UsuaSecuencia = proximoItem;
                            db.Usuarios.Add(usuario);

                            //modificar personal y asignarle el nuevo usuario
                            person.UsuaSecuencia = proximoItem;

                            int secConsul = 0;
                            //guardar a los consultorios que tiene acceso                          
                            foreach (int item in usar_UsuarioPersonal.ConsSecuencia)
                            {
                                secConsul++;
                                UsuarioConsultorio usuarioconsul = new UsuarioConsultorio();
                                usuarioconsul.UsuaSecuencia_fk = proximoItem;
                                usuarioconsul.PaisSecuencia_fk = usu.usuario.PaisSecuencia;
                                usuarioconsul.clinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                usuarioconsul.ConsSecuencia_fk = item;
                                usuarioconsul.UConsSecuencia = secConsul;
                                //buscar consultorio del doctor del cual pertenece este personal
                                vw_ConsultorioDoctor consu = db.vw_ConsultorioDoctor.Where(c =>
                                                                    c.DoctSecuencia_fk == usu.doctSecuencia
                                                                    && c.PaisSecuencia_fk == usu.Consultorio.PaisSecuencia_fk
                                                                    && c.ClinSecuencia_fk == usu.Consultorio.clinSecuencia_fk
                                                                    && c.ConsSecuencia == item).SingleOrDefault();
                                usuarioconsul.ConsCodigo = consu.ConsCodigo;
                                usuarioconsul.ConsDescripcion = consu.ConsDescripcion;
                                usuarioconsul.clinSecuencia_fk = consu.ClinSecuencia_fk;
                                usuarioconsul.clinRazonSocial = consu.clinRazonSocial;

                                usuarioconsul.UsuaSecuenciaCreacion = usu.usuario.UsuaSecuencia;
                                usuarioconsul.UConsFechaCreacion = Lib.GetLocalDateTime();
                                usuarioconsul.UsuaSecuenciaModificacion = usu.usuario.UsuaSecuencia;
                                db.UsuarioConsultorios.Add(usuarioconsul);
                            }
                            //llenar los nombres  de los consultorios

                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Nuevo.ToString(), null);
                        }
                        //Editando 
                        else
                        {
                            ////buscar usuario a  que va a editarse
                            //var EditarObje = (from usuari in db.Usuarios
                            //                  where   usuari.UsuaSecuencia == usar_UsuarioPersonal.UsuaSecuencia
                            //                  select usuari).SingleOrDefault();

                            //creo un inner join para poder validar que el usuario del personal que se buscara para editar
                            // pertenezca a el doctor loguiado 
                            var EditarObjeValido = (from Perso in db.Personals
                                                    where Perso.UsuaSecuencia == usar_UsuarioPersonal.UsuaSecuencia
                                                    && Perso.DoctSecuencia_fk == usu.doctSecuencia
                                                    select Perso).SingleOrDefault();

                            if (EditarObjeValido == null)
                            {

                                respuesta.respuesta = false;
                                respuesta.error = "Ya este usuario para borrar , no existe";
                                respuesta.redirect = Url.Action(vista, controler);
                                return Json(respuesta);
                            }

                            var  EditarObje = (from usuari in db.Usuarios
                                              where usuari.UsuaSecuencia == usar_UsuarioPersonal.UsuaSecuencia
                                              select usuari).SingleOrDefault();


                           // EditarObje.UsuaClave = usar_UsuarioPersonal.UsuaClave;

                            //insertar clave
                            if (!string.IsNullOrEmpty(usar_UsuarioPersonal.UsuaClave))
                            {
                                if (usar_UsuarioPersonal.UsuaClave.Length < 6)
                                {
                                    respuesta.respuesta = false;
                                    respuesta.error = "La Contraseña tiene que tener mas de 5 digitos y mayusculas";                            
                                    return Json(respuesta);
                                }
                                if (!usar_UsuarioPersonal.UsuaClave.Any(char.IsUpper) ||
                                    !usar_UsuarioPersonal.UsuaClave.Any(char.IsLower) ||
                                    !usar_UsuarioPersonal.UsuaClave.Any(char.IsDigit))
                                {
                                    respuesta.respuesta = false;
                                    respuesta.error = "La Contraseña tiene que tener mas de 5 digitos, mayusculas y números";                                   
                                    return Json(respuesta);
                                }
                                EditarObje.UsuaClave = usar_UsuarioPersonal.UsuaClave;
                            }



                            EditarObje.RoleSecuencia_fk = usar_UsuarioPersonal.RoleSecuencia_fk;
                            EditarObje.UsuaSecuenciaModificacion = usu.usuario.UsuaSecuencia;
                            EditarObje.UsuaFechaModificacion = Lib.GetLocalDateTime();

                            //BORRAR TODOS  FORMULARIOS ASIGNADOS A UN PERSONAL
                            List<UsuarioConsultorio> lpersonConsulBorrar = new List<UsuarioConsultorio>();
                            lpersonConsulBorrar = (from pcon in db.UsuarioConsultorios
                                                   where pcon.UsuaSecuencia_fk == usar_UsuarioPersonal.UsuaSecuencia

                                                   select pcon).ToList();
                            if (lpersonConsulBorrar.Count > 0)
                            {
                                foreach (var item in lpersonConsulBorrar)
                                {
                                    //borrar actuales consultorios
                                    db.UsuarioConsultorios.Remove(item);
                                }

                                ////ingresar nuevos consultorios
                                ////guardar Personal Consultorios a quien puede insertar
                                int secConsul = 0;
                                //guardar a los consultorios que tiene acceso                          
                                foreach (int item in usar_UsuarioPersonal.ConsSecuencia)
                                {
                                    secConsul++;
                                    UsuarioConsultorio usuarioconsul = new UsuarioConsultorio();
                                    usuarioconsul.UsuaSecuencia_fk = EditarObje.UsuaSecuencia;
                                    usuarioconsul.PaisSecuencia_fk = usu.usuario.PaisSecuencia;
                                    usuarioconsul.ConsSecuencia_fk = item;
                                    usuarioconsul.UConsSecuencia = secConsul;
                                    //buscar consultorio del doctor del cual pertenece este personal
                                    vw_ConsultorioDoctor consu = db.vw_ConsultorioDoctor.Where(c =>
                                                                        c.DoctSecuencia_fk == usu.doctSecuencia
                                                                        && c.PaisSecuencia_fk == usu.Consultorio.PaisSecuencia_fk
                                                                        && c.ClinSecuencia_fk == usu.Consultorio.clinSecuencia_fk
                                                                        && c.ConsSecuencia == item).SingleOrDefault();
                                    usuarioconsul.ConsCodigo = consu.ConsCodigo;
                                    usuarioconsul.ConsDescripcion = consu.ConsDescripcion;
                                    usuarioconsul.clinSecuencia_fk = consu.ClinSecuencia_fk;
                                    usuarioconsul.clinRazonSocial = consu.clinRazonSocial;
                                    usuarioconsul.UConsFechaModificacion = Lib.GetLocalDateTime();
                                    usuarioconsul.UsuaSecuenciaModificacion = usu.usuario.UsuaSecuencia;
                                    db.UsuarioConsultorios.Add(usuarioconsul);
                                }



                                new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Editar.ToString(), null);
                            }
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
        public JsonResult Editar(vw_UsuariosPersonalDelDoctor usar_UsuarioPersonal)
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
                if (usar_UsuarioPersonal != null)
                {
                    //usuario que  se selecciono en el grid
                    Usuario UsuarioObje = new Usuario();


                    //creo un inner join para poder validar que el usuario del personal que se buscara para editar
                    // pertenezca a el doctor loguiado 
                    var EditarObjeValido = (from Perso in db.Personals                                                                                   
                                            where Perso.UsuaSecuencia == usar_UsuarioPersonal.UsuaSecuencia
                                            && Perso.DoctSecuencia_fk == usu.doctSecuencia
                                            select  Perso).SingleOrDefault();

                    if (EditarObjeValido == null)
                    {

                        respuesta.respuesta = false;
                        respuesta.error = "Ya este usuario a editar, no existe";
                        respuesta.redirect = Url.Action(vista, controler);
                        return Json(respuesta);
                    }


                    UsuarioObje = (from objUsua in db.Usuarios
                                   where objUsua.UsuaSecuencia == usar_UsuarioPersonal.UsuaSecuencia
                                   select objUsua).SingleOrDefault();

                    if (UsuarioObje != null)
                    {
                        //objeto para cargarlo y enviarlo a la vista
                        Usar_UsuarioPersonal usarPersonal = new Usar_UsuarioPersonal();
                        //asignar contenido a otro objeto
                        CopyClass.CopyObject(UsuarioObje, ref usarPersonal);
                        usarPersonal.UsuaClaveConfirmacion = usarPersonal.UsuaClave;
                        usarPersonal.UsuaEmailConfirmacion = usarPersonal.UsuaEmail;

                        List<UsuarioConsultorio> usuconsul = new List<UsuarioConsultorio>();
                        //LLenar  los consultorios que tiene ese personal
                        usuconsul = (from con in db.UsuarioConsultorios
                                     where con.UsuaSecuencia_fk == usar_UsuarioPersonal.UsuaSecuencia
                                     select con).ToList();
                        List<int> lPeconsul = new List<int>();
                        foreach (UsuarioConsultorio item in usuconsul)
                        {

                            lPeconsul.Add(item.ConsSecuencia_fk);
                        }
                        usarPersonal.ConsSecuencia = lPeconsul;
                        //lleno el combo de Consultorio
                  

                        Personal person = db.Personals.Where(p => p.DoctSecuencia_fk == usu.doctSecuencia && p.UsuaSecuencia == usar_UsuarioPersonal.UsuaSecuencia).SingleOrDefault();

                        usarPersonal.PersSecuencia = person.PersSecuencia;
                        usarPersonal.DocumentoPasaportCedula = person.PersDocumento;
                        //llenno la sesion con el usuario a editar
                        //para abrirlo en  el metodo principal del controler
                        Session["Usar_UsuarioPersonal"] = usarPersonal;
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
        public JsonResult Borrar(Usar_UsuarioPersonal usar_UsuarioPersonal)
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
                if (string.IsNullOrEmpty(usar_UsuarioPersonal.UsuaSecuencia.ToString()) || usar_UsuarioPersonal.UsuaSecuencia < 1)
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
                        if (usar_UsuarioPersonal != null)
                        {

                            //Usuario usuarioObj = new Usuario();
                            ////asignar contenido a otro objeto
                            //CopyClass.CopyObject(usar_UsuarioPersonal, ref usuarioObj);
                            //creo un inner join para poder validar que el usuario del personal que se buscara para editar
                            // pertenezca a el doctor loguiado 
                            //var EditarObjeValido = (from usuari in db.Usuarios
                            //                        join doct in db.Doctors
                            //                        on usuari.UsuaSecuencia equals doct.UsuSecuencia                                                    
                            //                        where (usuari.UsuaSecuencia == usar_UsuarioPersonal.UsuaSecuencia)
                            //                        && (doct.DoctSecuencia == usu.doctSecuencia)
                            //                        select new { usuari }).SingleOrDefault();

                            var EditarObjeValido = (from Perso in db.Personals
                                                    where Perso.UsuaSecuencia == usar_UsuarioPersonal.UsuaSecuencia
                                                    && Perso.DoctSecuencia_fk == usu.doctSecuencia
                                                    select Perso).SingleOrDefault();


                            if (EditarObjeValido == null)
                            {

                                respuesta.respuesta = false;
                                respuesta.error = "Ya este usuario para borrar, no existe";
                                respuesta.redirect = Url.Action(vista, controler);
                                return Json(respuesta);
                            }


                            Usuario borrar = (from usuariopborr in db.Usuarios
                                              where usuariopborr.UsuaSecuencia == usar_UsuarioPersonal.UsuaSecuencia
                                              select usuariopborr).SingleOrDefault();

                            if (borrar != null)
                            {
                                //si el usuario se ha loguiado por lo menos una vez, queda registrado con auditoria, por lo cual no 
                                //puede borrarse.
                                Auditoria audi = db.Auditorias.Where(a => a.UsuaCodigo == usar_UsuarioPersonal.UsuaSecuencia).SingleOrDefault();

                                if (audi != null)
                                {
                                    respuesta.respuesta = false;
                                    respuesta.error = "No puede borrar este usuario, por que ya se ha loguiado anteriormente. ";
                                    respuesta.redirect = Url.Action(vista, controler);
                                    return Json(respuesta);

                                }


                                //quito de  la tabla personal el usuario
                                Personal pers = db.Personals.Where(p => p.UsuaSecuencia == usar_UsuarioPersonal.UsuaSecuencia).SingleOrDefault();
                                pers.UsuaSecuencia = null;

                                //BORRAR TODOS  FORMULARIOS ASIGNADOS A ese usuario
                                List<UsuarioConsultorio> lpersonConsulBorrar = new List<UsuarioConsultorio>();
                                lpersonConsulBorrar = (from pcon in db.UsuarioConsultorios
                                                       where pcon.UsuaSecuencia_fk == usar_UsuarioPersonal.UsuaSecuencia
                                                       select pcon).ToList();
                                if (lpersonConsulBorrar.Count > 0)
                                {
                                    foreach (var item in lpersonConsulBorrar)
                                    {
                                        //borrar actuales consultorios
                                        db.UsuarioConsultorios.Remove(item);
                                    }

                                }

                                //Borro el personal
                                db.Usuarios.Remove(borrar);


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

        public ActionResult Usuarioslista()
        {
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


                List<vw_UsuariosPersonalDelDoctor> vw_UsuariosPersonalDelDoctorList = (from vw_UsuariosPersonalDelDoctorob in db.vw_UsuariosPersonalDelDoctor
                                                                                       where vw_UsuariosPersonalDelDoctorob.DoctSecuencia_fk == usu.doctSecuencia
                                                                                       select vw_UsuariosPersonalDelDoctorob).ToList();
                //var listUsar_Personal = new List<Usar_Personal>();

                ////crear objeto tipo formulario y asignar a la lista de usartipoformulario
                //foreach (var item in vw_UsuariosPersonalDelDoctorList)
                //{
                //    Usar_Personal perso = new Usar_Personal();
                //    //asignar contenido a otro objeto
                //    CopyClass.CopyObject(item, ref perso);
                //    listUsar_Personal.Add(perso);
                //}
                ViewBag.datasource = vw_UsuariosPersonalDelDoctorList;
                return View();
            }
        }
    }
}
