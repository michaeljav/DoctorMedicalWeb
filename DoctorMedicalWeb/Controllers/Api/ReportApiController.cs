using Syncfusion.EJ.ReportViewer;
using Syncfusion.Reports.EJ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DoctorMedicalWeb.Controllers.Api
{
    public class ReportApiController : ApiController,IReportController
    {
        //Post action for processing the rdl/rdlc report 
        [HttpPost]
        public object PostReportAction(Dictionary < string, object > jsonResult )
        {
            var a = ReportHelper.ProcessReport(jsonResult, this);
            return a;
        }
        
        //Get action for getting resources from the report
        [System.Web.Http.ActionName("GetResource")]
        [AcceptVerbs("GET")]
        public object GetResource(string key, string resourcetype, bool isPrint) 
        {
            return ReportHelper.GetResource(key, resourcetype, isPrint);
        }
        
        //Method will be called when initialize the report options before start processing the report        
        public void OnInitReportOptions(ReportViewerOptions reportOption)
        {
            //You can update report options here
            //reportOption.ReportModel.ReportServerCredential = new System.Net.NetworkCredential("sa", "Michaeljav712345");
            //reportOption.ReportModel.DataSourceCredentials.Add(new DataSourceCredentials("DB_A1595D_DoctorMedicalWeb", "sa", "Michaeljav712345"));

            List<ReportParameter> parameters = new List<ReportParameter>();
            parameters.Add(new ReportParameter() { Name = "SalesYearParameter", Labels = new List<string>() { "2003" }, Values = new List<string>() { "2003" } });
            reportOption.ReportModel.Parameters = parameters;
        }
        
        //Method will be called when reported is loaded
        public void OnReportLoaded(ReportViewerOptions reportOption) 
        {
            //You can update report options here
        }
    }
}
