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
    public partial class frmFormulariosCrear : Form
    {
        public frmFormulariosCrear()
        {
            InitializeComponent();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            int secuencia = 0;
            using (var db = new DoctorMedicalWebEntities())
            {

                var fo = db.Formularios.Where(u => u.FormDescripcion == txtFormDescripcion.Text.Trim()).SingleOrDefault();
                if (fo != null)
                {
                    MessageBox.Show("existe este formulario");
                    return;

                }
                Formulario formulario = new Formulario();
                secuencia = ((from FormularioMax in db.Formularios
                              select (int?)FormularioMax.FormSecuencia).Max() ?? 0) + 1;
                formulario.FormSecuencia = secuencia;
                formulario.FormDescripcion = txtFormDescripcion.Text.Trim();
                formulario.TFormSecuencia_fk = int.Parse(cbTipoFormulario.SelectedValue.ToString());
                formulario.FormPantalla = txtformPantalla.Text.Trim();

                db.Formularios.Add(formulario);
                db.SaveChanges();




            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {

        }

        private void frmFormulariosCrear_Load(object sender, EventArgs e)
        {
            using (var db = new DoctorMedicalWebEntities())
            {

                var tf = db.TipoFormularios.ToList();
                cbTipoFormulario.DataSource = tf;
                cbTipoFormulario.DisplayMember = "TFormDescripcion";
                cbTipoFormulario.ValueMember = "TFormSecuencia";
            }
        }
    }
}
