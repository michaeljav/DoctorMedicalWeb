using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoctorMedicalWeb.LoginView;
using Newtonsoft.Json;
using System.Web.Helpers;
using System.Net;
using DoctorMedicalWeb.Models;
using System.Transactions;
using DoctorMedicalWeb.App_Data;
using System.Data.Entity;
using DoctorMedicalWeb.ModelsComplementarios;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Threading.Tasks;
using System.Web.SessionState;
using System.Web.UI;
using System.Threading;
using DoctorMedicalWeb.Libreria;
using System.Web.Services;






namespace DoctorMedicalWeb.Controllers
{
    //[SessionState(SessionStateBehavior.Default)]   

    public class PaginaPresentacionController : Controller
    {
        DoctorMedicalWebEntities db = new DoctorMedicalWebEntities();
        DbContextTransaction dbtrans = null;
        //guardar auditoria
        Libreria.Lib lib = new Libreria.Lib();


        //
        // GET: /PaginaPresentacion/
        public ActionResult Index()
        {

            //si esta logueado  osea ya 
            //selecciono un consultorio
            // o el doctor solo tenia un consultorio
            //y se lleno la variable sesion
            //por ende  redireccione al dash board
            //La Session["user"]  solo se llena  por esta pagina
            //o la pagina PaginaPresntacion
            if (Session["user"] != null)
            {
                return RedirectToAction("Index", "DashBoard");
            }

            IEnumerable<Especialidade> especialida = db.Especialidades.ToList();

            ViewBag.Especialidad = new SelectList(especialida, "EspeSecuencia", "EspeNombre");

            IEnumerable<Plane> plane = db.Planes.ToList();

            ViewBag.PlanesList = new SelectList(plane, "PlanSecuencia", "PlanDescripcion");

            IEnumerable<Pai> pais = db.Pais.ToList();

            ViewBag.PaisList = new SelectList(pais, "PaisSecuencia", "PaisNombre");

            //crear objetos de  Mes dropbox
            //var listaMes = new List<ListItem>();
            var listaMes = new List<ListItem> {   
          
        
          new System.Web.UI.WebControls.ListItem { Text = "Enero",  Value="01"},
          new System.Web.UI.WebControls.ListItem { Text = "Febrero", Value="02"},
          new System.Web.UI.WebControls.ListItem { Text = "Marzo",  Value="03"},
          new System.Web.UI.WebControls.ListItem { Text = "Abril",  Value="04"},
          new System.Web.UI.WebControls.ListItem { Text = "Mayo",   Value="05"},
          new System.Web.UI.WebControls.ListItem { Text = "Junio",  Value="06"},
          new System.Web.UI.WebControls.ListItem { Text = "Julio",     Value="07"},
          new System.Web.UI.WebControls.ListItem { Text = "Agosto",     Value="08" },
          new System.Web.UI.WebControls.ListItem { Text = "Septiembre", Value="09"},
          new System.Web.UI.WebControls.ListItem { Text = "Octubre",    Value="10" },
          new System.Web.UI.WebControls.ListItem { Text = "Noviembre",  Value="11" },
          new System.Web.UI.WebControls.ListItem { Text = "Diciembre",  Value="12" } 
            
            };
            // for (int i = 1; i <= 12; i++)
            //{
            //    listaMes.Add(new System.Web.UI.WebControls.ListItem { Text = System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(i).ToString(), Value = i.ToString() });
            //}

            ViewBag.lMes = listaMes;

            //crear objetos de  dia dropbox
            var listaDia = new List<ListItem>();

            for (int i = 1; i <= 31; i++)
            {
                string num = "";
                if (i < 10)
                {
                    num = "0" + i.ToString();
                }
                else
                {
                    num = i.ToString();
                }
                listaDia.Add(new System.Web.UI.WebControls.ListItem { Text = num, Value = num });
            }
            ViewBag.lDia = listaDia;

            //crear objetos de  anios dropbox
            var listaAnios = new List<ListItem>();

            for (int i = Lib.GetLocalDateTime().Year; i >= 1900; i--)
            {
                listaAnios.Add(new System.Web.UI.WebControls.ListItem { Text = i.ToString(), Value = i.ToString() });
            }
            ViewBag.lAnio = listaAnios;

            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginRegisterView LoginRegister)
        {


            var respuesta = new ResponseModel();
            try
            {
                //cada vez que vuelva a entrar los formularios seran borrados
                //para que lo entren nuevamente.
                if (Session["FormulariosPermitidos"] != null)
                { Session["FormulariosPermitidos"] = null; }

                //limpiar session de lista de todos los formularios para borrar

                if (Session["sessformularioParaDesabilitar"] != null)
                { Session["sessformularioParaDesabilitar"] = null; }

                //limpia session de  el usuario si tiene mas de un consultorio
                //
                if (Session["UsuarioSeleComsul"] != null)
                { Session["UsuarioSeleComsul"] = null; }

                //Usuario
                if (Session["user"] != null)
                { Session["user"] = null; }


                //ModelState.Clear();


                //si no esta validado los datos  no se logueara
                if (!ModelState.IsValid)
                {
                    return View(LoginRegister);
                   
                }

                var user = db.Usuarios.Where(u => u.UsuaEmail == LoginRegister.Usar_login.Email && u.UsuaClave.ToString().Equals(LoginRegister.Usar_login.Clave)).FirstOrDefault();

                //si existe este usuario, pero no tiene  numero ni mayusculas  ni minusculas, quiere decir
                //que apesar de que es la misma constrasenia, pero no tiene las exigencias para  escribirla
                if (user != null)
                {

                    if (!LoginRegister.Usar_login.Clave.Any(char.IsUpper) ||
                        !LoginRegister.Usar_login.Clave.Any(char.IsLower) ||
                        !LoginRegister.Usar_login.Clave.Any(char.IsDigit))
                    {
                        user = null;
                    }
                    //SI TIENE UPDATE EL EMAIL NO  SE LOGUIEA
                    if (LoginRegister.Usar_login.Email.Any(char.IsUpper))
                    {
                        user = null;
                    }

                }

                if (user != null)
                {



                    //este sera el objeto  loguiado
                    UsuarioLoguiado usuarioLoguiado = new UsuarioLoguiado();

                    //llenar  el personal o  docotr loguiado
                    Doctor doctor = null;
                    Personal personalObj = null;
                    //verifico si es un doctor o personal
                    doctor = db.Doctors.Where(d => d.UsuSecuencia == user.UsuaSecuencia).FirstOrDefault();

                    if (doctor != null)
                    {
                        usuarioLoguiado.doctSecuencia = doctor.DoctSecuencia;
                        usuarioLoguiado.NombreCompleto = user.UsuaNombre + " " + user.UsuaApellido;
                        usuarioLoguiado.imagPath = doctor.DoctFotoPath;
                    }
                    
                    //si es igual a null entonce busco el personal 
                    if (doctor == null)
                    {
                        personalObj = db.Personals.Where(p => p.UsuaSecuencia == user.UsuaSecuencia).FirstOrDefault();
                        if (personalObj != null)
                        {
                            usuarioLoguiado.doctSecuencia = personalObj.DoctSecuencia_fk;
                            usuarioLoguiado.persSecuencia = personalObj.PersSecuencia;
                            usuarioLoguiado.NombreCompleto = user.UsuaNombre + " " + user.UsuaApellido;                       
                        }
                    }




                    //si tiene mas de un consultorio paso
                    //a otra pantalla para que seleccione
                    //si tiene solo uno entro al sistema.
                    //esto es con los usuarios 
                    List<vw_UsuarioConsultorios> usuarioConsultorios = ((from cantConsultorio in db.vw_UsuarioConsultorios
                                                                         where cantConsultorio.UsuaSecuencia == user.UsuaSecuencia
                                                                               && cantConsultorio.EstaDesabilitado == false
                                                                         select cantConsultorio).ToList());





                    if (usuarioConsultorios != null)
                    {
                        if (usuarioConsultorios.Count > 1)
                        {

                            //variable para  poder buscar los consultorios de este usuario
                            Session["UsuarioSeleComsul"] = usuarioConsultorios[0];
                            respuesta.respuesta = true;
                            respuesta.redirect = Url.Content("~/SeleccioneConsultorio/Ini_SeleccioneConsultorio");
                            respuesta.error = "";
                            return Json(respuesta);
                        }
                    }
                    else//si es null
                    {

                        respuesta.respuesta = false;
                        respuesta.redirect = Url.Content("~/PaginaPresentacion/Index");
                        respuesta.error = "Cree Consultorios para este usuario";
                        return Json(respuesta);
                    }

                    //si no tiene consultorio
                    if (usuarioConsultorios.Count == 0)
                    {
                        respuesta.respuesta = false;
                        respuesta.redirect = Url.Content("~/PaginaPresentacion/Index");
                        respuesta.error = "Usted Tiene usuario pero  No tiene Consultorio Creado";
                        return Json(respuesta);

                    }



                    //sige corriendo si solo tiene un consultorio
                    //es decir no tiene mas de un solo consultorio
                    respuesta.respuesta = true;
                    respuesta.redirect = Url.Content("~/DashBoard/Index");
                    respuesta.error = "";
                    //usuario loguieado y consultorio seleccionado
                    usuarioLoguiado.usuario = user;
                    usuarioLoguiado.Consultorio = usuarioConsultorios[0];

                    Session["user"] = usuarioLoguiado;
                    //Auditoria Usuario Loguiado
                    lib.IsertarAuditoria(usuarioLoguiado.usuario.PaisSecuencia, usuarioLoguiado.Consultorio.clinSecuencia_fk, usuarioLoguiado.Consultorio.ConsSecuencia_fk,
                        usuarioLoguiado.doctSecuencia, usuarioLoguiado.persSecuencia, null, Lib.GetLocalDateTime(), null, null, usuarioLoguiado.usuario.UsuaSecuencia,
                        lib.GetIPAddress(), "Login", null, Accion.Iniciar_Sesion.ToString());


                    //busco todos los formularios para desabilitarlos
                    //en la vista _layout
                    List<Formulario> formParaDesabilitar = new List<Formulario>();
                    db.Configuration.ProxyCreationEnabled = false;
                    formParaDesabilitar = db.Formularios.ToList();
                    Session["sessformularioParaDesabilitar"] = formParaDesabilitar;


                    ////buscar formularios  acorde al plan del usuario
                    //List<vw_ListDeFormuriosbyRolyUser> formularios = new List<vw_ListDeFormuriosbyRolyUser>();
                    //db.Configuration.ProxyCreationEnabled = false;
                    //formularios = db.vw_ListDeFormuriosbyRolyUser.Where(f => f.UsuaSecuencia == user.UsuaSecuencia).ToList();

                    //buscar formularios  acorde al plan y rol del usuario

                    List<vw_ListDeFormuriosbyRolyUser> formularios = new List<vw_ListDeFormuriosbyRolyUser>();
                    db.Configuration.ProxyCreationEnabled = false;
                    formularios = db.vw_ListDeFormuriosbyRolyUser.Where(f => f.UsuaSecuencia == user.UsuaSecuencia
                                                                        //&& f.PlanSecuencia_fk == user.PlanSecuencia_fk // busco el rol ya que no busco tambien porp  plan
                                                                        && f.RoleSecuencia_fk == user.RoleSecuencia_fk).ToList();



                    if (formularios.Count == 0)
                    {

                        respuesta.respuesta = false;
                        respuesta.redirect = "/PaginaPresentacion/Index";
                        respuesta.error = "No existen formularios asignados a este plan,contacte con administrador.";
                        return Json(respuesta);
                    }

                    Session["FormulariosPermitidos"] = formularios;

                    return Json(respuesta, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    respuesta.respuesta = false;
                    respuesta.redirect = "/PaginaPresentacion/Index";
                    respuesta.error = "No existe este usuario.";
                    return Json(respuesta);

                }
            }
            catch (Exception ex)
            {

                respuesta.respuesta = false;
                respuesta.redirect = "/PaginaPresentacion/Index";
                respuesta.error = "No existe este usuario. " + ex.GetBaseException().ToString();
                return Json(respuesta);
            }

        }

        //[Authorize]
        public ActionResult About()
        {
            //var aa = (App_Data.Usuario)HttpContext.Session["user"];
            //var ase = Session["user"];


            //var a = System.Web.HttpContext.Current.Session["nuevo"];
            //Si NO esta loguieado lo redireccionara al loguin
            if (Session["user"] == null)
            {
                return RedirectToAction("Index", "PaginaPresentacion");
            }

            return View("About");
        }

        public ActionResult cerrarSesion()
        {
            if (Session["user"] != null)
            {
                //Session["user"] = null;
                //Session["UsuarioSeleComsul"] = null;
                //Session["sessformularioParaDesabilitar"] = null;
                //Session["FormulariosPermitidos"] = null;

                //var a = Session["user"];
                //a = Session["UsuarioSeleComsul"];
                //a = Session["sessformularioParaDesabilitar"];
                //a = Session["FormulariosPermitidos"];

                Session.Abandon();
                Session.Clear();
                Session.RemoveAll();
                System.Web.Security.FormsAuthentication.SignOut();


                // a= Session["user"] ;
                //a = Session["UsuarioSeleComsul"] ;
                //a = Session["sessformularioParaDesabilitar"] ;
                //a = Session["FormulariosPermitidos"] ;


                return RedirectToAction("Index", "PaginaPresentacion");
            }

            return View();
        }

        [HttpPost]
        public JsonResult CrearUsar_usuario(LoginRegisterView loginRegister)
        {

            var respuesta = new ResponseModel();

            //Esta linea de codigo es para no utilizar el crear usuario por ahora
            if (loginRegister != null || loginRegister == null)
            {
                respuesta.respuesta = false;
                respuesta.redirect = Url.Content("~/PaginaPresentacion/Index");
                respuesta.error = "Esto no es permitido ";
                return Json(respuesta);

            }
            //Esta linea de codigo es para no utilizar el crear usuario por ahora


            if (!ModelState.IsValid)
            {

                respuesta.respuesta = false;
                respuesta.redirect = Url.Content("~/PaginaPresentacion/Index");
                respuesta.error = "Llenar Campos requeridos ";
                var a = ModelState.IsValid;

                return Json(respuesta);
            }


            //concatenar  fecha de nacimiento en formato de mes-dia-año para guardarlo en la base de datos
            //aunque en la vista se muestra  como dia - mes y anio
            var fechanac = (

                  loginRegister.Usar_usuario.UsuaFechaNacimientoDia.ToString() + "-" +
                  loginRegister.Usar_usuario.UsuaFechaNacimientoMes.ToString() + "-" +
                  loginRegister.Usar_usuario.UsuaFechaNacimientoAnio.ToString());

            //+ "  00:00:00.00"

            #region para verificar si es una solitud de ajax

            ////We clear the response
            //Response.Clear();

            ////We check if we have an AJAX request and return JSON in this case
            //if (IsAjaxRequest())
            //{
            //    Response.Write("Your JSON here");
            //}
            //else
            //{
            //    //We don`t have an AJAX request, redirect to an error page
            //    Response.Redirect("Your error page URL here");
            //}

            ////We clear the error
            //Server.ClearError();
            #endregion



            try
            {
                if (loginRegister.Usar_usuario == null)
                {
                    respuesta.respuesta = false;
                    respuesta.redirect = Url.Content("~/PaginaPresentacion/Index");
                    respuesta.error = "El usuario objeto llego vacio, por favor contactar a los administradores ";
                    // ModelState.AddModelError("Validacion-Completa", respuesta.error);
                    return Json(respuesta);
                }
                //if (string.IsNullOrEmpty(loginRegister.Usar_usuario.UsuaNombre))
                //{
                //    ModelState.AddModelError("Validacion-Completa", respuesta.error);
                //}
                if (!ModelState.IsValid)
                {

                    respuesta.respuesta = false;
                    respuesta.redirect = Url.Content("~/PaginaPresentacion/Index");
                    respuesta.error = "Llenar Campos requeridos ";
                    var a = ModelState.IsValid;

                    return Json(respuesta);
                }

                //Buscar el usuario
                var user = ((from u in db.Usuarios
                             where u.UsuaEmail == loginRegister.Usar_usuario.UsuaEmail
                             select u).SingleOrDefault());

                if (user != null)
                {

                    respuesta.respuesta = false;
                    respuesta.redirect = Url.Content("~/PaginaPresentacion/Index");
                    respuesta.error = "Ya Existe este Correo";
                    //para que se pueda mostrar el mensjae de error debajo del textbox contrasenia
                    respuesta.errorAlconsultarelusuarioParaCrear = true;
                    return Json(respuesta);
                }
                //Crear Usuario                 

                Usuario usuario = new Usuario();

                usuario.UsuaEmail = loginRegister.Usar_usuario.UsuaEmail;
                usuario.UsuaClave = loginRegister.Usar_usuario.UsuaClave;
                usuario.UsuaNombre = loginRegister.Usar_usuario.UsuaNombre;
                usuario.UsuaApellido = loginRegister.Usar_usuario.UsuaApellido;
                usuario.UsuaFechaNacimiento = DateTime.ParseExact(fechanac.ToString(), "dd-MM-yyyy", null);
                usuario.UsuaGenero = loginRegister.Usar_usuario.UsuaGenero.Trim();
                usuario.RoleSecuencia_fk = 1;//siempre que se registra tendra el rol Doctor
                usuario.UsuaIntentos = 0;
                usuario.EstaDesabilitado = false;

                usuario.PlanSecuencia_fk = loginRegister.Usar_usuario.PlanSecuencia_fk;
                usuario.EspeSecuencia_fk = loginRegister.Usar_usuario.EspeSecuencia_fk;
                usuario.UsuaFechaCreacion = Lib.GetLocalDateTime();
                usuario.PaisSecuencia = loginRegister.Usar_usuario.PaisSecuencia;

                //creo en la tabla del doctor el usuario
                Doctor doctor = new Doctor();
                doctor.DoctNombre = usuario.UsuaNombre;
                doctor.DoctApellido = usuario.UsuaApellido;
                doctor.EspeSecuencia = (int)usuario.EspeSecuencia_fk;



                using (dbtrans = db.Database.BeginTransaction())
                {
                    //buscando la proxima secuencia
                    int proximoItem = (db.Usuarios.Select(x => (int?)x.UsuaSecuencia).Max() ?? 0) + 1;
                    usuario.UsuaSecuencia = proximoItem;
                    usuario.UsuaSecuenciaCreacion = proximoItem;
                    usuario.UsuaSecuenciaModificacion = proximoItem;

                    db.Usuarios.Add(usuario);
                    db.SaveChanges();

                    //insertando doctor
                    //buscando la proxima secuencia
                    int proximoItemDoct = (from doc in db.Doctors
                                           select doc.DoctSecuencia).Count() + 1;
                    doctor.DoctSecuencia = proximoItemDoct;
                    doctor.UsuSecuencia = usuario.UsuaSecuencia;
                    doctor.DoctGenero = usuario.UsuaGenero;
                    doctor.DoctFechaNacimiento = usuario.UsuaFechaNacimiento;
                    doctor.DoctFechaCreacion = usuario.UsuaFechaCreacion;
                    doctor.PaisSecuencia = usuario.PaisSecuencia;

                    db.Doctors.Add(doctor);
                    db.SaveChanges();
                    dbtrans.Commit();
                }

                //Buscar el usuario insertado
                Usuario usuarioNuevo = (from usu in db.Usuarios
                                        where usu.UsuaSecuencia == usuario.UsuaSecuencia
                                        select usu).SingleOrDefault();

                if (usuarioNuevo == null)
                {

                    respuesta.respuesta = false;
                    respuesta.redirect = Url.Content("~/PaginaPresentacion/Index");
                    respuesta.error = "Error al  buscar el nuevo usuario registrado";
                    //para que se pueda mostrar el mensjae de error debajo del textbox contrasenia
                    respuesta.errorAlconsultarelusuarioParaCrear = true;
                    return Json(respuesta);
                }

                //asignar usuario loguieado
                Session["user"] = usuarioNuevo;

                respuesta.respuesta = true;
                //respuesta.redirect = "/DashBoard/Index";
                //respuesta.redirect = HttpUtility.UrlEncode("~/DashBoard/Index");

                respuesta.redirect = Url.Content("~/DashBoard/Index");



                respuesta.error = "Guardado Correctamente";
                //Usar_Auditoria a = new Usar_Auditoria();                     
                //a.PaisSecuencia = 1;
                //a.ClinSecuencia = 1;
                //a.ConsSecuencia = 1;
                //a.DoctSecuencia = 1;
                //a.AudiFechaMaquina = Lib.GetLocalDateTime();
                //a.AudiFechaServidor = Lib.GetLocalDateTime();
                //a.UsuaCodigo = 1;
                //a.AudiIpMaquina = "1";
                //a.PagiSecuencia = 1;
                //a.Accisecuencia = 1;
                //lib.IsertarAuditoria(a);
                //   ModelState.Clear();
                //return Json(new { Urll = Url.Action("Index", "DashBoard") });
                //return  Json(new { Success = true, Message = "OK" });           
                //return Json(new { Success = false, Message = "Ya Existe este Correo" });



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
                respuesta.redirect = Url.Content("~/PaginaPresentacion/Index");
                respuesta.error = "Ocurrio Un inconveniente, Favor volver a tratar. " + ex.Message + "---" + ex.GetBaseException().ToString();
                return Json(respuesta);
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

        //This method checks if we have an AJAX request or not
        private bool IsAjaxRequest()
        {
            //The easy way
            bool isAjaxRequest = (Request["X-Requested-With"] == "XMLHttpRequest")
            || ((Request.Headers != null)
            && (Request.Headers["X-Requested-With"] == "XMLHttpRequest"));

            //If we are not sure that we have an AJAX request or that we have to return JSON 
            //we fall back to Reflection
            if (!isAjaxRequest)
            {
                try
                {
                    //The controller and action
                    string controllerName = Request.RequestContext.
                                            RouteData.Values["controller"].ToString();
                    string actionName = Request.RequestContext.
                                        RouteData.Values["action"].ToString();

                    //We create a controller instance
                    DefaultControllerFactory controllerFactory = new DefaultControllerFactory();
                    Controller controller = controllerFactory.CreateController(
                    Request.RequestContext, controllerName) as Controller;

                    //We get the controller actions
                    ReflectedControllerDescriptor controllerDescriptor =
                    new ReflectedControllerDescriptor(controller.GetType());
                    ActionDescriptor[] controllerActions =
                    controllerDescriptor.GetCanonicalActions();

                    //We search for our action
                    foreach (ReflectedActionDescriptor actionDescriptor in controllerActions)
                    {
                        if (actionDescriptor.ActionName.ToUpper().Equals(actionName.ToUpper()))
                        {
                            //If the action returns JsonResult then we have an AJAX request
                            if (actionDescriptor.MethodInfo.ReturnType
                            .Equals(typeof(JsonResult)))
                                return true;
                        }
                    }
                }
                catch
                {

                }
            }

            return isAjaxRequest;
        }


        [HttpPost]
        public JsonResult LogoutCheck()
        {
            var respuesta = new ResponseModel();
            if (HttpContext.Session["user"] == null)
            {
                respuesta.respuesta = true;
                respuesta.redirect = Url.Content("~/PaginaPresentacion/Index");
                respuesta.error = "Esta desloguiado ";
                return Json(respuesta);
            }

            return Json(respuesta);
        }


      


    }
}
