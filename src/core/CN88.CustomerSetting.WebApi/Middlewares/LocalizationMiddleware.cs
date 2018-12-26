namespace CN88.CustomerSetting.WebApi.Middlewares
{
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class LocalizationMiddleware
    {
        private const string DefaultLanguage = "en-US";
        public string languageKey = "language";
        private static readonly CultureInfo DefaultCulture = CultureInfo.CreateSpecificCulture(DefaultLanguage);

        private readonly RequestDelegate next;

        public LocalizationMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            var request = context.Request;

            var headerContainLanguageKey = request.Headers.Keys.Contains(languageKey);

            if (headerContainLanguageKey)
            {
                string language = request.Headers[languageKey];
                CultureInfo cultureInfo = null;

                if (!string.IsNullOrWhiteSpace(language))
                {
                    cultureInfo = new CultureInfo(language);
                }

                Thread.CurrentThread.CurrentCulture = cultureInfo ?? DefaultCulture;
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

                Thread.CurrentThread.CurrentCulture.DateTimeFormat = DefaultCulture.DateTimeFormat;
                Thread.CurrentThread.CurrentCulture.NumberFormat = DefaultCulture.NumberFormat;
            }

            await next(context);
        }
    }
}