using DoctorMedicalWeb.App_Data;
using DoctorMedicalWeb.Libreria;
using DoctorMedicalWeb.Models;
using DoctorMedicalWeb.ModelsComplementarios;
using Syncfusion.JavaScript.Shared.Serializer;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoctorMedicalWeb.Controllers
{
    public class TipoFormularioController : Controller
    {
        DoctorMedicalWebEntities db = new DoctorMedicalWebEntities();
        DbContextTransaction dbtrans = null;
        //
        // GET: /TipoFormulario/

        public ActionResult Ini_TipoFormulario()
        {


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


            //validar que si no tiene permiso a este formulario no entre
            List<vw_ListDeFormuriosbyRolyUser> formulariosPlanRoleUser = new List<vw_ListDeFormuriosbyRolyUser>();
            formulariosPlanRoleUser = (List<App_Data.vw_ListDeFormuriosbyRolyUser>)Session["FormulariosPermitidos"];
            //traera false o true;
            var EstaEsteFormulario = formulariosPlanRoleUser.Where(f => f.FormDescripcion == "Ini_TipoFormulario").Any();
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


            Usar_TipoFormulario usartipoformulario = new Usar_TipoFormulario();
            //si  esta lleno es por que esta editando
            if (Session["Usar_TipoFormulario"] != null)
            {
                usartipoformulario = (Usar_TipoFormulario)Session["Usar_TipoFormulario"];
                //limpiar sesion
                Session["Usar_TipoFormulario"] = null;
                return View(usartipoformulario);
            }
            return View(usartipoformulario);
        }

        [HttpPost]
        public JsonResult Borrar(Usar_TipoFormulario usar_TipoFormulario)
        {
            var respuesta = new ResponseModel();
            //si secuenca esta vacio devolver mensaje
            if (usar_TipoFormulario.TFormSecuencia < 1 || usar_TipoFormulario.TFormSecuencia == null)
            {
                respuesta.respuesta = false;
                respuesta.error = "Favor seleccionar un tipo formulario en el listado";
                respuesta.redirect = Url.Action("Ini_TipoFormulario", "TipoFormulario");
                return Json(respuesta);

            }

            try
            {
                //borro registro
                if (usar_TipoFormulario != null)
                {

                    TipoFormulario tf = new TipoFormulario();
                    //asignar contenido a otro objeto
                    CopyClass.CopyObject(usar_TipoFormulario, ref tf);

                    var borrar = (from tipof in db.TipoFormularios
                                  where tipof.TFormSecuencia == usar_TipoFormulario.TFormSecuencia
                                  select tipof).SingleOrDefault();

                    if (borrar != null)
                    {
                        db.TipoFormularios.Remove(borrar);
                        db.SaveChanges();
                        respuesta.someCollection = UltimosCincoRegistros().someCollection;
                        respuesta.respuesta = true;
                        respuesta.redirect = Url.Action("Ini_TipoFormulario", "TipoFormulario");
                        new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), Paginas.Ini_TipoFormulario.ToString(), Accion.Borrar.ToString(), null);
                    }
                    else
                    {

                        respuesta.someCollection = UltimosCincoRegistros().someCollection;
                        respuesta.respuesta = false;
                        respuesta.error = "No Existe este Formulario";
                        respuesta.redirect = Url.Action("Ini_TipoFormulario", "TipoFormulario");
                    }
                }

                return Json(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.someCollection = UltimosCincoRegistros().someCollection;
                respuesta.respuesta = false;
                respuesta.error = "Posiblemente usted tiene otro registro que depende de éste o No Existe este Formulario";
                respuesta.redirect = Url.Action("Ini_TipoFormulario", "TipoFormulario");
                new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), Paginas.Ini_TipoFormulario.ToString(), Accion.Error.ToString(), ex.GetBaseException().Message);
                return Json(respuesta);
            }
        }

        [HttpPost]
        public JsonResult Editar(Usar_TipoFormulario usar_TipoFormulario)
        {


            var respuesta = new ResponseModel();


            if (usar_TipoFormulario != null)
            {
                TipoFormulario tipof = db.TipoFormularios.Where(t => t.TFormSecuencia == usar_TipoFormulario.TFormSecuencia).FirstOrDefault();

                if (tipof != null)
                {
                    Usar_TipoFormulario usartipoformulario = new Usar_TipoFormulario();
                    //asignar contenido a otro objeto
                    CopyClass.CopyObject(tipof, ref usartipoformulario);

                    Session["Usar_TipoFormulario"] = usartipoformulario;
                    //existe este formulario
                    respuesta.respuesta = true;
                    //respuesta.redirect = Url.Action("Index", "TipoFormulario", new { usar = 1 });
                    respuesta.redirect = Url.Action("Ini_TipoFormulario", "TipoFormulario");
                    //respuesta.redirect = Url.Action("Index", "TipoFormulario", new { usar_TipoFormulario = tipof.Formularios});
                    //respuesta.redirect =   Url.Content("~/TipoFormulario/Index");
                    //return  RedirectToAction("Index", "TipoFormulario", new { Usar_TipoFormulario = tipof });
                    //return RedirectToAction("Index", "TipoFormulario");

                }

            }
            else
            {
                //No existe este formulario
                respuesta.respuesta = false;
                respuesta.error = "Este formulario no existe";

            }
            return Json(respuesta);
        }
        
        [HttpPost]
        public JsonResult Save(Usar_TipoFormulario usar_TipoFormulario)
        {
            var respuesta = new ResponseModel();
            int proximoItem = 0;


            //si no ha validado volver y mostrar mensajes de validacion

            if (!ModelState.IsValid)
            {
                respuesta.respuesta = false;
                respuesta.redirect = Url.Content("~/TipoFormulario/Ini_TipoFormulario");
                respuesta.error = "Llenar Campos requeridos ";
                return Json(respuesta);
            }

            
            try
            {

                TipoFormulario tipoformulario = new TipoFormulario();
                //asignar contenido a otro objeto
                CopyClass.CopyObject(usar_TipoFormulario, ref tipoformulario);


                using (dbtrans = db.Database.BeginTransaction())
                {
                    //add new 
          
                        if (string.IsNullOrEmpty(tipoformulario.TFormSecuencia.ToString()) || tipoformulario.TFormSecuencia == 0)
                    {

                        //si existe el tipo formulario no se introduce
                        var existeTipof = (from tipof in db.TipoFormularios
                                           where tipof.TFormDescripcion == usar_TipoFormulario.TFormDescripcion
                                           select tipof).SingleOrDefault();
                        if (existeTipof != null)
                        {
                            respuesta.respuesta = false;
                            respuesta.error = "Ya existe este formulario";
                            respuesta.redirect = Url.Action("Ini_TipoFormulario", "TipoFormulario");
                            return Json(respuesta);

                        }


                        //buscando la proxima secuencia
                        proximoItem = (db.TipoFormularios.Select(x => (int?)x.TFormSecuencia).Max() ?? 0) + 1;

                        tipoformulario.TFormSecuencia = proximoItem;

                        db.TipoFormularios.Add(tipoformulario);
                        new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), Paginas.Ini_TipoFormulario.ToString(), Accion.Nuevo.ToString(), null);
                    }
                    //Editando 
                    else
                    {

                        var tipoForm = db.TipoFormularios.Where(tf => tf.TFormSecuencia == tipoformulario.TFormSecuencia).SingleOrDefault();
                        tipoForm.TFormDescripcion = tipoformulario.TFormDescripcion;
                    }

                    db.SaveChanges();
                    dbtrans.Commit();


                    respuesta.respuesta = true;
                    respuesta.redirect = Url.Content("~/TipoFormulario/Ini_TipoFormulario");
                    new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), Paginas.Ini_TipoFormulario.ToString(), Accion.Editar.ToString(),null);
                }

                //Enlistar los 5 ultimos
                //respuesta.usarTipoformulario=UltimosCincoRegistros().usarTipoformulario;
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
                respuesta.redirect = Url.Content("~/TipoFormulario/Ini_TipoFormulario");
                respuesta.error = "Ocurrio Un inconveniente, Favor volver a tratar. " + ex.Message + "---" + ex.GetBaseException().ToString();
                new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), Paginas.Ini_TipoFormulario.ToString(), Accion.Error.ToString(), ex.GetBaseException().Message);
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

        }//end method
        
        [HttpPost]
        public JsonResult Delete(Usar_TipoFormulario usar_TipoFormulario)
        {
            return Json(null);

        }
        public ActionResult lista()
        {


            //Si NO esta loguieado lo redireccionara al loguin
            if (HttpContext.Session["user"] == null)
            {
                return RedirectToAction("Index", "PaginaPresentacion");
            }

            //para que el div de accion solo aparezca si son 
            //vistas difrentes a home
            ViewBag.isHome = true;

            List<TipoFormulario> tipoFormulario = db.TipoFormularios.ToList();
            var listUsar_tipoformulario = new List<Usar_TipoFormulario>();
            //crear objeto tipo formulario y asignar a la lista de usartipoformulario
            foreach (var item in tipoFormulario)
            {
                Usar_TipoFormulario formulario = new Usar_TipoFormulario();
                //asignar contenido a otro objeto
                CopyClass.CopyObject(item, ref formulario);
                listUsar_tipoformulario.Add(formulario);
            }
            ViewBag.datasource = listUsar_tipoformulario;
            return View();
        }
        
        // GET: /CommandColumn/

        //public ActionResult CommandColumn()
        //{
        //    var DataSource = OrderRepository.GetAllRecords().ToList();
        //    ViewBag.datasource = DataSource;
        //    return View();
        //}

        public ActionResult CommandUpdate(Usar_TipoFormulario value)
        {
            //OrderRepository.Update(value);
            //var data = OrderRepository.GetAllRecords();
            //return Json(value, JsonRequestBehavior.AllowGet);
            return null;
        }

        public ActionResult CommandDelete(int key)
        {
            //OrderRepository.Delete(key);
            //var data = OrderRepository.GetAllRecords();
            //return Json(data, JsonRequestBehavior.AllowGet);
            return null;
        }
                
        //buscar los ultimos 5 registros
        public ResponseModel UltimosCincoRegistros()
        {
            var respuesta = new ResponseModel();
            List<TipoFormulario> tipoFormulario = (from tf in db.TipoFormularios
                                                   orderby tf.TFormSecuencia descending
                                                   select tf).Take(5).ToList();

            var listUsar_tipoformulario = new List<Usar_TipoFormulario>();
            //crear objeto tipo formulario y asignar a la lista de usartipoformulario
            foreach (var item in tipoFormulario)
            {
                Usar_TipoFormulario formulario = new Usar_TipoFormulario();
                //asignar contenido a otro objeto
                CopyClass.CopyObject(item, ref formulario);
                listUsar_tipoformulario.Add(formulario);
            }
            ViewBag.ultimosCinco = listUsar_tipoformulario;

            //respuesta.usarTipoformulario = listUsar_tipoformulario;
            //ienumerable
            respuesta.someCollection = listUsar_tipoformulario;

            return respuesta;
        }

        public ActionResult PruebaAngular()
        {
            //validar que si no tiene permiso a este formulario no entre
            List<vw_ListDeFormuriosbyRolyUser> formulariosPlanRoleUser = new List<vw_ListDeFormuriosbyRolyUser>();
            formulariosPlanRoleUser = (List<App_Data.vw_ListDeFormuriosbyRolyUser>)Session["FormulariosPermitidos"];
             if (formulariosPlanRoleUser.Count == 0)
            {
                return RedirectToAction("Index", "DashBoard");
            }
            //traera false o true;
            var EstaEsteFormulario = formulariosPlanRoleUser.Where(f => f.FormDescripcion == "PruebaAngular").SingleOrDefault();
           
            //si es diferente de true quiere decir que no tiene permiso
             if (EstaEsteFormulario == null)
            {
                return RedirectToAction("Index", "DashBoard");
            }
            ViewBag.ListaFormulario = formulariosPlanRoleUser;


            //para que el div de accion solo aparezca si son 
            //vistas difrentes a home
            ViewBag.isHome = true;

            //var data = new NorthwindDataContext().OrdersViews.Where(e => e.OrderID == ord).FirstOrDefault();
            return View();

        }
         [HttpPost]
        public JsonResult AjaxMethod(string name)
        {
            PersonModel person = new PersonModel
            {
                Name = name,
                DateTime = Lib.GetLocalDateTime().ToString()
            };
            return Json(person);
        }
    }

    public class PersonModel
    {
        ///<summary>
        /// Gets or sets Name.
        ///</summary>
        public string Name { get; set; }

        ///<summary>
        /// Gets or sets DateTime.
        ///</summary>
        public string DateTime { get; set; }
    }
}
