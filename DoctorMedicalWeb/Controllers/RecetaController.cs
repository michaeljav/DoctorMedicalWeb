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
    public class RecetaController : Controller
    {

        string controler = "Receta", vista = "Ini_Receta", PaginaAutorizada = Paginas.pag_Receta.ToString();
        public ActionResult Ini_Receta()
        {
            using (DoctorMedicalWebEntities db = new DoctorMedicalWebEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;

                //Si NO esta loguieado lo redireccionara al loguin
                if (HttpContext.Session["user"] == null)
                {
                    return RedirectToAction("Index", "PaginaPresentacion");
                }

                //lleno el combo de roles
                UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];

                //pARA EN LA VISTA PODER SABER A QUE  CONTROLADOR EL AJAX  LLAMARA
                ViewBag.ControlCsharp = controler;
                ViewBag.VistaCsharp = vista;


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

                //para que el div de accion solo aparezca si son 
                //vistas difrentes a home
                ViewBag.isHome = false;


          


                //Lista de medicamentos del doctor

                List<Medicamento> medica = (from rform in db.Medicamentoes
                                            where rform.DoctSecuencia_fk == usu.doctSecuencia
                                            select rform).ToList();
                ViewBag.MedicamentosListaCheck = medica;
                //Lista de analisis del doctor

                List<AnalisisClinico> analisi = (from rform in db.AnalisisClinicoes
                                                 where rform.DoctSecuencia_fk == usu.doctSecuencia
                                                 select rform).ToList();
                ViewBag.AnalisisClinicoListaCheck = analisi;
                //Lista de imagenes del doctor

                List<Imagene> image = (from rform in db.Imagenes
                                       where rform.DoctSecuencia_fk == usu.doctSecuencia
                                       select rform).ToList();
                ViewBag.ImagenesListaCheck = image;


                //buscar listado de paciente
                List<Paciente> paciente = ((from pers in db.Pacientes
                                            where pers.DoctSecuencia_fk == usu.doctSecuencia
                                           && pers.EstaDesabilitado == false
                                            select pers).ToList());
                var usarPacientelist = new  List<Usar_Paciente>();
                foreach (var item in paciente)
                {
                    var nuevoUsarPaci = new Usar_Paciente();
                        //asignar contenido a otro objeto
                    CopyClass.CopyObject(item, ref nuevoUsarPaci);
                    usarPacientelist.Add(nuevoUsarPaci);
                }

                ViewBag.listPaciente = usarPacientelist;


                //Enlistar los 5 ultimos y lleno
                //el   ViewBag.ultimosCinco
                var a = UltimosCincoRegistros();


                var usarReceta = new Usar_RecetaComplementaria();
                //si  esta lleno es por que esta editando
                if (Session["Usar_RecetaComplementaria"] != null)
                {
                    usarReceta = (Usar_RecetaComplementaria)Session["Usar_RecetaComplementaria"];

                    //para llenar el dropdownlist y select el checklist

                    ViewBag.FillDropDownListCheckJsonMedicamentos = usarReceta.MediSecuencia_fk;
                    ViewBag.FillDropDownListCheckJsonAnalisis = usarReceta.AClinSecuencia_fk;
                    ViewBag.FillDropDownListCheckJsonImagenes = usarReceta.ImagSecuencia_fk;

                    //limpiar sesion
                    Session["Usar_RecetaComplementaria"] = null;

                    return View(usarReceta);
                }//SI NO SE ESTA EDITANDO
                else
                {
                    //hora actual @
                    var timeno = Lib.GetLocalDateTime().Date;
                    ViewBag.datNow = timeno;
                }

                return View(usarReceta);
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

                    //     vw_UsuarioDoctor docSecuencia = db.vw_UsuarioDoctor.Where(d => d.UsuaSecuencia == usu.usuario.UsuaSecuencia).FirstOrDefault();

                    //buscar los ultimos  cinco registros  de roles del doctor loguiado
                    var ObjetosListaUltimosCinco = (from tf in db.Recetas
                                                    where tf.DoctSecuencia_fk == usu.doctSecuencia
                                                    && tf.EstaDesabilitado == false
                                                    orderby tf.ReceFecha descending
                                                    select tf).Take(5).ToList();

                    //var listObjetosParaMotrarVista = new List<Usar_Receta>();
                    var listObjetosParaMotrarVista = new List<Usar_RecetaComplementaria>();
                    //crear objeto role y asignar a la lista de usaroles
                    foreach (var item in ObjetosListaUltimosCinco)
                    {
                        //var objNuevo = new Usar_Receta();
                        var objNuevo = new Usar_RecetaComplementaria();

                        //asignar contenido a otro objeto
                        CopyClass.CopyObject(item, ref objNuevo);

                        //LLenar paciente
                        var objPaci = db.Pacientes.Where(ro =>
                            ro.DoctSecuencia_fk == usu.doctSecuencia
                         && ro.PaciSecuencia == item.PaciSecuencia_fk).SingleOrDefault();

                        objNuevo.PaciSecuencia_fk = objPaci.PaciSecuencia;
                        objNuevo.PaciNombre = objPaci.PaciNombre;
                        objNuevo.PaciApellido1 = objPaci.PaciApellido1;
                        objNuevo.PaciApellido2 = objPaci.PaciApellido2;
                        objNuevo.PaciDocumento = objPaci.PaciDocumento;


                        // si  la receta a visualizar  se creo en una consulta
                        //quiere decir que este campo (RecSinConsultaNombre)   en momentos de 
                        //verlo por aqui por esta receta  estara vacio por ende en el grid
                        //se vera en blanco por eso,  yo hago esto para que se vea en el grid 
                        //pero se guardara en base de datos una vez se le de a guardar
                        objNuevo.RecSinConsultaNombre = (objPaci.PaciNombre + " " + objPaci.PaciApellido1 + " " + objPaci.PaciApellido2 + " - " + item.RecNombre);                          

                        

                        //crear fecha en string
                        //objNuevo.ReceFechaString = objNuevo.ReceFecha.ToString();

                        //.ToString("s")
                        listObjetosParaMotrarVista.Add(objNuevo);
                    }
                    //Utilizado para insertar el listado al grid
                    ViewBag.ultimosCinco = listObjetosParaMotrarVista;


                    //ienumerable lista
                    respuesta.someCollection = listObjetosParaMotrarVista;


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
        public JsonResult updateMaintenance(objAcualizarMantenimiento maintenance)
        {
            var respuesta = new ResponseModel();



            UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];
            if (usu == null)
            {
                respuesta.respuesta = false;
                respuesta.error = "Usted debe de loguiarse.";
                respuesta.redirect = Url.Action("Index", "PaginaPresentacion");
                return Json(respuesta);
            }


            try
            {
                if (maintenance != null)
                {

                    var value = maintenance.Maintenance;
                    switch (value)
                    {

                       
                        case Maintenance.Imagenes:
                            using (var db = new DoctorMedicalWebEntities())
                            {
                                Usar_Imagenes usarImagenes = new Usar_Imagenes();
                                usarImagenes.ImagNombre = maintenance.Nombre;
                                usarImagenes.ImagDescripcion = maintenance.Descripcion;
                                usarImagenes.EstaDesabilitado = false;
                                respuesta = SaveImagenes(usarImagenes);
                                respuesta.obj = Maintenance.Imagenes.ToString();

                            }
                            break;

                        case Maintenance.AnalisisClinico:
                            using (var db = new DoctorMedicalWebEntities())
                            {
                                Usar_AnalisisClinico usarAnalisisClinico = new Usar_AnalisisClinico();
                                usarAnalisisClinico.AClinNombre = maintenance.Nombre;
                                usarAnalisisClinico.AClinDescripcion = maintenance.Descripcion;
                                usarAnalisisClinico.EstaDesabilitado = false;
                                respuesta = SaveAnalisisClinico(usarAnalisisClinico);
                                respuesta.obj = Maintenance.AnalisisClinico.ToString();

                            }
                            break;

                        case Maintenance.Medicamento:
                            using (var db = new DoctorMedicalWebEntities())
                            {
                                Usar_Medicamento usarMedicamentos = new Usar_Medicamento();
                                usarMedicamentos.MediNombre = maintenance.Nombre;
                                usarMedicamentos.MediDescripcion = maintenance.Descripcion;
                                usarMedicamentos.EstaDesabilitado = false;
                                respuesta = SaveMedicamentos(usarMedicamentos);
                                respuesta.obj = Maintenance.Medicamento.ToString();

                            }
                            break;



                    }

                }

                return Json(respuesta);

            }
            catch (Exception)
            {

                throw;
            }


        }

        public ResponseModel SaveAnalisisClinico(Usar_AnalisisClinico usar_AnalisisClinico)
        {
            var respuesta = new ResponseModel();

            //Si NO esta loguieado lo redireccionara al loguin
            if (HttpContext.Session["user"] == null)
            {
                respuesta.respuesta = false;
                respuesta.error = "Usted debe de loguiarse.";
                respuesta.redirect = Url.Action("Index", "PaginaPresentacion");
                return (respuesta);
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
                    return (respuesta);
                }



                using (var dbtrans = db.Database.BeginTransaction())
                {

                    try
                    {


                        //add new 
                        if (string.IsNullOrEmpty(usar_AnalisisClinico.AClinSecuencia.ToString()) || usar_AnalisisClinico.AClinSecuencia == 0)
                        {
                            var objToSave = new AnalisisClinico();

                            //asignar contenido a otro objeto
                            CopyClass.CopyObject(usar_AnalisisClinico, ref objToSave);

                            //si existe la insituciona aseguradora del doctor no se introduce
                            var existe = (from objExist in db.AnalisisClinicoes
                                          where objExist.AClinNombre == usar_AnalisisClinico.AClinNombre
                                              && objExist.DoctSecuencia_fk == usu.doctSecuencia
                                          select objExist).SingleOrDefault();
                            if (existe != null)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "Ya existe este analisis clínico.";
                                respuesta.redirect = Url.Action(vista, controler);
                                return (respuesta);
                            }


                            objToSave.DoctSecuencia_fk = usu.doctSecuencia;
                            objToSave.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                            objToSave.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                            objToSave.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;

                            //buscando la proxima secuencia, el doctor en todas sus consultorios tiene los mismos roles 
                            proximoItem = ((from numSecu in db.AnalisisClinicoes
                                            where numSecu.DoctSecuencia_fk == usu.doctSecuencia
                                            select (int?)numSecu.AClinSecuencia).Max() ?? 0) + 1;
                            objToSave.AClinSecuencia = proximoItem;

                            db.AnalisisClinicoes.Add(objToSave);

                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Nuevo.ToString(), null);
                        }


                        db.SaveChanges();
                        dbtrans.Commit();

                        respuesta.respuesta = true;
                        respuesta.redirect = Url.Content("~/" + controler + "/" + vista + "");

                        List<AnalisisClinico> listObj = (from objExist in db.AnalisisClinicoes
                                                         where
                                                             objExist.DoctSecuencia_fk == usu.doctSecuencia
                                                         select objExist).ToList();
                        List<Usar_AnalisisClinico> objList = new List<Usar_AnalisisClinico>();
                        foreach (var item in listObj)
                        {
                            Usar_AnalisisClinico obj = new Usar_AnalisisClinico();
                            //asignar contenido a otro objeto
                            CopyClass.CopyObject(item, ref obj);
                            objList.Add(obj);
                        }

                        respuesta.someCollection = objList;
                        return (respuesta);
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
                        return (respuesta);
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

        public ResponseModel SaveImagenes(Usar_Imagenes usar_Imagenes)
        {
            var respuesta = new ResponseModel();

            //Si NO esta loguieado lo redireccionara al loguin
            if (HttpContext.Session["user"] == null)
            {
                respuesta.respuesta = false;
                respuesta.error = "Usted debe de loguiarse.";
                respuesta.redirect = Url.Action("Index", "PaginaPresentacion");
                return (respuesta);
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
                    return (respuesta);
                }



                using (var dbtrans = db.Database.BeginTransaction())
                {

                    try
                    {


                        //add new 
                        if (string.IsNullOrEmpty(usar_Imagenes.ImagSecuencia.ToString()) || usar_Imagenes.ImagSecuencia == 0)
                        {
                            var objToSave = new Imagene();

                            //asignar contenido a otro objeto
                            CopyClass.CopyObject(usar_Imagenes, ref objToSave);

                            //si existe la insituciona aseguradora del doctor no se introduce
                            var existe = (from objExist in db.Imagenes
                                          where objExist.ImagNombre == usar_Imagenes.ImagNombre
                                              && objExist.DoctSecuencia_fk == usu.doctSecuencia
                                          select objExist).SingleOrDefault();
                            if (existe != null)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "Ya existe esta imagen.";
                                respuesta.redirect = Url.Action(vista, controler);
                                return (respuesta);
                            }


                            objToSave.DoctSecuencia_fk = usu.doctSecuencia;
                            objToSave.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                            objToSave.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                            objToSave.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;

                            //buscando la proxima secuencia, el doctor en todas sus consultorios tiene los mismos roles 
                            proximoItem = ((from numSecu in db.Imagenes
                                            where numSecu.DoctSecuencia_fk == usu.doctSecuencia
                                            select (int?)numSecu.ImagSecuencia).Max() ?? 0) + 1;
                            objToSave.ImagSecuencia = proximoItem;

                            db.Imagenes.Add(objToSave);

                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Nuevo.ToString(), null);
                        }

                        db.SaveChanges();
                        dbtrans.Commit();

                        respuesta.respuesta = true;
                        respuesta.redirect = Url.Content("~/" + controler + "/" + vista + "");

                        List<Imagene> listObj = (from objExist in db.Imagenes
                                                 where
                                                     objExist.DoctSecuencia_fk == usu.doctSecuencia
                                                 select objExist).ToList();
                        List<Usar_Imagenes> objList = new List<Usar_Imagenes>();
                        foreach (var item in listObj)
                        {
                            Usar_Imagenes obj = new Usar_Imagenes();
                            //asignar contenido a otro objeto
                            CopyClass.CopyObject(item, ref obj);
                            objList.Add(obj);
                        }

                        respuesta.someCollection = objList;

                        return (respuesta);
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
                        //return (respuesta, JsonRequestBehavior.AllowGet); 
                        return (respuesta);
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


        public ResponseModel SaveMedicamentos(Usar_Medicamento usar_Medicamento)
        {
            var respuesta = new ResponseModel();

            //Si NO esta loguieado lo redireccionara al loguin
            if (HttpContext.Session["user"] == null)
            {
                respuesta.respuesta = false;
                respuesta.error = "Usted debe de loguiarse.";
                respuesta.redirect = Url.Action("Index", "PaginaPresentacion");
                return (respuesta);
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
                    return (respuesta);
                }



                using (var dbtrans = db.Database.BeginTransaction())
                {

                    try
                    {


                        //add new 
                        if (string.IsNullOrEmpty(usar_Medicamento.MediSecuencia.ToString()) || usar_Medicamento.MediSecuencia == 0)
                        {
                            var objToSave = new Medicamento();

                            //asignar contenido a otro objeto
                            CopyClass.CopyObject(usar_Medicamento, ref objToSave);

                            //si existe la insituciona aseguradora del doctor no se introduce
                            var existe = (from objExist in db.Medicamentoes
                                          where objExist.MediNombre == usar_Medicamento.MediNombre
                                              && objExist.DoctSecuencia_fk == usu.doctSecuencia
                                          select objExist).SingleOrDefault();
                            if (existe != null)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "Ya existe este medicamento.";
                                respuesta.redirect = Url.Action(vista, controler);
                                return (respuesta);
                            }


                            objToSave.DoctSecuencia_fk = usu.doctSecuencia;
                            objToSave.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                            objToSave.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                            objToSave.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;

                            //buscando la proxima secuencia, el doctor en todas sus consultorios tiene los mismos roles 
                            proximoItem = ((from numSecu in db.Medicamentoes
                                            where numSecu.DoctSecuencia_fk == usu.doctSecuencia
                                            select (int?)numSecu.MediSecuencia).Max() ?? 0) + 1;
                            objToSave.MediSecuencia = proximoItem;

                            db.Medicamentoes.Add(objToSave);

                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Nuevo.ToString(), null);
                        }


                        db.SaveChanges();
                        dbtrans.Commit();

                        respuesta.respuesta = true;
                        respuesta.redirect = Url.Content("~/" + controler + "/" + vista + "");


                        List<Medicamento> listObj = (from objExist in db.Medicamentoes
                                                     where
                                                         objExist.DoctSecuencia_fk == usu.doctSecuencia
                                                     select objExist).ToList();
                        List<Usar_Medicamento> objList = new List<Usar_Medicamento>();
                        foreach (var item in listObj)
                        {
                            Usar_Medicamento obj = new Usar_Medicamento();
                            //asignar contenido a otro objeto
                            CopyClass.CopyObject(item, ref obj);
                            objList.Add(obj);
                        }

                        respuesta.someCollection = objList;

                        return (respuesta);
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
                        return (respuesta);
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
          [HttpPost]
        public JsonResult Save(Usar_RecetaComplementaria usar_RecetaComplementaria)
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
                        //Yo Inserto con consulta en 0 pero edito con el numero de consultorio que venta
                        if (string.IsNullOrEmpty(usar_RecetaComplementaria.ReceSecuencia.ToString()) || usar_RecetaComplementaria.ReceSecuencia == 0)
                        {
                            var objToSave = new Receta();

                            //asignar contenido a otro objeto
                            CopyClass.CopyObject(usar_RecetaComplementaria, ref objToSave);

                            //si existe la insituciona aseguradora del doctor no se introduce
                            //var existe = (from objExist in db.Recetas
                            //              where objExist.ReceSecuencia == usar_RecetaComplementaria.ReceSecuencia
                            //                  && objExist.PaciSecuencia_fk == usar_RecetaComplementaria.PaciSecuencia_fk
                            //                   && objExist.ClinSecuencia_fk == usu.Consultorio.clinSecuencia_fk
                            //                    && objExist.ConsSecuencia_fk == usu.Consultorio.ConsSecuencia_fk
                            //                        && objExist.PaisSecuencia_fk == usu.Consultorio.PaisSecuencia
                            //                  && objExist.DoctSecuencia_fk == usu.doctSecuencia
                            //              select objExist).SingleOrDefault();
                            //if (existe != null)
                            //{
                            //    respuesta.respuesta = false;
                            //    respuesta.error = "Ya existe este codigo de receta.";
                            //    respuesta.redirect = Url.Action(vista, controler);
                            //    return Json(respuesta);
                            //}


                            objToSave.DoctSecuencia_fk = usu.doctSecuencia;
                            objToSave.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                            objToSave.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                            objToSave.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                            objToSave.CMHistSecuencia_fk = 0;//cuando la receta no pertenece a una consulta

                            //buscando la proxima secuencia, el doctor en todas sus consultorios tiene los mismos roles 
                            proximoItem = ((from numSecu in db.Recetas
                                            where numSecu.DoctSecuencia_fk == usu.doctSecuencia
                                            && numSecu.PaciSecuencia_fk == usar_RecetaComplementaria.PaciSecuencia_fk
                                            && numSecu.CMHistSecuencia_fk == 0//cuando la receta no pertenece a una consulta
                                            //&& numSecu.ClinSecuencia_fk == usar_RecetaComplementaria.ClinSecuencia_fk
                                            // && numSecu.ConsSecuencia_fk == usar_RecetaComplementaria.ConsSecuencia_fk
                                            //     && numSecu.PaisSecuencia_fk == usar_RecetaComplementaria.PaisSecuencia_fk
                                            select (int?)numSecu.ReceSecuencia).Max() ?? 0) + 1;
                            objToSave.ReceSecuencia = proximoItem;
                            objToSave.EstaDesabilitado = false;
                            //objToSave.ReceFecha = Lib.GetLocalDateTime(); // la fecha que coja la que viene de la vista
                            objToSave.UsuaFechaCreacion = Lib.GetLocalDateTime();
                            objToSave.UsuaSecuenciaCreacion = usu.usuario.UsuaSecuencia;
                            ////agregar codigo
                            //string recCodigo = objToSave.DoctSecuencia_fk.ToString() +
                            //            objToSave.PaciSecuencia_fk.ToString() +
                            //            objToSave.CMHistSecuencia_fk.ToString() +
                            //            objToSave.ReceSecuencia;

                            //var recCodGenr = Lib.FormatearCodigo(8, recCodigo);
                            //recCodGenr = "R-" + recCodGenr;
                            //objToSave.RecCodigo = recCodGenr;

                            //para crear el codigo
                            string cantidadReceDoct = ((from objBusc in db.Recetas
                                                        where objBusc.DoctSecuencia_fk == usu.doctSecuencia
                                                        orderby objBusc.RecCodigo.ToString().Substring(objBusc.RecCodigo.ToString().IndexOf("-") + 1) descending
                                                        select objBusc.RecCodigo)).Take(1).SingleOrDefault();

                            string extraerRecSoloSecuencia = "";
                            if (!string.IsNullOrEmpty(cantidadReceDoct))
                            {
                                
                                //cantidadDoct.IndexOf("-")  +1 es por que el index me trae desde donde esta este -(guion) y yo necesito
                                //desde el siguiente  letra
                                extraerRecSoloSecuencia = cantidadReceDoct.Substring(cantidadReceDoct.IndexOf("-") + 1);
                                //extaigo el codigo del doctor para dejar solo la secuencia
                                int n = int.Parse(extraerRecSoloSecuencia);
                                string valor = n.ToString();
                                //cantidad de digitos del numero del doctor
                                int cantDig = usu.doctSecuencia.ToString().Length;
                                //EL valor con solo la secuencia, ya borrado el  numero del doctor
                                string codigoSinDoctor = valor.Substring(cantDig);
                                //sumo  1 para la proxima secuencia
                                int proxim = int.Parse(codigoSinDoctor) + 1;
                                extraerRecSoloSecuencia = proxim.ToString();
                            }
                            else
                            {
                                extraerRecSoloSecuencia = "01";

                            }
                            //quito los ceros delanteros
                            int secMaxSinCerosDelanterosRec = int.Parse(extraerRecSoloSecuencia);
                            //codigo con ceros doctor y secuencia
                            string codigRecDoctFormateado = Lib.FormatearCodigo(3, usu.doctSecuencia.ToString());
                            string codiRecSecGeneral = Lib.FormatearCodigo(6, secMaxSinCerosDelanterosRec.ToString());
                            string codFormateadoDefiniRec = codigRecDoctFormateado + codiRecSecGeneral;
                            codFormateadoDefiniRec = "R-" + codFormateadoDefiniRec;
                            objToSave.RecCodigo = codFormateadoDefiniRec;


                            //LLenar paciente
                            var objPaci = db.Pacientes.Where(ro =>
                                ro.DoctSecuencia_fk == usu.doctSecuencia
                                         && ro.EstaDesabilitado == false
                             && ro.PaciSecuencia == usar_RecetaComplementaria.PaciSecuencia_fk).SingleOrDefault();

                            objToSave.RecSinConsultaNombre = (objPaci.PaciNombre + " " + objPaci.PaciApellido1 + " " + objPaci.PaciApellido2 + " " + usar_RecetaComplementaria.RecNombre);                          

                            db.Recetas.Add(objToSave);



                            //guardar receta Medicamentos
                            if (usar_RecetaComplementaria.MediSecuencia_fk != null)
                            {


                                int medsec = 0;

                                foreach (var medisec in usar_RecetaComplementaria.MediSecuencia_fk)
                                {
                                    //si el valor es 0 quiere decir que no hay elementso
                                    if (medisec > 0)
                                    {
                                        ++medsec;
                                        var RMedi = new RecetaMedicamento();
                                        RMedi.DoctSecuencia_fk = usu.doctSecuencia;
                                        RMedi.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia_fk;
                                        RMedi.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                        RMedi.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                        RMedi.PaciSecuencia_fk = objToSave.PaciSecuencia_fk;
                                        RMedi.ReceSecuencia_fk = objToSave.ReceSecuencia;
                                        RMedi.CMHistSecuencia_fk = 0;//cuando la receta no pertenece a una consulta
                                        RMedi.MediSecuencia_fk = (int)medisec;
                                        RMedi.RMediSecuencia = medsec;
                                        RMedi.EstaDesabilitado = false;
                                        db.RecetaMedicamentos.Add(RMedi);
                                    }
                                }
                            }


                            //guardar receta analisis
                            if (usar_RecetaComplementaria.AClinSecuencia_fk != null)
                            {


                                int analisecc = 0;
                                foreach (int anali in usar_RecetaComplementaria.AClinSecuencia_fk)
                                {
                                    //si el valor es 0 quiere decir que no hay elementso
                                    if (anali > 0)
                                    {
                                        ++analisecc;
                                        var Ranali = new RecetaAnalisisClinico();
                                        Ranali.DoctSecuencia_fk = usu.doctSecuencia;
                                        Ranali.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia_fk;
                                        Ranali.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                        Ranali.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                        Ranali.PaciSecuencia_fk = objToSave.PaciSecuencia_fk;
                                        Ranali.ReceSecuencia_fk = objToSave.ReceSecuencia;
                                        Ranali.CMHistSecuencia_fk = 0;//cuando la receta no pertenece a una consulta
                                        Ranali.AClinSecuencia_fk = anali;
                                        Ranali.RAClinSecuencia = analisecc;
                                        Ranali.EstaDesabilitado = false;

                                        db.RecetaAnalisisClinicoes.Add(Ranali);
                                    }
                                }
                            }
                            //guardar receta imagenes
                            if (usar_RecetaComplementaria.ImagSecuencia_fk != null)
                            {


                                int Imagsec = 0;
                                foreach (int imag in usar_RecetaComplementaria.ImagSecuencia_fk)
                                {
                                    //si el valor es 0 quiere decir que no hay elementso
                                    if (imag > 0)
                                    {
                                        ++Imagsec;
                                        var RImage = new RecetaImagene();
                                        RImage.DoctSecuencia_fk = usu.doctSecuencia;
                                        RImage.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia_fk;
                                        RImage.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                        RImage.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                        RImage.PaciSecuencia_fk = objToSave.PaciSecuencia_fk;
                                        RImage.ReceSecuencia_fk = objToSave.ReceSecuencia;
                                        RImage.CMHistSecuencia_fk = 0;//cuando la receta no pertenece a una consulta
                                        RImage.ImagSecuencia_fk = imag;
                                        RImage.RImagSecuencia = Imagsec;
                                        RImage.EstaDesabilitado = false;
                                        db.RecetaImagenes.Add(RImage);
                                    }
                                }
                            }

                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Nuevo.ToString(), null);
                        }
                        //Editando 
                        //Yo Inserto con consulta en 0 pero edito con el numero de consultorio que venta
                        else
                        {

                            //busco el objeto que se editara
                            //lo busco por la secuencia para que pueda editar ese mismo sin tener que borrarlo
                            //si lo busco por el nombre no lo  editar ya que no lo encontrara.
                            var objEditando = db.Recetas.Where(ro =>
                                               ro.DoctSecuencia_fk == usu.doctSecuencia
                                            && ro.PaciSecuencia_fk == usar_RecetaComplementaria.PaciSecuencia_fk
                                            && ro.CMHistSecuencia_fk == usar_RecetaComplementaria.CMHistSecuencia_fk
                                            && ro.ReceSecuencia == usar_RecetaComplementaria.ReceSecuencia).SingleOrDefault();

                            if (objEditando == null)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "No existe este receta";
                                //respuesta.redirect = Url.Action(vista, controler);
                                return Json(respuesta);
                            }

                            ////los campos de primarikey  como no viene los llenos, para asignarlos a la receta editada
                            usar_RecetaComplementaria.DoctSecuencia_fk = usu.doctSecuencia;
                            usar_RecetaComplementaria.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                            usar_RecetaComplementaria.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                            usar_RecetaComplementaria.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                            //usar_RecetaComplementaria.ReceFecha = objEditando.ReceFecha;// la fecha que coja la que viene de la vista
                            usar_RecetaComplementaria.UsuaFechaCreacion = objEditando.UsuaFechaCreacion;
                            usar_RecetaComplementaria.UsuaSecuenciaCreacion = objEditando.UsuaSecuenciaCreacion;
                            usar_RecetaComplementaria.CMHistSecuencia_fk = objEditando.CMHistSecuencia_fk;



                            //asigno al objeto que se editara los nuevos datos introducidos a editar
                            CopyClass.CopyObject(usar_RecetaComplementaria, ref objEditando);


                            objEditando.UsuaSecuenciaModificacion = usu.usuario.UsuaSecuencia;
                            objEditando.UsuaFechaModificacion = Lib.GetLocalDateTime();


                            //LLenar paciente
                            var objPaci = db.Pacientes.Where(ro =>
                                ro.DoctSecuencia_fk == usu.doctSecuencia
                                         && ro.EstaDesabilitado == false
                             && ro.PaciSecuencia == usar_RecetaComplementaria.PaciSecuencia_fk).SingleOrDefault();

                            objEditando.RecSinConsultaNombre = (objPaci.PaciNombre + " " + objPaci.PaciApellido1 + " " + objPaci.PaciApellido2 + " " + usar_RecetaComplementaria.RecNombre);                          



                            //BORRAR TODOS  receta medicamentos 
                            var receMEdicameBorrar = new List<RecetaMedicamento>();
                            receMEdicameBorrar = (from rform in db.RecetaMedicamentos
                                                  where rform.DoctSecuencia_fk == usu.doctSecuencia
                                                     && rform.PaciSecuencia_fk == usar_RecetaComplementaria.PaciSecuencia_fk
                                                    && rform.CMHistSecuencia_fk == usar_RecetaComplementaria.CMHistSecuencia_fk
                                                  && rform.ReceSecuencia_fk == usar_RecetaComplementaria.ReceSecuencia
                                                  select rform).ToList();


                            foreach (var item in receMEdicameBorrar)
                            {

                                db.RecetaMedicamentos.Remove(item);
                            }

                            //BORRAR TODOS  receta analisis clinico 
                            var receAnalisiBorrar = new List<RecetaAnalisisClinico>();
                            receAnalisiBorrar = (from rform in db.RecetaAnalisisClinicoes
                                                 where rform.DoctSecuencia_fk == usu.doctSecuencia
                                                    && rform.PaciSecuencia_fk == usar_RecetaComplementaria.PaciSecuencia_fk
                                                            && rform.CMHistSecuencia_fk == usar_RecetaComplementaria.CMHistSecuencia_fk
                                                 && rform.ReceSecuencia_fk == usar_RecetaComplementaria.ReceSecuencia
                                                 select rform).ToList();


                            foreach (var item in receAnalisiBorrar)
                            {

                                db.RecetaAnalisisClinicoes.Remove(item);
                            }

                            //BORRAR TODOS  receta imagenes
                            var receImagenesBorrar = new List<RecetaImagene>();
                            receImagenesBorrar = (from rform in db.RecetaImagenes
                                                  where rform.DoctSecuencia_fk == usu.doctSecuencia
                                                     && rform.PaciSecuencia_fk == usar_RecetaComplementaria.PaciSecuencia_fk
                                                      && rform.CMHistSecuencia_fk == usar_RecetaComplementaria.CMHistSecuencia_fk
                                                  && rform.ReceSecuencia_fk == usar_RecetaComplementaria.ReceSecuencia
                                                  select rform).ToList();


                            foreach (var item in receImagenesBorrar)
                            {

                                db.RecetaImagenes.Remove(item);
                            }

                            //guardar receta Medicamentos
                            if (usar_RecetaComplementaria.MediSecuencia_fk != null)
                            {
                                int medsec = 0;
                                foreach (int medisec in usar_RecetaComplementaria.MediSecuencia_fk)
                                {
                                    //si el valor es 0 quiere decir que no hay elementso
                                    if (medisec > 0)
                                    {
                                        ++medsec;
                                        var RMedi = new RecetaMedicamento();
                                        RMedi.DoctSecuencia_fk = usu.doctSecuencia;
                                        RMedi.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia_fk;
                                        RMedi.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                        RMedi.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                        RMedi.PaciSecuencia_fk = objEditando.PaciSecuencia_fk;
                                        RMedi.ReceSecuencia_fk = objEditando.ReceSecuencia;
                                        RMedi.CMHistSecuencia_fk = (int)usar_RecetaComplementaria.CMHistSecuencia_fk;
                                        RMedi.MediSecuencia_fk = medisec;
                                        RMedi.RMediSecuencia = medsec;
                                        RMedi.EstaDesabilitado = false;

                                        db.RecetaMedicamentos.Add(RMedi);
                                    }
                                }
                            }
                            //guardar receta analisis
                            if (usar_RecetaComplementaria.AClinSecuencia_fk != null)
                            {
                                int analisecc = 0;
                                foreach (int anali in usar_RecetaComplementaria.AClinSecuencia_fk)
                                {
                                    //si el valor es 0 quiere decir que no hay elementso
                                    if (anali > 0)
                                    {
                                        ++analisecc;
                                        var Ranali = new RecetaAnalisisClinico();
                                        Ranali.DoctSecuencia_fk = usu.doctSecuencia;
                                        Ranali.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia_fk;
                                        Ranali.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                        Ranali.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                        Ranali.PaciSecuencia_fk = objEditando.PaciSecuencia_fk;
                                        Ranali.ReceSecuencia_fk = objEditando.ReceSecuencia;
                                        Ranali.CMHistSecuencia_fk = (int)usar_RecetaComplementaria.CMHistSecuencia_fk;
                                        Ranali.AClinSecuencia_fk = anali;
                                        Ranali.RAClinSecuencia = analisecc;
                                        Ranali.EstaDesabilitado = false;
                                        db.RecetaAnalisisClinicoes.Add(Ranali);
                                    }
                                }
                            }
                            //guardar receta imagenes
                            if (usar_RecetaComplementaria.ImagSecuencia_fk != null)
                            {
                                int analisecc = 0;
                                foreach (int imag in usar_RecetaComplementaria.ImagSecuencia_fk)
                                {
                                    //si el valor es 0 quiere decir que no hay elementso
                                    if (imag > 0)
                                    {
                                        ++analisecc;
                                        var RImage = new RecetaImagene();
                                        RImage.DoctSecuencia_fk = usu.doctSecuencia;
                                        RImage.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia_fk;
                                        RImage.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                        RImage.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                        RImage.PaciSecuencia_fk = objEditando.PaciSecuencia_fk;
                                        RImage.ReceSecuencia_fk = objEditando.ReceSecuencia;
                                        RImage.CMHistSecuencia_fk = (int)usar_RecetaComplementaria.CMHistSecuencia_fk;
                                        RImage.ImagSecuencia_fk = imag;
                                        RImage.RImagSecuencia = analisecc;
                                        RImage.EstaDesabilitado = false;
                                        db.RecetaImagenes.Add(RImage);
                                    }
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
                        //Para limpiar  la lista de
                        respuesta.ObjectGridList = new List<Usar_InstitucionAseguradoraPlanes>();
                        return Json(respuesta, JsonRequestBehavior.AllowGet);
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
          [HttpPost]
        public JsonResult Borrar(Usar_RecetaComplementaria usar_RecetaComplementaria)
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
                if (usar_RecetaComplementaria.ReceSecuencia == null || usar_RecetaComplementaria.ReceSecuencia < 1)
                {
                    respuesta.respuesta = false;
                    respuesta.error = "Favor seleccionar una receta";
                    respuesta.redirect = Url.Action(vista, controler);
                    return Json(respuesta);

                }
                using (var dbtrans = db.Database.BeginTransaction())
                {
                    try
                    {
                        //borro registro
                        if (usar_RecetaComplementaria != null)
                        {

                            var ObjNuevToSave = new Medicamento();
                            //asignar contenido a otro objeto
                            CopyClass.CopyObject(usar_RecetaComplementaria, ref ObjNuevToSave);


                            var borrar = db.Recetas.Where(ro =>
                                              ro.DoctSecuencia_fk == usu.doctSecuencia
                                           && ro.PaciSecuencia_fk == usar_RecetaComplementaria.PaciSecuencia_fk
                                           && ro.CMHistSecuencia_fk == usar_RecetaComplementaria.CMHistSecuencia_fk
                                           && ro.ReceSecuencia == usar_RecetaComplementaria.ReceSecuencia).SingleOrDefault();

                            if (borrar != null)
                            {

                                borrar.EstaDesabilitado = true;
                                //db.AnalisisClinicoes.Remove(borrar);

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
        public JsonResult Editar(Usar_RecetaComplementaria usar_RecetaComplementaria)
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
                if (usar_RecetaComplementaria != null)
                {



                    var objEdit = db.Recetas.Where(ro =>
                                          ro.DoctSecuencia_fk == usu.doctSecuencia
                                       && ro.PaciSecuencia_fk == usar_RecetaComplementaria.PaciSecuencia_fk
                                       && ro.CMHistSecuencia_fk == usar_RecetaComplementaria.CMHistSecuencia_fk
                                       && ro.ReceSecuencia == usar_RecetaComplementaria.ReceSecuencia
                                                && ro.EstaDesabilitado == false).SingleOrDefault();



                    // TODOS  receta medicamentos 
                    var EditreceMEdicame = new List<RecetaMedicamento>();
                    EditreceMEdicame = (from rform in db.RecetaMedicamentos
                                        where rform.DoctSecuencia_fk == usu.doctSecuencia
                                           && rform.PaciSecuencia_fk == usar_RecetaComplementaria.PaciSecuencia_fk
                                             && rform.CMHistSecuencia_fk == usar_RecetaComplementaria.CMHistSecuencia_fk
                                        && rform.ReceSecuencia_fk == usar_RecetaComplementaria.ReceSecuencia
                                        select rform).ToList();




                    // TODOS  receta analisis clinico 
                    var EditreceAnalisi = new List<RecetaAnalisisClinico>();
                    EditreceAnalisi = (from rform in db.RecetaAnalisisClinicoes
                                       where rform.DoctSecuencia_fk == usu.doctSecuencia
                                          && rform.PaciSecuencia_fk == usar_RecetaComplementaria.PaciSecuencia_fk
                                           && rform.CMHistSecuencia_fk == usar_RecetaComplementaria.CMHistSecuencia_fk
                                       && rform.ReceSecuencia_fk == usar_RecetaComplementaria.ReceSecuencia
                                       select rform).ToList();



                    // TODOS  receta imagenes
                    var EditreceImagenes = new List<RecetaImagene>();
                    EditreceImagenes = (from rform in db.RecetaImagenes
                                        where rform.DoctSecuencia_fk == usu.doctSecuencia
                                           && rform.PaciSecuencia_fk == usar_RecetaComplementaria.PaciSecuencia_fk
                                            && rform.CMHistSecuencia_fk == usar_RecetaComplementaria.CMHistSecuencia_fk
                                        && rform.ReceSecuencia_fk == usar_RecetaComplementaria.ReceSecuencia
                                        select rform).ToList();

                    if (objEdit != null)
                    {
                        //objeto usar_...
                        var objUsarRecetaComplementariaEditar = new Usar_RecetaComplementaria();
                        //asignar contenido a otro objeto receta
                        CopyClass.CopyObject(objEdit, ref objUsarRecetaComplementariaEditar);

                        //LLenar paciente
                        var objPaci = db.Pacientes.Where(ro =>
                            ro.DoctSecuencia_fk == usu.doctSecuencia
                                     && ro.EstaDesabilitado == false
                         && ro.PaciSecuencia == usar_RecetaComplementaria.PaciSecuencia_fk).SingleOrDefault();

                        objUsarRecetaComplementariaEditar.PaciSecuencia_fk = objPaci.PaciSecuencia;
                        objUsarRecetaComplementariaEditar.PaciNombre = objPaci.PaciNombre;
                        objUsarRecetaComplementariaEditar.PaciApellido1 = objPaci.PaciApellido1;
                        objUsarRecetaComplementariaEditar.PaciApellido2 = objPaci.PaciApellido2;
                        objUsarRecetaComplementariaEditar.PaciDocumento = objPaci.PaciDocumento;



                        //crear  int de items
                        objUsarRecetaComplementariaEditar.MediSecuencia_fk = new List<int>();
                        //   var objreceMedicamentos = new Usar_RecetaMedicamento();
                        //agregar todos los medicamentos de esta receta
                        foreach (var item in EditreceMEdicame)
                        {

                            //objreceMedicamentos.DoctSecuencia_fk = item.DoctSecuencia_fk;
                            //objreceMedicamentos.PaisSecuencia_fk = item.PaisSecuencia_fk;
                            //objreceMedicamentos.ClinSecuencia_fk = item.ClinSecuencia_fk;
                            //objreceMedicamentos.ConsSecuencia_fk = item.ConsSecuencia_fk;
                            //objreceMedicamentos.PaciSecuencia_fk = item.PaciSecuencia_fk;
                            objUsarRecetaComplementariaEditar.MediSecuencia_fk.Add(item.MediSecuencia_fk);

                        }

                        //var objRECEANALIS = new Usar_RecetaAnalisisClinico();
                        //crear  int de items
                        objUsarRecetaComplementariaEditar.AClinSecuencia_fk = new List<int>();
                        //agregar todos los analisis de esta receta
                        foreach (var item in EditreceAnalisi)
                        {

                            //objRECEANALIS.DoctSecuencia_fk = item.DoctSecuencia_fk;
                            //objRECEANALIS.PaisSecuencia_fk = item.PaisSecuencia_fk;
                            //objRECEANALIS.ClinSecuencia_fk = item.ClinSecuencia_fk;
                            //objRECEANALIS.ConsSecuencia_fk = item.ConsSecuencia_fk;
                            //objRECEANALIS.PaciSecuencia_fk = item.PaciSecuencia_fk;
                            objUsarRecetaComplementariaEditar.AClinSecuencia_fk.Add(item.AClinSecuencia_fk);

                        }

                        //var objRECEImagenes = new Usar_RecetaImagenes();
                        //crear  int de items
                        objUsarRecetaComplementariaEditar.ImagSecuencia_fk = new List<int>();
                        //agregar todos las imagenes de esta receta
                        foreach (var item in EditreceImagenes)
                        {

                            //objRECEImagenes.DoctSecuencia_fk = item.DoctSecuencia_fk;
                            //objRECEImagenes.PaisSecuencia_fk = item.PaisSecuencia_fk;
                            //objRECEImagenes.ClinSecuencia_fk = item.ClinSecuencia_fk;
                            //objRECEImagenes.ConsSecuencia_fk = item.ConsSecuencia_fk;
                            //objRECEImagenes.PaciSecuencia_fk = item.PaciSecuencia_fk;
                            objUsarRecetaComplementariaEditar.ImagSecuencia_fk.Add(item.ImagSecuencia_fk);

                        }




                        //llenno la sesion con el role a editar
                        //para abrirlo en  el metodo principal del controler
                        Session["Usar_RecetaComplementaria"] = objUsarRecetaComplementariaEditar;

                        //existe este formulario
                        respuesta.respuesta = true;
                        respuesta.redirect = Url.Action(vista, controler);

                    }

                }
                else
                {
                    //No existe este formulario
                    respuesta.respuesta = false;
                    respuesta.error = "Este registro  no existe";

                }
            }
            return Json(respuesta);
        }

        public ActionResult Recetalista()
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


                //buscar los  registros  de roles del doctor loguiado
                var objelist = (from tf in db.Recetas
                                where tf.DoctSecuencia_fk == usu.doctSecuencia
                                && tf.EstaDesabilitado == false
                                orderby tf.ReceFecha descending
                                select tf).ToList();

                //var listObjToShow = new List<Usar_Receta>();
                var listObjToShow = new List<Usar_RecetaComplementaria>();
                //crear objeto tipo formulario y asignar a la lista de usartipoformulario
                foreach (var item in objelist)
                {
                    //var UsarObjNewToShow = new Usar_Receta();
                    var UsarObjNewToShow = new Usar_RecetaComplementaria();
                    //asignar contenido a otro objeto
                    CopyClass.CopyObject(item, ref UsarObjNewToShow);
                    //LLenar paciente
                    var objPaci = db.Pacientes.Where(ro =>
                        ro.DoctSecuencia_fk == usu.doctSecuencia
                     && ro.PaciSecuencia == item.PaciSecuencia_fk).SingleOrDefault();

                    UsarObjNewToShow.PaciSecuencia_fk = objPaci.PaciSecuencia;
                    UsarObjNewToShow.PaciNombre = objPaci.PaciNombre;
                    UsarObjNewToShow.PaciApellido1 = objPaci.PaciApellido1;
                    UsarObjNewToShow.PaciApellido2 = objPaci.PaciApellido2;
                    UsarObjNewToShow.PaciDocumento = objPaci.PaciDocumento;

                    // si  la receta a visualizar  se creo en una consulta
                    //quiere decir que este campo (RecSinConsultaNombre)   en momentos de 
                    //verlo por aqui por esta receta  estara vacio por ende en el grid
                    //se vera en blanco por eso,  yo hago esto para que se vea en el grid 
                    //pero se guardara en base de datos una vez se le de a guardar
                    UsarObjNewToShow.RecSinConsultaNombre = (objPaci.PaciNombre + " " + objPaci.PaciApellido1 + " " + objPaci.PaciApellido2 + " - " + item.RecNombre); 


                    //crear fecha en string
                    //UsarObjNewToShow.ReceFechaString = UsarObjNewToShow.ReceFecha.ToString();
                    listObjToShow.Add(UsarObjNewToShow);
                }
                ViewBag.datasource = listObjToShow;
                return View();
            }
        }

    }


}
