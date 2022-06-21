using System.Web;
using System.Web.Optimization;

namespace DoctorMedicalWeb
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {


            /*Comunes css*/
            /*NO TRABAJO POR EL CSS*/
            //bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/AdminFiles/vendors/bootstrap/dist/css/bootstrap.css"
            //    )
                       
            //    );
            /*NO TRABAJO POR EL CSS*/
            //bundles.Add(new StyleBundle("~/Content/csss").Include("~/Content/AdminFiles/vendors/bootstrap/dist/css/bootstrap.min.css"       
            // ,"~/Content/AdminFiles/build/css/custom.css"
            // , "~/Content/AdminFiles/css/estilosDashboard.css"
            // , "~/Content/AdminFiles/estilos.css"
            // )
            // );
            bundles.Add(new StyleBundle("~/Content/csss").Include("~/Content/AdminFiles/vendors/bootstrap/dist/css/bootstrap.css"
            //, "~/Content/AdminFiles/build/css/custom.css"
            //, "~/Content/AdminFiles/css/estilosDashboard.css"
            //, "~/Content/AdminFiles/estilos.css"
            )
            );
            bundles.Add(new StyleBundle("~/bundles/custtt").Include("~/Content/AdminFiles/build/css/custom.css"
                //, "~/Content/AdminFiles/build/css/custom.css"
                //, "~/Content/AdminFiles/css/estilosDashboard.css"
                //, "~/Content/AdminFiles/estilos.css"
           )
           );

            bundles.Add(new StyleBundle("~/Content/Syn").Include("~/Content/ej/web/responsive-css/ejgrid.responsive.css"
         
            ));

           
           
            //bundles.Add(new StyleBundle("~/Content/css/ic").Include("~/Content/AdminFiles/vendors/bootstrap/dist/css/bootstrap.min.css"
            //   )

            //   );
         //   .Include("~/Content/AdminFiles/vendors/font-awesome/css/font-awesome.min.css"), new CssRewriteUrlTransform()

            /*Comunes js*/


            bundles.Add(new ScriptBundle("~/javaScriptComu").Include(
                    "~/Scripts/jquery-3.1.1.min.js"
                    , "~/Scripts/bootstrap.min.js"
                    , "~/Scripts/jsrender.min.js"
                    , "~/Scripts/jquery.serializeToJSON.js"
                    , "~/Scripts/loadingoverlay.min.js"));










            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                 "~/Content/themes/base/jquery-ui.min.css",
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));


        }
    }
}