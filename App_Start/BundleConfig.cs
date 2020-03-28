using System.Web;
using System.Web.Optimization;

namespace AidatPro
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Theme/files/bower_components/bootstrap-maxlength/js/bootstrap-maxlength.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));


            bundles.Add(new StyleBundle("~/Content/myStyle").Include(
           "~/Theme/files/bower_components/bootstrap/css/bootstrap.min.css",
           "~/Theme/files/assets/icon/themify-icons/themify-icons.css",
           "~/Theme/files/assets/icon/icofont/css/icofont.css",
           "~/Theme/files/assets/css/font-awesome.min.css",
           "~/Theme/files/assets/icon/feather/css/feather.css",          
           "~/Theme/files/bower_components/select2/css/select2.min.css",
           "~/Theme/files/bower_components/bootstrap-tagsinput/css/bootstrap-tagsinput.css",
           "~/Theme/files/bower_components/sweetalert/sweetalert2.css",
           "~/Theme/files/assets/css/style.css",
           "~/Theme/files/assets/css/jquery.mCustomScrollbar.css"
           ));


            bundles.Add(new ScriptBundle("~/Scripts/myScript").Include(
             "~/Theme/files/bower_components/jquery-ui/js/jquery-ui.min.js",
             "~/Scripts/jquery.unobtrusive-ajax.js",
             "~/Scripts/jquery.validate.js",
             "~/Scripts/jquery.validate.unobtrusive.js",
             "~/Scripts/messages_tr.js",
             "~/Theme/files/bower_components/sweetalert/sweetalert2.all.js",
             "~/Theme/files/bower_components/popper.js/js/popper.min.js",
             "~/Theme/files/bower_components/bootstrap/js/bootstrap.min.js",
             "~/Theme/files/bower_components/jquery-slimscroll/js/jquery.slimscroll.js",
             "~/Theme/files/bower_components/modernizr/js/modernizr.js",
             "~/Theme/files/bower_components/modernizr/js/css-scrollbars.js",
             "~/Theme/files/bower_components/i18next/js/i18next.min.js",
             "~/Theme/files/bower_components/i18next-xhr-backend/js/i18nextXHRBackend.min.js",
             "~/Theme/files/bower_components/i18next-browser-languagedetector/js/i18nextBrowserLanguageDetector.min.js",
             "~/Theme/files/bower_components/jquery-i18next/js/jquery-i18next.min.js",
             "~/Theme/files/assets/js/pcoded.min.js",
             "~/Theme/files/bower_components/bootstrap-tagsinput/js/bootstrap-tagsinput.js",             
             "~/Theme/files/assets/js/vartical-layout.min.js",
             "~/Theme/files/assets/js/jquery.mCustomScrollbar.concat.min.js",
             "~/Theme/files/assets/js/script.js"
             ));

            bundles.Add(new ScriptBundle("~/Scripts/dataTable").Include(
                "~/Theme/files/bower_components/datatables.net/js/jquery.dataTables.min.js",
                "~/Theme/files/bower_components/datatables.net-buttons/js/dataTables.buttons.min.js",
                "~/Theme/files/assets/pages/data-table/js/jszip.min.js",
                "~/Theme/files/assets/pages/data-table/js/pdfmake.min.js",
                "~/Theme/files/assets/pages/data-table/js/vfs_fonts.js",
                "~/Theme/files/bower_components/datatables.net-buttons/js/buttons.print.min.js",
                "~/Theme/files/bower_components/datatables.net-buttons/js/buttons.html5.min.js",
                "~/Theme/files/bower_components/datatables.net-bs4/js/dataTables.bootstrap4.min.js",
                "~/Theme/files/bower_components/datatables.net-responsive/js/dataTables.responsive.min.js",
                "~/Theme/files/bower_components/datatables.net-responsive-bs4/js/responsive.bootstrap4.min.js",
                "~/Theme/files/assets/pages/data-table/js/data-table-custom.js"
                ));

            bundles.Add(new StyleBundle("~/Content/dataTable").Include(
                "~/Theme/files/bower_components/datatables.net-bs4/css/dataTables.bootstrap4.min.css",
                "~/Theme/files/assets/pages/data-table/css/buttons.dataTables.min.css",
                "~/Theme/files/bower_components/datatables.net-responsive-bs4/css/responsive.bootstrap4.min.css"
                ));
        }
    }
}
