namespace CN88.CustomerSetting.Client.Controllers
{
    using System.Web.Mvc;
    using CN88.CustomerSetting.Client.Attributes;
    using CN88.CustomerSetting.Client.Models;
    using CN88.CustomerSetting.Client.Models.Requests;
    using CN88.CustomerSetting.Client.Services;
    using RestSharp;

    [ValidateAntiForgery]
    public class CreateCN88CustomerController : Controller
    {
        private readonly ICustomerSettingService _customerSettingService;

        public CreateCN88CustomerController()
            : this(null)
        {
        }

        internal CreateCN88CustomerController(ICustomerSettingService customerSettingService)
        {
            _customerSettingService = customerSettingService ?? new CustomerSettingService();
        }

        [HttpPost, CustomerSettingAuth(IsUpdate = true)]
        public ContentResult Create(CreateCustomerRequest request)
        {
            SetActorInformation(request);
            IRestResponse result = _customerSettingService.CreateCustomer(request);

            ModifyResponse(result);

            return Content(result.Content);
        }

        [HttpGet, CustomerSettingAuth(IsUpdate = false)]
        public ContentResult CheckExistedUsername(string username)
        {
            IRestResponse result = _customerSettingService.CheckExistedUsername(username);

            ModifyResponse(result);

            return Content(result.Content);
        }

        [HttpGet, CustomerSettingAuth(IsUpdate = false)]
        public ContentResult GetMainShareholderSuggestionNames()
        {
            IRestResponse result = _customerSettingService.GetMainShareholderSuggestionNames();

            ModifyResponse(result);

            return Content(result.Content);
        }

        private void ModifyResponse(IRestResponse result)
        {
            Response.StatusCode = (int)result.StatusCode;
            Response.TrySkipIisCustomErrors = true;
        }

        private static void SetActorInformation(CreateCustomerRequest request)
        {
            if (request != null)
            {
                SettingContext settingContext = CN88CustomerSettingRegistration.SettingContext;

                request.ActorId = settingContext.ActorId;
                request.ActorLevel = settingContext.ActorLevel;
                request.ActorName = settingContext.ActorName;
                request.SubAccountId = settingContext.SubAccountId;
                request.Domain = settingContext.Domain;
                request.Language = settingContext.Language;
                request.ClientIP = settingContext.IP;
                request.System = settingContext.System;
                request.HashKey = settingContext.HashKey;
            }
        }
    }
}