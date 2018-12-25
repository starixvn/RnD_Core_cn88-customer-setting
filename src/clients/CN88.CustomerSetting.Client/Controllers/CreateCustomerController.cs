namespace CN88.CustomerSetting.Client.Controllers
{
    using System.Web.Mvc;
    using CN88.CustomerSetting.Client.Attributes;
    using CN88.CustomerSetting.Client.Models;
    using CN88.CustomerSetting.Client.Models.Requests;
    using CN88.CustomerSetting.Client.Services;
    using RestSharp;

    public class CreateCustomerController : Controller
    {
        private readonly ICustomerSettingService _customerSettingService;

        public CreateCustomerController()
            : this(null)
        {
        }

        internal CreateCustomerController(ICustomerSettingService customerSettingService)
        {
            _customerSettingService = customerSettingService ?? new CustomerSettingService();
        }

        [HttpPost, CustomerSettingAuth(IsUpdate = true)]
        public ContentResult Create(CreateCustomerRequest request)
        {
            SetActorInformation(request);
            IRestResponse updateResult = _customerSettingService.CreateCustomer(request);
            Response.StatusCode = (int)updateResult.StatusCode;
            Response.TrySkipIisCustomErrors = true;

            return Content(updateResult.Content);
        }

        [HttpGet, CustomerSettingAuth(IsUpdate = false)]
        public ContentResult CheckExistedUsername(string username)
        {
            IRestResponse result = _customerSettingService.CheckExistedUsername(username);
            Response.StatusCode = (int)result.StatusCode;
            Response.TrySkipIisCustomErrors = true;

            return Content(result.Content);
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