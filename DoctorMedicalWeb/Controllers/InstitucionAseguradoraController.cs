using DoctorMedicalWeb.App_Data;
using DoctorMedicalWeb.Libreria;
using DoctorMedicalWeb.Models;
using DoctorMedicalWeb.ModelsComplementarios;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoctorMedicalWeb.Controllers
{
    public class InstitucionAseguradoraController : Controller
    {
        string controler = "InstitucionAseguradora", vista = "Ini_InstitucionAseguradora", PaginaAutorizada = Paginas.pag_InstitucionesAseguradoras.ToString();

        //DoctorMedicalWebEntities db = null;
        //DbContextTransaction dbtrans = null;
        //
        // GET: /InstitucionAseguradora/
        public ActionResult Ini_InstitucionAseguradora()
        {

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
            ViewBag.ListaPlanesDeSeguro = new List<Usar_InstitucionAseguradoraPlanes>();

            Usar_InstitucionesAseguradora usarinstitucionase = new Usar_InstitucionesAseguradora();
            //si  esta lleno es por que esta editando
            if (Session["Usar_InstitucionesAseguradora"] != null)
            {
                usarinstitucionase = (Usar_InstitucionesAseguradora)Session["Usar_InstitucionesAseguradora"];
                //limpiar sesion
                Session["Usar_InstitucionesAseguradora"] = null;

                //llenar  los planes del seguro
                ViewBag.ListaPlanesDeSeguro = (List<Usar_InstitucionAseguradoraPlanes>)Session["planesDeAseguradora"];
                Session["planesDeAseguradora"] = null;

                return View(usarinstitucionase);
            }

            return View(usarinstitucionase);
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
                    List<InstitucionesAseguradora> InsitucionAsegListaUltimosCinco = (from tf in db.InstitucionesAseguradoras
                                                                                      where tf.DoctSecuencia == usu.doctSecuencia
                                                                                      orderby tf.IAsegSecuencia descending
                                                                                      select tf).Take(5).ToList();

                    var listUsar_InstitucionAsegur = new List<Usar_InstitucionesAseguradora>();
                    //crear objeto role y asignar a la lista de usaroles
                    foreach (var item in InsitucionAsegListaUltimosCinco)
                    {
                        Usar_InstitucionesAseguradora formulario = new Usar_InstitucionesAseguradora();
                        //asignar contenido a otro objeto
                        CopyClass.CopyObject(item, ref formulario);
                        listUsar_InstitucionAsegur.Add(formulario);
                    }
                    //Utilizado para insertar el listado al grid
                    ViewBag.ultimosCinco = listUsar_InstitucionAsegur;


                    //ienumerable lista
                    respuesta.someCollection = listUsar_InstitucionAsegur;


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

        public ActionResult InstitucionAseguradoralista()
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
                List<InstitucionesAseguradora> InstitucionAseg = (from tf in db.InstitucionesAseguradoras
                                                                  where tf.DoctSecuencia == usu.doctSecuencia
                                                                  orderby tf.IAsegNombre
                                                                  select tf).ToList();

                var listInstitucionAseg = new List<Usar_InstitucionesAseguradora>();
                //crear objeto tipo formulario y asignar a la lista de usartipoformulario
                foreach (var item in InstitucionAseg)
                {
                    Usar_InstitucionesAseguradora UsarInstitucionAseg = new Usar_InstitucionesAseguradora();
                    //asignar contenido a otro objeto
                    CopyClass.CopyObject(item, ref UsarInstitucionAseg);
                    listInstitucionAseg.Add(UsarInstitucionAseg);
                }
                ViewBag.datasource = listInstitucionAseg;
                return View();
            }
        }
          [HttpPost]
        public JsonResult Borrar(Usar_InstitucionesAseguradora usar_InstitucionAsegur)
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
                if (usar_InstitucionAsegur.IAsegSecuencia < 1 || usar_InstitucionAsegur.IAsegSecuencia == null)
                {
                    respuesta.respuesta = false;
                    respuesta.error = "Favor seleccionar una Institucion Aseguradora en el listado";
                    respuesta.redirect = Url.Action(vista, controler);
                    return Json(respuesta);

                }
                using (var dbtrans = db.Database.BeginTransaction())
                {
                    try
                    {
                        //borro registro
                        if (usar_InstitucionAsegur != null)
                        {

                            InstitucionesAseguradora institucionAse = new InstitucionesAseguradora();
                            //asignar contenido a otro objeto
                            CopyClass.CopyObject(usar_InstitucionAsegur, ref institucionAse);


                            InstitucionesAseguradora borrar = (from InstiSeg in db.InstitucionesAseguradoras
                                                               where InstiSeg.DoctSecuencia == usu.doctSecuencia
                                                                  && InstiSeg.IAsegSecuencia == usar_InstitucionAsegur.IAsegSecuencia
                                                               select InstiSeg).SingleOrDefault();

                            if (borrar != null)
                            {
                                //BORRAR TODOS  planes 
                                List<InstitucionAseguradoraPlane> PlanesBorrar = new List<InstitucionAseguradoraPlane>();
                                PlanesBorrar = (from rform in db.InstitucionAseguradoraPlanes
                                                where rform.DoctSecuencia == usu.doctSecuencia
                                                  && rform.IAsegSecuencia == borrar.IAsegSecuencia
                                                select rform).ToList();


                                foreach (var item in PlanesBorrar)
                                {
                                    //borrar actuales formulraios asignados a este rol
                                    db.InstitucionAseguradoraPlanes.Remove(item);
                                }

                                db.InstitucionesAseguradoras.Remove(borrar);
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
        public JsonResult Editar(Usar_InstitucionesAseguradora usar_InsitucionAseg)
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
                if (usar_InsitucionAseg != null)
                {

                    InstitucionesAseguradora InstAsegur = (from IA in db.InstitucionesAseguradoras
                                                           where IA.DoctSecuencia == usu.doctSecuencia
                                                           && IA.IAsegSecuencia == usar_InsitucionAseg.IAsegSecuencia

                                                           select IA).SingleOrDefault();

                    if (InstAsegur != null)
                    {
                        Usar_InstitucionesAseguradora usarInsitucion = new Usar_InstitucionesAseguradora();
                        //asignar contenido a otro objeto
                        CopyClass.CopyObject(InstAsegur, ref usarInsitucion);
                        //llenno la sesion con el role a editar
                        //para abrirlo en  el metodo principal del controler
                        Session["Usar_InstitucionesAseguradora"] = usarInsitucion;

                        //List<Usar_InstitucionAseguradoraPlanes> AseguraPlanes = new List<Usar_InstitucionAseguradoraPlanes>();
                        //AseguraPlanes=listaInstitucionAseguradoraPlanes((int)usarInsitucion.IAsegSecuencia);

                        Session["planesDeAseguradora"] = listaInstitucionAseguradoraPlanes((int)usarInsitucion.IAsegSecuencia);
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
        public JsonResult Save(Usar_InstitucionesAseguradora usar_InsitucionAseg, List<Usar_InstitucionAseguradoraPlanes> planes)
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

            //si el rnc pasa de 19 digitos no lo dejo seguir

            if (usar_InsitucionAseg.IAsegRNC.ToString().Length > 9 || usar_InsitucionAseg.IAsegRNC.ToString().Length < 1)
            {
                respuesta.respuesta = false;
                respuesta.error = "El Rnc debe de estar entre 1 y 9 digitos";

                return Json(respuesta);
            }



            //validar que solo sea numero en  numero seguro social y poliza
            if (!string.IsNullOrEmpty(usar_InsitucionAseg.IAsegCodigo) )
            {

                if (usar_InsitucionAseg.IAsegCodigo.All(char.IsDigit) == false)
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
                        if (string.IsNullOrEmpty(usar_InsitucionAseg.IAsegSecuencia.ToString()) || usar_InsitucionAseg.IAsegSecuencia == 0)
                        {
                            InstitucionesAseguradora insitucionAsegra = new InstitucionesAseguradora();

                            //asignar contenido a otro objeto
                            CopyClass.CopyObject(usar_InsitucionAseg, ref insitucionAsegra);

                            //vw_UsuarioDoctor usuarioDoctor = db.vw_UsuarioDoctor.Where(d => d.UsuaSecuencia == usu.usuario.UsuaSecuencia).FirstOrDefault();

                            //si existe la insituciona aseguradora del doctor no se introduce
                            var existe = (from insitucionAsegurado in db.InstitucionesAseguradoras
                                          where insitucionAsegurado.IAsegNombre == usar_InsitucionAseg.IAsegNombre
                                              && insitucionAsegurado.DoctSecuencia == usu.doctSecuencia
                                          select insitucionAsegurado).SingleOrDefault();
                            if (existe != null)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "Ya existe esta Insitucion Aseguradora";
                                respuesta.redirect = Url.Action(vista, controler);
                                return Json(respuesta);
                            }


                            insitucionAsegra.DoctSecuencia = usu.doctSecuencia;
                            insitucionAsegra.ClinSecuencia = usu.Consultorio.clinSecuencia_fk;
                            insitucionAsegra.ConsSecuencia = usu.Consultorio.ConsSecuencia_fk;
                            insitucionAsegra.usuaSecuenciaCreacion = usu.usuario.UsuaSecuencia;
                            insitucionAsegra.IAsegFechaCreacion = Lib.GetLocalDateTime();

                            //buscando la proxima secuencia, el doctor en todas sus consultorios tiene los mismos roles 
                            proximoItem = ((from InstiAsegMax in db.InstitucionesAseguradoras
                                            where InstiAsegMax.DoctSecuencia == usu.doctSecuencia
                                            select (int?)InstiAsegMax.IAsegSecuencia).Max() ?? 0) + 1;
                            insitucionAsegra.IAsegSecuencia = proximoItem;
                            db.InstitucionesAseguradoras.Add(insitucionAsegra);


                            //guardar planes


                            //ingresar nuevos Formularios perteneciente a este rol
                            if (planes != null)
                            {
                                int sec = 0;
                                foreach (Usar_InstitucionAseguradoraPlanes item in planes)
                                {
                                    ++sec;
                                    InstitucionAseguradoraPlane planesInsertar = new InstitucionAseguradoraPlane();
                                    planesInsertar.DoctSecuencia = usu.doctSecuencia;
                                    planesInsertar.ClinSecuencia = usu.Consultorio.clinSecuencia_fk;
                                    planesInsertar.ConsSecuencia = usu.Consultorio.ConsSecuencia_fk;
                                    planesInsertar.IAPlanSecuencia = sec;
                                    planesInsertar.IAPlanDescripcion = item.IAPlanDescripcion;
                                    planesInsertar.IAsegSecuencia = proximoItem;
                                    db.InstitucionAseguradoraPlanes.Add(planesInsertar);
                                }
                            }

                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Nuevo.ToString(), null);
                        }
                        //Editando 
                        else
                        {

                            //busco el objeto que se editara
                            //lo busco por la secuencia para que pueda editar ese mismo sin tener que borrarlo
                            //si lo busco por el nombre no lo  editar ya que no lo encontrara.
                            var InsAsegEditando = db.InstitucionesAseguradoras.Where(ro => ro.DoctSecuencia == usu.doctSecuencia
                                            && ro.IAsegSecuencia == usar_InsitucionAseg.IAsegSecuencia).SingleOrDefault();

                            if (InsAsegEditando == null)
                            {
                                respuesta.respuesta = false;
                                respuesta.error = "No existe Insitucion Aseguradora";
                                //respuesta.redirect = Url.Action(vista, controler);
                                return Json(respuesta);
                            }


                            //asigno al objeto que se editara los nuevos datos introducidos a editar
                            CopyClass.CopyObject(usar_InsitucionAseg, ref InsAsegEditando);

                            InsAsegEditando.usuaSecuenciaModificacion = usu.usuario.UsuaSecuencia;
                            InsAsegEditando.IAsegFechaModificacion = Lib.GetLocalDateTime();

                            //guardar planes
                            //BORRAR TODOS  planes 
                            List<InstitucionAseguradoraPlane> PlanesBorrar = new List<InstitucionAseguradoraPlane>();
                            PlanesBorrar = (from rform in db.InstitucionAseguradoraPlanes
                                            where rform.DoctSecuencia == usu.doctSecuencia
                                              && rform.IAsegSecuencia == usar_InsitucionAseg.IAsegSecuencia
                                            select rform).ToList();


                            if (planes != null)
                            {
                                //si existe los modifico                       
                                //sino existe los agrego                              
                                //si vienen menos borro el que falto

                                //busco la proxima secuencia
                                var secProxima = ((from plalit in db.InstitucionAseguradoraPlanes
                                                   where plalit.DoctSecuencia == usu.doctSecuencia
                                                   && plalit.IAsegSecuencia == usar_InsitucionAseg.IAsegSecuencia
                                                   select (int?)plalit.IAPlanSecuencia).Max() ?? 0) + 1;

                                  //si  existen lo agrego a la lista para  modificarlo
                                foreach (Usar_InstitucionAseguradoraPlanes item in planes)
                                {
                                    //si  existen lo agrego a la lista para  modificarlo
                                    var obj = PlanesBorrar.Where(p => p.IAPlanSecuencia == (int)item.IAPlanSecuencia).SingleOrDefault();
                                    if (obj != null)
                                    {

                                        obj.IAPlanDescripcion = item.IAPlanDescripcion;
                                        //  ListaParamodificar.Add(item);
                                    }
                                    //si no existe lo inserto insertar
                                    else
                                    {
                                        InstitucionAseguradoraPlane planesInsertar = new InstitucionAseguradoraPlane();
                                        planesInsertar.DoctSecuencia = usu.doctSecuencia;
                                        planesInsertar.ClinSecuencia = usu.Consultorio.clinSecuencia_fk;
                                        planesInsertar.ConsSecuencia = usu.Consultorio.ConsSecuencia_fk;

                                      

                                        planesInsertar.IAPlanSecuencia = secProxima;
                                        planesInsertar.IAPlanDescripcion = item.IAPlanDescripcion;
                                        planesInsertar.IAsegSecuencia = (int)usar_InsitucionAseg.IAsegSecuencia;
                                        db.InstitucionAseguradoraPlanes.Add(planesInsertar);
                                        secProxima++;
                                    }

                                }

                               
                                if (PlanesBorrar.Count > 0)
                                {
                                    ////si vienen menos lo elimino el que no vino desde la vista 
                                    foreach (InstitucionAseguradoraPlane item in PlanesBorrar)
                                    {
                                        //si vienen  en la lista que vino de la vista lo elimino en la base de datos.
                                        //por que quiere decir que lo eliminaron de la vista en el grid.
                                        var obj = planes.Where(p => (int)p.IAPlanSecuencia == item.IAPlanSecuencia).SingleOrDefault();
                                        if (obj == null)
                                        {
                                            db.InstitucionAseguradoraPlanes.Remove(item);
                                        }
                                    }
                                }

                            }//Cuando han borrado todos los planes de este seguro 
                            else
                            {
                               
                                        //si vienen  en la lista que vino de la vista lo elimino en la base de datos.
                                        //por que quiere decir que lo eliminaron de la vista en el grid.
                                        var obj = db.InstitucionAseguradoraPlanes.Where(p => 
                                            p.DoctSecuencia== usu.doctSecuencia
                                            && p.IAsegSecuencia == (int) usar_InsitucionAseg.IAsegSecuencia).ToList();

                                        if (obj != null)
                                        {
                                            foreach (var item in obj)
                                            {
                                                db.InstitucionAseguradoraPlanes.Remove(item);
                                                
                                            }
                                           
                                        }
                                    
                            }
                            
                            new Lib().IsertarAuditoria(((UsuarioLoguiado)HttpContext.Session["user"]), PaginaAutorizada, Accion.Editar.ToString(), null);
                        }

                        db.SaveChanges();
                        dbtrans.Commit();

                        respuesta.respuesta = true;
                        respuesta.redirect = Url.Content("~/" + controler + "/" + vista + "");

                        //Enlistar los 5 ultimos Registros                
                        respuesta.someCollection = UltimosCincoRegistros().someCollection;
                        //Para limpiar  la lista de
                        respuesta.ObjectGridList = new List<Usar_InstitucionAseguradoraPlanes>();
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

        public List<Usar_InstitucionAseguradoraPlanes> listaInstitucionAseguradoraPlanes(int IAsegSecuencia)
        {

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

                    //  vw_UsuarioDoctor docSecuencia = db.vw_UsuarioDoctor.Where(d => d.UsuaSecuencia == usu.usuario.UsuaSecuencia).FirstOrDefault();

                    //buscar los ultimos  cinco registros  de roles del doctor loguiado
                    List<InstitucionAseguradoraPlane> InsitucionAsegListaUltimosCinco = (from tf in db.InstitucionAseguradoraPlanes
                                                                                         where tf.DoctSecuencia == usu.doctSecuencia &&
                                                                                         tf.IAsegSecuencia == IAsegSecuencia
                                                                                         orderby tf.IAPlanDescripcion
                                                                                         select tf).ToList();

                    var listUsar_InstitucionAsegurPlanes = new List<Usar_InstitucionAseguradoraPlanes>();
                    //crear objeto role y asignar a la lista de usaroles
                    foreach (var item in InsitucionAsegListaUltimosCinco)
                    {
                        Usar_InstitucionAseguradoraPlanes ObjNuev = new Usar_InstitucionAseguradoraPlanes();
                        //asignar contenido a otro objeto
                        CopyClass.CopyObject(item, ref ObjNuev);
                        listUsar_InstitucionAsegurPlanes.Add(ObjNuev);
                    }


                    //ienumerable lista
                    return listUsar_InstitucionAsegurPlanes;


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
        }



        [HttpPost]
        /// <summary>
        /// Metodo para verificar que un paciente no tenga este plan asigando
        /// para que asi no se puda borrar
        /// </summary>
        /// <returns></returns>

        public JsonResult VerifivarPaciente(string IAPlanDescripcion, int asegusec, int plansec)
        {

            var respuesta = new ResponseModel();
            if (IAPlanDescripcion == "" || asegusec == 0 || plansec == 0)
            {

                respuesta.respuesta = false;
                respuesta.error = "No se ha ingresado el codigo de aseguradora o plan  ";
                return Json(respuesta);
            }

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

                    //verifico para ver si hay algun paciente de la doctora que tenga el plan que se quiere borrar
                    bool pacienteTieneAsignPlan = (from tf in db.Pacientes
                                                   where tf.DoctSecuencia_fk == usu.doctSecuencia
                                                   && tf.IAsegSecuencia == asegusec
                                                   && tf.IAPlanSecuencia == plansec
                                                   select tf).Any();

                    respuesta.respuesta = pacienteTieneAsignPlan;



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

            return Json(respuesta);
        }


    }
}
