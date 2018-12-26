namespace CN88.CustomerSetting.WebApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CN88.CustomerSetting.AppResources;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Enumerations;
    using CN88.CustomerSetting.Models.Results;
    using CN88.CustomerSetting.Services;
    using CN88.CustomerSetting.SettingType1.Models;
    using CN88.CustomerSetting.WebApi.Models.Binders;
    using CN88.CustomerSetting.WebApi.Models.Requests;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomerSettingController : Controller
    {
        private static string CustomerSettingTemplateCache;
        private const string CustomerSettingTemplateFilePath = "/Templates/customer-setting.htm";
        private static string CustomerSettingTemplateIe8Cache;
        private const string CustomerSettingTemplateIe8FilePath = "/Templates/customer-setting-ie8.htm";
        private const byte CachingHours = 4;
        private readonly QuerySettingService querySettingService;
        private readonly CreateCustomerService createCustomerService;
        private readonly IHostingEnvironment hostingEnvironment;

        public CustomerSettingController(
            QuerySettingService querySettingService,
            CreateCustomerService createCustomerService,
            IHostingEnvironment hostingEnvironment)
        {
            this.querySettingService = querySettingService;
            this.hostingEnvironment = hostingEnvironment;
            this.createCustomerService = createCustomerService;
        }

        [HttpPost]
        public ISetting Query([FromBody]QuerySettingRequest request)
            => querySettingService.GetCustomerSetting(request.GetSettingContext(), request.GetCustomerSettingQuery());

        [HttpPost]
        public IResult Create(
            [ModelBinder(typeof(SettingRequestModelBinder<CreateCustomerRequest>))]CreateCustomerRequest request)
            => createCustomerService.Create(request.GetSettingContext(), request.GetCreateCustomerCommand());

        [HttpGet]
        public IResult CheckExistedUsername(string username)
            => createCustomerService.IsUsernameExisted(username);

        [HttpGet]
        public string Test()
            => Messages.AccessDeniedMsg;

        [HttpGet]
        public string GetTemplate([FromQuery]bool isIe8)
        {
            string template = string.Empty;
            string customerSettingTemplatePath = CustomerSettingTemplateFilePath;

            if (isIe8)
            {
                customerSettingTemplatePath = CustomerSettingTemplateIe8FilePath;
            }

            template = isIe8 
                ? CustomerSettingTemplateIe8Cache
                : CustomerSettingTemplateCache;

            if (string.IsNullOrEmpty(template))
            {
                var file = hostingEnvironment.WebRootFileProvider.GetFileInfo(customerSettingTemplatePath);
                template = System.IO.File.ReadAllText(file.PhysicalPath);
                if(isIe8)
                {
                    CustomerSettingTemplateIe8Cache = template;
                }
                else
                {
                    CustomerSettingTemplateCache = template;
                }
            }

            return template;
        }
    }
}