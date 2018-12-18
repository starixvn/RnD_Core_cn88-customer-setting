namespace CN88.CustomerSetting.Client.Attributes
{
    using System.Web.Helpers;

    internal static class AntiForgeryExtension
    {
        public static string GetTokenHeaderValue()
        {
            var cookieToken = string.Empty;
            var formToken = string.Empty;

            AntiForgery.GetTokens(null, out cookieToken, out formToken);

            return string.Format("{0}:{1}", cookieToken, formToken);
        }
    }
}