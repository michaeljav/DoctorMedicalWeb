using DoctorMedicalWeb.App_Data;
using DoctorMedicalWeb.Libreria;
using DoctorMedicalWeb.LoginView;
using DoctorMedicalWeb.Models;
using DoctorMedicalWeb.ModelsComplementarios;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace DoctorMedicalWeb.Controllers
{
    public class RegistroSistemaController : Controller
    {
        //
        // GET: /RegistroSistema/

        public ActionResult Ini_RegistrosSistema()
        {
            using (var db = new DoctorMedicalWebEntities())
            {
                IEnumerable<Pai> pais = db.Pais.ToList().OrderBy(x => x.PaisNombre);

                ViewBag.PaisList = new SelectList(pais, "PaisSecuencia", "PaisNombre");

                IEnumerable<Especialidade> especialida = db.Especialidades.ToList().Where(x => x.EstaDesabilitado == false);

                ViewBag.Especialidad = new SelectList(especialida, "EspeSecuencia", "EspeNombre");

                IEnumerable<Plane> plane = db.Planes.ToList().Where( x => x.PlanSecuencia > 1);

                ViewBag.PlanesList = new SelectList(plane, "PlanSecuencia", "PlanDescripcion");

              
                IEnumerable<Plane> clinica = db.Planes.ToList().Where(x => x.PlanSecuencia > 1);

                ViewBag.PlanesList = new SelectList(plane, "PlanSecuencia", "PlanDescripcion");


                IEnumerable<Plane> zHoraria = db.Planes.ToList().Where(x => x.PlanSecuencia > 1);

                ViewBag.ZonaHoraria = new SelectList(plane, "PlanSecuencia", "PlanDescripcion");


                //crear objetos de  Mes dropbox
                //var listaMes = new List<ListItem>();
                var listaMes = new List<ListItem> {            
        
                  new ListItem { Text = "Enero",  Value="01"},
                  new ListItem { Text = "Febrero", Value="02"},
                  new ListItem { Text = "Marzo",  Value="03"},
                  new ListItem { Text = "Abril",  Value="04"},
                  new ListItem { Text = "Mayo",   Value="05"},
                  new ListItem { Text = "Junio",  Value="06"},
                  new ListItem { Text = "Julio",     Value="07"},
                  new ListItem { Text = "Agosto",     Value="08" },
                  new ListItem { Text = "Septiembre", Value="09"},
                  new ListItem { Text = "Octubre",    Value="10" },
                  new ListItem { Text = "Noviembre",  Value="11" },
                  new ListItem { Text = "Diciembre",  Value="12" } 
            
            };
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

        }
        [HttpPost]
        public JsonResult CrearUsar_usuario(Usar_Usuario usar_usuario)
        {

            DoctorMedicalWebEntities db = new DoctorMedicalWebEntities();
            DbContextTransaction dbtrans = null;
            //guardar auditoria
            Libreria.Lib lib = new Libreria.Lib();

            var respuesta = new ResponseModel();

            //Esta linea de codigo es para no utilizar el crear usuario por ahora
            if (usar_usuario != null || usar_usuario == null)
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


            ////concatenar  fecha de nacimiento en formato de mes-dia-año para guardarlo en la base de datos
            ////aunque en la vista se muestra  como dia - mes y anio
            //var fechanac = (

            //      usar_usuario.UsuaFechaNacimientoDia.ToString() + "-" +
            //      usar_usuario.UsuaFechaNacimientoMes.ToString() + "-" +
            //      usar_usuario.UsuaFechaNacimientoAnio.ToString());

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
                if (usar_usuario == null)
                {
                    respuesta.respuesta = false;
                    respuesta.redirect = Url.Content("~/PaginaPresentacion/Index");
                    respuesta.error = "El usuario objeto llego vacio, por favor contactar a los administradores ";
                    // ModelState.AddModelError("Validacion-Completa", respuesta.error);
                    return Json(respuesta);
                }
                //if (string.IsNullOrEmpty(loginRegister.UsuaNombre))
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
                             where u.UsuaEmail == usar_usuario.UsuaEmail
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

                usuario.UsuaEmail = usar_usuario.UsuaEmail;
                usuario.UsuaClave = usar_usuario.UsuaClave;
                usuario.UsuaNombre = usar_usuario.UsuaNombre;
                usuario.UsuaApellido = usar_usuario.UsuaApellido;
                //usuario.UsuaFechaNacimiento = DateTime.ParseExact(fechanac.ToString(), "dd-MM-yyyy", null);
                usuario.UsuaFechaNacimiento = usar_usuario.UsuaFechaNacimiento;
                usuario.UsuaGenero = usar_usuario.UsuaGenero.Trim();
                usuario.RoleSecuencia_fk = 1;//siempre que se registra tendra el rol Doctor
                usuario.UsuaIntentos = 0;
                usuario.EstaDesabilitado = false;

                usuario.PlanSecuencia_fk = usar_usuario.PlanSecuencia_fk;
                usuario.EspeSecuencia_fk = usar_usuario.EspeSecuencia_fk;
                usuario.UsuaFechaCreacion = Lib.GetLocalDateTime();
                usuario.PaisSecuencia = usar_usuario.PaisSecuencia;

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

    }
}
