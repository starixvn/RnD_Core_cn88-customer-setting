namespace CN88.CustomerSetting.DemoSite.App_Start
{
    using System.Web.Optimization;

    public static class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                        "~/Scripts/Libraries/JQuery/jquery.js",
                        "~/Scripts/Libraries/Angular/angular.js",
                        "~/Scripts/DemoForm/demo-form.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/site.css"));
        }
    }
}