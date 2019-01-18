namespace CN88.CustomerSetting.WebApi.Controllers
{
    using System.Threading.Tasks;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Results;
    using CN88.CustomerSetting.Services;
    using CN88.CustomerSetting.WebApi.Models.Binders;
    using CN88.CustomerSetting.WebApi.Models.Requests;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "api")]
    [ApiController]
    public class CustomerSettingController : Controller
    {
        private static string TemplateCached;
        private const string TemplateFilePath = "/Templates/customer-setting.htm";
        private const byte CachingHours = 4;
        private readonly QuerySettingService querySettingService;
        private readonly CreateCustomerService createCustomerService;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly UpdateSettingService updateSettingService;

        public CustomerSettingController(
            QuerySettingService querySettingService,
            CreateCustomerService createCustomerService,
            UpdateSettingService updateSettingService,
            IHostingEnvironment hostingEnvironment)
        {
            this.querySettingService = querySettingService;
            this.hostingEnvironment = hostingEnvironment;
            this.createCustomerService = createCustomerService;
            this.updateSettingService = updateSettingService;
        }

        [HttpPost]
        public Task<ISetting> Query([FromBody]QuerySettingRequest request)
            => querySettingService.GetCustomerSetting(request.GetSettingContext(), request.GetCustomerSettingQuery());

        [HttpPost]
        public Task<IResult> Create(
            [ModelBinder(typeof(SettingRequestModelBinder<CreateCustomerRequest>))]CreateCustomerRequest request)
            => createCustomerService.Create(request.GetSettingContext(), request.GetCreateCustomerCommand());

        [HttpPost]
        public Task<IResult> Update(
            [ModelBinder(typeof(SettingRequestModelBinder<UpdateSettingRequest>))]UpdateSettingRequest request)
            => updateSettingService.Update(request.GetSettingContext(), request.GetUpdateCustomerSettingCommand());

        [HttpGet]
        public Task<IResult> CheckExistedUsername(string username)
            => createCustomerService.IsUsernameExisted(username);

        [HttpGet]
        public string GetTemplate()
        {
            string template = TemplateCached;
            string templatePath = TemplateFilePath;

            if (string.IsNullOrEmpty(template))
            {
                var file = hostingEnvironment.WebRootFileProvider.GetFileInfo(templatePath);
                template = System.IO.File.ReadAllText(file.PhysicalPath);
                TemplateCached = template;
            }

            return template;
        }
    }
}