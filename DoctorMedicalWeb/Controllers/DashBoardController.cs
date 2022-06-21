using DoctorMedicalWeb.App_Data;
using DoctorMedicalWeb.ModelsComplementarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoctorMedicalWeb.Libreria;
using DoctorMedicalWeb.Models;

namespace DoctorMedicalWeb.Controllers
{
    public class DashBoardController : Controller
    {
        DoctorMedicalWebEntities db = new DoctorMedicalWebEntities();
        ResponseModel respuesta = new ResponseModel();
        //
        // GET: /DashBoard/
        //[Authorize]
        public ActionResult Index()
        {

            //para que el div de accion solo aparezca si son 
            //vistas difrentes a home
            ViewBag.isHome = true;
            UsuarioLoguiado u = (UsuarioLoguiado)HttpContext.Session["user"];
            //Si NO esta loguieado lo redireccionara al loguin
            if (HttpContext.Session["user"] == null)
            {
                return RedirectToAction("Index", "PaginaPresentacion");
            }

            //si no tiene  el listado formulario devuelvo al loguin
            List<Formulario> _sessformularioParaDesabilitar = new List<Formulario>();
            _sessformularioParaDesabilitar = (List<App_Data.Formulario>)Session["sessformularioParaDesabilitar"];

            if (_sessformularioParaDesabilitar.Count == 0)
            {
                return RedirectToAction("Index", "DashBoard");
            }
            ViewBag.VBformularioParaDesabilitar = _sessformularioParaDesabilitar;




            //si no tiene este usuario formulario asignados devuelvo al loguin
            List<vw_ListDeFormuriosbyRolyUser> formulariosPlanRoleUser = new List<vw_ListDeFormuriosbyRolyUser>();
           formulariosPlanRoleUser=   (List<App_Data.vw_ListDeFormuriosbyRolyUser>)Session["FormulariosPermitidos"];

           if (formulariosPlanRoleUser==null  || formulariosPlanRoleUser.Count == 0)
           {
               HttpContext.Session["user"] = null;
               return RedirectToAction("Index", "PaginaPresentacion");
           }
        ViewBag.ListaFormulario = formulariosPlanRoleUser;
          
           
            //ViewBag.NombreCompleto = u.usuario.UsuaNombre + " " + u.usuario.UsuaApellido; 
      
            return View();
        }


        /// <summary>
        /// Verifico que la fecha y hora del cliente este igual que la del servidor
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult IsRightDate(DateTime dateTimeclient)
        {
            var respuesta = new ResponseModel();
            Dictionary<string, object> dictionaryStringObjec = new Dictionary<string, object>();
            if (HttpContext.Session["user"] == null)
            {
                respuesta.respuesta = true;
                respuesta.redirect = Url.Content("~/PaginaPresentacion/Index");
                respuesta.error = "Esta desloguiado ";
                return Json(respuesta);
            }
          

            //Validar que la fecha del cliente sea igual a la del servidor
            respuesta = Lib.IsRightDate(dateTimeclient);
            //si fecha esta diferente
            if ((bool)respuesta.dictionaryStringObjec["isDateDiference"] == true)
            {

                respuesta.redirect = Url.Action("cerrarSesion", "PaginaPresentacion");//Url.Content("~/PaginaPresentacion/Index");
                return Json(respuesta);
            }
            //si tiempo esta fuera del rango
            if ((bool)respuesta.dictionaryStringObjec["isTimeIntoRange"] == false)
            {
                respuesta.redirect = @Url.Action("cerrarSesion", "PaginaPresentacion");//Url.Content("~/PaginaPresentacion/Index");
                return Json(respuesta);
            }

            
            return Json(respuesta);
        }


        [HttpPost]
        //public PartialViewResult reportRect()
        public PartialViewResult reportRect(int paciSecuencia, int CMHistSecuencia, int ReceSecuencia)
        {
            
            //Si NO esta loguieado lo redireccionara al loguin
            if (HttpContext.Session["user"] == null)
            {
                return null;
            }
            //lleno el combo de roles
            UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];
            using (var db = new DoctorMedicalWebEntities())
            {

                //buscar los  registros  de roles del doctor loguiado
                vw_receta recet = (from tf in db.vw_receta
                                   where
                                   tf.ClinSecuencia_fk == usu.Consultorio.clinSecuencia_fk &&
                                    tf.DoctSecuencia_fk == usu.doctSecuencia &&
                                    tf.ConsSecuencia == usu.Consultorio.ConsSecuencia_fk &&
                                    tf.PaciSecuencia_fk == paciSecuencia &&
                                    tf.CMHistSecuencia_fk == CMHistSecuencia &&
                                    tf.ReceSecuencia == ReceSecuencia
                                   select tf).SingleOrDefault();

                Usar_vw_receta usar_vw_receta = new Usar_vw_receta();

                CopyClass.CopyObject(recet, ref usar_vw_receta);

                var a = PartialView(@"~/Views/Shared/ReportViewer.cshtml", usar_vw_receta);

                return a;
            }
        }

      
 [HttpPost]
        public JsonResult ShowreportRect(int paciSecuencia, int CMHistSecuencia, int ReceSecuencia)
        {
            ResponseModel respuesta = new ResponseModel();
             //Si NO esta loguieado lo redireccionara al loguin
            if (HttpContext.Session["user"] == null)
            {
                return null;
            }
             //lleno el combo de roles
            UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];
                using (var db = new DoctorMedicalWebEntities())
            {
                
                //buscar los  registros  de roles del doctor loguiado
                vw_receta recet = (from tf in db.vw_receta
                                where 
                                tf.ClinSecuencia_fk == usu.Consultorio.clinSecuencia_fk &&
                                 tf.DoctSecuencia_fk == usu.doctSecuencia &&
                                 tf.ConsSecuencia == usu.Consultorio.ConsSecuencia_fk   &&
                                 tf.PaciSecuencia_fk==paciSecuencia &&
                                 tf.CMHistSecuencia_fk== CMHistSecuencia &&
                                 tf.ReceSecuencia == ReceSecuencia
                                select tf).SingleOrDefault();

                respuesta.obj = recet;
                    return Json(respuesta);
              
                }
        

           

        
        }

        [HttpPost]
 public JsonResult GetTimeActual()
 {

     ResponseModel respon = new ResponseModel();
     respon.obj = Lib.GetLocalDateTime().ToString() ;
     return Json(respon);
 }


    }
}
