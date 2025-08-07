using System.Web;
using System.Web.Optimization;

namespace WebApplicationHsApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/Plugins/bootstrap-timepicker.min.js",
                      "~/Scripts/select2.full.min.js"
                     ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/jquery-ui.min.css",
                       "~/Content/fullcalendar.min.css",
                        "~/Content/Plugins/bootstrap-timepicker.min.css",
                        "~/Content/select2.min.css"
                      ));
            bundles.Add(new StyleBundle("~/Content/DataTables").Include(
                     "~/Content/datatables.css"));

            bundles.Add(new ScriptBundle("~/bundles/DataTables").Include(
                    "~/Scripts/datatables.min.js",
                    "~/Scripts/jquery.dataTables.editable.js",
                    "~/Scripts/jquery.jeditable.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                       "~/Scripts/jquery-ui.min.js"));

            bundles.Add(new ScriptBundle("~/Scripts/DutyRoasterReportsPage_JS").Include(
                  "~/Scripts/View_Related_JS/Report/DutyRoaster_Reports_JS.js"));

            bundles.Add(new ScriptBundle("~/bundles/Roster").Include(
                  "~/Scripts/Roster.js"));

            bundles.Add(new ScriptBundle("~/bundles/fullcalendar").Include(
                      "~/Scripts/fullcalendar.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/Customjs").Include(
                     "~/Scripts/Custom.js"));
            bundles.Add(new ScriptBundle("~/bundles/highcharts").Include(
                     "~/Scripts/highcharts/highcharts.js",
                     "~/Scripts/highcharts/highcharts-3d.js",
                     "~/Scripts/highcharts/highcharts-more.js"));
            bundles.Add(new ScriptBundle("~/bundles/highchartsmodules").Include(
                     "~/Scripts/highcharts/modules/exporting.js",
                     "~/Scripts/highcharts/modules/data.js",
                     "~/Scripts/highcharts/modules/drilldown.js",
                      "~/Scripts/highcharts/modules/cylinder.js"));

            bundles.Add(new StyleBundle("~/Content/BSFontAwesome").Include(
                     "~/Content/Plugins/Bootstrap_Font-Awesome.css",
                     "~/Content/Plugins/Bootstrap_Social_Icons.css"));

            bundles.Add(new StyleBundle("~/Content/BSConfirmPopup_CSS").Include(
                     "~/Content/Plugins/jquery-confirm.min.css"));

            bundles.Add(new ScriptBundle("~/Scripts/BSConfirmPopup_JS").Include(
                      "~/Scripts/Plugins/jquery-confirm.min.js"));

            bundles.Add(new ScriptBundle("~/Scripts/GlobalFunctionJS").Include(
                      "~/Scripts/CustomJS/GlobalFunctionJS.js"));

            bundles.Add(new ScriptBundle("~/Scripts/GlobalValidation_JS").Include(
                      "~/Scripts/CustomJS/GlobalValidation_JS.js"));

            bundles.Add(new ScriptBundle("~/Scripts/CustomDataTable_JS").Include(
                      "~/Scripts/CustomJS/CustomDataTable_DataJS.js"));

            bundles.Add(new ScriptBundle("~/Scripts/HOAnimation_JS").Include(
                      "~/Scripts/Plugins/HoldOn.min.js"));

            bundles.Add(new StyleBundle("~/Content/CustomCSS").Include(
                     "~/Content/CustomCSS/CustomCSS.css"));

            bundles.Add(new StyleBundle("~/Content/HOAnimation_CSS").Include(
                     "~/Content/Plugins/HoldOn.min.css"));

            bundles.Add(new ScriptBundle("~/Scripts/CustomConfirmBoxMethods_JS").Include(
                      "~/Scripts/CustomJS/Custom_ConfirmBox_Methods.js"));
            bundles.Add(new ScriptBundle("~/Scripts/HrLeaveManegent").Include(
                     "~/Scripts/HrLeaveManagement.js"));
            bundles.Add(new StyleBundle("~/Content/CustomAdmincss").Include(
                     "~/Content/CustomCSS/AdminLTE.min.css",
                      "~/Content/CustomCSS/_all-skins.min.css",
                      "~/Content/CustomCSS/font-awesome.css",
                      "~/Content/CustomCSS/ionicframework.css"
                     ));
            bundles.Add(new ScriptBundle("~/Scripts/CustomAppadmin").Include(
                    "~/Scripts/CustomJS/app.min.js"));
        }
    }
}
