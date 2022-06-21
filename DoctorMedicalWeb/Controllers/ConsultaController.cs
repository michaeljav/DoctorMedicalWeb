using DoctorMedicalWeb.App_Data;
using DoctorMedicalWeb.Libreria;
using DoctorMedicalWeb.Models;
using DoctorMedicalWeb.ModelsComplementarios;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoctorMedicalWeb.Controllers
{


    public class ConsultaController : Controller
    {
        //
        // GET: /Consulta/
        string controler = "Consulta", vista = "Ini_Consulta", PaginaAutorizada = Paginas.pag_Consulta.ToString();
        public ActionResult Ini_Consulta()
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

                //Enlistar los 5 ultimos y lleno
                //el   ViewBag.ultimosCinco
                var a = UltimosCincoRegistros();

                //Lista de Motivos de consulta del doctor
                List<MotivoConsulta> MotivoConsultaLIs = ((from objSearch in db.MotivoConsultas
                                                           where objSearch.DoctSecuencia_fk == usu.doctSecuencia
                                                           select objSearch).ToList());
                ViewBag.MotivoConsulta = MotivoConsultaLIs;

                //Lista de Evaluacion fisica
                List<EvaluacionFisica> EvaluacionFisicaLIs = ((from objSearch in db.EvaluacionFisicas
                                                               where objSearch.DoctSecuencia_fk == usu.doctSecuencia
                                                               select objSearch).ToList());
                ViewBag.EvaluacionFisica = EvaluacionFisicaLIs;

                //Lista de Diagnostico
                List<Diagnostico> DiagnosticoLIs = ((from objSearch in db.Diagnosticoes
                                                     where objSearch.DoctSecuencia_fk == usu.doctSecuencia
                                                     select objSearch).ToList());
                ViewBag.Diagnostico = DiagnosticoLIs;

                //Lista de Tratamiento
                List<Tratamiento> TratamientoLIs = ((from objSearch in db.Tratamientoes
                                                     where objSearch.DoctSecuencia_fk == usu.doctSecuencia
                                                     select objSearch).ToList());
                ViewBag.Tratamiento = TratamientoLIs;



                IEnumerable<GrupoSanguineo> grupoSanguineo = db.GrupoSanguineos.ToList();
                ViewBag.grupoSanguineo = new SelectList(grupoSanguineo, "GSangSecuencia", "GSangNombre");

                List<PatronMenstrual> listPatronMenstrual = new List<PatronMenstrual> {
                      new PatronMenstrual(){PropiedadpatronMenstual ="Regular"},
                      new PatronMenstrual(){PropiedadpatronMenstual ="Irregular"}                       
            };
                ViewBag.patronMenstrual = new SelectList(listPatronMenstrual, "PropiedadpatronMenstual", "PropiedadpatronMenstual");

                List<DuracionMenstruacion> listDuracionMenstruacion = new List<DuracionMenstruacion> {
                      new DuracionMenstruacion(){duracionMenstruacion ="Normal"},
                      new DuracionMenstruacion(){duracionMenstruacion ="Anormal"}                       
            };
                ViewBag.duracionMenstrual = new SelectList(listDuracionMenstruacion, "duracionMenstruacion", "duracionMenstruacion");

                List<AntecedentesFamiliar> listAntecedentesFamiliar = new List<AntecedentesFamiliar> {
                      new AntecedentesFamiliar(){Familiar ="Madre"},
                       new AntecedentesFamiliar(){Familiar ="Padre"},
                      new AntecedentesFamiliar(){Familiar ="Hermanos"},
                       new AntecedentesFamiliar(){Familiar ="Otros"}     
            };
                ViewBag.familiar = new SelectList(listAntecedentesFamiliar, "Familiar", "Familiar");

                List<TipoIndicacionMedica> listTipoIndicacionMedica = new List<TipoIndicacionMedica> {
                      new TipoIndicacionMedica(){tipoIndicacionMedica ="Medicamentos"},
                       new TipoIndicacionMedica(){tipoIndicacionMedica ="Analíticas"},
                      new TipoIndicacionMedica(){tipoIndicacionMedica ="Imagenes"}  
            };
                ViewBag.tipoIndicacionMedica = new SelectList(listTipoIndicacionMedica, "tipoIndicacionMedica", "tipoIndicacionMedica");


                List<Menarquia> listMenarquia = new List<Menarquia> {
                      new Menarquia(){menarquia ="Menor de 9"},
                       new Menarquia(){menarquia ="9-16"},
                      new Menarquia(){menarquia ="Mayor de 16"}                       
                        
            };
                ViewBag.menarquia = new SelectList(listMenarquia, "menarquia", "menarquia");


                List<Enfermedad> enferme = ((from objSearch in db.Enfermedads
                                             where objSearch.DoctSecuencia_fk == usu.doctSecuencia
                                             select objSearch).ToList());
                ViewBag.Enfermedades = enferme;


                //Lista de Unidad de Peso

                List<UnidadesDePeso> listUnidadesDePeso = new List<UnidadesDePeso> {
                      new UnidadesDePeso(){UnidadPeso ="Lbs"},
              new UnidadesDePeso(){UnidadPeso ="Kgs"}
                       
            };
                ViewBag.unidadPeso = new SelectList(listUnidadesDePeso, "UnidadPeso", "UnidadPeso");

                List<UnidadDeLongitud> listUnidadDeLongitud = new List<UnidadDeLongitud> {
              new UnidadDeLongitud(){unidadLongitud ="pie(s)"},
              new UnidadDeLongitud(){unidadLongitud ="mt(s)."}           
            };
                ViewBag.unidadLongitud = new SelectList(listUnidadDeLongitud, "unidadLongitud", "unidadLongitud", 1);


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


                //Lista de pacientes
                List<Paciente> PacienteLIs = ((from objSearch in db.Pacientes
                                               where objSearch.DoctSecuencia_fk == usu.doctSecuencia
                                               && objSearch.EstaDesabilitado == false
                                               select objSearch).ToList());


                List<PacienteInfo> pacil = new List<PacienteInfo>();
                foreach (var item in PacienteLIs)
                {
                    PacienteInfo pac = new PacienteInfo();
                    //asignar contenido a otro objeto
                    CopyClass.CopyObject(item, ref pac);
                    pacil.Add(pac);
                }
                ViewBag.listPacientes = pacil;



                Usar_Consultar usarConsultar = new Usar_Consultar();

                //si  esta lleno es por que esta editando
                if (Session["Usar_Consultar"] != null)
                {
                    usarConsultar = (Usar_Consultar)Session["Usar_Consultar"];
                    //limpiar sesion
                    Session["Usar_Consultar"] = null;


                    //Para llenar checkbox  motivo de consulta
                    ViewBag.CheckMotivo = usarConsultar.MConsSecuencia_fk;

                    //para llenar checkbox evaluacion fisica
                    ViewBag.CheckEvaluacionFisi = usarConsultar.EFisiSecuencia_fk;

                    //para llenar checkbox  diagnostico
                    ViewBag.CheckDiagno = usarConsultar.DiagSecuencia;

                    //para llenar checkbox tratamiento
                    ViewBag.CheckTrata = usarConsultar.TratSecuencia_fk;

                    //para llenar checkbox enfermedades personales
                    ViewBag.CheckEnferPerso = usarConsultar.EnfeSecuenciaPersonal;

                    if (Session["Usar_RecetaComplementariaList"] != null)
                    {

                        //llenar  recetas
                        ViewBag.RecetasConsulta = (List<Usar_RecetaComplementaria>)Session["Usar_RecetaComplementariaList"];
                        //limpiar sesion
                        Session["Usar_RecetaComplementariaList"] = null;
                        //Nota
                        //Medicamentos, analisis clinico e imagenes  de la receta 
                        //los lleno  con jquery al seleccionar  el grid                  


                    }

                    return View(usarConsultar);
                }

                //La especialidad del usuario  doctor que esta loguiado
                //esto cuando tenga otra especialidad
                //llamare una vista acorde a su especialidad
                ViewBag.Especialidad = usu.usuario.EspeSecuencia_fk;

                //usarConsultar.PaciNombre = "Michael";
                //usarConsultar.PaciApellido1 = "javier";
                //usarConsultar.PaciApellido2 = "Mota";
                //usarConsultar.PaciEdad = 50;
                //usarConsultar.PaciDocumento = "02900161072";
                //   usarConsultar.PaciFotoPath = "/Content/AdminFiles/images/_user-alt.png";
                //usarConsultar.FechaNacimientoString = Lib.GetLocalDateTime().Date.ToString("dd-MMM-yyyy");

                return View(usarConsultar);
            }

        }

        [HttpPost]
        public JsonResult buscarConsultasAnteriores(int paciente)
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


                using (var dbtrans = db.Database.BeginTransaction())
                {

                    try
                    {
                        if (paciente <= 0)
                        {
                            respuesta.respuesta = false;
                            respuesta.error = "Error.";
                            respuesta.redirect = Url.Action(vista, controler);
                            return Json(respuesta);
                        }

                        //si existe la insituciona aseguradora del doctor no se introduce
                        var historiales = (from objHistorial in db.ConsultaMedicaHistorials
                                           where objHistorial.DoctSecuencia_fk == usu.doctSecuencia
                                          && objHistorial.PaciSecuencia_fk == paciente
                                          && objHistorial.EstaDesabilitado == false
                                           select objHistorial).ToList();
                        if (historiales == null || historiales.Count <= 0)
                        {
                            respuesta.respuesta = false;
                            respuesta.menssage = "No existe consultas anteriores.";
                            respuesta.redirect = Url.Action(vista, controler);
                            return Json(respuesta);
                        }

                        respuesta.respuesta = true;
                        List<Usar_ConsultaMedicaHistorial> objList = new List<Usar_ConsultaMedicaHistorial>();
                        foreach (var item in historiales)
                        {
                            Usar_ConsultaMedicaHistorial obj = new Usar_ConsultaMedicaHistorial();
                            //asignar contenido a otro objeto
                            CopyClass.CopyObject(item, ref obj);
                            objList.Add(obj);
                        }

                        respuesta.someCollection = objList;

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
                        //return (respuesta, JsonRequestBehavior.AllowGet); 
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


            //Si NO  ha introducido datos
            if (string.IsNullOrEmpty(maintenance.Nombre))
            {
                respuesta.respuesta = false;
                respuesta.error = "Favor introduzca nombre.";
                return Json(respuesta);
            }


            try
            {
                if (maintenance != null)
                {

                    var value = maintenance.Maintenance;
                    switch (value)
                    {

                        case Maintenance.MotivoConsulta:
                            using (var db = new DoctorMedicalWebEntities())
                            {
                                //guardar motivo consulta
                                Usar_MotivoConsulta usarMotivo = new Usar_MotivoConsulta();
                                usarMotivo.MConsMotivoConsulta = maintenance.Nombre;
                                usarMotivo.MConsDescripcion = maintenance.Descripcion;
                                usarMotivo.EstaDesabilitado = false;
                                respuesta = SaveMotivoConsulta(usarMotivo);

                                respuesta.obj = Maintenance.MotivoConsulta.ToString();

                            }
                            break;
                        case Maintenance.EvaluacionFisica:
                            using (var db = new DoctorMedicalWebEntities())
                            {
                                Usar_EvaluacionFisica usarEvaluaFisi = new Usar_EvaluacionFisica();
                                usarEvaluaFisi.EFisiCodigoNombre = maintenance.Nombre;
                                usarEvaluaFisi.EFisiDescripcion = maintenance.Descripcion;
                                usarEvaluaFisi.EstaDesabilitado = false;
                                respuesta = SaveEvaluacionFisica(usarEvaluaFisi);
                                respuesta.obj = Maintenance.EvaluacionFisica.ToString();

                            }
                            break;

                        case Maintenance.Diagnostico:
                            using (var db = new DoctorMedicalWebEntities())
                            {
                                Usar_Diagnostico usarDiagnostico = new Usar_Diagnostico();
                                usarDiagnostico.DiagNombre = maintenance.Nombre;
                                usarDiagnostico.DiagDescripcion = maintenance.Descripcion;
                                usarDiagnostico.EstaDesabilitado = false;
                                respuesta = SaveDiagnostico(usarDiagnostico);
                                respuesta.obj = Maintenance.Diagnostico.ToString();

                            }
                            break;

                        case Maintenance.Tratamiento:
                            using (var db = new DoctorMedicalWebEntities())
                            {
                                Usar_Tratamiento usarTratamiento = new Usar_Tratamiento();
                                usarTratamiento.TratNombre = maintenance.Nombre;
                                usarTratamiento.TratDescripcion = maintenance.Descripcion;
                                usarTratamiento.EstaDesabilitado = false;
                                respuesta = SaveTratamiento(usarTratamiento);
                                respuesta.obj = Maintenance.Tratamiento.ToString();

                            }
                            break;

                        case Maintenance.Enfermedad:
                            using (var db = new DoctorMedicalWebEntities())
                            {
                                Usar_Enfermedad usarEnfermedad = new Usar_Enfermedad();
                                usarEnfermedad.EnfeNombre = maintenance.Nombre;
                                usarEnfermedad.EnfeDescripcion = maintenance.Descripcion;
                                respuesta = SaveEnfermedad(usarEnfermedad);
                                respuesta.obj = Maintenance.Enfermedad.ToString();

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

            //Si NO  ha introducido datos
            if (string.IsNullOrEmpty(usar_AnalisisClinico.AClinNombre))
            {
                respuesta.respuesta = false;
                respuesta.error = "Favor introduzca nombre.";
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

            //Si NO  ha introducido datos
            if (string.IsNullOrEmpty(usar_Imagenes.ImagNombre))
            {
                respuesta.respuesta = false;
                respuesta.error = "Favor introduzca nombre.";
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


        public ResponseModel SaveEnfermedad(Usar_Enfermedad usar_Enfermedad)
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
                        if (string.IsNullOrEmpty(usar_Enfermedad.EnfeSecuencia.ToString()) || usar_Enfermedad.EnfeSecuencia == 0)
                        {
                            var objToSave = new Enfermedad();

                            //asignar contenido a otro objeto
                            CopyClass.CopyObject(usar_Enfermedad, ref objToSave);

                            //si existe la insituciona aseguradora del doctor no se introduce
                            var existe = (from objExist in db.Enfermedads
                                          where objExist.EnfeNombre == usar_Enfermedad.EnfeNombre
                                              && objExist.DoctSecuencia_fk == usu.doctSecuencia
                                          select objExist).SingleOrDefault();
                            if (existe != null)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "Ya existe esta enfermedad.";
                                respuesta.redirect = Url.Action(vista, controler);
                                return (respuesta);
                            }


                            objToSave.DoctSecuencia_fk = usu.doctSecuencia;
                            objToSave.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                            objToSave.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                            objToSave.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;

                            //buscando la proxima secuencia, el doctor en todas sus consultorios tiene los mismos roles 
                            proximoItem = ((from numSecu in db.Enfermedads
                                            where numSecu.DoctSecuencia_fk == usu.doctSecuencia
                                            select (int?)numSecu.EnfeSecuencia).Max() ?? 0) + 1;
                            objToSave.EnfeSecuencia = proximoItem;

                            db.Enfermedads.Add(objToSave);

                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Nuevo.ToString(), null);
                        }


                        db.SaveChanges();
                        dbtrans.Commit();

                        respuesta.respuesta = true;
                        respuesta.redirect = Url.Content("~/" + controler + "/" + vista + "");


                        List<Enfermedad> listObj = (from objExist in db.Enfermedads
                                                    where
                                                        objExist.DoctSecuencia_fk == usu.doctSecuencia
                                                    select objExist).ToList();
                        List<Usar_Enfermedad> objList = new List<Usar_Enfermedad>();
                        foreach (var item in listObj)
                        {
                            Usar_Enfermedad obj = new Usar_Enfermedad();
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

        public ResponseModel SaveTratamiento(Usar_Tratamiento usar_Tratamiento)
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
                        if (string.IsNullOrEmpty(usar_Tratamiento.TratSecuencia.ToString()) || usar_Tratamiento.TratSecuencia == 0)
                        {
                            Tratamiento objToSave = new Tratamiento();

                            //asignar contenido a otro objeto
                            CopyClass.CopyObject(usar_Tratamiento, ref objToSave);

                            //si existe la insituciona aseguradora del doctor no se introduce
                            var existe = (from objExist in db.Tratamientoes
                                          where objExist.TratNombre == usar_Tratamiento.TratNombre
                                              && objExist.DoctSecuencia_fk == usu.doctSecuencia
                                          select objExist).SingleOrDefault();
                            if (existe != null)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "Ya existe este tratamiento.";
                                respuesta.redirect = Url.Action(vista, controler);
                                return (respuesta);
                            }


                            objToSave.DoctSecuencia_fk = usu.doctSecuencia;
                            objToSave.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                            objToSave.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                            objToSave.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;

                            //buscando la proxima secuencia, el doctor en todas sus consultorios tiene los mismos roles 
                            proximoItem = ((from numSecu in db.Tratamientoes
                                            where numSecu.DoctSecuencia_fk == usu.doctSecuencia
                                            select (int?)numSecu.TratSecuencia).Max() ?? 0) + 1;
                            objToSave.TratSecuencia = proximoItem;

                            db.Tratamientoes.Add(objToSave);

                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Nuevo.ToString(), null);
                        }

                        db.SaveChanges();
                        dbtrans.Commit();

                        respuesta.respuesta = true;
                        respuesta.redirect = Url.Content("~/" + controler + "/" + vista + "");

                        List<Tratamiento> listObj = (from objExist in db.Tratamientoes
                                                     where
                                                         objExist.DoctSecuencia_fk == usu.doctSecuencia
                                                     select objExist).ToList();
                        List<Usar_Tratamiento> objList = new List<Usar_Tratamiento>();
                        foreach (var item in listObj)
                        {
                            Usar_Tratamiento obj = new Usar_Tratamiento();
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


        public ResponseModel SaveDiagnostico(Usar_Diagnostico usar_Diagnostico)
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
                        if (string.IsNullOrEmpty(usar_Diagnostico.DiagSecuencia.ToString()) || usar_Diagnostico.DiagSecuencia == 0)
                        {
                            Diagnostico diagnost = new Diagnostico();

                            //asignar contenido a otro objeto
                            CopyClass.CopyObject(usar_Diagnostico, ref diagnost);

                            //si existe la insituciona aseguradora del doctor no se introduce
                            var existe = (from dgtico in db.Diagnosticoes
                                          where dgtico.DiagNombre == usar_Diagnostico.DiagNombre
                                              && dgtico.DoctSecuencia_fk == usu.doctSecuencia
                                          select dgtico).SingleOrDefault();
                            if (existe != null)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "Ya existe este diagnostico";
                                respuesta.redirect = Url.Action(vista, controler);
                                return (respuesta);
                            }


                            diagnost.DoctSecuencia_fk = usu.doctSecuencia;
                            diagnost.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                            diagnost.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                            diagnost.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;

                            //buscando la proxima secuencia, el doctor en todas sus consultorios tiene los mismos roles 
                            proximoItem = ((from diag in db.Diagnosticoes
                                            where diag.DoctSecuencia_fk == usu.doctSecuencia
                                            select (int?)diag.DiagSecuencia).Max() ?? 0) + 1;
                            diagnost.DiagSecuencia = proximoItem;

                            db.Diagnosticoes.Add(diagnost);

                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Nuevo.ToString(), null);
                        }


                        db.SaveChanges();
                        dbtrans.Commit();

                        respuesta.respuesta = true;
                        respuesta.redirect = Url.Content("~/" + controler + "/" + vista + "");



                        List<Diagnostico> listObj = (from objExist in db.Diagnosticoes
                                                     where
                                                         objExist.DoctSecuencia_fk == usu.doctSecuencia
                                                     select objExist).ToList();
                        List<Usar_Diagnostico> objList = new List<Usar_Diagnostico>();
                        foreach (var item in listObj)
                        {
                            Usar_Diagnostico obj = new Usar_Diagnostico();
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


        public ResponseModel SaveEvaluacionFisica(Usar_EvaluacionFisica usar_EvaluacionFisica)
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
                        if (string.IsNullOrEmpty(usar_EvaluacionFisica.EFisiSecuencia.ToString()) || usar_EvaluacionFisica.EFisiSecuencia == 0)
                        {
                            var objToSave = new EvaluacionFisica();

                            //asignar contenido a otro objeto
                            CopyClass.CopyObject(usar_EvaluacionFisica, ref objToSave);

                            //si existe la insituciona aseguradora del doctor no se introduce
                            var existe = (from objExist in db.EvaluacionFisicas
                                          where objExist.EFisiCodigoNombre == usar_EvaluacionFisica.EFisiCodigoNombre
                                              && objExist.DoctSecuencia_fk == usu.doctSecuencia
                                          select objExist).SingleOrDefault();
                            if (existe != null)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "Ya existe esta evaluación física.";
                                respuesta.redirect = Url.Action(vista, controler);
                                return (respuesta);
                            }


                            objToSave.DoctSecuencia_fk = usu.doctSecuencia;
                            objToSave.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                            objToSave.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                            objToSave.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;

                            //buscando la proxima secuencia, el doctor en todas sus consultorios tiene los mismos roles 
                            proximoItem = ((from numSecu in db.EvaluacionFisicas
                                            where numSecu.DoctSecuencia_fk == usu.doctSecuencia
                                            select (int?)numSecu.EFisiSecuencia).Max() ?? 0) + 1;

                            objToSave.EFisiSecuencia = proximoItem;
                            db.EvaluacionFisicas.Add(objToSave);

                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Nuevo.ToString(), null);
                        }


                        db.SaveChanges();
                        dbtrans.Commit();

                        respuesta.respuesta = true;
                        respuesta.redirect = Url.Content("~/" + controler + "/" + vista + "");


                        List<EvaluacionFisica> listObj = (from objExist in db.EvaluacionFisicas
                                                          where
                                                              objExist.DoctSecuencia_fk == usu.doctSecuencia
                                                          select objExist).ToList();
                        List<Usar_EvaluacionFisica> objList = new List<Usar_EvaluacionFisica>();
                        foreach (var item in listObj)
                        {
                            Usar_EvaluacionFisica obj = new Usar_EvaluacionFisica();
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


        public ResponseModel SaveMotivoConsulta(Usar_MotivoConsulta usar_MotivoConsulta)
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

            //si no ha validado volver y mostrar mensajes de validacion
            if (!ModelState.IsValid)
            {
                respuesta.respuesta = false;
                //respuesta.redirect = Url.Content("~/Roles/Roles");
                respuesta.redirect = Url.Content("~/" + controler + "/" + vista + "");
                respuesta.error = "Llenar Campos requeridos ";
                return (respuesta);
            }

            UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];

            using (var db = new DoctorMedicalWebEntities())
            {
                int proximoItem = 0;

                using (var dbtrans = db.Database.BeginTransaction())
                {

                    try
                    {

                        //add new 
                        if (string.IsNullOrEmpty(usar_MotivoConsulta.MConsSecuencia.ToString()) || usar_MotivoConsulta.MConsSecuencia == 0)
                        {
                            MotivoConsulta motiv = new MotivoConsulta();

                            //asignar contenido a otro objeto
                            CopyClass.CopyObject(usar_MotivoConsulta, ref motiv);

                            //si existe la insituciona aseguradora del doctor no se introduce
                            var existe = (from objExist in db.MotivoConsultas
                                          where objExist.MConsMotivoConsulta == usar_MotivoConsulta.MConsMotivoConsulta
                                              && objExist.DoctSecuencia_fk == usu.doctSecuencia
                                          select objExist).SingleOrDefault();
                            if (existe != null)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "Ya existe este motivo de consulta";
                                respuesta.redirect = Url.Action(vista, controler);
                                return (respuesta);
                            }


                            motiv.DoctSecuencia_fk = usu.doctSecuencia;
                            motiv.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                            motiv.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                            motiv.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;

                            //buscando la proxima secuencia, el doctor en todas sus consultorios tiene los mismos roles 
                            proximoItem = ((from diag in db.MotivoConsultas
                                            where diag.DoctSecuencia_fk == usu.doctSecuencia
                                            select (int?)diag.MConsSecuencia).Max() ?? 0) + 1;
                            motiv.MConsSecuencia = proximoItem;

                            db.MotivoConsultas.Add(motiv);

                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Nuevo.ToString(), null);
                        }


                        db.SaveChanges();
                        dbtrans.Commit();
                        List<MotivoConsulta> listObj = (from objExist in db.MotivoConsultas
                                                        where
                                                            objExist.DoctSecuencia_fk == usu.doctSecuencia
                                                        select objExist).ToList();
                        List<Usar_MotivoConsulta> objList = new List<Usar_MotivoConsulta>();
                        foreach (var item in listObj)
                        {
                            Usar_MotivoConsulta obj = new Usar_MotivoConsulta();
                            //asignar contenido a otro objeto
                            CopyClass.CopyObject(item, ref obj);
                            objList.Add(obj);
                        }

                        respuesta.someCollection = objList;

                        respuesta.respuesta = true;
                        respuesta.redirect = Url.Content("~/" + controler + "/" + vista + "");

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
                        //return Json(respuesta, JsonRequestBehavior.AllowGet);
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
        public JsonResult validFechaProbableParto(string fechEmbarazo, string fechProbableParto)
        {
            var respuesta = new ResponseModel();
            Dictionary<string, object> valores = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(fechEmbarazo) && !string.IsNullOrEmpty(fechProbableParto))
            {
                //    string[] DatesFormat =  {
                //    "dd/MM/yyyy",                 
                //    "d/MM/yyyy",
                //    "dd/M/yyyy",
                //    "d/M/yyyy",
                //    "d/M/yy"
                //};
                DateTime fEmbarazo = DateTime.ParseExact(fechEmbarazo, "dd/MM/yyyy", new CultureInfo("en-US"));
                DateTime fProbableEmbara = DateTime.ParseExact(fechProbableParto, "dd/MM/yyyy", new CultureInfo("en-US"));

                //si fecha probable es menor que fecha embarazo no puede procesguir
                if (fProbableEmbara <= fEmbarazo)
                {
                    valores.Add("FprobMenorIgual", true);
                }
                else { valores.Add("FprobMenorIgual", false); }


                //si fecha probable no puede ser menor de la fecha actual 
                if (fProbableEmbara < Lib.GetLocalDateTime().Date)
                {
                    valores.Add("FprobMenorFechaActual", true);
                }
                else { valores.Add("FprobMenorFechaActual", false); }



                LocalDate start = new LocalDate(fEmbarazo.Year, fEmbarazo.Month, fEmbarazo.Day);
                LocalDate end = new LocalDate(fProbableEmbara.Year, fProbableEmbara.Month, fProbableEmbara.Day);


                Period TranscurridosDias = Period.Between(start, end, PeriodUnits.Days);
                //si fecha probable no puede ser mayor a  280 dias = 40 semanas
                if (TranscurridosDias.Days > 280)
                {

                    valores.Add("FproTieneMas40Week", true);
                }
                else { valores.Add("FproTieneMas40Week", false); }



                respuesta.respuesta = true;
                respuesta.dictionaryStringObjec = valores;

            }
            return Json(respuesta);

        }

          [HttpPost]
        public JsonResult validFecha(string fechEmbarazo)
        {
            var respuesta = new ResponseModel();

            if (!string.IsNullOrEmpty(fechEmbarazo))
            {

                DateTime dt = DateTime.ParseExact(fechEmbarazo, "dd/MM/yyyy", new CultureInfo("en-US"));
                //la fecha debe de ser menor a la actuar 
                if (dt.Date > Lib.GetLocalDateTime().Date)
                {

                    respuesta.respuesta = true;

                }
            }
            return Json(respuesta);

        }

          [HttpPost]
        public JsonResult buscarFechaProbableParto(string fechEmbarazo)
        {
            var respuesta = new ResponseModel();

            if (!string.IsNullOrEmpty(fechEmbarazo))
            {
                //ESTO CONVIERTE AL FORMATO MES DIA ANIO
                //DateTime dt = DateTime.ParseExact(fechEmbarazo, "dd/MM/yyyy",  new CultureInfo("en-US"));
                DateTime dt = DateTime.ParseExact(fechEmbarazo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                DateTime DateActual = Lib.GetLocalDateTime().Date;
                //DateTime dt = DateTime.Parse(fechEmbarazo);
                //la fecha debe de ser menor a la actuar 
                if (dt.Date <= DateActual)
                {
                    respuesta.menssage = obtenerFechaProbable(fechEmbarazo);
                    respuesta.respuesta = true;
                }
            }
            return Json(respuesta);

        }

        public string obtenerFechaProbable(string fechaEmbarazo)
        {
            if (!string.IsNullOrEmpty(fechaEmbarazo))
            {


                DateTime Inicio = DateTime.ParseExact(fechaEmbarazo, "dd/MM/yyyy", new CultureInfo("en-US"));
                DateTime FechaProbable = Inicio;

                bool Tiene40Semanas = false;
                ////mientras no tenga 40 semanas no parara de agragar una semana a la fecha actual
                while (!Tiene40Semanas)
                {
                    //agrego de 7 en 7 hasta llegar a las 40 semanas

                    FechaProbable = FechaProbable.AddDays(7);
                    var diasTranscurridos = (FechaProbable - Inicio).TotalDays;
                    var semanas = diasTranscurridos / 7;
                    if (semanas == 40.0)
                    {
                        Tiene40Semanas = true;
                        break;

                    }
                }
                return FechaProbable.ToShortDateString();
            }

            return "";
        }

        private DatosEmbarazo DatosEmbarazo(DateTime comienzo)
        {
            //DateTime FechaActual = Lib.GetLocalDateTime().Date;
            LocalDate FechaActual = new LocalDate(Lib.GetLocalDateTime().Year, Lib.GetLocalDateTime().Month, Lib.GetLocalDateTime().Day);
            //la fecha de embarazo siempre debe de ser de fecha actual para atras

            //agrego de 7 en 7 hasta llegar a las 40 semanas
            //DateTime comienzo = new DateTime(2017, 05, 7);
            DatosEmbarazo demb = new DatosEmbarazo();


            if (!string.IsNullOrEmpty(comienzo.ToShortDateString()))
            {

                LocalDate start = new LocalDate(comienzo.Year, comienzo.Month, comienzo.Day);
                //DateTime fin = DateTime.Parse(obtenerFechaProbable(comienzo.ToShortDateString()));

                Period TranscurridosDias = Period.Between(start, FechaActual, PeriodUnits.Days);
                Period TranscurridosSemanas = Period.Between(start, FechaActual, PeriodUnits.Weeks);
                Period TranscurridosMeses = Period.Between(start, FechaActual, PeriodUnits.Months);



                demb.TranscurridosSemanas = TranscurridosSemanas.Weeks;
                demb.TranscurridosDias = TranscurridosDias.Days;
                demb.TranscurridosMeses = TranscurridosMeses.Months;


            }



            return demb;
        }
          [HttpPost]
        public JsonResult calulosdeEmbarazo(string fechEmbarazo)
        {
            var respuesta = new ResponseModel();

            if (!string.IsNullOrEmpty(fechEmbarazo))
            {

                DateTime d = DateTime.ParseExact(fechEmbarazo, "dd/MM/yyyy", new CultureInfo("en-US"));
                DatosEmbarazo dem = DatosEmbarazo(new DateTime(d.Year, d.Month, d.Day));
                respuesta.respuesta = true;
                respuesta.obj = dem;
            }
            return Json(respuesta);

        }

          [HttpPost]
        public JsonResult buscarAntecedentesPaciente(int idPaciente)
        {

            var respuesta = new ResponseModel();
            //    //Si NO esta loguieado lo redireccionara al loguin
            if (HttpContext.Session["user"] == null)
            {
                respuesta.respuesta = false;
                respuesta.error = "Usted debe de loguiarse.";
                respuesta.redirect = Url.Action("Index", "PaginaPresentacion");
                return Json(respuesta);
            }
            UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];
            if (idPaciente > 0)
            {
                using (var db = new DoctorMedicalWebEntities())
                {



                    //Send consult Patiens's background 
                    ConsultaMedica consulMedica = (from d in db.ConsultaMedicas
                                                   where d.DoctSecuencia_fk == usu.doctSecuencia
                                                   && d.PaciSecuencia_fk == idPaciente
                                                   select d).FirstOrDefault();

                    //tuve que pasar la data a otro objeto por que me daba error
                    //considero que es por  la referencia a otros objeto.
                    ConsultaMedicaAntecedentes consultMediAntecedent = new ConsultaMedicaAntecedentes();
                    CopyClass.CopyObject(consulMedica, ref consultMediAntecedent);


                    //enfermedades familiares
                    ConsultaMedicaEnfermedaFamiliar consulMedicaEnfermeFamiliar = (from d in db.ConsultaMedicaEnfermedaFamiliars
                                                                                   where d.DoctSecuencia_fk == usu.doctSecuencia
                                                                                   && d.PaciSecuencia_fk == idPaciente
                                                                                   select d).FirstOrDefault();
                    //enfermedades personales
                    List<int> consulMedicaEnfermePersonal = new List<int>();
                    consulMedicaEnfermePersonal = (from d in db.ConsultaMedicaEnfermedas
                                                   where d.DoctSecuencia_fk == usu.doctSecuencia
                                                   && d.PaciSecuencia_fk == idPaciente
                                                   select d.EnfeSecuencia_fk).ToList();



                    Dictionary<string, object> dictionaryStringObjec = new Dictionary<string, object>();
                    Dictionary<string, List<object>> dictionaryStringListObjec = new Dictionary<string, List<object>>();

                    //someCollection
                    //ObjectGridList
                    //Ultimo historial para saber si esta embarazada
                    if (consulMedica != null)
                    {
                        //consultaMedica
                        dictionaryStringObjec.Add("consulMedica", (object)consultMediAntecedent);

                    }
                    else
                    {
                        dictionaryStringObjec.Add("consulMedica", null);
                    }


                    if (consulMedicaEnfermePersonal.Count > 0)
                    {
                        //Lista enfermedades personales
                        dictionaryStringObjec.Add("ListaEnferPersonales", consulMedicaEnfermePersonal);
                    }
                    else
                    {
                        //Lista enfermedades personales
                        dictionaryStringListObjec.Add("ListaEnferPersonales", null);
                    }

                    if (consulMedicaEnfermeFamiliar != null)
                    {
                        //enfermedades familiars
                        dictionaryStringObjec.Add("EnfermedadFamiliar", (object)consulMedicaEnfermeFamiliar);
                    }
                    else
                    {
                        //enfermedades familiars
                        dictionaryStringObjec.Add("EnfermedadFamiliar", null);
                    }

                    respuesta.dictionaryStringObjec = dictionaryStringObjec;
                    respuesta.dictionaryStringListObjec = dictionaryStringListObjec;


                    respuesta.respuesta = true;

                }

            }
            return Json(respuesta);
        }

          [HttpPost]
        public JsonResult buscarUltimaFechaConsulta(int idPaciente)
        {
            var respuesta = new ResponseModel();

            //    //Si NO esta loguieado lo redireccionara al loguin
            if (HttpContext.Session["user"] == null)
            {
                respuesta.respuesta = false;
                respuesta.error = "Usted debe de loguiarse.";
                respuesta.redirect = Url.Action("Index", "PaginaPresentacion");
                return Json(respuesta);
            }

            UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];
            if (idPaciente > 0)
            {


                using (var db = new DoctorMedicalWebEntities())
                {
                    ConsultaMedicaHistorial ultimaHisotorial = (from d in db.ConsultaMedicaHistorials
                                                                where d.DoctSecuencia_fk == usu.doctSecuencia
                                                                && d.PaciSecuencia_fk == idPaciente
                                                                orderby d.CMHistSecuencia descending
                                                                select d).FirstOrDefault();


                    respuesta.respuesta = true;
                    if (ultimaHisotorial != null)
                    {
                        if (ultimaHisotorial.CMHistFecha != null)
                            respuesta.menssage = ultimaHisotorial.CMHistFecha.Value.ToString("dd/MM/yyyy");
                        else
                        {
                            respuesta.menssage = "[N/A]";
                        }

                    }
                    else
                    {
                        respuesta.menssage = "[N/A]";

                    }

                }
            }
            return Json(respuesta);
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

                    //buscar los ultimos  cinco registros  de roles del doctor loguiado
                    List<ConsultaMedicaHistorial> consultaMedicaHistorialListaUltimosCinco = (from tf in db.ConsultaMedicaHistorials
                                                                                              where tf.DoctSecuencia_fk == usu.doctSecuencia
                                                                                              && tf.EstaDesabilitado == false
                                                                                              orderby tf.CMHistFecha descending
                                                                                              select tf).Take(5).ToList();



                    var listUsar_Consultar = new List<Usar_Consultar>();
                    //crear objeto role y asignar a la lista de usaroles
                    foreach (var item in consultaMedicaHistorialListaUltimosCinco)
                    {
                        Usar_Consultar consultarHist = new Usar_Consultar();
                        //asignar contenido a otro objeto
                        CopyClass.CopyObject(item, ref consultarHist);
                        //insertar nombre del paciente
                        Paciente pac = (from tf in db.Pacientes
                                        where tf.DoctSecuencia_fk == usu.doctSecuencia
                                        && tf.PaciSecuencia == item.PaciSecuencia_fk
                                        select tf).SingleOrDefault();
                        consultarHist.PaciSecuencia_fk = pac.PaciSecuencia;
                        consultarHist.PaciNombre = pac.PaciNombre;
                        consultarHist.PaciApellido1 = pac.PaciApellido1;
                        consultarHist.PaciApellido2 = pac.PaciApellido2;
                        consultarHist.PaciDocumento = pac.PaciDocumento;
                        //consultarHist.CMHistFechaString = consultarHist.CMHistFecha.Value.ToString("dd/MM/yyyy");
                        //consultarHist.CMHistHoraString = consultarHist.CMHistHora.Value.ToString();

                        //en el grid mostrare
                        //
                        listUsar_Consultar.Add(consultarHist);
                    }
                    //Utilizado para insertar el listado al grid
                    ViewBag.ultimosCinco = listUsar_Consultar;


                    //ienumerable lista
                    respuesta.someCollection = listUsar_Consultar;


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

        /// <summary>
        /// Este metodo devuelve null lo uso para convertir la fecha  en mes dia anio, el cual vienen de la vista
        ///  (datepicker synfusion) en formato de dia mes  anio
        /// y asi la base de datos sql server guardarla  en el formato de 
        /// anio mes dia 
        ///
        /// 0jo este metodo recibe formato  dia mes anio y devuelve mes dia anio
        /// </summary>
        /// <param name="FECHA">Fecha Datetime? osea que pude ser null</param>
        /// <returns></returns>
        public DateTime? ConvertirddmmTOmmdd(DateTime? FECHA)
        {
            DateTime? fecha = null;
            //aqui entra el formato dia mes anio y devuelve un string en formato mes dia anio
            string strdatetime = FECHA.HasValue ? FECHA.Value.ToString("dd/MM/yyyy") : string.Empty;
            if (!string.IsNullOrEmpty(strdatetime))
            {//no utilizo esta conversion porq ue como la fecha que entra en string se conviertio en mes dia anio
                //si la paso por este otro convertidor, de nuevo me hara otra conversion  y me la pondra en el formato en que entro a esta 
                //funcion es decir dia mes anio. y el formato que necesito es mes dia anio
                //fecha = DateTime.ParseExact(strdatetime, "dd/MM/yyyy",  new CultureInfo("en-US"));

                fecha = DateTime.Parse(strdatetime);
            }

            return fecha;

        }
         
        ///// <summary>
        ///// 0jo este metodo recibe formato  dia mes anio y devuelve mes dia anio
        ///// Este metodo no devuelve null lo uso para convertir a tipo fecha 
        ///// 
        ///// </summary>
        ///// <param name="FECHA">Fecha Datetime</param>
        ///// <returns></returns>
        //public DateTime datefromUSTODO(DateTime FECHA)
        //{
        //    string fec = FECHA.Date.ToString("MM/dd/yyyy");
        //    //DateTime dt = DateTime.Parse("24/01/2013", new CultureInfo("en-CA"));
        //    DateTime dt = DateTime.Parse(FECHA.ToShortDateString(), new CultureInfo("en-US"));

        //    //DateTime fecha = DateTime.Parse(fec);
        //    return dt;

        //}

        //public DateTime StringtoFechaMDY(string FECHA)
        //{            

        //    DateTime dt = DateTime.ParseExact(FECHA, "dd/MM/yyyy",  new CultureInfo("en-US"));

        //    string fec = dt.Date.ToString("MM/dd/yyyy");

        //    DateTime PR = DateTime.Parse(fec, new CultureInfo("en-US", true));
        //    DateTime dtF = DateTime.ParseExact(fec, "MM/dd/yyyy",  new CultureInfo("en-US"));

        //    //DateTime fecha = DateTime.Parse(fec);
        //    return dtF;

        //}

        //public JsonResult PatientBackground(int idPatient)
        //{
        //    var respuesta = new ResponseModel();

        //    //Si NO esta loguieado lo redireccionara al loguin
        //    if (HttpContext.Session["user"] == null)
        //    {
        //        respuesta.respuesta = false;
        //        respuesta.error = "Usted debe de loguiarse.";
        //        respuesta.redirect = Url.Action("Index", "PaginaPresentacion");
        //        return Json(respuesta);
        //    }

        //    using (var db = new DoctorMedicalWebEntities())
        //    {
        //        respuesta.respuesta = true;

        //        UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];

        //        //consulta antecedentes
        //        ConsultaMedica consultaMedic = db.ConsultaMedicas.Where(ro => ro.DoctSecuencia_fk == usu.doctSecuencia
        //                                                                            && ro.PaciSecuencia_fk == idPatient).SingleOrDefault();

        //        //Enfermedades personales anteriores
        //        List<ConsultaMedicaEnfermeda> ConsultaMedicaEnferPersonal = new List<ConsultaMedicaEnfermeda>();
        //        ConsultaMedicaEnferPersonal = (from rform in db.ConsultaMedicaEnfermedas
        //                                       where rform.DoctSecuencia_fk == usu.doctSecuencia
        //                                         && rform.PaciSecuencia_fk == idPatient
        //                                       select rform).ToList();
        //        //Enfermedad familiar
        //        ConsultaMedicaEnfermedaFamiliar cmediEnfermedadFamiliar = db.ConsultaMedicaEnfermedaFamiliars
        //                                                                                  .Where(ro => ro.DoctSecuencia_fk == usu.doctSecuencia
        //                                                                                   && ro.PaciSecuencia_fk == idPatient).SingleOrDefault();


        //        //Enfermedad y consulta de antecedentes
        //        Dictionary<string, object> dictionaryStringObjec = new Dictionary<string, object>();
        //        dictionaryStringObjec.Add("ConsultaAntecedentes", consultaMedic);
        //        dictionaryStringObjec.Add("AntecedentesFamiliares", cmediEnfermedadFamiliar);

        //        respuesta.dictionaryStringObjec = dictionaryStringObjec;

        //        //Lista Enfermedades personales
        //        Dictionary<string, List<object>> dictionaryStringListObjec = new Dictionary<string, List<object>>();
        //        if (ConsultaMedicaEnferPersonal.Count > 0)
        //        {
        //            dictionaryStringListObjec.Add("EnfermedadesPersonales", ConsultaMedicaEnferPersonal.ToList<object>());

        //        }
        //        else
        //        {
        //            dictionaryStringListObjec.Add("EnfermedadesPersonales", null);
        //        }
        //        respuesta.dictionaryStringListObjec = dictionaryStringListObjec;

        //    }

        //    return Json(respuesta);
        //}

         [HttpPost]
        public JsonResult Save(Usar_Consultar usar_Consultar, IEnumerable<Usar_RecetaComplementaria> usar_resetaCompList)
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
            //si no ha validado volver y mostrar mensajes de validacion
            if (!ModelState.IsValid)
            {
                respuesta.respuesta = false;
                //respuesta.redirect = Url.Content("~/Roles/Roles");
                respuesta.redirect = Url.Content("~/" + controler + "/" + vista + "");
                respuesta.error = "Llenar Campos requeridos ";
                return Json(respuesta);
            }

            //  Motivo de consulta
            if (usar_Consultar.MConsSecuencia_fk == null || usar_Consultar.MConsSecuencia_fk.Count == 0)
            {
                respuesta.respuesta = false;
                respuesta.error = "Favor seleccionar el motivo de consulta ";
                return Json(respuesta);
            }
            //  Evaluacion fisica
            if (usar_Consultar.EFisiSecuencia_fk == null || usar_Consultar.EFisiSecuencia_fk.Count == 0)
            {

                respuesta.respuesta = false;
                respuesta.error = "Favor seleccionar la evaluación física ";
                return Json(respuesta);
            }
            // Diagnostico
            if (usar_Consultar.DiagSecuencia == null || usar_Consultar.DiagSecuencia.Count == 0)
            {

                respuesta.respuesta = false;
                respuesta.error = "Favor seleccionar el diagnóstico ";
                return Json(respuesta);
            }
            // Tratamiento
            if (usar_Consultar.TratSecuencia_fk == null || usar_Consultar.TratSecuencia_fk.Count == 0)
            {
                respuesta.respuesta = false;
                respuesta.error = "Favor seleccionar el tratamiento";
                return Json(respuesta);
            }

            //si se ha seleccionado embarazo pero no se ha introducido fecha embarazo y fecha aproximada
            //devuelvo
            if (usar_Consultar.CMedEmbarazada == true)
            {
                //si la fecha de embarazo es mayor que la actual no se puede guardar
                if (usar_Consultar.CMedEmbarazadaFecha == null || usar_Consultar.CMedEmbarazadaFechaProbableParto == null)
                {
                    respuesta.respuesta = false;
                    respuesta.error = "Fecha de embarazo o fecha probable esta vacia.";
                    return Json(respuesta);
                }

            }



            if (usar_Consultar.CMedEmbarazadaFecha != null)
            {
                //si la fecha de embarazo es mayor que la actual no se puede guardar
                if (usar_Consultar.CMedEmbarazadaFecha > Lib.GetLocalDateTime().Date)
                {
                    respuesta.respuesta = false;
                    respuesta.error = "Fecha de embarazo es mayor que la fecha actual.";
                    return Json(respuesta);
                }

            }



            UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];

            using (var db = new DoctorMedicalWebEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                int proximoItem = 0;

                using (var dbtrans = db.Database.BeginTransaction())
                {

                    try
                    {

                        //add new 
                        if (usar_Consultar.CMHistSecuencia == 0 || string.IsNullOrEmpty(usar_Consultar.CMHistSecuencia.ToString()))
                        {
                            #region Insertando Una Nueva  Consulta Medica del paciente
                            ConsultaMedicaHistorial CMHistorial = new ConsultaMedicaHistorial();

                            //No la utilizo por que en la consulta historial digo en diagnostico lo que tengo
                            //ConsultaMedicaHistorialEnfermeda cmhistEnferme = new ConsultaMedicaHistorialEnfermeda();

                            //Guardo la consulta actual                            
                            CMHistorial.DoctSecuencia_fk = usu.doctSecuencia;
                            CMHistorial.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                            CMHistorial.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                            CMHistorial.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                            CMHistorial.PaciSecuencia_fk = (int)usar_Consultar.PaciSecuencia_fk;
                            CMHistorial.EstaDesabilitado = false;
                            CMHistorial.UsuaSecuenciaCreacion = usu.usuario.UsuaSecuencia;
                            CMHistorial.UsuaFechaCreacion = Lib.GetLocalDateTime();

                            CMHistorial.CMHistIndiceMasaCorporal = usar_Consultar.CMHistIndiceMasaCorporal;
                            CMHistorial.CMHistUnidadesMedidaPeso = usar_Consultar.CMHistUnidadesMedidaPeso;
                            CMHistorial.CMHistPeso = usar_Consultar.CMHistPeso;
                            CMHistorial.CMHistUnidadesMedidaTalla = usar_Consultar.CMHistUnidadesMedidaTalla;
                            CMHistorial.CMHistTalla = usar_Consultar.CMHistTalla;
                            CMHistorial.CMHistFecha = Lib.GetLocalDateTime();
                            CMHistorial.CMHistHora = Lib.GetLocalDateTime().TimeOfDay;//Lib.GetLocalDateTime().TimeOfDay;
                            CMHistorial.CMHistComentario = usar_Consultar.CMHistComentario;

                            CMHistorial.CMHistFechaUltimaRegla = (usar_Consultar.CMHistFechaUltimaRegla);


                            CMHistorial.MotiComentario = usar_Consultar.MotiComentario;
                            CMHistorial.EFisiComentario = usar_Consultar.EFisiComentario;
                            CMHistorial.DiagComentario = usar_Consultar.DiagComentario;
                            CMHistorial.TratComentario = usar_Consultar.TratComentario;

                            //buscando la proxima secuencia, el doctor en todas sus consultorios tiene los mismos roles 
                            proximoItem = ((from objBusc in db.ConsultaMedicaHistorials
                                            where objBusc.DoctSecuencia_fk == usu.doctSecuencia
                                            && objBusc.PaciSecuencia_fk == usar_Consultar.PaciSecuencia_fk
                                            select (int?)objBusc.CMHistSecuencia).Max() ?? 0) + 1;
                            CMHistorial.CMHistSecuencia = proximoItem;
                            //agregar codigo
                            //busco la maxima secuencia en consultas que hizo el  doctor
                            //para crear el codigo
                            string cantidadConsultasDoct = ((from objBusc in db.ConsultaMedicaHistorials
                                                             where objBusc.DoctSecuencia_fk == usu.doctSecuencia
                                                             orderby objBusc.CMHistCodigo.ToString().Substring(objBusc.CMHistCodigo.ToString().IndexOf("-") + 1) descending
                                                             select objBusc.CMHistCodigo)).Take(1).SingleOrDefault();

                            string extraerSoloSecuencia = "";
                            if (!string.IsNullOrEmpty(cantidadConsultasDoct))
                            {

                                //cantidadDoct.IndexOf("-")  +1 es por que el index me trae desde donde esta este -(guion) y yo necesito
                                //desde el siguiente  letra
                                extraerSoloSecuencia = cantidadConsultasDoct.Substring(cantidadConsultasDoct.IndexOf("-") + 1);
                                //extaigo el codigo del doctor para dejar solo la secuencia
                                int n = int.Parse(extraerSoloSecuencia);
                                string valor = n.ToString();
                                //cantidad de digitos del numero del doctor
                                int cantDig = usu.doctSecuencia.ToString().Length;
                                //EL valor con solo la secuencia, ya borrado el  numero del doctor
                                string codigoSinDoctor = valor.Substring(cantDig);
                                //sumo  1 para la proxima secuencia
                                int proxim = int.Parse(codigoSinDoctor) + 1;
                                extraerSoloSecuencia = proxim.ToString();
                            }
                            else
                            {
                                extraerSoloSecuencia = "01";

                            }
                            //quito los ceros delanteros
                            int secMaxSinCerosDelanteros = int.Parse(extraerSoloSecuencia);
                            //codigo con ceros
                            string codigDoctFormateado = Lib.FormatearCodigo(3, CMHistorial.DoctSecuencia_fk.ToString());
                            string codiSecGeneralConsultas = Lib.FormatearCodigo(6, secMaxSinCerosDelanteros.ToString());

                            string codFormateadoDefini = codigDoctFormateado + codiSecGeneralConsultas;
                            codFormateadoDefini = "C-" + codFormateadoDefini;
                            CMHistorial.CMHistCodigo = codFormateadoDefini;

                            db.ConsultaMedicaHistorials.Add(CMHistorial);

                            //si no hay secuencia de historial no  seguira
                            if (CMHistorial.CMHistSecuencia <= 0)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "No se creo la secuencia del historial.";
                                return Json(respuesta);
                            }

                            //Tipo de sangre en paciente
                            //LLenar paciente
                            var objPaci = db.Pacientes.Where(ro =>
                                ro.DoctSecuencia_fk == usu.doctSecuencia
                             && ro.PaciSecuencia == usar_Consultar.PaciSecuencia_fk).SingleOrDefault();
                            objPaci.GSangSecuencia_fk = usar_Consultar.GSangSecuencia_fk;

                            #region INSERTO  LOS DATOS DE EMBARAZO EN LA TABLA DE DETALLE
                            //si esta  seleccionado de que esta embarazada  entonces guardo
                            if (usar_Consultar.CMedEmbarazada)
                            {
                                if (usar_Consultar.CMedEmbarazadaFecha != null)
                                {

                                    ConsultaMedicaEmbarazo consultaMedicaEmbarazo = new ConsultaMedicaEmbarazo();
                                    consultaMedicaEmbarazo.DoctSecuencia_fk = usu.doctSecuencia;
                                    consultaMedicaEmbarazo.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                                    consultaMedicaEmbarazo.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                    consultaMedicaEmbarazo.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                    consultaMedicaEmbarazo.PaciSecuencia_fk = (int)usar_Consultar.PaciSecuencia_fk;
                                    consultaMedicaEmbarazo.CMHistSecuencia_fk = CMHistorial.CMHistSecuencia;
                                    consultaMedicaEmbarazo.CMEmbEmbarazada = usar_Consultar.CMedEmbarazada;
                                    consultaMedicaEmbarazo.CMEmbEmbarazadaFecha = (DateTime)usar_Consultar.CMedEmbarazadaFecha;
                                    consultaMedicaEmbarazo.CMEmbEmbarazadaSemanas = usar_Consultar.CMedEmbarazadaSemanas;
                                    consultaMedicaEmbarazo.CMEmbEmbarazadaDias = usar_Consultar.CMedEmbarazadaDias;
                                    consultaMedicaEmbarazo.CMEmbEmbarazadaMeses = usar_Consultar.CMedEmbarazadaMeses;
                                    consultaMedicaEmbarazo.CMEmbEmbarazadaFechaProbableParto = (DateTime)usar_Consultar.CMedEmbarazadaFechaProbableParto;

                                    db.ConsultaMedicaEmbarazoes.Add(consultaMedicaEmbarazo);
                                }
                            }
                            #endregion

                            #region  INSERTANDO DETALLES DE CONSULTA :MOTIVO,EVALUACIONFISICA ETC
                            //Historial Motivo   
                            if (usar_Consultar.MConsSecuencia_fk != null)
                            {

                                foreach (var item in usar_Consultar.MConsSecuencia_fk)
                                {
                                    ConsultaMedicaHistorialMotivoConsulta cmhistMotivoConsul = new ConsultaMedicaHistorialMotivoConsulta();
                                    cmhistMotivoConsul.DoctSecuencia_fk = usu.doctSecuencia;
                                    cmhistMotivoConsul.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                                    cmhistMotivoConsul.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                    cmhistMotivoConsul.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                    cmhistMotivoConsul.PaciSecuencia_fk = (int)usar_Consultar.PaciSecuencia_fk;
                                    cmhistMotivoConsul.EstaDesabilitado = false;
                                    cmhistMotivoConsul.CMHistSecuencia_fk = (int)CMHistorial.CMHistSecuencia;
                                    cmhistMotivoConsul.MConsSecuencia_fk = item;

                                    db.ConsultaMedicaHistorialMotivoConsultas.Add(cmhistMotivoConsul);
                                }

                            }

                            //Historial Evaluacion Fisica   
                            if (usar_Consultar.EFisiSecuencia_fk != null)
                            {
                                foreach (var item in usar_Consultar.EFisiSecuencia_fk)
                                {
                                    ConsultaMedicaHistorialEvaluacionFisica cmhistEvaluacionFisica = new ConsultaMedicaHistorialEvaluacionFisica();
                                    cmhistEvaluacionFisica.DoctSecuencia_fk = usu.doctSecuencia;
                                    cmhistEvaluacionFisica.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                                    cmhistEvaluacionFisica.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                    cmhistEvaluacionFisica.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                    cmhistEvaluacionFisica.PaciSecuencia_fk = (int)usar_Consultar.PaciSecuencia_fk;
                                    cmhistEvaluacionFisica.EstaDesabilitado = false;
                                    cmhistEvaluacionFisica.CMHistSecuencia_fk = (int)CMHistorial.CMHistSecuencia;

                                    cmhistEvaluacionFisica.EFisiSecuencia_fk = item;
                                    db.ConsultaMedicaHistorialEvaluacionFisicas.Add(cmhistEvaluacionFisica);
                                }
                            }

                            //Historial Diagnostico
                            if (usar_Consultar.DiagSecuencia != null)
                            {
                                foreach (var item in usar_Consultar.DiagSecuencia)
                                {
                                    ConsultaMedicaHistorialDiagnostico cmhistDiagnostico = new ConsultaMedicaHistorialDiagnostico();

                                    cmhistDiagnostico.DoctSecuencia_fk = usu.doctSecuencia;
                                    cmhistDiagnostico.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                                    cmhistDiagnostico.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                    cmhistDiagnostico.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                    cmhistDiagnostico.PaciSecuencia_fk = (int)usar_Consultar.PaciSecuencia_fk;
                                    cmhistDiagnostico.EstaDesabilitado = false;
                                    cmhistDiagnostico.CMHistSecuencia_fk = (int)CMHistorial.CMHistSecuencia;

                                    cmhistDiagnostico.DiagSecuencia = item;
                                    db.ConsultaMedicaHistorialDiagnosticoes.Add(cmhistDiagnostico);
                                }
                            }


                            //Historial tratamiento
                            if (usar_Consultar.TratSecuencia_fk != null)
                            {
                                foreach (var item in usar_Consultar.TratSecuencia_fk)
                                {
                                    ConsultaMedicaHistorialTratamiento cmhistTratamiento = new ConsultaMedicaHistorialTratamiento();
                                    cmhistTratamiento.DoctSecuencia_fk = usu.doctSecuencia;
                                    cmhistTratamiento.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                                    cmhistTratamiento.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                    cmhistTratamiento.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                    cmhistTratamiento.PaciSecuencia_fk = (int)usar_Consultar.PaciSecuencia_fk;
                                    cmhistTratamiento.EstaDesabilitado = false;
                                    cmhistTratamiento.CMHistSecuencia_fk = (int)CMHistorial.CMHistSecuencia;

                                    cmhistTratamiento.TratSecuencia_fk = item;
                                    db.ConsultaMedicaHistorialTratamientoes.Add(cmhistTratamiento);
                                }
                            }

                            #endregion

                            #region INSERTANDO  DATOS INICIALES O HISTORIAL DEL PACIENTE COMO ANTECEDENTES PERSONALES ETC.

                            ConsultaMedica CMedica = new ConsultaMedica();

                            //si es la primera consulta del paciente
                            //esta sera tambien la primera y unica insercion de datos medicos  iniciales  del paciente
                            if (proximoItem == 1)
                            {

                                //Consulta Datos del estado  inicial del paciente                          
                                CMedica.DoctSecuencia_fk = usu.doctSecuencia;
                                CMedica.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                                CMedica.clinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                CMedica.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                CMedica.PaciSecuencia_fk = (int)usar_Consultar.PaciSecuencia_fk;
                                CMedica.EstaDesabilitado = false;
                                //un historial solo puede tener un solo registro Datos del estado  inicial del paciente    
                                //buscando la proxima secuencia, el doctor en todas sus consultorios tiene los mismos roles 

                                CMedica.CMediSecuencia = 1;//siempre debe de ser un solo  por que el paciente solo tendra una sola insercion de datos iniciales
                                CMedica.CMediFecha = Lib.GetLocalDateTime();
                                CMedica.CMediHora = Lib.GetLocalDateTime().TimeOfDay;
                                CMedica.CMediUnidadesMedidaTalla = usar_Consultar.CMediUnidadesMedidaTalla;
                                CMedica.CMediTalla = usar_Consultar.CMediTalla;
                                CMedica.CMediUnidadesMedidaPeso = usar_Consultar.CMediUnidadesMedidaPeso;
                                CMedica.CMediPeso = usar_Consultar.CMediPeso;



                                if (usar_Consultar.CMedEmbarazada)
                                {
                                    if (usar_Consultar.CMedEmbarazadaFecha != null)
                                    {
                                        CMedica.CMedEmbarazada = usar_Consultar.CMedEmbarazada;
                                        CMedica.CMedEmbarazadaFecha = usar_Consultar.CMedEmbarazadaFecha;
                                        CMedica.CMedEmbarazadaSemanas = usar_Consultar.CMedEmbarazadaSemanas;
                                        CMedica.CMedEmbarazadaDias = usar_Consultar.CMedEmbarazadaDias;
                                        CMedica.CMedEmbarazadaMeses = usar_Consultar.CMedEmbarazadaMeses;

                                        CMedica.CMedEmbarazadaFechaProbableParto = usar_Consultar.CMedEmbarazadaFechaProbableParto;
                                    }
                                }

                                CMedica.CMediMenarquia = usar_Consultar.CMediMenarquia;
                                CMedica.CMediPatronMenstrual = usar_Consultar.CMediPatronMenstrual;
                                CMedica.CMediMensutracionDuracion = usar_Consultar.CMediMensutracionDuracion;
                                CMedica.CMediDismenorrea = usar_Consultar.CMediDismenorrea;
                                CMedica.CMediPrimerCoito = usar_Consultar.CMediPrimerCoito;
                                CMedica.CMediDispareunia = usar_Consultar.CMediDispareunia;
                                CMedica.CMediVidaSexualActiva = usar_Consultar.CMediVidaSexualActiva;
                                CMedica.CMediNumeroParejasSexual = usar_Consultar.CMediNumeroParejasSexual;
                                CMedica.CMediFechaUltimoParto = usar_Consultar.CMediFechaUltimoParto;
                                CMedica.CMediFechaUltimoAborto = usar_Consultar.CMediFechaUltimoAborto;
                                CMedica.CMediFechaUltimaMenstruacion = usar_Consultar.CMediFechaUltimaMenstruacion;
                                CMedica.CMediMenopausia = usar_Consultar.CMediMenopausia;
                                CMedica.CMediGestacionVeces = usar_Consultar.CMediGestacionVeces;
                                CMedica.CMediPartosVeces = usar_Consultar.CMediPartosVeces;
                                CMedica.CMediAbortosVeces = usar_Consultar.CMediAbortosVeces;
                                CMedica.CMediCesariasVeces = usar_Consultar.CMediCesariasVeces;
                                CMedica.CMediEctopico = usar_Consultar.CMediEctopico;
                                CMedica.EnfeComentario = usar_Consultar.EnfeComentarioPersonal;
                                CMedica.CMediFechaUltimoPapanicolau = usar_Consultar.CMediFechaUltimoPapanicolau;
                                CMedica.UsuaSecuenciaCreacion = usu.usuario.UsuaSecuencia;
                                CMedica.UsuaFechaCreacion = Lib.GetLocalDateTime();

                                db.ConsultaMedicas.Add(CMedica);


                                //Enfermedades Antecedentes personales
                                if (usar_Consultar.EnfeSecuenciaPersonal != null)
                                {
                                    foreach (var item in usar_Consultar.EnfeSecuenciaPersonal)
                                    {
                                        ConsultaMedicaEnfermeda CMediEnfermedica = new ConsultaMedicaEnfermeda();

                                        CMediEnfermedica.DoctSecuencia_fk = usu.doctSecuencia;
                                        CMediEnfermedica.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                                        CMediEnfermedica.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                        CMediEnfermedica.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                        CMediEnfermedica.PaciSecuencia_fk = (int)usar_Consultar.PaciSecuencia_fk;
                                        CMediEnfermedica.CMediSecuencia_fk = 1;
                                        CMediEnfermedica.EnfeSecuencia_fk = item;

                                        db.ConsultaMedicaEnfermedas.Add(CMediEnfermedica);
                                    }
                                }
                                ConsultaMedicaEnfermedaFamiliar cMediEnfermedadFamiliar = new ConsultaMedicaEnfermedaFamiliar();
                                //enfermedades o dolencias  familiares
                                cMediEnfermedadFamiliar.DoctSecuencia_fk = usu.doctSecuencia;
                                cMediEnfermedadFamiliar.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                                cMediEnfermedadFamiliar.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                cMediEnfermedadFamiliar.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                cMediEnfermedadFamiliar.PaciSecuencia_fk = (int)usar_Consultar.PaciSecuencia_fk;
                                cMediEnfermedadFamiliar.CMediSecuencia_fk = CMedica.CMediSecuencia;
                                cMediEnfermedadFamiliar.EnfeComentario = usar_Consultar.EnfeComentarioFamiliar;
                                cMediEnfermedadFamiliar.EnfeSecuencia_fk = 1;//simpre sera 1 por que de los familiares solo ingreso los comentario donde estan las enfermedades
                                cMediEnfermedadFamiliar.EnfermedadActiva = false;
                                cMediEnfermedadFamiliar.EnfermedadInfecciosa = false;

                                db.ConsultaMedicaEnfermedaFamiliars.Add(cMediEnfermedadFamiliar);



                            }
                            //caundo se va a crear otra consulta que no se la primera
                            //se editara los datos iniciales clinicos del paciente
                            // editar consulta medica cuando es de la segunda consulta en adelante del paciente
                            else
                            {
                                ConsultaMedica cmediEditSegundaHacia = db.ConsultaMedicas.Where(ro => ro.DoctSecuencia_fk == usu.doctSecuencia
                                                                                 && ro.PaciSecuencia_fk == usar_Consultar.PaciSecuencia_fk).SingleOrDefault();
                                //Consulta Datos del estado  inicial del paciente                          

                                //un historial solo puede tener un solo registro Datos del estado  inicial del paciente    
                                //buscando la proxima secuencia, el doctor en todas sus consultorios tiene los mismos roles 

                                //cmediEditSegundaHacia.CMediSecuencia = 1;//siempre debe de ser un solo  por que el paciente solo tendra una sola insercion de datos iniciales
                                cmediEditSegundaHacia.UsuaFechaModificacion = Lib.GetLocalDateTime();
                                cmediEditSegundaHacia.CMediUnidadesMedidaTalla = usar_Consultar.CMediUnidadesMedidaTalla;
                                cmediEditSegundaHacia.CMediTalla = usar_Consultar.CMediTalla;
                                cmediEditSegundaHacia.CMediUnidadesMedidaPeso = usar_Consultar.CMediUnidadesMedidaPeso;
                                cmediEditSegundaHacia.CMediPeso = usar_Consultar.CMediPeso;



                                cmediEditSegundaHacia.CMedEmbarazada = usar_Consultar.CMedEmbarazada;
                                cmediEditSegundaHacia.CMedEmbarazadaFecha = usar_Consultar.CMedEmbarazadaFecha;
                                cmediEditSegundaHacia.CMedEmbarazadaSemanas = usar_Consultar.CMedEmbarazadaSemanas;
                                cmediEditSegundaHacia.CMedEmbarazadaDias = usar_Consultar.CMedEmbarazadaDias;
                                cmediEditSegundaHacia.CMedEmbarazadaMeses = usar_Consultar.CMedEmbarazadaMeses;
                                cmediEditSegundaHacia.CMedEmbarazadaFechaProbableParto = usar_Consultar.CMedEmbarazadaFechaProbableParto;

                                cmediEditSegundaHacia.CMediMenarquia = usar_Consultar.CMediMenarquia;
                                cmediEditSegundaHacia.CMediPatronMenstrual = usar_Consultar.CMediPatronMenstrual;
                                cmediEditSegundaHacia.CMediMensutracionDuracion = usar_Consultar.CMediMensutracionDuracion;
                                cmediEditSegundaHacia.CMediDismenorrea = usar_Consultar.CMediDismenorrea;

                                cmediEditSegundaHacia.CMediPrimerCoito = usar_Consultar.CMediPrimerCoito;
                                cmediEditSegundaHacia.CMediDispareunia = usar_Consultar.CMediDispareunia;
                                cmediEditSegundaHacia.CMediVidaSexualActiva = usar_Consultar.CMediVidaSexualActiva;
                                cmediEditSegundaHacia.CMediNumeroParejasSexual = usar_Consultar.CMediNumeroParejasSexual;
                                cmediEditSegundaHacia.CMediFechaUltimoParto = usar_Consultar.CMediFechaUltimoParto;
                                cmediEditSegundaHacia.CMediFechaUltimoAborto = usar_Consultar.CMediFechaUltimoAborto;
                                cmediEditSegundaHacia.CMediFechaUltimaMenstruacion = usar_Consultar.CMediFechaUltimaMenstruacion;

                                cmediEditSegundaHacia.CMediMenopausia = usar_Consultar.CMediMenopausia;
                                cmediEditSegundaHacia.CMediGestacionVeces = usar_Consultar.CMediGestacionVeces;
                                cmediEditSegundaHacia.CMediPartosVeces = usar_Consultar.CMediPartosVeces;
                                cmediEditSegundaHacia.CMediAbortosVeces = usar_Consultar.CMediAbortosVeces;
                                cmediEditSegundaHacia.CMediCesariasVeces = usar_Consultar.CMediCesariasVeces;
                                cmediEditSegundaHacia.CMediEctopico = usar_Consultar.CMediEctopico;
                                cmediEditSegundaHacia.EnfeComentario = usar_Consultar.EnfeComentarioPersonal;
                                cmediEditSegundaHacia.CMediFechaUltimoPapanicolau = usar_Consultar.CMediFechaUltimoPapanicolau;
                                cmediEditSegundaHacia.UsuaSecuenciaCreacion = usu.usuario.UsuaSecuencia;
                                cmediEditSegundaHacia.UsuaFechaCreacion = Lib.GetLocalDateTime();
                                //cmediEditSegundaHacia.UsuaSecuenciaModificacion = usar_Consultar.UsuaSecuenciaModificacion;
                                //cmediEditSegundaHacia.UsuaFechaModificacion = usar_Consultar.UsuaFechaModificacion;


                                //BORRAR TODOS  enfermedades Personales 
                                List<ConsultaMedicaEnfermeda> ConsultaMedicaEnfermedaBorrar = new List<ConsultaMedicaEnfermeda>();
                                ConsultaMedicaEnfermedaBorrar = (from rform in db.ConsultaMedicaEnfermedas
                                                                 where rform.DoctSecuencia_fk == usu.doctSecuencia
                                                                   && rform.PaciSecuencia_fk == usar_Consultar.PaciSecuencia_fk

                                                                 select rform).ToList();


                                foreach (var item in ConsultaMedicaEnfermedaBorrar)
                                {
                                    //borrar actuales formulraios asignados a este rol
                                    db.ConsultaMedicaEnfermedas.Remove(item);
                                }



                                //Enfermedades Antecedentes personales
                                if (usar_Consultar.EnfeSecuenciaPersonal != null)
                                {
                                    foreach (var item in usar_Consultar.EnfeSecuenciaPersonal)
                                    {
                                        ConsultaMedicaEnfermeda CMediEnfermedica = new ConsultaMedicaEnfermeda();
                                        CMediEnfermedica.DoctSecuencia_fk = usu.doctSecuencia;
                                        CMediEnfermedica.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                                        CMediEnfermedica.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                        CMediEnfermedica.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                        CMediEnfermedica.PaciSecuencia_fk = (int)usar_Consultar.PaciSecuencia_fk;
                                        CMediEnfermedica.CMediSecuencia_fk = cmediEditSegundaHacia.CMediSecuencia;
                                        CMediEnfermedica.EnfeSecuencia_fk = item;
                                        db.ConsultaMedicaEnfermedas.Add(CMediEnfermedica);
                                    }
                                }

                                //enfermedades o dolencias  familiares
                                //siempre debe de haber solo uno
                                ConsultaMedicaEnfermedaFamiliar cmediEnfermedadFamiliarEdit = db.ConsultaMedicaEnfermedaFamiliars
                                                                                            .Where(ro => ro.DoctSecuencia_fk == usu.doctSecuencia
                                                                                             && ro.PaciSecuencia_fk == usar_Consultar.PaciSecuencia_fk).SingleOrDefault();

                                //cmediEnfermedadFamiliarEdit.DoctSecuencia_fk = usu.doctSecuencia;
                                //cmediEnfermedadFamiliarEdit.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                                //cmediEnfermedadFamiliarEdit.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                //cmediEnfermedadFamiliarEdit.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                //cmediEnfermedadFamiliarEdit.PaciSecuencia_fk = (int)usar_Consultar.PaciSecuencia_fk;
                                //cmediEnfermedadFamiliarEdit.CMediSecuencia_fk = 1;
                                // cmediEnfermedadFamiliarEdit.EnfeSecuencia_fk = 1;//simpre sera 1 por que de los familiares solo ingreso los comentario donde estan las enfermedades
                                cmediEnfermedadFamiliarEdit.EnfeComentario = usar_Consultar.EnfeComentarioFamiliar;
                                // db.ConsultaMedicaEnfermedaFamiliars.Add(cmediEnfermedadFamiliarEdit);

                            }//end editar consulta medica cuando es de la segunda consulta en adelante del paciente


                            #endregion

                            #region INSERTANDO RECETA
                            //insertar Recetas
                            if (usar_resetaCompList != null)
                            {

                                int secRect = 0;
                                //buscando la proxima secuencia, de historial
                                secRect = ((from numSecu in db.Recetas
                                            where numSecu.DoctSecuencia_fk == usu.doctSecuencia
                                            && numSecu.CMHistSecuencia_fk == (int)CMHistorial.CMHistSecuencia
                                            && numSecu.PaciSecuencia_fk == usar_Consultar.PaciSecuencia_fk
                                            select (int?)numSecu.ReceSecuencia).Max() ?? 0) + 1;
                                ////agregar codigo      
                                string cantidadReceDoct = "";

                                //para crear el codigo
                                cantidadReceDoct = ((from objBusc in db.Recetas
                                                     where objBusc.DoctSecuencia_fk == usu.doctSecuencia
                                                     orderby objBusc.RecCodigo.ToString().Substring(objBusc.RecCodigo.ToString().IndexOf("-") + 1) descending
                                                     select objBusc.RecCodigo)).Take(1).SingleOrDefault();


                                if (!string.IsNullOrEmpty(cantidadReceDoct))
                                {

                                    //cantidadDoct.IndexOf("-")  +1 es por que el index me trae desde donde esta este -(guion) y yo necesito
                                    //desde el siguiente  letra
                                    cantidadReceDoct = cantidadReceDoct.Substring(cantidadReceDoct.IndexOf("-") + 1);
                                    // para dejar solo el numero convierto a int y asi borrar los ceros que tiene a lante
                                    int n = int.Parse(cantidadReceDoct);
                                    // convierto a string nuevamente sin los ceros que tenia alante
                                    string valor = n.ToString();
                                    //cantidad de digitos del numero del doctor
                                    int cantDig = usu.doctSecuencia.ToString().Length;
                                    //quito la lantidad delantera que corresponde al doctor y dejo solo la secuencia con los ceros alante
                                    cantidadReceDoct = valor.Substring(cantDig);


                                }


                                foreach (var item in usar_resetaCompList)
                                {
                                    Receta objReceta = new Receta();

                                    objReceta.DoctSecuencia_fk = usu.doctSecuencia;
                                    objReceta.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                                    objReceta.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                    objReceta.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                    objReceta.PaciSecuencia_fk = (int)usar_Consultar.PaciSecuencia_fk;
                                    objReceta.EstaDesabilitado = false;
                                    objReceta.CMHistSecuencia_fk = (int)CMHistorial.CMHistSecuencia;
                                    objReceta.RecNombre = item.RecNombre;
                                    objReceta.ReceSecuencia = secRect;
                                    objReceta.ReceFecha = Lib.GetLocalDateTime();
                                    objReceta.ReceComentario = item.ReceComentario;
                                    objReceta.ReceInstruccionesFarmacia = "";
                                    objReceta.ReceInstruccionesAlPaciente = "";
                                    objReceta.UsuaSecuenciaCreacion = usu.usuario.UsuaSecuencia;
                                    objReceta.UsuaFechaCreacion = Lib.GetLocalDateTime();
                                    //objReceta.UsuaSecuenciaModificacion=usar_resetaCompList.UsuaSecuenciaModificacion;
                                    //objReceta.UsuaFechaModificacion=usar_resetaCompList.UsuaFechaModificacion;




                                    ////agregar codigo                           

                                    ////agregar codigo      
                                    //aqui sumo uno a la secuencia
                                    if (string.IsNullOrEmpty(cantidadReceDoct))
                                        cantidadReceDoct = "0";
                                    int sum = int.Parse(cantidadReceDoct) + 1;
                                    cantidadReceDoct = sum.ToString();


                                    //quito los ceros delanteros
                                    //int secMaxSinCerosDelanterosRec = int.Parse(cantidadReceDoct);
                                    //codigo con ceros doctor y secuencia
                                    string codigRecDoctFormateado = Lib.FormatearCodigo(3, usu.doctSecuencia.ToString());
                                    string codiRecSecGeneral = Lib.FormatearCodigo(6, cantidadReceDoct);
                                    string codFormateadoDefiniRec = codigRecDoctFormateado + codiRecSecGeneral;
                                    codFormateadoDefiniRec = "CR-" + codFormateadoDefiniRec;
                                    objReceta.RecCodigo = codFormateadoDefiniRec;


                                    db.Recetas.Add(objReceta);

                                    //Medicamentos de receta   
                                    if (item.MediSecuencia_fk != null)
                                    {
                                        int recMediSecu = 1;
                                        foreach (var medis in item.MediSecuencia_fk)
                                        {

                                            RecetaMedicamento recMedicamento = new RecetaMedicamento();
                                            recMedicamento.DoctSecuencia_fk = usu.doctSecuencia;
                                            recMedicamento.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                                            recMedicamento.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                            recMedicamento.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                            recMedicamento.PaciSecuencia_fk = (int)usar_Consultar.PaciSecuencia_fk;
                                            recMedicamento.CMHistSecuencia_fk = (int)CMHistorial.CMHistSecuencia;
                                            recMedicamento.MediSecuencia_fk = medis;
                                            recMedicamento.ReceSecuencia_fk = secRect;
                                            recMedicamento.RMediSecuencia = recMediSecu;
                                            recMedicamento.EstaDesabilitado = false;
                                            db.RecetaMedicamentos.Add(recMedicamento);

                                            recMediSecu++;
                                        }
                                    }

                                    //Analisis de receta  
                                    if (item.AClinSecuencia_fk != null)
                                    {
                                        int recAnalisis = 1;
                                        foreach (var anali in item.AClinSecuencia_fk)
                                        {
                                            RecetaAnalisisClinico recAnalisiClini = new RecetaAnalisisClinico();

                                            recAnalisiClini.DoctSecuencia_fk = usu.doctSecuencia;
                                            recAnalisiClini.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                                            recAnalisiClini.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                            recAnalisiClini.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                            recAnalisiClini.PaciSecuencia_fk = (int)usar_Consultar.PaciSecuencia_fk;
                                            recAnalisiClini.CMHistSecuencia_fk = (int)CMHistorial.CMHistSecuencia;
                                            recAnalisiClini.AClinSecuencia_fk = (int)anali;
                                            recAnalisiClini.ReceSecuencia_fk = secRect;
                                            recAnalisiClini.RAClinSecuencia = recAnalisis;
                                            recAnalisiClini.EstaDesabilitado = false;
                                            db.RecetaAnalisisClinicoes.Add(recAnalisiClini);

                                            recAnalisis++;
                                        }
                                    }

                                    //Imagenes de receta 
                                    if (item.ImagSecuencia_fk != null)
                                    {
                                        int recSecImagen = 1;
                                        foreach (var imgRec in item.ImagSecuencia_fk)
                                        {
                                            RecetaImagene recImagene = new RecetaImagene();
                                            recImagene.DoctSecuencia_fk = usu.doctSecuencia;
                                            recImagene.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                                            recImagene.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                            recImagene.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                            recImagene.PaciSecuencia_fk = (int)usar_Consultar.PaciSecuencia_fk;
                                            recImagene.CMHistSecuencia_fk = (int)CMHistorial.CMHistSecuencia;
                                            recImagene.ImagSecuencia_fk = imgRec;
                                            recImagene.ReceSecuencia_fk = secRect;
                                            recImagene.RImagSecuencia = recSecImagen;
                                            recImagene.EstaDesabilitado = false;
                                            db.RecetaImagenes.Add(recImagene);

                                            recSecImagen++;
                                        }
                                    }



                                    secRect++;
                                }//fin insertar receta
                            }
                            #endregion

                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Nuevo.ToString(), null);
                            #endregion
                        }
                        //Editando 
                        else
                        {
                            #region Editando  Consulta Medica del paciente


                            //Buco Historial a editar
                            ConsultaMedicaHistorial objetoConsultaHistorialEditar = (from x in db.ConsultaMedicaHistorials
                                                                                     where x.DoctSecuencia_fk == usu.doctSecuencia
                                                                                      && x.PaciSecuencia_fk == usar_Consultar.PaciSecuencia_fk
                                                                                      && x.CMHistSecuencia == usar_Consultar.CMHistSecuencia
                                                                                      && x.EstaDesabilitado == false
                                                                                     select x).SingleOrDefault();


                            if (objetoConsultaHistorialEditar == null)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "No existe esta consulta";
                                respuesta.redirect = Url.Action(vista, controler);
                                return Json(respuesta);
                            }


                            objetoConsultaHistorialEditar.TratComentario = usar_Consultar.TratComentario;
                            objetoConsultaHistorialEditar.MotiComentario = usar_Consultar.MotiComentario;
                            objetoConsultaHistorialEditar.EFisiComentario = usar_Consultar.EFisiComentario;
                            objetoConsultaHistorialEditar.DiagComentario = usar_Consultar.DiagComentario;
                            objetoConsultaHistorialEditar.CMHistFechaUltimaRegla = usar_Consultar.CMHistFechaUltimaRegla;
                            objetoConsultaHistorialEditar.CMHistIndiceMasaCorporal = usar_Consultar.CMHistIndiceMasaCorporal;
                            objetoConsultaHistorialEditar.CMHistUnidadesMedidaPeso = usar_Consultar.CMHistUnidadesMedidaPeso;
                            objetoConsultaHistorialEditar.CMHistPeso = usar_Consultar.CMHistPeso;
                            objetoConsultaHistorialEditar.CMHistUnidadesMedidaTalla = usar_Consultar.CMHistUnidadesMedidaTalla;
                            objetoConsultaHistorialEditar.CMHistTalla = usar_Consultar.CMHistTalla;
                            objetoConsultaHistorialEditar.CMHistComentario = usar_Consultar.CMHistComentario;
                            objetoConsultaHistorialEditar.UsuaSecuenciaModificacion = usu.usuario.UsuaSecuencia;
                            objetoConsultaHistorialEditar.UsuaFechaModificacion = Lib.GetLocalDateTime();


                            //Tipo de sangre en paciente
                            //LLenar paciente
                            var objPaci = db.Pacientes.Where(ro =>
                                ro.DoctSecuencia_fk == usu.doctSecuencia
                             && ro.PaciSecuencia == usar_Consultar.PaciSecuencia_fk).SingleOrDefault();
                            objPaci.GSangSecuencia_fk = usar_Consultar.GSangSecuencia_fk;

                            #region BORRO Y  VUELVO A INSERTAR LOS DETALLES DE LA CONSULTA
                            //asigno al objeto que se editara los nuevos datos introducidos a editar
                            //CopyClass.CopyObject(usar_Consultar, ref objetoConsultaHistorialEditar);

                            //BORRAR TODOS  Motivo Consulta 
                            List<ConsultaMedicaHistorialMotivoConsulta> BorrarConsultaMedicaHistorialMotivoConsulta = new List<ConsultaMedicaHistorialMotivoConsulta>();
                            BorrarConsultaMedicaHistorialMotivoConsulta = (from rform in db.ConsultaMedicaHistorialMotivoConsultas
                                                                           where rform.DoctSecuencia_fk == usu.doctSecuencia
                                                                             && rform.PaciSecuencia_fk == usar_Consultar.PaciSecuencia_fk
                                                                             && rform.CMHistSecuencia_fk == usar_Consultar.CMHistSecuencia
                                                                           select rform).ToList();


                            foreach (var item in BorrarConsultaMedicaHistorialMotivoConsulta)
                            {
                                //borrar actuales formulraios asignados a este rol
                                db.ConsultaMedicaHistorialMotivoConsultas.Remove(item);
                            }

                            //Editar Motivo Consulta Historial

                            if (usar_Consultar.MConsSecuencia_fk != null)
                            {

                                foreach (var item in usar_Consultar.MConsSecuencia_fk)
                                {
                                    ConsultaMedicaHistorialMotivoConsulta cmhistMotivoConsul = new ConsultaMedicaHistorialMotivoConsulta();
                                    cmhistMotivoConsul.DoctSecuencia_fk = usu.doctSecuencia;
                                    cmhistMotivoConsul.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                                    cmhistMotivoConsul.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                    cmhistMotivoConsul.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                    cmhistMotivoConsul.PaciSecuencia_fk = (int)usar_Consultar.PaciSecuencia_fk;
                                    cmhistMotivoConsul.EstaDesabilitado = false;
                                    cmhistMotivoConsul.CMHistSecuencia_fk = (int)usar_Consultar.CMHistSecuencia;
                                    cmhistMotivoConsul.MConsSecuencia_fk = item;
                                    db.ConsultaMedicaHistorialMotivoConsultas.Add(cmhistMotivoConsul);
                                }

                            }

                            //BORRAR TODOS  Evaluaciones Fisicas
                            List<ConsultaMedicaHistorialEvaluacionFisica> BorrarConsultaMedicaHistorialEvaluacionFisica = new List<ConsultaMedicaHistorialEvaluacionFisica>();
                            BorrarConsultaMedicaHistorialEvaluacionFisica = (from rform in db.ConsultaMedicaHistorialEvaluacionFisicas
                                                                             where rform.DoctSecuencia_fk == usu.doctSecuencia
                                                                               && rform.PaciSecuencia_fk == usar_Consultar.PaciSecuencia_fk
                                                                               && rform.CMHistSecuencia_fk == usar_Consultar.CMHistSecuencia
                                                                             select rform).ToList();

                            foreach (var item in BorrarConsultaMedicaHistorialEvaluacionFisica)
                            {
                                //borrar actuales formulraios asignados a este rol
                                db.ConsultaMedicaHistorialEvaluacionFisicas.Remove(item);
                            }

                            //MJM ESTA BORRANDO PARA PRUEBA//Editando Historial Evaluacion Fisica   
                            if (usar_Consultar.EFisiSecuencia_fk != null)
                            {
                                foreach (var item in usar_Consultar.EFisiSecuencia_fk)
                                {
                                    ConsultaMedicaHistorialEvaluacionFisica cmhistEvaluacionFisica = new ConsultaMedicaHistorialEvaluacionFisica();
                                    cmhistEvaluacionFisica.DoctSecuencia_fk = usu.doctSecuencia;
                                    cmhistEvaluacionFisica.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                                    cmhistEvaluacionFisica.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                    cmhistEvaluacionFisica.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                    cmhistEvaluacionFisica.PaciSecuencia_fk = (int)usar_Consultar.PaciSecuencia_fk;
                                    cmhistEvaluacionFisica.EstaDesabilitado = false;
                                    cmhistEvaluacionFisica.CMHistSecuencia_fk = (int)usar_Consultar.CMHistSecuencia;
                                    cmhistEvaluacionFisica.EFisiSecuencia_fk = item;
                                    db.ConsultaMedicaHistorialEvaluacionFisicas.Add(cmhistEvaluacionFisica);
                                }
                            }


                            //BORRAR TODOS  Diagnostico
                            List<ConsultaMedicaHistorialDiagnostico> BorrarConsultaMedicaHistorialDiagnostico = new List<ConsultaMedicaHistorialDiagnostico>();
                            BorrarConsultaMedicaHistorialDiagnostico = (from rform in db.ConsultaMedicaHistorialDiagnosticoes
                                                                        where rform.DoctSecuencia_fk == usu.doctSecuencia
                                                                          && rform.PaciSecuencia_fk == usar_Consultar.PaciSecuencia_fk
                                                                          && rform.CMHistSecuencia_fk == usar_Consultar.CMHistSecuencia
                                                                        select rform).ToList();

                            foreach (var item in BorrarConsultaMedicaHistorialDiagnostico)
                            {
                                //borrar actuales formulraios asignados a este rol
                                db.ConsultaMedicaHistorialDiagnosticoes.Remove(item);
                            }

                            //Historial Diagnostico
                            if (usar_Consultar.DiagSecuencia != null)
                            {
                                foreach (var item in usar_Consultar.DiagSecuencia)
                                {
                                    ConsultaMedicaHistorialDiagnostico cmhistDiagnostico = new ConsultaMedicaHistorialDiagnostico();

                                    cmhistDiagnostico.DoctSecuencia_fk = usu.doctSecuencia;
                                    cmhistDiagnostico.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                                    cmhistDiagnostico.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                    cmhistDiagnostico.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                    cmhistDiagnostico.PaciSecuencia_fk = (int)usar_Consultar.PaciSecuencia_fk;
                                    cmhistDiagnostico.EstaDesabilitado = false;
                                    cmhistDiagnostico.CMHistSecuencia_fk = (int)usar_Consultar.CMHistSecuencia;
                                    cmhistDiagnostico.DiagSecuencia = item;
                                    db.ConsultaMedicaHistorialDiagnosticoes.Add(cmhistDiagnostico);
                                }
                            }

                            //BORRAR TODOS  Tratamiento
                            List<ConsultaMedicaHistorialTratamiento> BorrarConsultaMedicaHistorialTratamiento = new List<ConsultaMedicaHistorialTratamiento>();
                            BorrarConsultaMedicaHistorialTratamiento = (from rform in db.ConsultaMedicaHistorialTratamientoes
                                                                        where rform.DoctSecuencia_fk == usu.doctSecuencia
                                                                          && rform.PaciSecuencia_fk == usar_Consultar.PaciSecuencia_fk
                                                                          && rform.CMHistSecuencia_fk == usar_Consultar.CMHistSecuencia
                                                                        select rform).ToList();

                            foreach (var item in BorrarConsultaMedicaHistorialTratamiento)
                            {
                                //borrar actuales formulraios asignados a este rol
                                db.ConsultaMedicaHistorialTratamientoes.Remove(item);
                            }

                            //Historial tratamiento
                            if (usar_Consultar.TratSecuencia_fk != null)
                            {
                                foreach (var item in usar_Consultar.TratSecuencia_fk)
                                {
                                    ConsultaMedicaHistorialTratamiento cmhistTratamiento = new ConsultaMedicaHistorialTratamiento();
                                    cmhistTratamiento.DoctSecuencia_fk = usu.doctSecuencia;
                                    cmhistTratamiento.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                                    cmhistTratamiento.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                    cmhistTratamiento.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                    cmhistTratamiento.PaciSecuencia_fk = (int)usar_Consultar.PaciSecuencia_fk;
                                    cmhistTratamiento.EstaDesabilitado = false;
                                    cmhistTratamiento.CMHistSecuencia_fk = (int)usar_Consultar.CMHistSecuencia;
                                    cmhistTratamiento.TratSecuencia_fk = item;
                                    db.ConsultaMedicaHistorialTratamientoes.Add(cmhistTratamiento);
                                }
                            }


                            #endregion

                            #region MODIFICO DATOS DEL EMBARAZO
                            //editar Datos de Embarazo en el historial
                            //Buco Historial a editar
                            ConsultaMedicaEmbarazo EditarConsultaMedicaEmbarazo = (from x in db.ConsultaMedicaEmbarazoes
                                                                                   where x.DoctSecuencia_fk == usu.doctSecuencia
                                                                                    && x.PaciSecuencia_fk == usar_Consultar.PaciSecuencia_fk
                                                                                    && x.CMHistSecuencia_fk == usar_Consultar.CMHistSecuencia
                                                                                   select x).SingleOrDefault();

                            //objetoEmbarazo a insertar por proceso de edicion

                            ConsultaMedicaEmbarazo objEmbarazoEdicionInsertar = new ConsultaMedicaEmbarazo();


                            if (EditarConsultaMedicaEmbarazo != null)
                            {
                                CopyClass.CopyObject(EditarConsultaMedicaEmbarazo, ref objEmbarazoEdicionInsertar);
                                //objEmbarazoEdicionInsertar=EditarConsultaMedicaEmbarazo;
                                //borro 
                                db.ConsultaMedicaEmbarazoes.Remove(EditarConsultaMedicaEmbarazo);
                            }

                            if (usar_Consultar.CMedEmbarazada == true &&
                                   usar_Consultar.CMedEmbarazadaFecha != null &&
                                   usar_Consultar.CMedEmbarazadaFechaProbableParto != null)
                            {
                                objEmbarazoEdicionInsertar.DoctSecuencia_fk = usu.doctSecuencia;
                                objEmbarazoEdicionInsertar.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                                objEmbarazoEdicionInsertar.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                objEmbarazoEdicionInsertar.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                objEmbarazoEdicionInsertar.PaciSecuencia_fk = usar_Consultar.PaciSecuencia_fk;
                                objEmbarazoEdicionInsertar.CMHistSecuencia_fk = (int)usar_Consultar.CMHistSecuencia;

                                objEmbarazoEdicionInsertar.CMEmbEmbarazada = usar_Consultar.CMedEmbarazada;
                                objEmbarazoEdicionInsertar.CMEmbEmbarazadaFecha = (DateTime)usar_Consultar.CMedEmbarazadaFecha;
                                objEmbarazoEdicionInsertar.CMEmbEmbarazadaSemanas = usar_Consultar.CMedEmbarazadaSemanas;
                                objEmbarazoEdicionInsertar.CMEmbEmbarazadaDias = usar_Consultar.CMedEmbarazadaDias;
                                objEmbarazoEdicionInsertar.CMEmbEmbarazadaMeses = usar_Consultar.CMedEmbarazadaMeses;
                                objEmbarazoEdicionInsertar.CMEmbEmbarazadaFechaProbableParto = (DateTime)usar_Consultar.CMedEmbarazadaFechaProbableParto;
                                db.ConsultaMedicaEmbarazoes.Add(objEmbarazoEdicionInsertar);
                            }

                            #endregion

                            #region  EDITAR CONSULTA DE ANTECEDENTES DEL PACIENTE
                            //Antecedentes paciente consulta medica Solo se podran modificar en una nueva consulta
                            //No se guardaran los datos de embarazo en momentos de editar Porque  los datos de embarazo debe de ser de la ultima consulta

                            ConsultaMedica cmediEdit = db.ConsultaMedicas.Where(ro => ro.DoctSecuencia_fk == usu.doctSecuencia
                                                                                && ro.PaciSecuencia_fk == usar_Consultar.PaciSecuencia_fk).SingleOrDefault();
                            //Consulta Datos del estado  inicial del paciente 
                            //un historial solo puede tener un solo registro Datos del estado  inicial del paciente    
                            //buscando la proxima secuencia, el doctor en todas sus consultorios tiene los mismos roles 
                            cmediEdit.CMediUnidadesMedidaTalla = usar_Consultar.CMediUnidadesMedidaTalla;
                            cmediEdit.CMediTalla = usar_Consultar.CMediTalla;
                            cmediEdit.CMediUnidadesMedidaPeso = usar_Consultar.CMediUnidadesMedidaPeso;
                            cmediEdit.CMediPeso = usar_Consultar.CMediPeso;

                            //al editar una consulta los datos de embarazo en la tabla de consulta medica
                            //no se modifican, por que hai esta los actuales datos de embarazo solo en una nueva consulta se modifican
                            //Nota: Lo datos de embarazo que se modifican es en la tabla de historial del embarazo
                            #region datos de embarazo de la consulta antecedentes, que no se puede modifical cuando se esta editando
                            //cmediEdit.CMedEmbarazada = usar_Consultar.CMedEmbarazada;
                            //cmediEdit.CMedEmbarazadaFecha = usar_Consultar.CMedEmbarazadaFecha;
                            //cmediEdit.CMedEmbarazadaSemanas = usar_Consultar.CMedEmbarazadaSemanas;
                            //cmediEdit.CMedEmbarazadaDias = usar_Consultar.CMedEmbarazadaDias;
                            //cmediEdit.CMedEmbarazadaMeses = usar_Consultar.CMedEmbarazadaMeses;
                            //cmediEdit.CMedEmbarazadaFechaProbableParto = usar_Consultar.CMedEmbarazadaFechaProbableParto;
                            #endregion
                            cmediEdit.CMediMenarquia = usar_Consultar.CMediMenarquia;
                            cmediEdit.CMediPatronMenstrual = usar_Consultar.CMediPatronMenstrual;
                            cmediEdit.CMediMensutracionDuracion = usar_Consultar.CMediMensutracionDuracion;
                            cmediEdit.CMediDismenorrea = usar_Consultar.CMediDismenorrea;
                            cmediEdit.CMediPrimerCoito = usar_Consultar.CMediPrimerCoito;
                            cmediEdit.CMediDispareunia = usar_Consultar.CMediDispareunia;
                            cmediEdit.CMediVidaSexualActiva = usar_Consultar.CMediVidaSexualActiva;
                            cmediEdit.CMediNumeroParejasSexual = usar_Consultar.CMediNumeroParejasSexual;
                            cmediEdit.CMediFechaUltimoParto = usar_Consultar.CMediFechaUltimoParto;
                            cmediEdit.CMediFechaUltimoAborto = usar_Consultar.CMediFechaUltimoAborto;
                            cmediEdit.CMediFechaUltimaMenstruacion = usar_Consultar.CMediFechaUltimaMenstruacion;
                            cmediEdit.CMediMenopausia = usar_Consultar.CMediMenopausia;
                            cmediEdit.CMediGestacionVeces = usar_Consultar.CMediGestacionVeces;
                            cmediEdit.CMediPartosVeces = usar_Consultar.CMediPartosVeces;
                            cmediEdit.CMediAbortosVeces = usar_Consultar.CMediAbortosVeces;
                            cmediEdit.CMediCesariasVeces = usar_Consultar.CMediCesariasVeces;
                            cmediEdit.CMediEctopico = usar_Consultar.CMediEctopico;
                            cmediEdit.EnfeComentario = usar_Consultar.EnfeComentarioPersonal;
                            cmediEdit.CMediFechaUltimoPapanicolau = usar_Consultar.CMediFechaUltimoPapanicolau;
                            cmediEdit.UsuaSecuenciaModificacion = usu.usuario.UsuaSecuencia;
                            cmediEdit.UsuaFechaModificacion = Lib.GetLocalDateTime();


                            //BORRAR TODOS  enfermedades Personales 
                            List<ConsultaMedicaEnfermeda> ConsultaMedicaEnfermedaBorrar = new List<ConsultaMedicaEnfermeda>();
                            ConsultaMedicaEnfermedaBorrar = (from rform in db.ConsultaMedicaEnfermedas
                                                             where rform.DoctSecuencia_fk == usu.doctSecuencia
                                                               && rform.PaciSecuencia_fk == usar_Consultar.PaciSecuencia_fk

                                                             select rform).ToList();


                            foreach (var item in ConsultaMedicaEnfermedaBorrar)
                            {
                                //borrar actuales formulraios asignados a este rol
                                db.ConsultaMedicaEnfermedas.Remove(item);
                            }



                            //Enfermedades Antecedentes personales
                            if (usar_Consultar.EnfeSecuenciaPersonal != null)
                            {
                                foreach (var item in usar_Consultar.EnfeSecuenciaPersonal)
                                {
                                    ConsultaMedicaEnfermeda CMediEnfermedica = new ConsultaMedicaEnfermeda();
                                    CMediEnfermedica.DoctSecuencia_fk = usu.doctSecuencia;
                                    CMediEnfermedica.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                                    CMediEnfermedica.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                    CMediEnfermedica.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                    CMediEnfermedica.PaciSecuencia_fk = (int)usar_Consultar.PaciSecuencia_fk;
                                    CMediEnfermedica.CMediSecuencia_fk = cmediEdit.CMediSecuencia;
                                    CMediEnfermedica.EnfeSecuencia_fk = item;
                                    db.ConsultaMedicaEnfermedas.Add(CMediEnfermedica);
                                }
                            }

                            //enfermedades o dolencias  familiares
                            //siempre debe de haber solo uno
                            ConsultaMedicaEnfermedaFamiliar cmediEnfermedadFamiliarEdit = db.ConsultaMedicaEnfermedaFamiliars
                                                                                        .Where(ro => ro.DoctSecuencia_fk == usu.doctSecuencia
                                                                                         && ro.PaciSecuencia_fk == usar_Consultar.PaciSecuencia_fk).SingleOrDefault();

                            cmediEnfermedadFamiliarEdit.EnfeComentario = usar_Consultar.EnfeComentarioFamiliar;

                            #endregion

                            #region TRABAJANDO CON LA RECETA EN EDICION
                            //Receta editar
                            if (usar_resetaCompList != null)
                            {
                                #region Borro  en la base de datos las recetas que se eliminaron en la vista
                                //borro las recetas que  eliminaron en la vista
                                //lo hago de este modo por que quiero preservar el codigo de las otras recetas.
                                //asi que solo borro la receta que elmino el doctor en la vista
                                List<Receta> listaRectServidor = (from numSecu in db.Recetas
                                                                  where numSecu.DoctSecuencia_fk == usu.doctSecuencia
                                                                  && numSecu.CMHistSecuencia_fk == usar_Consultar.CMHistSecuencia
                                                                  && numSecu.PaciSecuencia_fk == usar_Consultar.PaciSecuencia_fk
                                                                  select numSecu).ToList();


                                //borrar todas receta en edicion
                                foreach (var recetaParaBorrar in listaRectServidor)
                                {
                                    //busco cada receta que vino de la vista en el listado 
                                    //que acabo de buscar en el servidor, y la que no exista  
                                    //en el listado que acabo de buscar desde la base de datos
                                    //quiere decir que el doctor la elimino

                                    //OJO en esta lista EditRectlist solo estan las recetas de esta consulta de este paciente
                                    var existe = usar_resetaCompList.Where(x => x.DoctSecuencia_fk == usu.doctSecuencia
                                                                          && x.PaciSecuencia_fk == recetaParaBorrar.PaciSecuencia_fk
                                                                          && x.RecCodigo == recetaParaBorrar.RecCodigo
                                                                          && x.ReceSecuencia == recetaParaBorrar.ReceSecuencia).Any();
                                    if (existe)
                                    {
                                        continue;
                                    }

                                    //BORRAR TODOS  receta medicamentos 
                                    var receMEdicameBorrar = new List<RecetaMedicamento>();
                                    receMEdicameBorrar = (from rform in db.RecetaMedicamentos
                                                          where rform.DoctSecuencia_fk == usu.doctSecuencia
                                                             && rform.PaciSecuencia_fk == usar_Consultar.PaciSecuencia_fk
                                                              && rform.CMHistSecuencia_fk == usar_Consultar.CMHistSecuencia
                                                          && rform.ReceSecuencia_fk == recetaParaBorrar.ReceSecuencia
                                                          select rform).ToList();


                                    if (receMEdicameBorrar != null)
                                    {
                                        foreach (var Recmedi in receMEdicameBorrar)
                                        {

                                            db.RecetaMedicamentos.Remove(Recmedi);
                                        }
                                    }

                                    //BORRAR TODOS  receta analisis clinico 
                                    var receAnalisiBorrar = new List<RecetaAnalisisClinico>();
                                    receAnalisiBorrar = (from rform in db.RecetaAnalisisClinicoes
                                                         where rform.DoctSecuencia_fk == usu.doctSecuencia
                                                            && rform.PaciSecuencia_fk == usar_Consultar.PaciSecuencia_fk
                                                                && rform.CMHistSecuencia_fk == usar_Consultar.CMHistSecuencia
                                                         && rform.ReceSecuencia_fk == recetaParaBorrar.ReceSecuencia
                                                         select rform).ToList();

                                    if (receAnalisiBorrar != null)
                                    {

                                        foreach (var ReAnaliClini in receAnalisiBorrar)
                                        {

                                            db.RecetaAnalisisClinicoes.Remove(ReAnaliClini);
                                        }
                                    }

                                    //BORRAR TODOS  receta imagenes
                                    var receImagenesBorrar = new List<RecetaImagene>();
                                    receImagenesBorrar = (from rform in db.RecetaImagenes
                                                          where rform.DoctSecuencia_fk == usu.doctSecuencia
                                                             && rform.PaciSecuencia_fk == usar_Consultar.PaciSecuencia_fk
                                                                     && rform.CMHistSecuencia_fk == usar_Consultar.CMHistSecuencia
                                                          && rform.ReceSecuencia_fk == recetaParaBorrar.ReceSecuencia
                                                          select rform).ToList();

                                    if (receImagenesBorrar != null)
                                    {
                                        foreach (var rimag in receImagenesBorrar)
                                        {

                                            db.RecetaImagenes.Remove(rimag);
                                        }
                                    }

                                    //borro la receta
                                    db.Recetas.Remove(recetaParaBorrar);
                                }//fin receta borrar  todas  en edicion
                                #endregion


                                //Sino existe la receta la  Inserto y esta es  la secuencia
                                int insertNuevaRecEdicion = 0;
                                string cantidadReceDoct = "";

                                //buscando la proxima secuencia, de historial
                                insertNuevaRecEdicion = ((from numSecu in db.Recetas
                                                          where numSecu.DoctSecuencia_fk == usu.doctSecuencia
                                                          && numSecu.CMHistSecuencia_fk == (int)usar_Consultar.CMHistSecuencia
                                                          && numSecu.PaciSecuencia_fk == usar_Consultar.PaciSecuencia_fk
                                                          select (int?)numSecu.ReceSecuencia).Max() ?? 0) + 1;

                                //para crear el codigo de secuencia
                                cantidadReceDoct = ((from objBusc in db.Recetas
                                                     where objBusc.DoctSecuencia_fk == usu.doctSecuencia
                                                     orderby objBusc.RecCodigo.ToString().Substring(objBusc.RecCodigo.ToString().IndexOf("-") + 1) descending
                                                     select objBusc.RecCodigo)).Take(1).SingleOrDefault();


                                if (!string.IsNullOrEmpty(cantidadReceDoct))
                                {

                                    //cantidadDoct.IndexOf("-")  +1 es por que el index me trae desde donde esta este -(guion) y yo necesito
                                    //desde el siguiente  letra
                                    cantidadReceDoct = cantidadReceDoct.Substring(cantidadReceDoct.IndexOf("-") + 1);
                                    // para dejar solo el numero convierto a int y asi borrar los ceros que tiene a lante
                                    int n = int.Parse(cantidadReceDoct);
                                    // convierto a string nuevamente sin los ceros que tenia alante
                                    string valor = n.ToString();
                                    //cantidad de digitos del numero del doctor
                                    int cantDig = usu.doctSecuencia.ToString().Length;
                                    //quito la lantidad delantera que corresponde al doctor y dejo solo la secuencia con los ceros alante
                                    cantidadReceDoct = valor.Substring(cantDig);


                                }


                                foreach (var item in usar_resetaCompList)
                                {

                                    //busco la
                                    var EditRect = (from numSecu in db.Recetas
                                                    where numSecu.DoctSecuencia_fk == usu.doctSecuencia
                                                    && numSecu.CMHistSecuencia_fk == usar_Consultar.CMHistSecuencia
                                                    && numSecu.PaciSecuencia_fk == usar_Consultar.PaciSecuencia_fk
                                                    && numSecu.ReceSecuencia == item.ReceSecuencia
                                                    select numSecu).SingleOrDefault();

                                    #region EN MODO EDICION  EDITO LAS RECETAS
                                    //si existe edita la receta edito
                                    if (EditRect != null)
                                    {
                                        EditRect.ReceComentario = item.ReceComentario;
                                        EditRect.RecNombre = item.RecNombre;
                                        EditRect.ReceInstruccionesFarmacia = "";
                                        EditRect.ReceInstruccionesAlPaciente = "";
                                        EditRect.UsuaSecuenciaModificacion = usu.usuario.UsuaSecuencia;
                                        EditRect.UsuaFechaModificacion = Lib.GetLocalDateTime();

                                        #region DETALLE DE LA RECETA COMO MEDICAMENTEOS ANALISI IMAGENES
                                        //BORRAR TODOS  receta medicamentos 
                                        var receMEdicameBorrar = new List<RecetaMedicamento>();
                                        receMEdicameBorrar = (from rform in db.RecetaMedicamentos
                                                              where rform.DoctSecuencia_fk == usu.doctSecuencia
                                                                 && rform.PaciSecuencia_fk == usar_Consultar.PaciSecuencia_fk
                                                                  && rform.CMHistSecuencia_fk == usar_Consultar.CMHistSecuencia
                                                              && rform.ReceSecuencia_fk == EditRect.ReceSecuencia
                                                              select rform).ToList();

                                        if (receMEdicameBorrar != null)
                                        {
                                            foreach (var Recmedi in receMEdicameBorrar)
                                            {

                                                db.RecetaMedicamentos.Remove(Recmedi);
                                            }
                                        }

                                        //BORRAR TODOS  receta analisis clinico 
                                        var receAnalisiBorrar = new List<RecetaAnalisisClinico>();
                                        receAnalisiBorrar = (from rform in db.RecetaAnalisisClinicoes
                                                             where rform.DoctSecuencia_fk == usu.doctSecuencia
                                                                && rform.PaciSecuencia_fk == usar_Consultar.PaciSecuencia_fk
                                                                    && rform.CMHistSecuencia_fk == usar_Consultar.CMHistSecuencia
                                                             && rform.ReceSecuencia_fk == EditRect.ReceSecuencia
                                                             select rform).ToList();

                                        if (receAnalisiBorrar != null)
                                        {

                                            foreach (var ReAnaliClini in receAnalisiBorrar)
                                            {

                                                db.RecetaAnalisisClinicoes.Remove(ReAnaliClini);
                                            }
                                        }

                                        //BORRAR TODOS  receta imagenes
                                        var receImagenesBorrar = new List<RecetaImagene>();
                                        receImagenesBorrar = (from rform in db.RecetaImagenes
                                                              where rform.DoctSecuencia_fk == usu.doctSecuencia
                                                                 && rform.PaciSecuencia_fk == usar_Consultar.PaciSecuencia_fk
                                                                         && rform.CMHistSecuencia_fk == usar_Consultar.CMHistSecuencia
                                                              && rform.ReceSecuencia_fk == EditRect.ReceSecuencia
                                                              select rform).ToList();

                                        if (receImagenesBorrar != null)
                                        {
                                            foreach (var rimag in receImagenesBorrar)
                                            {

                                                db.RecetaImagenes.Remove(rimag);
                                            }
                                        }


                                        //insertar  receta Medicamentos
                                        if (item.MediSecuencia_fk != null)
                                        {
                                            int medsec = 0;
                                            foreach (int medisec in item.MediSecuencia_fk)
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
                                                    RMedi.PaciSecuencia_fk = usar_Consultar.PaciSecuencia_fk;
                                                    RMedi.ReceSecuencia_fk = (int)EditRect.ReceSecuencia;
                                                    RMedi.CMHistSecuencia_fk = (int)usar_Consultar.CMHistSecuencia;
                                                    RMedi.MediSecuencia_fk = medisec;
                                                    RMedi.RMediSecuencia = medsec;
                                                    RMedi.EstaDesabilitado = false;

                                                    db.RecetaMedicamentos.Add(RMedi);
                                                }
                                            }
                                        }
                                        //insertar receta analisis
                                        if (item.AClinSecuencia_fk != null)
                                        {
                                            int analisecc = 0;
                                            foreach (int anali in item.AClinSecuencia_fk)
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
                                                    Ranali.PaciSecuencia_fk = usar_Consultar.PaciSecuencia_fk;
                                                    Ranali.ReceSecuencia_fk = (int)EditRect.ReceSecuencia;
                                                    Ranali.CMHistSecuencia_fk = (int)usar_Consultar.CMHistSecuencia;
                                                    Ranali.AClinSecuencia_fk = anali;
                                                    Ranali.RAClinSecuencia = analisecc;
                                                    Ranali.EstaDesabilitado = false;
                                                    db.RecetaAnalisisClinicoes.Add(Ranali);
                                                }
                                            }
                                        }
                                        //insertar  receta imagenes
                                        if (item.ImagSecuencia_fk != null)
                                        {
                                            int analisecc = 0;
                                            foreach (int imag in item.ImagSecuencia_fk)
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
                                                    RImage.PaciSecuencia_fk = usar_Consultar.PaciSecuencia_fk;
                                                    RImage.ReceSecuencia_fk = (int)EditRect.ReceSecuencia;
                                                    RImage.CMHistSecuencia_fk = (int)usar_Consultar.CMHistSecuencia;
                                                    RImage.ImagSecuencia_fk = imag;
                                                    RImage.RImagSecuencia = analisecc;
                                                    RImage.EstaDesabilitado = false;
                                                    db.RecetaImagenes.Add(RImage);
                                                }
                                            }

                                        }
                                        #endregion

                                    }
                                    #endregion

                                    #region EN MODO DE EDICION INSERTO RECETAS NUEVAS
                                    //si NO existe ESTA RECETA LA INERTO
                                    else
                                    {

                                        Receta objReceta = new Receta();
                                        objReceta.DoctSecuencia_fk = usu.doctSecuencia;
                                        objReceta.PaisSecuencia_fk = usu.Consultorio.PaisSecuencia;
                                        objReceta.ClinSecuencia_fk = usu.Consultorio.clinSecuencia_fk;
                                        objReceta.ConsSecuencia_fk = usu.Consultorio.ConsSecuencia_fk;
                                        objReceta.PaciSecuencia_fk = (int)usar_Consultar.PaciSecuencia_fk;
                                        objReceta.EstaDesabilitado = false;
                                        objReceta.CMHistSecuencia_fk = (int)usar_Consultar.CMHistSecuencia;
                                        objReceta.RecNombre = item.RecNombre;
                                        objReceta.ReceSecuencia = insertNuevaRecEdicion;
                                        objReceta.ReceFecha = Lib.GetLocalDateTime();
                                        objReceta.ReceComentario = item.ReceComentario;
                                        objReceta.ReceInstruccionesFarmacia = "";
                                        objReceta.ReceInstruccionesAlPaciente = "";
                                        objReceta.UsuaSecuenciaCreacion = usu.usuario.UsuaSecuencia;
                                        objReceta.UsuaFechaCreacion = Lib.GetLocalDateTime();


                                        ////agregar codigo      
                                        //aqui sumo uno a la secuencia
                                        if (string.IsNullOrEmpty(cantidadReceDoct))
                                            cantidadReceDoct = "0";
                                        int sum = int.Parse(cantidadReceDoct) + 1;
                                        cantidadReceDoct = sum.ToString();


                                        //quito los ceros delanteros
                                        //int secMaxSinCerosDelanterosRec = int.Parse(cantidadReceDoct);
                                        //codigo con ceros doctor y secuencia
                                        string codigRecDoctFormateado = Lib.FormatearCodigo(3, usu.doctSecuencia.ToString());
                                        string codiRecSecGeneral = Lib.FormatearCodigo(6, cantidadReceDoct);
                                        string codFormateadoDefiniRec = codigRecDoctFormateado + codiRecSecGeneral;
                                        codFormateadoDefiniRec = "CR-" + codFormateadoDefiniRec;
                                        objReceta.RecCodigo = codFormateadoDefiniRec;

                                        db.Recetas.Add(objReceta);



                                        //insertar  receta Medicamentos
                                        if (item.MediSecuencia_fk != null)
                                        {
                                            int medsec = 0;
                                            foreach (int medisec in item.MediSecuencia_fk)
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
                                                    RMedi.PaciSecuencia_fk = usar_Consultar.PaciSecuencia_fk;
                                                    RMedi.ReceSecuencia_fk = objReceta.ReceSecuencia;
                                                    RMedi.CMHistSecuencia_fk = (int)usar_Consultar.CMHistSecuencia;
                                                    RMedi.MediSecuencia_fk = medisec;
                                                    RMedi.RMediSecuencia = medsec;
                                                    RMedi.EstaDesabilitado = false;
                                                    db.RecetaMedicamentos.Add(RMedi);
                                                }
                                            }
                                        }
                                        //insertar receta analisis
                                        if (item.AClinSecuencia_fk != null)
                                        {
                                            int analisecc = 0;
                                            foreach (int anali in item.AClinSecuencia_fk)
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
                                                    Ranali.PaciSecuencia_fk = usar_Consultar.PaciSecuencia_fk;
                                                    Ranali.ReceSecuencia_fk = objReceta.ReceSecuencia;
                                                    Ranali.CMHistSecuencia_fk = (int)usar_Consultar.CMHistSecuencia;
                                                    Ranali.AClinSecuencia_fk = anali;
                                                    Ranali.RAClinSecuencia = analisecc;
                                                    Ranali.EstaDesabilitado = false;
                                                    db.RecetaAnalisisClinicoes.Add(Ranali);
                                                }
                                            }
                                        }
                                        //insertar  receta imagenes
                                        if (item.ImagSecuencia_fk != null)
                                        {
                                            int analisecc = 0;
                                            foreach (int imag in item.ImagSecuencia_fk)
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
                                                    RImage.PaciSecuencia_fk = usar_Consultar.PaciSecuencia_fk;
                                                    RImage.ReceSecuencia_fk = objReceta.ReceSecuencia;
                                                    RImage.CMHistSecuencia_fk = (int)usar_Consultar.CMHistSecuencia;
                                                    RImage.ImagSecuencia_fk = imag;
                                                    RImage.RImagSecuencia = analisecc;
                                                    RImage.EstaDesabilitado = false;
                                                    db.RecetaImagenes.Add(RImage);
                                                }
                                            }

                                        }

                                        insertNuevaRecEdicion++;

                                    }//Fin si NO existe ESTA RECETA LA INERTO

                                    #endregion


                                }//fin editRect
                            }

                            #endregion



                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Editar.ToString(), null);
                            #endregion
                        }

                        int returnsave = db.SaveChanges();
                        dbtrans.Commit();

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

         [HttpPost]
        public JsonResult Editar(Usar_Consultar usar_ConsultarEditando)
        {


            var respuesta = new ResponseModel();
            //Si NO esta loguieado lo redireccionara al loguin
            if (HttpContext.Session["user"] == null)
            {
                respuesta.respuesta = false;
                respuesta.error = "Usted debe de loguiarse.";
                respuesta.redirect = Url.Action("Index", "PaginaPresentacion");

                //return Json(respuesta);

                return new JsonNetResult() { Data = respuesta };
            }
            UsuarioLoguiado usu = (UsuarioLoguiado)HttpContext.Session["user"];
            using (var db = new DoctorMedicalWebEntities())
            {
                if (usar_ConsultarEditando != null)
                {

                    var objEdit = db.ConsultaMedicaHistorials.Where(ro =>
                                          ro.DoctSecuencia_fk == usu.doctSecuencia
                                          && ro.CMHistSecuencia == usar_ConsultarEditando.CMHistSecuencia
                                       && ro.PaciSecuencia_fk == usar_ConsultarEditando.PaciSecuencia_fk
                                   ).SingleOrDefault();

                    if (objEdit != null)
                    {

                        // motivo consulta

                        var motiConsultalist = db.ConsultaMedicaHistorialMotivoConsultas.Where(ro =>
                                          ro.DoctSecuencia_fk == usu.doctSecuencia
                                              && ro.CMHistSecuencia_fk == usar_ConsultarEditando.CMHistSecuencia
                                           && ro.PaciSecuencia_fk == usar_ConsultarEditando.PaciSecuencia_fk

                                    ).ToList().Select(t => t.MConsSecuencia_fk);


                        //// Evaluacion Fisica consulta
                        var EvaluacionFisicalist = db.ConsultaMedicaHistorialEvaluacionFisicas.Where(ro =>
                                           ro.DoctSecuencia_fk == usu.doctSecuencia
                                           && ro.CMHistSecuencia_fk == usar_ConsultarEditando.CMHistSecuencia
                                        && ro.PaciSecuencia_fk == usar_ConsultarEditando.PaciSecuencia_fk
                                    ).ToList().Select(t => t.EFisiSecuencia_fk);

                        // Diagnostico consulta
                        var DiagnostiConsulList = db.ConsultaMedicaHistorialDiagnosticoes.Where(ro =>
                                           ro.DoctSecuencia_fk == usu.doctSecuencia
                                           && ro.CMHistSecuencia_fk == usar_ConsultarEditando.CMHistSecuencia
                                        && ro.PaciSecuencia_fk == usar_ConsultarEditando.PaciSecuencia_fk
                                    ).ToList().Select(t => t.DiagSecuencia);

                        // Tratamiento consulta
                        var TratamientoList = db.ConsultaMedicaHistorialTratamientoes.Where(ro =>
                                           ro.DoctSecuencia_fk == usu.doctSecuencia
                                           && ro.CMHistSecuencia_fk == usar_ConsultarEditando.CMHistSecuencia
                                        && ro.PaciSecuencia_fk == usar_ConsultarEditando.PaciSecuencia_fk
                                    ).ToList().Select(t => t.TratSecuencia_fk); ;



                        //Antecedentes master
                        var objAntesedentesEdit = db.ConsultaMedicas.Where(ro =>
                                              ro.DoctSecuencia_fk == usu.doctSecuencia
                                           && ro.PaciSecuencia_fk == usar_ConsultarEditando.PaciSecuencia_fk
                                       ).SingleOrDefault();

                        //Nota: los datos de embarazo como fecha, dias,etc. como ese esta buscando 
                        //una consulta en particular para editar, se debe de buscar en la tabla de historial de datos
                        //de embarazo (ConsultaMedicaEmbarazo) y asignar esos valores a este objeto de ConsultaMedicas,
                        //por que los controles text razor que tienen los datos de embarazo  pertenecen a esta clase ConsultaMedica
                        //(Esta clase consultamedica es para los datos antecedentes y se almacenan los datos de embarazo, para indicar el estado
                        //Actual del paciente.) y asi cuando el razor llene los controles podra llenar los datos de embarazo,
                        //de la fecha en cuestion de la consulta.

                        //Historial embarazo
                        var objDatosEmbarazoEdit = db.ConsultaMedicaEmbarazoes.Where(ro =>
                                              ro.DoctSecuencia_fk == usu.doctSecuencia
                                           && ro.PaciSecuencia_fk == usar_ConsultarEditando.PaciSecuencia_fk
                                           && ro.CMHistSecuencia_fk == usar_ConsultarEditando.CMHistSecuencia
                                       ).SingleOrDefault();
                        if (objDatosEmbarazoEdit != null)
                        {
                            //asigno los datos de embarazo para que se muestren en los input text con razor
                            objAntesedentesEdit.CMedEmbarazada = objDatosEmbarazoEdit.CMEmbEmbarazada;
                            objAntesedentesEdit.CMedEmbarazadaFecha = objDatosEmbarazoEdit.CMEmbEmbarazadaFecha;
                            objAntesedentesEdit.CMedEmbarazadaSemanas = objDatosEmbarazoEdit.CMEmbEmbarazadaSemanas;
                            objAntesedentesEdit.CMedEmbarazadaDias = objDatosEmbarazoEdit.CMEmbEmbarazadaDias;
                            objAntesedentesEdit.CMedEmbarazadaMeses = objDatosEmbarazoEdit.CMEmbEmbarazadaMeses;
                            objAntesedentesEdit.CMedEmbarazadaFechaProbableParto = objDatosEmbarazoEdit.CMEmbEmbarazadaFechaProbableParto;
                        }
                        //para esa fecha no estaba embarazada
                        else
                        {

                            objAntesedentesEdit.CMedEmbarazada = false;
                            objAntesedentesEdit.CMedEmbarazadaFecha = null;
                            objAntesedentesEdit.CMedEmbarazadaSemanas = null;
                            objAntesedentesEdit.CMedEmbarazadaDias = null;
                            objAntesedentesEdit.CMedEmbarazadaMeses = null;
                            objAntesedentesEdit.CMedEmbarazadaFechaProbableParto = null;
                        }



                        // TODOS  Antecedentes familiares 
                        var objEnfermeFamiliEdit = db.ConsultaMedicaEnfermedaFamiliars.Where(ro =>
                                              ro.DoctSecuencia_fk == usu.doctSecuencia
                                                  && ro.CMediSecuencia_fk == objAntesedentesEdit.CMediSecuencia
                                           && ro.PaciSecuencia_fk == usar_ConsultarEditando.PaciSecuencia_fk
                                       ).SingleOrDefault();




                        // TODOs antecedentes Enfermedades Personales                 
                        var AntecedentesPersonalesEnfeList = (from rform in db.ConsultaMedicaEnfermedas
                                                              where rform.DoctSecuencia_fk == usu.doctSecuencia
                                                               && rform.PaciSecuencia_fk == usar_ConsultarEditando.PaciSecuencia_fk
                                                               && rform.CMediSecuencia_fk == objAntesedentesEdit.CMediSecuencia
                                                              select rform).ToList().Select(t => t.EnfeSecuencia_fk); ;


                        //usar Consulta historial 
                        Usar_Consultar usar_Consultar = new Usar_Consultar();
                        CopyClass.CopyObject(objEdit, ref usar_Consultar);


                        // motivo consulta
                        usar_Consultar.MConsSecuencia_fk = motiConsultalist.ToList();
                        usar_Consultar.EFisiSecuencia_fk = EvaluacionFisicalist.ToList();
                        usar_Consultar.DiagSecuencia = DiagnostiConsulList.ToList();
                        usar_Consultar.TratSecuencia_fk = TratamientoList.ToList();
                        usar_Consultar.EnfeComentarioPersonal = objAntesedentesEdit.EnfeComentario;
                        usar_Consultar.EnfeComentarioFamiliar = objEnfermeFamiliEdit.EnfeComentario;
                        //antecedentes consultaMedica
                        CopyClass.CopyObject(objAntesedentesEdit, ref usar_Consultar);
                        //enfermidad familia  antece
                        usar_Consultar.EnfeComentarioFamiliar = objEnfermeFamiliEdit.EnfeComentario;
                        //Lista Enfermedades personales anteced
                        usar_Consultar.EnfeSecuenciaPersonal = AntecedentesPersonalesEnfeList.ToList();
                        //

                        //LLenar paciente
                        var objPaci = db.Pacientes.Where(ro =>
                            ro.DoctSecuencia_fk == usu.doctSecuencia
                         && ro.PaciSecuencia == usar_ConsultarEditando.PaciSecuencia_fk).SingleOrDefault();

                        usar_Consultar.PaciSecuencia_fk = objPaci.PaciSecuencia;
                        usar_Consultar.PaciNombre = objPaci.PaciNombre;
                        usar_Consultar.PaciApellido1 = objPaci.PaciApellido1;
                        usar_Consultar.PaciApellido2 = objPaci.PaciApellido2;
                        usar_Consultar.PaciDocumento = objPaci.PaciDocumento;
                        usar_Consultar.PaciEdad = objPaci.PaciEdad;
                        usar_Consultar.PaciFotoPath = objPaci.PaciFotoPath;
                        usar_Consultar.GSangSecuencia_fk = objPaci.GSangSecuencia_fk;
                        //Ultima fecha de consulta
                        DateTime? ultimacon = (from d in db.ConsultaMedicaHistorials
                                               where d.DoctSecuencia_fk == usu.doctSecuencia
                                               && d.PaciSecuencia_fk == usar_ConsultarEditando.PaciSecuencia_fk
                                               orderby d.CMHistSecuencia descending
                                               select d.CMHistFecha).FirstOrDefault();


                        if (ultimacon != null)
                        {
                            usar_Consultar.UltimaFechaConsulta = ultimacon.Value.ToString("dd/MM/yyyy");

                        }
                        else
                        {
                            usar_Consultar.UltimaFechaConsulta = "[N/A]";

                        }
                        usar_Consultar.PaciFechaNacimiento = objPaci.PaciFechaNacimiento;



                        // Recetas  master
                        List<Receta> RecetaList = db.Recetas.Where(ro =>
                                           ro.DoctSecuencia_fk == usu.doctSecuencia
                                           && ro.CMHistSecuencia_fk == usar_ConsultarEditando.CMHistSecuencia
                                        && ro.PaciSecuencia_fk == usar_ConsultarEditando.PaciSecuencia_fk

                                    ).OrderByDescending(x => x.ReceSecuencia).ToList();

                        List<Usar_RecetaComplementaria> usar_recetaComplementariaLIst = new List<Usar_RecetaComplementaria>();

                        //lista de recetas 
                        foreach (var rece in RecetaList)
                        {
                            Usar_RecetaComplementaria usar_recetaComplemen = new Usar_RecetaComplementaria();
                            //antecedentes consultaMedica
                            CopyClass.CopyObject(rece, ref usar_recetaComplemen);

                            //LLenar paciente para la receta


                            usar_recetaComplemen.PaciSecuencia_fk = objPaci.PaciSecuencia;
                            //usar_recetaComplemen.PaciNombre = objPaci.PaciNombre;
                            //usar_recetaComplemen.PaciApellido1 = objPaci.PaciApellido1;
                            //usar_recetaComplemen.PaciApellido2 = objPaci.PaciApellido2;
                            //usar_recetaComplemen.PaciDocumento = objPaci.PaciDocumento;

                            usar_recetaComplemen.PaciNombre = "Receta";

                            // TODOS  receta medicamentos                   
                            List<int> RecetaMedicaList = (from rform in db.RecetaMedicamentos
                                                          where rform.DoctSecuencia_fk == usu.doctSecuencia
                                                          && rform.PaciSecuencia_fk == usar_ConsultarEditando.PaciSecuencia_fk
                                                          && rform.CMHistSecuencia_fk == rece.CMHistSecuencia_fk
                                                          && rform.ReceSecuencia_fk == rece.ReceSecuencia
                                                          select rform.MediSecuencia_fk).ToList();

                            usar_recetaComplemen.MediSecuencia_fk = RecetaMedicaList.ToList();

                            // TODOS  receta analisis clinico 
                            List<int> RecetaAnalisiList = (from rform in db.RecetaAnalisisClinicoes
                                                           where rform.DoctSecuencia_fk == usu.doctSecuencia
                                                           && rform.PaciSecuencia_fk == usar_ConsultarEditando.PaciSecuencia_fk
                                                            && rform.CMHistSecuencia_fk == usar_ConsultarEditando.CMHistSecuencia
                                                             && rform.ReceSecuencia_fk == rece.ReceSecuencia
                                                           select rform.AClinSecuencia_fk).ToList();

                            usar_recetaComplemen.AClinSecuencia_fk = RecetaAnalisiList;

                            // TODOS  receta imagenes
                            List<int> RecetaImagenesList = (from rform in db.RecetaImagenes
                                                            where rform.DoctSecuencia_fk == usu.doctSecuencia
                                                            && rform.PaciSecuencia_fk == usar_ConsultarEditando.PaciSecuencia_fk
                                                             && rform.CMHistSecuencia_fk == usar_ConsultarEditando.CMHistSecuencia
                                                               && rform.ReceSecuencia_fk == rece.ReceSecuencia
                                                            select rform.ImagSecuencia_fk).ToList();

                            usar_recetaComplemen.ImagSecuencia_fk = RecetaImagenesList;

                            usar_recetaComplementariaLIst.Add(usar_recetaComplemen);
                        }

                        //consultar  las consultar
                        Session["Usar_Consultar"] = usar_Consultar;

                        //recetas
                        Session["Usar_RecetaComplementariaList"] = usar_recetaComplementariaLIst;


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
            //return Json(respuesta);
            //new JsonNetResult() { Data=data.FirstOrDefault() }
            return new JsonNetResult() { Data = respuesta };
        }

         [HttpPost]
        //Borrar Paciente
        public JsonResult Borrar(Usar_Consultar usar_Consultar)
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
                if (usar_Consultar.CMHistSecuencia == null || usar_Consultar.CMHistSecuencia < 1)
                {
                    respuesta.respuesta = false;
                    respuesta.error = "Favor seleccionar una Consulta";
                    respuesta.redirect = Url.Action(vista, controler);
                    return Json(respuesta);

                }
                using (var dbtrans = db.Database.BeginTransaction())
                {
                    try
                    {
                        //borro registro
                        if (usar_Consultar != null)
                        {

                            //var ObjNuevToSave = new Medicamento();
                            ////asignar contenido a otro objeto
                            //CopyClass.CopyObject(usar_Consultar, ref ObjNuevToSave);


                            var borrar = db.ConsultaMedicaHistorials.Where(ro =>
                                              ro.DoctSecuencia_fk == usu.doctSecuencia
                                           && ro.PaciSecuencia_fk == usar_Consultar.PaciSecuencia_fk
                                           && ro.CMHistSecuencia == usar_Consultar.CMHistSecuencia).SingleOrDefault();

                            if (borrar != null)
                            {
                                //Desabilitar tambien las recetas de esta consulta
                                var borrarRecet = db.Recetas.Where(ro =>
                                            ro.DoctSecuencia_fk == usu.doctSecuencia
                                         && ro.PaciSecuencia_fk == usar_Consultar.PaciSecuencia_fk
                                         && ro.CMHistSecuencia_fk == usar_Consultar.CMHistSecuencia).ToList();
                                if (borrarRecet != null)
                                {
                                    foreach (var item in borrarRecet)
                                    {
                                        //desabilito las recetas de esta consulta
                                        item.EstaDesabilitado = true;

                                    }
                                }


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

        public ActionResult Consultalista()
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
                var objelist = (from tf in db.ConsultaMedicaHistorials
                                where tf.DoctSecuencia_fk == usu.doctSecuencia
                                && tf.EstaDesabilitado == false
                                orderby tf.CMHistSecuencia descending
                                select tf).ToList();

                //var listObjToShow = new List<Usar_Receta>();
                var listObjToShow = new List<Usar_Consultar>();
                //crear objeto tipo formulario y asignar a la lista de usartipoformulario
                foreach (var item in objelist)
                {
                    //var UsarObjNewToShow = new Usar_Receta();
                    var UsarObjNewToShow = new Usar_Consultar();
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
