namespace AsignacionformularioPlan
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvFormulario = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.btnguardar = new System.Windows.Forms.Button();
            this.btnCargarFormularios = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.CargarFomulariosActualesyparaAsignar = new System.Windows.Forms.Button();
            this.btnEditarYGuardar = new System.Windows.Forms.Button();
            this.btnCrearFormulario = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFormulario)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvFormulario
            // 
            this.dgvFormulario.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFormulario.Location = new System.Drawing.Point(12, 105);
            this.dgvFormulario.Name = "dgvFormulario";
            this.dgvFormulario.Size = new System.Drawing.Size(522, 444);
            this.dgvFormulario.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Formularios";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(15, 29);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(182, 21);
            this.comboBox1.TabIndex = 2;
            // 
            // dataGridView2
            // 
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Location = new System.Drawing.Point(540, 105);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.Size = new System.Drawing.Size(522, 444);
            this.dataGridView2.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(540, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(146, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Formularios Asignados al plan";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Plan";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(543, 40);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(97, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "VerFormularios aSIGNADOS";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnguardar
            // 
            this.btnguardar.Location = new System.Drawing.Point(6, 48);
            this.btnguardar.Name = "btnguardar";
            this.btnguardar.Size = new System.Drawing.Size(205, 23);
            this.btnguardar.TabIndex = 3;
            this.btnguardar.Text = "Guardar Nuevo";
            this.btnguardar.UseVisualStyleBackColor = true;
            this.btnguardar.Click += new System.EventHandler(this.btnguardar_Click);
            // 
            // btnCargarFormularios
            // 
            this.btnCargarFormularios.Location = new System.Drawing.Point(6, 19);
            this.btnCargarFormularios.Name = "btnCargarFormularios";
            this.btnCargarFormularios.Size = new System.Drawing.Size(205, 23);
            this.btnCargarFormularios.TabIndex = 3;
            this.btnCargarFormularios.Text = "CargarFormularios para Crear Nuevo";
            this.btnCargarFormularios.UseVisualStyleBackColor = true;
            this.btnCargarFormularios.Click += new System.EventHandler(this.btnCargarFormularios_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnguardar);
            this.groupBox1.Controls.Add(this.btnCargarFormularios);
            this.groupBox1.Location = new System.Drawing.Point(928, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(217, 89);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Para Crear Nuevo  desde cero";
            this.groupBox1.Visible = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.CargarFomulariosActualesyparaAsignar);
            this.groupBox2.Controls.Add(this.btnEditarYGuardar);
            this.groupBox2.Location = new System.Drawing.Point(203, 26);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(262, 66);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Para Editar";
            // 
            // CargarFomulariosActualesyparaAsignar
            // 
            this.CargarFomulariosActualesyparaAsignar.Location = new System.Drawing.Point(6, 14);
            this.CargarFomulariosActualesyparaAsignar.Name = "CargarFomulariosActualesyparaAsignar";
            this.CargarFomulariosActualesyparaAsignar.Size = new System.Drawing.Size(250, 23);
            this.CargarFomulariosActualesyparaAsignar.TabIndex = 3;
            this.CargarFomulariosActualesyparaAsignar.Text = "Cargar Formulario para Editar";
            this.CargarFomulariosActualesyparaAsignar.UseVisualStyleBackColor = true;
            this.CargarFomulariosActualesyparaAsignar.Click += new System.EventHandler(this.CargarFomulariosActualesyparaAsignar_Click);
            // 
            // btnEditarYGuardar
            // 
            this.btnEditarYGuardar.Location = new System.Drawing.Point(6, 37);
            this.btnEditarYGuardar.Name = "btnEditarYGuardar";
            this.btnEditarYGuardar.Size = new System.Drawing.Size(250, 23);
            this.btnEditarYGuardar.TabIndex = 3;
            this.btnEditarYGuardar.Text = "Editar y guardar";
            this.btnEditarYGuardar.UseVisualStyleBackColor = true;
            this.btnEditarYGuardar.Click += new System.EventHandler(this.btnEditarYGuardar_Click);
            // 
            // btnCrearFormulario
            // 
            this.btnCrearFormulario.Location = new System.Drawing.Point(78, 60);
            this.btnCrearFormulario.Name = "btnCrearFormulario";
            this.btnCrearFormulario.Size = new System.Drawing.Size(97, 23);
            this.btnCrearFormulario.TabIndex = 3;
            this.btnCrearFormulario.Text = "Crear Formulario";
            this.btnCrearFormulario.UseVisualStyleBackColor = true;
            this.btnCrearFormulario.Click += new System.EventHandler(this.btnCrearFormulario_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(705, 40);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1140, 561);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCrearFormulario);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView2);
            this.Controls.Add(this.dgvFormulario);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFormulario)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvFormulario;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnguardar;
        private System.Windows.Forms.Button btnCargarFormularios;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnEditarYGuardar;
        private System.Windows.Forms.Button CargarFomulariosActualesyparaAsignar;
        private System.Windows.Forms.Button btnCrearFormulario;
        private System.Windows.Forms.TextBox textBox1;
    }
}

