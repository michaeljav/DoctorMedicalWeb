using DoctorMedicalWeb.App_Data;
using DoctorMedicalWeb.Libreria;
using DoctorMedicalWeb.LoginView;
using DoctorMedicalWeb.ModelsComplementarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoctorMedicalWeb.Controllers
{
    public class SeleccioneConsultorioController : Controller
    {

        //
        // GET: /SeleccioneConsultorio/

        public ActionResult Ini_SeleccioneConsultorio()
        {


            //si esta sesion esta vacia quiere decir 
            //que no se ha pasado por la pagina principal de
            //logueo y lo redirecciono a la misma
            if (Session["UsuarioSeleComsul"] == null)
            {
                return RedirectToAction("Index", "PaginaPresentacion");
            }

            //si esta logueado que se redireccione al dash board
            //La Session["user"]  solo se llena  por esta pagina
            //o la pagina PaginaPresntacion
            if (Session["user"] != null)
            {
                return RedirectToAction("Index", "DashBoard");
            }

            using (var db = new DoctorMedicalWebEntities())
            {
                vw_UsuarioConsultorios consultorioUsuarioSeleccionado = (vw_UsuarioConsultorios)HttpContext.Session["UsuarioSeleComsul"];
                //si tiene mas de un consultorio paso
                //a otra pantalla para que seleccione
                //si tiene solo uno entro al sistema.
                List<vw_UsuarioConsultorios> CLinicaConsoltoriosDoctUsuar = ((from cantConsultorio in db.vw_UsuarioConsultorios
                                                                              where cantConsultorio.UsuaSecuencia == consultorioUsuarioSeleccionado.UsuaSecuencia
                                                                              && cantConsultorio.EstaDesabilitado == false
                                                                              select cantConsultorio).ToList());

                ViewBag.ConsultoriosDoc = new SelectList(CLinicaConsoltoriosDoctUsuar, "ConsSecuencia_fk", "NombreConsultorio");

                //Session["SeleccioneConsultorio"]
                return View();
            }
        }


        [HttpPost]
        public ActionResult Ini_SeleccioneConsultorio(vw_UsuarioConsultorios SeleccionadoConsultorio)
        {

            var respuesta = new ResponseModel();
            UsuarioLoguiado usuarioLoguiado = new UsuarioLoguiado();


            try
            {
                //si no esta validado los datos  no se logueara
                if (!ModelState.IsValid)
                {
                    return View(SeleccionadoConsultorio);
                }


                using (var db = new DoctorMedicalWebEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;


                    //de la anterior consulta digase la pagina donde valido usuario y contrasenaia
                    //traigo esta variable para poder cojer el usuario loguiado
                    vw_UsuarioConsultorios clinicasConsultoriosUsuario = (vw_UsuarioConsultorios)Session["UsuarioSeleComsul"];

                    //busco el usuario loguiado
                    var user = db.Usuarios.Where(u => u.UsuaSecuencia == clinicasConsultoriosUsuario.UsuaSecuencia).FirstOrDefault();


                    //Busco el consultorio seleccionado por el doctor
                    //clinicasConsultoriosUsuario[0] selecciono uno de items de la lista
                    //por que todos los items osea consultorios de la lista son del doctor
                    //seleccionados en el Controller PaginaPresentacionController anterior.
                    vw_UsuarioConsultorios CLinicaConsoltoriosDoctUsuar = ((from cantConsultorio in db.vw_UsuarioConsultorios
                                                                            where cantConsultorio.UsuaSecuencia == clinicasConsultoriosUsuario.UsuaSecuencia
                                                                            && cantConsultorio.ConsSecuencia_fk == SeleccionadoConsultorio.ConsSecuencia_fk
                                                                            select cantConsultorio).SingleOrDefault());



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



                    respuesta.respuesta = true;
                    respuesta.redirect = Url.Content("~/DashBoard/Index");
                    respuesta.error = "";
                    usuarioLoguiado.usuario = user;
                    usuarioLoguiado.Consultorio = CLinicaConsoltoriosDoctUsuar;
                    Session["user"] = usuarioLoguiado;

                    //Auditoria Usuario Loguiado
                    Lib lib = new Lib();
                    lib.IsertarAuditoria(usuarioLoguiado.usuario.PaisSecuencia, usuarioLoguiado.Consultorio.clinSecuencia_fk, usuarioLoguiado.Consultorio.ConsSecuencia_fk,
                        usuarioLoguiado.doctSecuencia, usuarioLoguiado.persSecuencia, null, Lib.GetLocalDateTime(), null, null, usuarioLoguiado.usuario.UsuaSecuencia,
                        lib.GetIPAddress(), "SeleccioneConsultorio", null, Accion.Iniciar_Sesion.ToString());


                    //busco todos los formularios para desabilitarlos
                    //en la vista _layout
                    List<Formulario> formParaDesabilitar = new List<Formulario>();
                    db.Configuration.ProxyCreationEnabled = false;
                    formParaDesabilitar = db.Formularios.ToList();
                    Session["sessformularioParaDesabilitar"] = formParaDesabilitar;


                    //buscar formularios  acorde al plan y rol del usuario

                    List<vw_ListDeFormuriosbyRolyUser> formularios = new List<vw_ListDeFormuriosbyRolyUser>();
                    db.Configuration.ProxyCreationEnabled = false;
                    formularios = db.vw_ListDeFormuriosbyRolyUser.Where(f => f.UsuaSecuencia == CLinicaConsoltoriosDoctUsuar.UsuaSecuencia
                                                                    // && f.PlanSecuencia_fk== CLinicaConsoltoriosDoctUsuar.PlanSecuencia_fk // busco el rol ya que no busco tambien porp  plan
                                                                        && f.RoleSecuencia_fk == CLinicaConsoltoriosDoctUsuar.RoleSecuencia_fk).ToList();

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
            }
            catch (Exception ex)
            {

                respuesta.respuesta = false;
                respuesta.redirect = "/PaginaPresentacion/Index";
                respuesta.error = "No existen formularios asignados a este plan,contacte con administrador. " + ex.GetBaseException().ToString();
                return Json(respuesta);
            }

        }//End Loguear


    }
}


