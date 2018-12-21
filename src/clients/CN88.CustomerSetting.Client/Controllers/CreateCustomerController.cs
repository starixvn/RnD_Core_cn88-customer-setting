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

        [HttpPost]
        [CustomerSettingAuth(IsUpdate = true, IsMultipleUpdate = false)]
        public ContentResult UpdateSetting(UpdateSettingRequest updateSettingRequest)
        {
            SetActorInformation(updateSettingRequest);
            IRestResponse updateResult = _customerSettingService.UpdateCustomerSetting(updateSettingRequest);
            Response.StatusCode = (int)updateResult.StatusCode;
            Response.TrySkipIisCustomErrors = true;

            return Content(updateResult.Content);
        }

        [HttpPost]
        [CustomerSettingAuth(IsUpdate = true, IsMultipleUpdate = true)]
        public ContentResult UpdateMultipleSetting(MultipleUpdateSettingRequest multipleUpdateSetting)
        {
            IRestResponse updateResult;

            SetActorInformation(multipleUpdateSetting);
            updateResult = _customerSettingService.MultipleUpdateCustomerSetting(multipleUpdateSetting);
            Response.StatusCode = (int)updateResult.StatusCode;
            Response.TrySkipIisCustomErrors = true;

            return Content(updateResult.Content);
        }

        private static void SetActorInformation(UpdateSettingRequest updateSetting)
        {
            if (updateSetting != null)
            {
                SettingContext settingContext = CN88CustomerSettingRegistration.SettingContext;

                updateSetting.ActorId = settingContext.ActorId;
                updateSetting.ActorLevel = settingContext.ActorLevel;
                updateSetting.ActorName = settingContext.ActorName;
                updateSetting.SubAccountId = settingContext.SubAccountId;
                updateSetting.Domain = settingContext.Domain;
                updateSetting.Language = settingContext.Language;
                updateSetting.ClientIP = settingContext.IP;
                updateSetting.System = settingContext.System;
            }
        }
    }
}