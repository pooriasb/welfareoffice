using System.Web;
using System.Web.Optimization;

namespace BehzistiMaskan
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Assets/javascripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Assets/javascripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/vanta").Include(
                "~/Assets/vendor/vanta/three.r92.min.js",
                "~/Assets/vendor/vanta/vanta.birds.min.js",
                "~/Assets/vendor/vanta/vanta.clouds.min.js",
                "~/Assets/vendor/vanta/vanta.net.min.js",
                "~/Assets/vendor/vanta/vanta.waves.min.js"
            ));
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Assets/javascripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Assets/vendor/bootstrap/js/bootstrap.min.js",
                "~/Assets/javascripts/respond.js"));

            bundles.Add(new StyleBundle("~/stylesheets/bootstrap").Include(
                "~/Assets/vendor/bootstrap/css/bootstrap.css",
                "~/Assets/stylesheets/bootstrap.rtl.css"));
        }
    }
}
