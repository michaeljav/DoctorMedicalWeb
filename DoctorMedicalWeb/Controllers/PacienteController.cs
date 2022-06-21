using DoctorMedicalWeb.App_Data;
using DoctorMedicalWeb.Libreria;
using DoctorMedicalWeb.Models;
using DoctorMedicalWeb.ModelsComplementarios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoctorMedicalWeb.Controllers
{
    public class PacienteController : Controller
    {
        string controler = "Paciente", vista = "Ini_Paciente", PaginaAutorizada = Paginas.pag_Paciente.ToString();
        DoctorMedicalWebEntities db = new DoctorMedicalWebEntities();
        DbContextTransaction dbtrans = null;
        //
        // GET: /Paciente/
        UsuarioLoguiado usu = null;
        public ActionResult Ini_Paciente()
        {
            ViewBag.currentYear = Lib.GetLocalDateTime().Year;
            ViewBag.currentMonth = Lib.GetLocalDateTime().Month;
            ViewBag.currentDay = Lib.GetLocalDateTime().Day;
            //ViewBag.DaysOfCurrentYear =  Lib.GetLocalDateTime().;
            //Si NO esta loguieado lo redireccionara al loguin
            if (HttpContext.Session["user"] == null)
            {
                return RedirectToAction("Index", "PaginaPresentacion");
            }

            //pARA EN LA VISTA PODER SABER A QUE  CONTROLADOR EL AJAX  LLAMARA
            ViewBag.ControlCsharp = controler;
            ViewBag.VistaCsharp = vista;

            usu = (UsuarioLoguiado)HttpContext.Session["user"];
            //ESTO LO COMENTE PARA  QUE LA SECRETARIA PUEDA TAMBIEN CREAR EL PACIENTE.
            //si es personal que esta creando un usuario, no permitir proseguir.
            //if (usu.persSecuencia > 0)
            //{
            //    TempData["NoTienePermisoParaPagina"] = "Usted no tiene Permiso!";
            //    //respuesta.respuesta = false;
            //    //respuesta.error = "Usted No tiene permiso para crear usuario. Favor solicitarle al doctor.";
            //    //return Json(respuesta);
            //    return RedirectToAction("Index", "DashBoard");
            //}

            //para que el div de accion solo aparezca si son 
            //vistas difrentes a home
            ViewBag.isHome = false;

            IEnumerable<TipoDocumento> tipoDocumento = db.TipoDocumentoes.ToList();
            ViewBag.tipoDocumentos = new SelectList(tipoDocumento, "TDSecuencia", "TDDocumento");

            IEnumerable<InstitucionesAseguradora> Insticuionaseg = db.InstitucionesAseguradoras.Where(i => i.DoctSecuencia == usu.doctSecuencia).ToList();
            ViewBag.institucionAseguBag = new SelectList(Insticuionaseg, "IAsegSecuencia", "IAsegNombre");

            IEnumerable<GrupoSanguineo> grupoSanguineo = db.GrupoSanguineos.ToList();
            ViewBag.grupoSanguineo = new SelectList(grupoSanguineo, "GSangSecuencia", "GSangNombre");

            IEnumerable<Usar_InstitucionAseguradoraPlanes> InsticuionasegPlan =  new List<Usar_InstitucionAseguradoraPlanes>();
                //db.InstitucionAseguradoraPlanes.Where(i => i.DoctSecuencia == usu.doctSecuencia).ToList(); 
            ViewBag.InsticuionasegPlanBag = new SelectList(InsticuionasegPlan, "IAPlanSecuencia", "IAPlanDescripcion");

          


            List<estadoCivil> listEsta = new List<estadoCivil> {
              new estadoCivil(){EstadoCivil ="Solter@"},
              new estadoCivil(){EstadoCivil ="Casad@"},
              new estadoCivil(){EstadoCivil ="Divolciad@"},
              new estadoCivil(){EstadoCivil ="Viud@"}
            };
            ViewBag.estCivil = new SelectList(listEsta, "EstadoCivil", "EstadoCivil");




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

            Usar_Paciente usarPaciente = new Usar_Paciente();
            //si  esta lleno es por que esta editando
            if (Session["Usar_Paciente"] != null)
            {
                usarPaciente = (Usar_Paciente)Session["Usar_Paciente"];
                
                //llenar el combo del planes del seguro del paciente para que razor seleccione el plan del paciente
                if (Session["PlanDeSeguroDelPaciente"] != null)
                {
                    //ViewBag.InsticuionasegPlanBag = (List<Usar_InstitucionAseguradoraPlanes>)Session["PlanDeSeguroDelPaciente"];
                    ViewBag.InsticuionasegPlanBag = new SelectList((List<Usar_InstitucionAseguradoraPlanes>)Session["PlanDeSeguroDelPaciente"], "IAPlanSecuencia", "IAPlanDescripcion");
                    Session["PlanDeSeguroDelPaciente"] = null;
                }
                //limpiar sesion
                Session["Usar_Paciente"] = null;
                return View(usarPaciente);
            }

            return View(usarPaciente);
        }

        //private SelectList GetPlanesSeguros(int IAseguraSecuenciavist)
          [HttpPost]
        public JsonResult GetPlanesSeguros(int IAseguraSecuenciaSelect)
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

            //Listado de planes del seguro
            IEnumerable<InstitucionAseguradoraPlane> InsticuionasegPlan = db.InstitucionAseguradoraPlanes.Where(i => i.DoctSecuencia == usu.doctSecuencia
                && i.IAsegSecuencia == IAseguraSecuenciaSelect).ToList();
           

        //llenar  plan de seguro seleccionado                  
    

        List<Usar_InstitucionAseguradoraPlanes> usar_InsticuionasegPlan = new List<Usar_InstitucionAseguradoraPlanes>();
        foreach (var item in InsticuionasegPlan)
        {
            Usar_InstitucionAseguradoraPlanes objNuevoPlanes = new Usar_InstitucionAseguradoraPlanes();
            //asignar contenido a otro objeto
            CopyClass.CopyObject(item, ref objNuevoPlanes);
            usar_InsticuionasegPlan.Add(objNuevoPlanes);
        }

              respuesta.someCollection=  new SelectList(usar_InsticuionasegPlan, "IAPlanSecuencia", "IAPlanDescripcion");


            return Json(respuesta);
        }

        //[Required]
        //public virtual int SeguroSecuencia { get; set; }
        //public SelectList getState()
        //{
        //    //IEnumerable<SelectListItem> stateList = (from m in db.mstrstates where m.bstatus == true select m).AsEnumerable().Select(m => new SelectListItem() { Text = m.vstate, Value = m.istateid.ToString() });
        //  //  IEnumerable<InstitucionAseguradoraPlane> InsticuionasegPlanes = (db.InstitucionAseguradoraPlanes.Where(i => i.DoctSecuencia ==usu.doctSecuencia && i.IAsegSecuencia == SeguroSecuencia)).AsEnumerable();
        //  //  var a = new SelectList(InsticuionasegPlanes, "IAPlanSecuencia", "IAPlanDescripcion",SeguroSecuencia);
        //  //return a;
        //  //  return new SelectList(InsticuionasegPlanes, "Value", "Text", SeguroSecuencia);
        //}

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
                    List<Paciente> ListaUltimosCinco = (from tf in db.Pacientes
                                                        where tf.DoctSecuencia_fk == usu.doctSecuencia
                                                                 && tf.EstaDesabilitado == false
                                                        orderby tf.PaciSecuencia descending
                                                        select tf).Take(5).ToList();

                    var listUsar_UltimosCincoVista = new List<Usar_Paciente>();
                    //crear objeto role y asignar a la lista de usaroles
                    foreach (var item in ListaUltimosCinco)
                    {
                        Usar_Paciente ObjetoNuevo = new Usar_Paciente();
                        //asignar contenido a otro objeto
                        CopyClass.CopyObject(item, ref ObjetoNuevo);
                        listUsar_UltimosCincoVista.Add(ObjetoNuevo);
                    }
                    //Utilizado para insertar el listado al grid
                    ViewBag.ultimosCinco = listUsar_UltimosCincoVista;


                    //ienumerable lista
                    respuesta.someCollection = listUsar_UltimosCincoVista;


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
        public JsonResult Borrar(Usar_Paciente usar_pacient)
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
                if ( string.IsNullOrEmpty(usar_pacient.PaciSecuencia.ToString()) || usar_pacient.PaciSecuencia < 1 )
                {
                    respuesta.respuesta = false;
                    respuesta.error = "Favor seleccionar un Paciente  en el listado";
                    respuesta.redirect = Url.Action(vista, controler);
                    return Json(respuesta);

                }
                using (var dbtrans = db.Database.BeginTransaction())
                {
                    try
                    {
                        //borro registro
                        if (usar_pacient != null)
                        {

                            //Paciente objetoNuevo = new Paciente();
                            ////asignar contenido a otro objeto
                            //CopyClass.CopyObject(usar_pacient, ref objetoNuevo);


                            Paciente borrar = (from objquery in db.Pacientes
                                               where objquery.DoctSecuencia_fk == usu.doctSecuencia
                                                  && objquery.PaciSecuencia == usar_pacient.PaciSecuencia
                                               select objquery).SingleOrDefault();

                            if (borrar != null)
                            {
                                borrar.EstaDesabilitado = true;
                                //db.Pacientes.Remove(borrar);
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
        public JsonResult Editar(Usar_Paciente usar_pacient)
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
                if (usar_pacient != null)
                {

                    Paciente ObjetoEditar = (from ObjQuery in db.Pacientes
                                             where ObjQuery.DoctSecuencia_fk == usu.doctSecuencia
                                             && ObjQuery.PaciSecuencia == usar_pacient.PaciSecuencia
                                                      && ObjQuery.EstaDesabilitado == false
                                             select ObjQuery).SingleOrDefault();

                    if (ObjetoEditar != null)
                    {
                        Usar_Paciente objNuevo = new Usar_Paciente();
                        //asignar contenido a otro objeto
                        CopyClass.CopyObject(ObjetoEditar, ref objNuevo);
                        //llenno la sesion con el role a editar
                        //para abrirlo en  el metodo principal del controler
                        Session["Usar_Paciente"] = objNuevo;
                        
                        //lleno el combo de los  planes por que razor se encarga de elegir cual es el  plan elegido por el  paciente                  
                        IEnumerable<InstitucionAseguradoraPlane> InsticuionasegPlan = db.InstitucionAseguradoraPlanes.Where(i => i.DoctSecuencia == usu.doctSecuencia
                            && i.IAsegSecuencia == objNuevo.IAsegSecuencia  ).ToList();

                        List<Usar_InstitucionAseguradoraPlanes> usar_InsticuionasegPlan = new List<Usar_InstitucionAseguradoraPlanes>();
                        foreach (var item in InsticuionasegPlan)
                        {
                              Usar_InstitucionAseguradoraPlanes objNuevoPlanes = new Usar_InstitucionAseguradoraPlanes();
                        //asignar contenido a otro objeto
                              CopyClass.CopyObject(item, ref objNuevoPlanes);
                              usar_InsticuionasegPlan.Add(objNuevoPlanes);
                        }
                        Session["PlanDeSeguroDelPaciente"] = usar_InsticuionasegPlan;
                        
                        //Session["PlanDeSeguroDelPaciente"] = new SelectList(usar_InsticuionasegPlan, "IAPlanSecuencia", "IAPlanDescripcion");

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
        public JsonResult Save(Usar_Paciente usar_pacient, HttpPostedFileBase file)
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

            //validar que solo sea numero en  numero seguro social y poliza
            if (!string.IsNullOrEmpty(usar_pacient.PaciNumeroSeguroSocial) )
            {              
              
                if (usar_pacient.PaciNumeroSeguroSocial.All(char.IsDigit) == false)
                {
                    respuesta.respuesta = false;
                    respuesta.error = "Usted esta realizando intento de hackeo, el campo solo permite números.";                   
                    return Json(respuesta);
                }
                
            }

            if (!string.IsNullOrEmpty(usar_pacient.PaciNumeroPoliza))
            {                              
                if (usar_pacient.PaciNumeroPoliza.All(char.IsDigit) == false)
                {
                    respuesta.respuesta = false;
                    respuesta.error = "Usted esta realizando intento de hackeo, el campo solo permite números.";
                    return Json(respuesta);
                }
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
                        if (string.IsNullOrEmpty(usar_pacient.PaciSecuencia.ToString()) || usar_pacient.PaciSecuencia == 0)
                        {Paciente objNuevo = new Paciente();

                            //asignar contenido a otro objeto
                            CopyClass.CopyObject(usar_pacient, ref objNuevo);



                            //si existe el paciente del doctor no se introduce
                            var existe = (from objbuscar in db.Pacientes
                                          where objbuscar.PaciDocumento == usar_pacient.PaciDocumento
                                               && objbuscar.TDSecuencia == usar_pacient.TDSecuencia
                                             && objbuscar.DoctSecuencia_fk == usu.doctSecuencia
                                                       && objbuscar.EstaDesabilitado == false
                                          select objbuscar).SingleOrDefault();
                            if (existe != null)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "Ya existe esta paciente";
                                respuesta.redirect = Url.Action(vista, controler);
                                return Json(respuesta);
                            }


                         




                            objNuevo.DoctSecuencia_fk = usu.doctSecuencia;
                            objNuevo.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                            objNuevo.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                            objNuevo.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                            objNuevo.UsuaSecuenciaCreacion = usu.usuario.UsuaSecuencia;
                            objNuevo.UsuaFechaCreacion = Lib.GetLocalDateTime();

                            //buscando la proxima secuencia, el doctor en todas sus consultorios tiene los mismos roles 
                            proximoItem = ((from objBusc in db.Pacientes
                                            where objBusc.DoctSecuencia_fk == usu.doctSecuencia
                                            select (int?)objBusc.PaciSecuencia).Max() ?? 0) + 1;
                            objNuevo.PaciSecuencia = proximoItem;

                            // Verify that the user selected a file
                            if (file != null && file.ContentLength > 0)
                            {
                                //get extension
                                string ext = Path.GetExtension(file.FileName);
                                // extract only the fielname
                                // Path.GetFileName(file.FileName);
                                fileName = "Paciente";
                                //fileName = Path.GetFileNameWithoutExtension(fileName).ToString()+ usu.usuario.UsuaSecuencia.ToString();
                                fileName = fileName + proximoItem.ToString();
                                fileName = fileName + ext;
                                // store the file inside ~/App_Data/uploads folder
                                path = Path.Combine(Server.MapPath("/Content/ImagenesUploads"), fileName);


                                //extraigo el directorio
                                //bool folderExists = Directory.Exists(Path.GetDirectoryName(path));
                                //sino existe  el directorio  lo creo
                                if (Directory.Exists(Path.GetDirectoryName(path)) == false)
                                {

                                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                                }


                                string imagenPath = "/Content/ImagenesUploads/" + fileName;


                                objNuevo.PaciFotoNombre = fileName;
                                objNuevo.PaciFotoPath = imagenPath;
                            }



                            //agregar codigo
                            //string PacCodigoGen = objNuevo.DoctSecuencia_fk.ToString() +
                            //            objNuevo.PaciSecuencia.ToString();
                            //var _pacCodigoGen = Lib.FormatearCodigo(8, PacCodigoGen);
                            //_pacCodigoGen = "Pc-" + _pacCodigoGen;
                            //objNuevo.PaciCodigo = _pacCodigoGen;

                            //para crear el codigo
                            string cantidadDoct = ((from objBusc in db.Pacientes
                                                        where objBusc.DoctSecuencia_fk == usu.doctSecuencia                                                     
                                                    orderby objBusc.PaciCodigo.ToString().Substring(objBusc.PaciCodigo.ToString().IndexOf("-") + 1) descending
                                                    select objBusc.PaciCodigo)).Take(1).SingleOrDefault();

                            string extraerRecSoloSecuencia = "";
                            if (!string.IsNullOrEmpty(cantidadDoct))
                            {
                                //cantidadDoct.IndexOf("-")  +1 es por que el index me trae desde donde esta este -(guion) y yo necesito
                                //desde el siguiente  letra
                                extraerRecSoloSecuencia = cantidadDoct.Substring(cantidadDoct.IndexOf("-") + 1);
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
                                extraerRecSoloSecuencia = "1";

                            }
                            //quito los ceros delanteros
                            int secMaxSinCerosDelanteros = int.Parse(extraerRecSoloSecuencia);

                            //codigo con ceros doctor y secuencia
                            string codigDoctFormateado = Lib.FormatearCodigo(3, usu.doctSecuencia.ToString());
                            string codiSecGeneral = Lib.FormatearCodigo(6, secMaxSinCerosDelanteros.ToString());
                            string codFormateadoDefiniRec = codigDoctFormateado + codiSecGeneral;
                            codFormateadoDefiniRec = "P-" + codFormateadoDefiniRec;
                            objNuevo.PaciCodigo = codFormateadoDefiniRec;

                            db.Pacientes.Add(objNuevo);
                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Nuevo.ToString(), null);
                        }
                        //Editando 
                        else
                        {

                            //busco el objeto que se editara
                            //lo busco por la secuencia para que pueda editar ese mismo sin tener que borrarlo
                            //si lo busco por el nombre no lo  editar ya que no lo encontrara.
                            var objetoEditar = db.Pacientes.Where(ro => ro.DoctSecuencia_fk == usu.doctSecuencia
                                            && ro.PaciSecuencia == usar_pacient.PaciSecuencia).SingleOrDefault();

                            if (objetoEditar == null)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "No existe esta paciente";
                                respuesta.redirect = Url.Action(vista, controler);
                                return Json(respuesta);
                            }

                            //asigno al objeto que se editara los nuevos datos introducidos a editar
                            CopyClass.CopyObject(usar_pacient, ref objetoEditar);


                            objetoEditar.UsuaSecuenciaModificacion = usu.usuario.UsuaSecuencia;
                            objetoEditar.UsuaFechaModificacion = Lib.GetLocalDateTime();

                            // Verify that the user selected a file
                            if (file != null && file.ContentLength > 0)
                            {
                                //get extension
                                string ext = Path.GetExtension(file.FileName);
                                // extract only the fielname
                                // Path.GetFileName(file.FileName);
                                fileName = "Paciente";
                                //fileName = Path.GetFileNameWithoutExtension(fileName).ToString()+ usu.usuario.UsuaSecuencia.ToString();
                                fileName = fileName + objetoEditar.PaciSecuencia.ToString();
                                fileName = fileName + ext;
                                // store the file inside ~/App_Data/uploads folder
                                path = Path.Combine(Server.MapPath("/Content/ImagenesUploads"), fileName);


                                //extraigo el directorio
                                //bool folderExists = Directory.Exists(Path.GetDirectoryName(path));
                                //sino existe  el directorio  lo creo
                                if (Directory.Exists(Path.GetDirectoryName(path)) == false)
                                {

                                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                                }


                                string imagenPath = "/Content/ImagenesUploads/" + fileName;


                                objetoEditar.PaciFotoNombre = fileName;
                                objetoEditar.PaciFotoPath = imagenPath;
                            }


                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Editar.ToString(), null);
                        }

                       int returnsave= db.SaveChanges();
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

        public ActionResult Pacientelista()
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
                List<Paciente> objetoEnlistar = (from tf in db.Pacientes
                                                 where tf.DoctSecuencia_fk == usu.doctSecuencia
                                                 && tf.EstaDesabilitado==false
                                                 orderby tf.PaciNombre
                                                 select tf).ToList();

                var listObjetoNuevo = new List<Usar_Paciente>();
                //crear objeto tipo formulario y asignar a la lista de usartipoformulario
                foreach (var item in objetoEnlistar)
                {
                    Usar_Paciente objNuevoParaLista = new Usar_Paciente();
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
