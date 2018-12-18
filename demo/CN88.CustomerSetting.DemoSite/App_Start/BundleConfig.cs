namespace CN88.CustomerSetting.DemoSite.App_Start
{
    using System.Web.Optimization;

    public static class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                        "~/Scripts/Libraries/Angular/angular.js",
                        "~/Scripts/DemoForm/demo-form.js"));

            bundles.Add(new ScriptBundle("~/bundles/ie8tricks").Include(
                        "~/Scripts/Libraries/Es5-Shim/es5-shim.js",
                        "~/Scripts/Libraries/Html5shiv/html5shiv.js"));

            bundles.Add(new Bundle("~/bundles/ie8js").Include(
                        "~/Scripts/Libraries/JQuery/jquery.js",
                        "~/Scripts/Libraries/Angular-Legacy/angular.js",
                        "~/Scripts/Libraries/Underscore/underscore.js",
                        "~/Scripts/DemoForm.IE8/demo-form.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/site.css"));
        }
    }
}