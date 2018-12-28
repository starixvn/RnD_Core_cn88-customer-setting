namespace CN88.CustomerSetting.DemoSite.Controllers
{
    using System;
    using System.Web.Mvc;
    using System.Web.Security;
    using CN88.CustomerSetting.Client.Configurations;
    using CN88.CustomerSetting.Client.Helpers;
    using CN88.CustomerSetting.DemoSite.Configurations;
    using CN88.CustomerSetting.DemoSite.Constants;
    using CN88.CustomerSetting.DemoSite.Filters;

    public class HomeController : Controller
    {
        private const byte AgencySystem = 2;

        [RestrictAccessFilter]
        public ActionResult Index()
        {
            FormsAuthentication.SetAuthCookie("DemoName", true);
            if (Request.Browser.Browser == "IE" && (Request.Browser.MajorVersion < 9))
            {
                return View("Index-IE8");
            }
            else
            {
                return View();
            }
        }

        public JsonResult SaveSession(
            int actorId,
            string actorName,
            byte role,
            int subAccountId = 0,
            byte system = 1,
            string language = "en-US")
        {
            if (AgencySystem != CN88CustomerSettingConfiguration.Settings.System)
            {
                subAccountId = 0;
            }

            Session[SessionKeys.ActorIdSessionKey] = actorId;
            Session[SessionKeys.ActorNameSessionKey] = actorName;
            Session[SessionKeys.ActorLevelSessionKey] = role;
            Session[SessionKeys.SubAccountIdSessionKey] = subAccountId;
            Session[SessionKeys.SystemIdSessionKey] = system;
            Session[SessionKeys.LanguageSessionKey] = language;
            Session[SessionKeys.HashKeySessionKey] = Session.SessionID;

            return Json(new
            {
                Code = 0,
                Message = string.Empty,
                IsSuccessful = true
            });
        }

        public ActionResult RedirectAction()
        {
            return RedirectToAction("DemoAction", "DemoCtrl");
        }

        public ActionResult Login(string securityCode)
        {
            if (securityCode == DemoSiteConfiguration.Settings.SecurityCode)
            {
                Session["IsAuthenticated"] = true;
                return RedirectToAction("Index");
            }

            return Content("You are not authorized!");
        }
    }
}