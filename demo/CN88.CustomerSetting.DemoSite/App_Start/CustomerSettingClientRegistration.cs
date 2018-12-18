namespace CN88.CustomerSetting.DemoSite.App_Start
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Web;
    using CN88.CustomerSetting.Client;
    using CN88.CustomerSetting.Client.Models;
    using CN88.CustomerSetting.DemoSite.Configurations;
    using CN88.CustomerSetting.DemoSite.Constants;

    public class CustomerSettingClientRegistration : CN88CustomerSettingRegistration
    {
        public override bool VerifyQueryPermission(PermissionContext permissionContext)
        {
            return true;
        }

        public override bool VerifyUpdatePermission(PermissionContext permissionContext)
        {
            if (DemoSiteConfiguration.Settings.EnableLogin && HttpContext.Current.Session["IsAuthenticated"] == null)
            {
                return false;
            }

            return true;
        }

        public override bool VerifyMultipleUpdatePermission(PermissionContext permissionContext)
        {
            if (DemoSiteConfiguration.Settings.EnableLogin && HttpContext.Current.Session["IsAuthenticated"] == null)
            {
                return false;
            }

            return true;
        }

        public override SettingContext FetchSettingContext()
        {
            var settingContext = new SettingContext();

            settingContext.ActorId = (int)HttpContext.Current.Session[SessionKeys.ActorIdSessionKey];
            settingContext.ActorName = (string)HttpContext.Current.Session[SessionKeys.ActorNameSessionKey];
            settingContext.ActorLevel = (byte)HttpContext.Current.Session[SessionKeys.ActorLevelSessionKey];
            settingContext.SubAccountId = (int)HttpContext.Current.Session[SessionKeys.SubAccountIdSessionKey];
            settingContext.System = (byte)HttpContext.Current.Session[SessionKeys.SystemIdSessionKey];
            settingContext.Language = (string)HttpContext.Current.Session[SessionKeys.LanguageSessionKey];
            settingContext.Domain = HttpContext.Current.Request.Url.Host;
            settingContext.IP = GetClientIp();

            return settingContext;
        }

        private string GetClientIp()
        {
            var forwardHeader = GetForwardHeader();

            if (!string.IsNullOrEmpty(forwardHeader))
            {
                return forwardHeader;
            }

            return GetInternalAddress();
        }

        private string GetForwardHeader()
        {
            var httpRequest = HttpContext.Current.Request;
            var forwardHeader = httpRequest.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (forwardHeader != null)
            {
                return forwardHeader.Split(',').First().Trim();
            }

            return string.Empty;
        }

        private string GetInternalAddress()
        {
            var httpRequest = HttpContext.Current.Request;
            string userHostAddress = httpRequest.UserHostAddress;
            var internalIPAddress = GetInternalIPAddress(userHostAddress);

            if (internalIPAddress == null)
            {
                var hostName = Dns.GetHostName();

                internalIPAddress = GetInternalIPAddress(hostName);
            }

            return internalIPAddress != null ? internalIPAddress.ToString() : string.Empty;
        }

        private IPAddress GetInternalIPAddress(string hostAddress)
        {
            foreach (var itemIPA in Dns.GetHostAddresses(hostAddress))
            {
                if (itemIPA.AddressFamily.ToString().Equals("InterNetwork", StringComparison.OrdinalIgnoreCase))
                {
                    return itemIPA;
                }
            }

            return null;
        }
    }
}