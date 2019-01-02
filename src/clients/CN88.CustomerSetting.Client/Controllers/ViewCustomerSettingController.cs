namespace CN88.CustomerSetting.Client.Controllers
{
    using System;
    using System.Net;
    using System.Runtime.Caching;
    using System.Web.Mvc;
    using Attributes;
    using Configurations;
    using Exceptions;
    using Helpers;
    using Models;
    using Models.Requests;
    using RestSharp;
    using Services;

    public class ViewCustomerSettingController : Controller
    {
        private const string SettingTemplateCachingKey = "CustomerSettingTemplate";
        private const byte CachingHours = 4;

        private readonly ICustomerSettingService _customerSettingService;

        public ViewCustomerSettingController()
            : this(null)
        {
        }

        internal ViewCustomerSettingController(
            ICustomerSettingService customerSettingService)
        {
            _customerSettingService = customerSettingService ?? new CustomerSettingService();
        }

        [CustomerSettingAuth(IsUpdate = false)]
        public ContentResult Edit(SettingFormQuery query)
        {
            query.IsCreateNew = false;

            return GetSettingPage(query);
        }

        [CustomerSettingAuth(IsUpdate = false)]
        public ContentResult Create(SettingFormQuery query)
        {
            query.IsCreateNew = true;

            return GetSettingPage(query);
        }

        private ContentResult GetSettingPage(SettingFormQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            ObjectCache cache = MemoryCache.Default;
            string template = cache.Get(SettingTemplateCachingKey) as string;

            if (string.IsNullOrEmpty(template))
            {
                template = BuildTemplate();
                cache.Set(SettingTemplateCachingKey, template, DateTime.Now.AddHours(CachingHours));
            }

            if (!string.IsNullOrEmpty(template))
            {
                SettingContext settingContext = CN88CustomerSettingRegistration.SettingContext;

                query.System = settingContext.System;
                query.Language = settingContext.Language;
                query.HashKey = settingContext.HashKey;
                query.ActorLevel = settingContext.ActorLevel;

                template = ReplaceParameters(template, query);
            }

            return Content(template, "text/HTML");
        }

        [HttpPost]
        [CustomerSettingAuth(IsUpdate = false)]
        public ContentResult QuerySetting(QuerySettingRequest querySettingRequest)
        {
            if (querySettingRequest == null)
            {
                throw new ArgumentNullException(nameof(querySettingRequest), "Query Setting cannot be null");
            }

            IRestResponse customerSettingResponse;
            SettingContext settingContext = CN88CustomerSettingRegistration.SettingContext;

            querySettingRequest.ActorId = settingContext.ActorId;
            querySettingRequest.ActorLevel = settingContext.ActorLevel;
            querySettingRequest.ActorName = settingContext.ActorName;
            querySettingRequest.SubAccountId = settingContext.SubAccountId;
            querySettingRequest.Domain = settingContext.Domain;
            querySettingRequest.Language = settingContext.Language;
            querySettingRequest.ClientIP = settingContext.IP;
            querySettingRequest.System = settingContext.System;

            customerSettingResponse = _customerSettingService.QueryCustomerSetting(querySettingRequest);
            Response.StatusCode = (int)customerSettingResponse.StatusCode;
            Response.TrySkipIisCustomErrors = true;

            return Content(customerSettingResponse.Content);
        }

        private string BuildTemplate()
        {
            IRestResponse response = _customerSettingService.QueryCustomerSettingTemplate();
            bool isDebug = HttpContext.IsDebuggingEnabled;
            string cdn = RandomHelper.GetRandomUri(CN88CustomerSettingConfiguration.Settings.CdnUris);

            if (!cdn.Contains("http"))
            {
                cdn = Url.Content(cdn);
            }

            string customCss = CN88CustomerSettingConfiguration.Settings.CustomCssUrl != null
                ? Url.Content(CN88CustomerSettingConfiguration.Settings.CustomCssUrl.ToString())
                : string.Empty;
            string template = string.Empty;

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var errorMessage = response.StatusCode == 0 ? response.ErrorMessage : response.Content;

                throw new CustomerSettingException(errorMessage);
            }

            template = SimpleJson.DeserializeObject<string>(response.Content);
            template = template.Replace("{{cdnUri}}", cdn);
            template = template.Replace("{{customCss}}", customCss);
            template = template.Replace("{{version}}", isDebug ? "0.0.1" : Guid.NewGuid().ToString("N"));
            template = template.Replace("{{min}}", isDebug ? string.Empty : ".min");

            return template;
        }

        private string ReplaceParameters(string template, SettingFormQuery query)
        {
            string finalTemplate = template;
            var appConfig = new AppConfiguration(Response.ApplyAppPathModifier($"~/{CN88CustomerSettingRegistration.UrlRootName}"));

            appConfig.Editable = !query.IsReadOnly;

            if (!string.IsNullOrEmpty(finalTemplate))
            {
                finalTemplate = finalTemplate.Replace("{{appConfig}}", appConfig.ToJson());
                finalTemplate = finalTemplate.Replace("{{params}}", query.ToJson());
                finalTemplate = finalTemplate.Replace("{{productId}}", query.ProductId.ToString());
            }

            return finalTemplate;
        }
    }
}