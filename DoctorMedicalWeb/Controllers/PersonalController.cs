using DoctorMedicalWeb.App_Data;
using DoctorMedicalWeb.Libreria;
using DoctorMedicalWeb.Models;
using DoctorMedicalWeb.ModelsComplementarios;
using Syncfusion.JavaScript.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoctorMedicalWeb.Controllers
{
    public class Languages
    {
        public string skill { get; set; }
    }

    public class PersonalController : Controller
    {
        //
        // GET: /Personal/
        string controler = "Personal", vista = "Ini_Personal", PaginaAutorizada = Paginas.pag_Personal.ToString();
        //string Pagina_Lista = "pag_Personal";

        public ActionResult Ini_Personal()
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





                //prueba de combo blox
                //
                // GET: /Dropdwnlistmultiselect/
                //List<Languages> lang = new List<Languages>();

                //    lang.Add(new Languages { skill = "ActionScript" });
                //    lang.Add(new Languages { skill = "AppleScript" });
                //    lang.Add(new Languages { skill = "Asp" });
                //    lang.Add(new Languages { skill = "BASIC" });
                //    lang.Add(new Languages { skill = "C" });
                //    lang.Add(new Languages { skill = "C++" });
                //    lang.Add(new Languages { skill = "Clojure" });
                //    lang.Add(new Languages { skill = "COBOL" });
                //    lang.Add(new Languages { skill = "ColdFusion" });
                //    lang.Add(new Languages { skill = "Erlang" });
                //    lang.Add(new Languages { skill = "Fortran" });
                //    lang.Add(new Languages { skill = "Groovy" });
                //    lang.Add(new Languages { skill = "Haskell" });
                //    lang.Add(new Languages { skill = "Java" });
                //    lang.Add(new Languages { skill = "JavaScript" });
                //    lang.Add(new Languages { skill = "Lisp" });
                //    lang.Add(new Languages { skill = "Perl" });
                //    lang.Add(new Languages { skill = "PHP" });
                //    lang.Add(new Languages { skill = "Python" });
                //    lang.Add(new Languages { skill = "Ruby" });
                //    lang.Add(new Languages { skill = "Scala" });
                //    lang.Add(new Languages { skill = "Scheme" });
                //    ViewBag.datasourceb = lang;
                //fin prueba de combo box



    
                ////si tiene mas de un consultorio paso
                ////a otra pantalla para que seleccione
                ////si tiene solo uno entro al sistema.
                //List<vw_UsuarioConsultorios> CLinicaConsoltoriosDoctUsuar = ((from cantConsultorio in db.vw_UsuarioConsultorios
                //                                                                       where cantConsultorio.DoctSecuencia_fk == consultorioUsuarioSeleccionado.DoctSecuencia_fk
                //                                                                       select cantConsultorio).ToList());
                //ViewBag.ConsultoriosListaCheck = CLinicaConsoltoriosDoctUsuar;


                //ViewData["DropDownSource"] = rolesList;

                //DropDownListProperties DropdownProperties = new DropDownListProperties();
                //DropdownProperties.DataSource = rolesList;
                //DropdownProperties.Width = "100%";
                ////DropdownProperties.AllowMultiSelection = true;
                //DropdownProperties.ShowCheckbox = true;
                //DropDownListFields DropdownFields = new DropDownListFields();
                //DropdownFields.Text = "NombreConsultorio";
                //DropdownFields.Id = "ConsSecuencia";
                //DropdownFields.Value = "ConsSecuenciap";
                //DropdownProperties.DropDownListFields = DropdownFields;
                //ViewData["DropdownModel"] = DropdownProperties;



                //DropDownListProperties ddl = new DropDownListProperties();
                //ddl.DataSource = rolesList;
                //DropDownListFields ddf = new DropDownListFields();
                //ddf.Text = "NombreConsultorio";
                //ddf.Value = "ConsSecuencia";
                //ddl.DropDownListFields = ddf;
                //ViewData["properties"] = ddl;


                //List<DropDownValue> data = new List<DropDownValue>() { };
                //data.Add(new DropDownValue() { Value = "item1", Text = "List Item 1" });
                //data.Add(new DropDownValue() { Value = "item2", Text = "List Item 2" });
                //data.Add(new DropDownValue() { Value = "item3", Text = "List Item 3" });
                //data.Add(new DropDownValue() { Value = "item4", Text = "List Item 4" });
                //data.Add(new DropDownValue() { Value = "item5", Text = "List Item 5" });
                //DropDownListProperties ddl = new DropDownListProperties();
                //ddl.DataSource = data;
                //DropDownListFields ddf = new DropDownListFields();
                //ddf.Text = "Text";
                //ddf.Value = "Value";
                //ddl.DropDownListFields = ddf;
                //ViewData["properties"] = ddl;


                //lleno el combo de tipo Documento
                IEnumerable<TipoDocumento> tipoDocumento = db.TipoDocumentoes.ToList();
                ViewBag.tipoDocumentos = new SelectList(tipoDocumento, "TDSecuencia", "TDDocumento");

                //lleno el combo de de categoria personal
                List<CategoriaPersonal> categoriaPersonal = new List<CategoriaPersonal>();

                vw_UsuarioDoctor docSecuencia = db.vw_UsuarioDoctor.Where(d => d.UsuaSecuencia == usu.usuario.UsuaSecuencia).FirstOrDefault();
                categoriaPersonal = (from tf in db.CategoriaPersonals
                                     where tf.DoctSecuencia_fk == docSecuencia.DoctSecuencia
                                     orderby tf.CPersNombre
                                     select tf).ToList();

                ViewBag.CategoriaPersonal = new SelectList(categoriaPersonal, "CPersSecuencia", "CPersNombre");


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
                ViewBag.isHome = false;

                //Enlistar los 5 ultimos y lleno
                //el   ViewBag.ultimosCinco
                var a = UltimosCincoRegistros();

                Usar_Personal usarPersonal = new Usar_Personal();

                //si  esta lleno es por que esta editando
                if (Session["Usar_Personal"] != null)
                {
                    usarPersonal = (Usar_Personal)Session["Usar_Personal"];
                    //para llenar el dropdownlist y select el checklist
                    //ViewBag.FillDropDownListCheckJson = usarPersonal.ConsSecuencia;
                    //ViewData["FillDropDownListCheckJson"] = usarPersonal.ConsSecuencia;
                    //limpiar sesion
                    Session["Usar_Personal"] = null;
                    return View(usarPersonal);
                }

                return View(usarPersonal);
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

                    //buscar los ultimos  cinco registros  de roles del doctor loguiado
                    List<Personal> PersonalListaUltimosCinco = (from tf in db.Personals
                                                                where tf.DoctSecuencia_fk == docSecuencia.DoctSecuencia
                                                                         && tf.EstaDesabilitado == false
                                                                orderby tf.PersSecuencia descending
                                                                select tf).Take(5).ToList();

                    var listUsar_Personal = new List<Usar_Personal>();
                    //crear objeto role y asignar a la lista de usaroles
                    foreach (var item in PersonalListaUltimosCinco)
                    {
                        Usar_Personal personal = new Usar_Personal();
                        //asignar contenido a otro objeto
                        CopyClass.CopyObject(item, ref personal);
                        listUsar_Personal.Add(personal);
                    }
                    //Utilizado para insertar el listado al grid
                    ViewBag.ultimosCinco = listUsar_Personal;

                    //ienumerable lista
                    respuesta.someCollection = listUsar_Personal;

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
        public JsonResult Editar(Usar_Personal usar_Personal)
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
                if (usar_Personal != null)
                {
                    Personal Personal = new Personal();

                    Personal = (from cPersonal in db.Personals
                                where cPersonal.PersSecuencia == usar_Personal.PersSecuencia
                                                   && cPersonal.DoctSecuencia_fk == usu.doctSecuencia
                                                            && cPersonal.EstaDesabilitado == false
                                select cPersonal).SingleOrDefault();

                    if (Personal != null)
                    {
                        Usar_Personal usarPersonal = new Usar_Personal();
                        //asignar contenido a otro objeto
                        CopyClass.CopyObject(Personal, ref usarPersonal);

                        //List<UsuarioConsultorio> usuconsul = new List<UsuarioConsultorio>();
                        ////LLenar  los consultorios que tiene ese personal
                        //usuconsul = (from con in db.UsuarioConsultorios
                        //             where con.DoctSecuencia_fk == Personal.DoctSecuencia_fk
                        //             && con.persSecuencia_fk == Personal.PersSecuencia
                        //             select con).ToList();
                        //List<int> lPeconsul = new List<int>();
                        //foreach (usuarioconsultorio item in usuconsul)
                        //{

                        //    lPeconsul.Add(item.ConsSecuencia_fk);
                        //}
                        //usarPersonal.ConsSecuencia = lPeconsul;


                        //llenno la sesion con el Personal a editar
                        //para abrirlo en  el metodo principal del controler
                        Session["Usar_Personal"] = usarPersonal;
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
        public JsonResult Save(Usar_Personal usar_Personal)
        {
            ResponseModel respuesta = new ResponseModel();
            List<string> fielsError = new List<string>();
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

                //bool TieneElementos = false;
                ////validar que la lista de consultorio se haya seleccionado por lo menos uno
                //foreach (int item in usar_Personal.ConsSecuencia)
                //{
                //    if (item > 0)
                //    {
                //        TieneElementos = true;
                //        continue;

                //    }
                //}
                //if (TieneElementos == false)
                //{
                //    fielsError.Add("ErrorCombo");
                //    respuesta.fielsWithError = fielsError;
                //}

                //si no ha validado volver y mostrar mensajes de validacion
                if (!ModelState.IsValid)
                {
                    respuesta.respuesta = false;
                    respuesta.error = "Llenar Campos requeridos ";
                    return Json(respuesta);
                }
                using (var dbtrans = db.Database.BeginTransaction())
                {
                    try
                    {
                       
                        Personal personal = new Personal();


                        //vw_UsuarioDoctor usuarioDoctor = null;
                        //add new 
                        if (usar_Personal.PersSecuencia == null || usar_Personal.PersSecuencia == 0 )
                        {

                            //busco el usuario a
                            //usuarioDoctor = db.vw_UsuarioDoctor.Where(d => d.UsuaSecuencia == consultorioUsuarioSeleccionado.UsuaSecuencia).FirstOrDefault();

                            //si existe el Personal lo ubico con el documento cedula o pasaporte del doctor no se introduce
                            var existePersonal = (from cpersonal in db.Personals
                                                  where cpersonal.PersDocumento == usar_Personal.PersDocumento
                                                   && cpersonal.TDSecuencia_fk == usar_Personal.TDSecuencia_fk
                                                      && cpersonal.DoctSecuencia_fk == usu.doctSecuencia
                                                               && cpersonal.EstaDesabilitado == false
                                                  select cpersonal).SingleOrDefault();
                            if (existePersonal != null)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "Ya existe este Personal";
                                respuesta.redirect = Url.Action(vista, controler);
                                return Json(respuesta);
                            }

                            //asignar contenido a otro objeto
                            CopyClass.CopyObject(usar_Personal, ref personal);

                            //Introduzco los campos que faltan a la tabla a insertar
                            personal.DoctSecuencia_fk = usu.doctSecuencia;
                            personal.UsuaCreacion = usu.usuario.UsuaSecuencia;
                            personal.PersFechaCreacion = Lib.GetLocalDateTime();
                            personal.PersFechaModificacion = Lib.GetLocalDateTime();
                            personal.PaisSecuencia_fk = usu.usuario.PaisSecuencia;
                            personal.EstaActivo = true;
                          

                            //buscando la proxima secuencia, el doctor en todas sus consultorios tiene los mismos roles 
                            proximoItem = ((from PersonalMax in db.Personals
                                            where PersonalMax.DoctSecuencia_fk == usu.doctSecuencia
                                            select (int?)PersonalMax.PersSecuencia).Max() ?? 0) + 1;
                            personal.PersSecuencia = proximoItem;
                            db.Personals.Add(personal);


                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Nuevo.ToString(), null);
                           

                        }
                        //Editando 
                        else
                        {
                            //si consigue el personal
                            var PersonalEditando = db.Personals.Where(ro => ro.DoctSecuencia_fk == usu.doctSecuencia
                                            && ro.PersSecuencia == usar_Personal.PersSecuencia
                                            && ro.TDSecuencia_fk == usar_Personal.TDSecuencia_fk
                                            && ro.PersDocumento == usar_Personal.PersDocumento).SingleOrDefault();

                            if (PersonalEditando != null)
                            {

                                PersonalEditando.PersNombre = usar_Personal.PersNombre;
                                PersonalEditando.PersApellido = usar_Personal.PersApellido;
                                PersonalEditando.PersFechaNacimiento = usar_Personal.PersFechaNacimiento;
                                PersonalEditando.PersDireccion = usar_Personal.PersDireccion;
                                PersonalEditando.PersTelefono = usar_Personal.PersTelefono;
                                PersonalEditando.PersCelular = usar_Personal.PersCelular;
                                PersonalEditando.PersGenero = usar_Personal.PersGenero;
                                PersonalEditando.UsuaModificacion = usu.usuario.UsuaSecuencia;
                                PersonalEditando.PersFechaModificacion = Lib.GetLocalDateTime();
                                PersonalEditando.CPersSecuencia_FK = usar_Personal.CPersSecuencia_FK;
                                

                             
                            }

                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Editar.ToString(), null);

                        }

                        db.SaveChanges();
                        dbtrans.Commit();

                        respuesta.respuesta = true;
                        //Reenvio a la vista principal
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
        public JsonResult Borrar(Usar_Personal usar_Personal)
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
                if (usar_Personal.PersSecuencia < 1 || usar_Personal.PersSecuencia == null)
                {
                    respuesta.respuesta = false;
                    respuesta.error = "Favor seleccionar una  Personal en el listado";
                    respuesta.redirect = Url.Action(vista, controler);
                    return Json(respuesta);

                }
                using (var dbtrans = db.Database.BeginTransaction())
                {
                    try
                    {
                        //borro registro
                        if (usar_Personal != null)
                        {

                            //Personal Personal = new Personal();
                            ////asignar contenido a otro objeto
                            //CopyClass.CopyObject(usar_Personal, ref Personal);


                            Personal borrar = (from bPersonal in db.Personals
                                               where bPersonal.DoctSecuencia_fk == usu.doctSecuencia
                                                                  && bPersonal.PersSecuencia == usar_Personal.PersSecuencia
                                                                  && bPersonal.TDSecuencia_fk == usar_Personal.TDSecuencia_fk
                                                                  && bPersonal.PersDocumento == usar_Personal.PersDocumento
                                               select bPersonal).SingleOrDefault();

                            if (borrar != null)
                            {
                                //si el personal ya tiene asignado un usuario, no puede borrarse.
                              //  if (!string.IsNullOrEmpty(borrar.UsuaSecuencia.ToString()) )
                                    if (borrar.UsuaSecuencia != null || borrar.UsuaSecuencia== 0)
                                {
                                    respuesta.respuesta = false;
                                    respuesta.error = "No puede borrar este personal, por que tiene un usuario asignado.  Codigo Usuario: " + borrar.UsuaSecuencia.ToString();
                                    respuesta.redirect = Url.Action(vista, controler);
                                    return Json(respuesta);

                                }

                               borrar.EstaDesabilitado = true;

                                //Borro el personal
                               // db.Personals.Remove(borrar);

                      

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
                        respuesta.error = "Posiblemente usted tiene otro registro que depende de éste o No Existe este Formulario " + ex.Message.ToString();
                        //respuesta.redirect = Url.Action("Ini_TipoFormulario", "TipoFormulario");
                        respuesta.redirect = Url.Action(vista, controler);
                        new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Error.ToString(), ex.GetBaseException().Message);
                        return Json(respuesta);
                    }
                }
            }
        }//End Borrar Metodo

        public ActionResult Personallista()
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

                List<Personal> PersonalList = (from bPersonal in db.Personals
                                               where bPersonal.DoctSecuencia_fk == usu.doctSecuencia
                                                        && bPersonal.EstaDesabilitado == false
                                               select bPersonal).ToList();
                var listUsar_Personal = new List<Usar_Personal>();

                //crear objeto tipo formulario y asignar a la lista de usartipoformulario
                foreach (var item in PersonalList)
                {
                    Usar_Personal perso = new Usar_Personal();
                    //asignar contenido a otro objeto
                    CopyClass.CopyObject(item, ref perso);
                    listUsar_Personal.Add(perso);
                }
                ViewBag.datasource = listUsar_Personal;
                return View();
            }
        }
    }
}
