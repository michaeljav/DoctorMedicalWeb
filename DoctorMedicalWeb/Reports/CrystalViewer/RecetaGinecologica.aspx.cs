
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.Web;
using DoctorMedicalWeb.App_Data;
using DoctorMedicalWeb.Libreria;
using DoctorMedicalWeb.ModelsComplementarios;
using DoctorMedicalWeb.Reports.Crystalrpt;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DoctorMedicalWeb.Reports.CrystalViewer
{
    public partial class RecetaGinecologica : System.Web.UI.Page
    {
        //public string thisConnectionString = ConfigurationManager.ConnectionStrings["ConnectionStringOther"].ConnectionString;
        CrystalDecisions.CrystalReports.Engine.ReportDocument reportDocument = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            String d = Request.QueryString["d"];
            String p = Request.QueryString["p"];
            String c = Request.QueryString["c"];
            String r = Request.QueryString["r"];



            if (Page.IsPostBack)
            {
                //LoadReport();
            }
            if (!string.IsNullOrEmpty(d) && !string.IsNullOrEmpty(p) && !string.IsNullOrEmpty(c) && !string.IsNullOrEmpty(r))
            {
                LoadReport(int.Parse(d), int.Parse(p), int.Parse(c), int.Parse(r));
            }
        }

        private void LoadReport(int CodigoDoctor, int paciente, int CMHistSecuencia, int recetaSec)
        {
            if (this.reportDocument != null)
            {
                this.reportDocument.Close();
                this.reportDocument.Dispose();
            }

            //SqlConnectionStringBuilder SConn = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConnectionStringOther"].ConnectionString);
            //SqlConnection thisConnection = new SqlConnection(thisConnectionString);
            //// store procedure
            //SqlCommand mySelectCommand = new System.Data.SqlClient.SqlCommand("PIS_GetAllEmployeeInfo", thisConnection);
            ////mySelectCommand.CommandType = CommandType.StoredProcedure;
            //reportDocument = new ReportDocument();
            ////Report path
            ////string reportPath = Server.MapPath("~/Reports/Crystalrpt/EmployeeListCrystalReport.rpt");
            ////lleno el combo de roles
            //string nombreReporte = "DocConsuRec_" + CodigoDoctor.ToString();

            //string reportPath = Server.MapPath("~/Reports/Crystalrpt/" + nombreReporte + ".rpt");
            //reportDocument.Load(reportPath);
            // Report connection
            //ConnectionInfo connInfo = new ConnectionInfo();
            //connInfo.ServerName = SConn.DataSource;
            //connInfo.DatabaseName = SConn.InitialCatalog;
            //connInfo.UserID = SConn.UserID;
            //connInfo.Password = SConn.Password;
            //TableLogOnInfo tableLogOnInfo = new TableLogOnInfo();
            //tableLogOnInfo.ConnectionInfo = connInfo;
            //foreach (CrystalDecisions.CrystalReports.Engine.Table table in reportDocument.Database.Tables)
            //{
            //    table.ApplyLogOnInfo(tableLogOnInfo);
            //    table.LogOnInfo.ConnectionInfo.ServerName = connInfo.ServerName;
            //    table.LogOnInfo.ConnectionInfo.DatabaseName = connInfo.DatabaseName;
            //    table.LogOnInfo.ConnectionInfo.UserID = connInfo.UserID;
            //    table.LogOnInfo.ConnectionInfo.Password = connInfo.Password;
            //    table.Location = "dbo." + table.Location;
            //}
            // You can pass parameter in your store procedure if you need
            //reportDocument.SetParameterValue("@FromDate", ProjectUtilities.ConvertToDate(txtFromDate.Text));
            //reportDocument.SetParameterValue("@ToDate", ProjectUtilities.ConvertToDate(txtToDate.Text));

            //RecetaGinecologicaListCrystalReportViewer.ReportSource = reportDocument;
            //RecetaGinecologicaListCrystalReportViewer.Zoom(80);
            //RecetaGinecologicaListCrystalReportViewer.DataBind();



            reportDocument = new ReportDocument();
      try
            {
            
            string nombreReporte = "DocConsuRec_" + CodigoDoctor.ToString();
            string reportPath = Server.MapPath("~/Reports/Crystalrpt/" + nombreReporte + ".rpt");
            reportDocument.Load(reportPath);
            


            var recetaObj = GetData( CodigoDoctor,  paciente,  CMHistSecuencia,  recetaSec);

           
                //reportDocument.Database.Tables[0].SetDataSource(recetaObj);
                //CollectionHelper ch = new CollectionHelper();
                //DataTable dt = ch.ConvertTo(recetaObj);

              reportDocument.SetDataSource(recetaObj.Tables[0]);
               // crystalReport.Database.Tables[0].SetDataSource(recetaObj);
         

            RecetaGinecologicaListCrystalReportViewer.ReportSource = reportDocument;
            //RecetaGinecologicaListCrystalReportViewer.DisplayGroupTree = false;
            RecetaGinecologicaListCrystalReportViewer.ToolPanelView = ToolPanelViewType.None;//off
            //CrystalReportViewer.ToolPanelView = ToolPanelViewType.GroupTree;//turn on


            RecetaGinecologicaListCrystalReportViewer.Zoom(80);
            RecetaGinecologicaListCrystalReportViewer.DataBind();

            }
      catch (Exception ex)
      {
          string err = "";
          if (ex.GetBaseException() != null) {
              err = ex.GetBaseException().ToString();
          }
          Response.Write("<script>alert('Usted no tiene Reporte Asignado :__'"+err+")</script>");

          //throw ex;
      }
            //RecetaGinecologicaListCrystalReportViewer.RefreshReport();

        }

        protected void Preview_Click(object sender, EventArgs e)
        {
            //  LoadReport();
        }
        private DataSet GetData(int CodigoDoctor, int paciente, int CMHistSecuencia, int recetaSec)
        {
            using (var db = new DoctorMedicalWebEntities())
            {
                // Recetas  master
                //vw_receta receta = (from x in db.vw_receta
                //                          where
                //                         x.DoctSecuencia_fk == CodigoDoctor
                //                         && x.CMHistSecuencia_fk == CMHistSecuencia
                //                      && x.PaciSecuencia_fk == paciente
                //                          && x.ReceSecuencia == recetaSec
                //                          select x).();

                SqlConnection conn = new SqlConnection(db.Database.Connection.ConnectionString);
                // 1. declare command object with parameter
	SqlCommand cmd = new SqlCommand(
		"select * from vw_receta where ReceSecuencia =@receSecuencia "+
        "and PaciSecuencia_fk = @paciSecuencia " +
        "and CMHistSecuencia_fk = @cMHistSecuencia_fk "+
        "and DoctSecuencia_fk = @doctSecuencia ", conn);
               
               
                cmd.Parameters.AddWithValue("@receSecuencia",recetaSec);
                cmd.Parameters.AddWithValue("@paciSecuencia",paciente);
                cmd.Parameters.AddWithValue("@cMHistSecuencia_fk",CMHistSecuencia);
                cmd.Parameters.AddWithValue("@doctSecuencia", CodigoDoctor);
                               
                cmd.Connection.Open();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds,"vw_receta");

                return ds;
            }

        }
    }


}