using System.Web;
using System.Web.Optimization;

namespace THT
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            //================================================ Scripts ==========================================

            bundles.Add(new ScriptBundle("~/bundles/Home").Include(
             "~/Scripts/app/app.js"));
           
            //nguoi dung
            bundles.Add(new ScriptBundle("~/bundles/appAuth_User").Include(
                "~/Scripts/app/app.js",
                "~/Scripts/app/Auth_User.js"));
           
            //phan quyen nguoi dung
            bundles.Add(new ScriptBundle("~/bundles/appAuth_Role").Include(
                "~/Scripts/app/app.js",
                "~/Scripts/app/Auth_Role.js"));
           
            //thong bao
            bundles.Add(new ScriptBundle("~/bundles/appUtilities_Announcement").Include(
           "~/Scripts/app/app.js",
           "~/Scripts/app/Utilities_Announcement.js"));

            //cac doan script duoc su dung lai
            bundles.Add(new ScriptBundle("~/bundles/appUtilities_Announcement").Include(
          "~/Scripts/app/app.js",
          "~/Scripts/app/Utilities_Announcement.js"));
            //Phan cap vung mien
            bundles.Add(new ScriptBundle("~/bundles/appUtilities_Territory").Include(
                "~/Scripts/app/app.js",
                "~/Scripts/app/Utilities_Territory.js"));
            //Quản lý lịch nghỉ
            bundles.Add(new ScriptBundle("~/bundles/appUtilities_Holiday").Include(
                "~/Scripts/app/app.js",
                "~/Scripts/app/Utilities_Holiday.js"));
            //Quản lý lịch nghỉ
            bundles.Add(new ScriptBundle("~/bundles/appDelivery").Include(
                "~/Scripts/app/app.js",
                "~/Scripts/app/DeliveryManagement.js"));
            //================================================ Scripts ==========================================

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
        }
    }
}
