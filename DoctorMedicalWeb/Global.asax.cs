using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DoctorMedicalWeb
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            //DateTime timeValue = Convert.ToDateTime("01:00 PM");
            //TimeSpan a = new TimeSpan();
            //a = timeValue.TimeOfDay;
            //Console.WriteLine(timeValue.ToString("HH:mm"));
        

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleTable.EnableOptimizations = true;

            BundleConfig.RegisterBundles(BundleTable.Bundles);
    

           
        }

    
    }
}