using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsignacionformularioPlan
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //DoctorMedicalWebEntities db = new DoctorMedicalWebEntities();
        //DoctorMedicalWebEntities db = null;
        private void Form1_Load(object sender, EventArgs e)
        {
            using (var db = new DoctorMedicalWebEntities())
            {
                comboBox1.DataSource = db.Planes.ToList();
                comboBox1.DisplayMember = "PlanDescripcion";
                comboBox1.ValueMember = "PlanSecuencia";
            }
            
        }

        private void CargarFormularios()
        {
            using (var db = new DoctorMedicalWebEntities())
            {
                List<formularioSeleccionado> fo = new List<formularioSeleccionado>();
                var a = db.Formularios.ToList();

                foreach (Formulario formulario in a)
                {

                    formularioSeleccionado f = new formularioSeleccionado();
                    f.PlanSecuencia_fk = int.Parse(comboBox1.SelectedValue.ToString());
                    f.PlanDescripcion = db.Planes.Where(p => p.PlanSecuencia == f.PlanSecuencia_fk).Select(t => t.PlanDescripcion).SingleOrDefault();
                    f.FormSecuencia_fk = formulario.FormSecuencia;
                    f.FormDescripcion = formulario.FormDescripcion;
                    fo.Add(f);
                }

                dgvFormulario.DataSource = fo;
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var db = new DoctorMedicalWebEntities())
            {
                int a = 0;
                a = int.Parse(comboBox1.SelectedValue.ToString());
                if (a <= 0)
                {
                    return;
                }
                dataGridView2.DataSource = null;
                dataGridView2.DataSource = db.vw_planFormulario.Where(x => x.PlanSecuencia == a).ToList();
            }
        }

        private void btnguardar_Click(object sender, EventArgs e)
        {
            using (var db = new DoctorMedicalWebEntities())
            {
                List<formularioSeleccionado> fo = (List<formularioSeleccionado>)dgvFormulario.DataSource;

                int sec = 0;
                foreach (formularioSeleccionado item in fo)
                {
                    //creo objeto para guardar
                    if (item.seleccionar == true)
                    {
                        sec++;
                        //agrego a objeto
                        PlanFormulario pformu = new PlanFormulario();
                        pformu.PFSecuencia = sec;
                        pformu.PlanSecuencia_fk = item.PlanSecuencia_fk;
                        pformu.FormSecuencia_fk = item.FormSecuencia_fk;
                        db.PlanFormularios.Add(pformu);
                        db.SaveChanges();
                    }
                }

            }
            button1_Click(null, e);


        }

        private void btnCargarFormularios_Click(object sender, EventArgs e)
        {
            CargarFormularios();

        }

        private void btnEditarYGuardar_Click(object sender, EventArgs e)
        {
            using (var db = new DoctorMedicalWebEntities())
            {
                List<formularioSeleccionado> fo = new List<formularioSeleccionado>();
                fo = (List<formularioSeleccionado>)dgvFormulario.DataSource;
                if (fo == null || fo.Count < 1)
                {
                    return;
                }

                using (var dbtransa = db.Database.BeginTransaction())
                {
                    try
                    {
                        //borrar todos los formularios que le pertenecen a ese plan 
                        //para volverlo a insertar.
                        List<PlanFormulario> listaPlanFormuBorrar = new List<PlanFormulario>();
                        int secuencia = int.Parse(comboBox1.SelectedValue.ToString());
                        listaPlanFormuBorrar = (from x in db.PlanFormularios
                                                where x.PlanSecuencia_fk == secuencia
                                                select x).ToList();

                        //borrar cada uno
                        foreach (PlanFormulario item in listaPlanFormuBorrar)
                        {
                            db.PlanFormularios.Remove(item);
                         //   db.SaveChanges();
                        }


                        //volverlo a insertar ahora con el nuevo formulario asignado
                        int sec = 0;
                        foreach (formularioSeleccionado item in fo)
                        {

                            //creo objeto para guardar
                            if (item.seleccionar == true)
                            {

                                sec++;
                                //agrego a objeto
                                PlanFormulario pformu = new PlanFormulario();
                                pformu.PFSecuencia = sec;
                                pformu.PlanSecuencia_fk = item.PlanSecuencia_fk;
                                pformu.FormSecuencia_fk = item.FormSecuencia_fk;
                                pformu.EstaDesabilitado = false;
                                db.PlanFormularios.Add(pformu);
                                //db.SaveChanges();


                            }
                        }


                      

                        db.SaveChanges();
                        dbtransa.Commit();
                    }
                    catch (Exception EX)
                    {
                        dbtransa.Rollback();

                    }

                }
            }
        }


        //Formularios que faltan por asignar a algun plan
        private void CargarActualesYFaltantesFormulariosDePlan()
        {
            using (var db = new DoctorMedicalWebEntities())
            {
                int PlanSeleccionadoCombo = 0;
                PlanSeleccionadoCombo = int.Parse(comboBox1.SelectedValue.ToString());
                if (PlanSeleccionadoCombo <= 0)
                {
                    MessageBox.Show("el valor del combo plan es menor de 0");
                    return;
                }

                List<formularioSeleccionado> formuSeleccionadoyNo = new List<formularioSeleccionado>();

                //lista formularios
                var Formula = (from f in db.Formularios
                               select f).ToList();

                foreach (Formulario item in Formula)
                {
                    //verifico cuales formularios le falta al 
                    //usuario en cuestion en la tabla planformulario 
                    PlanFormulario planFormularios = new PlanFormulario();
                    planFormularios = (from f in db.PlanFormularios
                                       where f.PlanSecuencia_fk == PlanSeleccionadoCombo
                                          && f.FormSecuencia_fk == item.FormSecuencia
                                       select f).SingleOrDefault();
                    formularioSeleccionado objformuario = new formularioSeleccionado();

                    objformuario.PlanSecuencia_fk = PlanSeleccionadoCombo;
                    Plane pla = (from x in db.Planes
                                 where x.PlanSecuencia == PlanSeleccionadoCombo
                                 select x).SingleOrDefault();
                    objformuario.PlanDescripcion = pla.PlanDescripcion;

                    //si esta el formulario lo agrego a la lista y lo 
                    //selecciono en el checkbox
                    if (planFormularios != null)
                    {
                        objformuario.FormSecuencia_fk = item.FormSecuencia;
                        objformuario.FormDescripcion = item.FormDescripcion;
                        //si esta el valor  de plan quiere decir   que
                        //este plan tiene  ese formulario asignado.
                        objformuario.seleccionar = true;
                    }
                    else
                    {
                        objformuario.FormSecuencia_fk = item.FormSecuencia;
                        objformuario.FormDescripcion = item.FormDescripcion;
                        //sino esta el formulario asignado al plan
                        //no lo selecciono
                        objformuario.seleccionar = false;
                    }
                    formuSeleccionadoyNo.Add(objformuario);
                }
                dgvFormulario.DataSource = null;
                dgvFormulario.DataSource = formuSeleccionadoyNo;


            }

        }

        private void CargarFomulariosActualesyparaAsignar_Click(object sender, EventArgs e)
        {
            CargarActualesYFaltantesFormulariosDePlan();
        }

        private void btnCrearFormulario_Click(object sender, EventArgs e)
        {
            frmFormulariosCrear fr = new frmFormulariosCrear();
            fr.ShowDialog();
        }



    }
}
