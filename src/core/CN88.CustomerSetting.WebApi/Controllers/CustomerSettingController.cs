namespace CN88.CustomerSetting.WebApi.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Results;
    using CN88.CustomerSetting.Services;
    using CN88.CustomerSetting.SettingType2.Models;
    using CN88.CustomerSetting.SettingType2.Services;
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
        private static volatile string templateCached;
        private const string templateFilePath = "/Templates/customer-setting.htm";
        private readonly QuerySettingService querySettingService;
        private readonly CreateCustomerService createCustomerService;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly UpdateSettingService updateSettingService;
        private readonly CurrencyBetSettingService currencyBetSettingService;

        public CustomerSettingController(
            QuerySettingService querySettingService,
            CreateCustomerService createCustomerService,
            UpdateSettingService updateSettingService,
            IHostingEnvironment hostingEnvironment,
            CurrencyBetSettingService currencyBetSettingService)
        {
            this.querySettingService = querySettingService;
            this.hostingEnvironment = hostingEnvironment;
            this.createCustomerService = createCustomerService;
            this.updateSettingService = updateSettingService;
            this.currencyBetSettingService = currencyBetSettingService;
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
        public Task<IEnumerable<string>> GetHeadlineSuggestionNames(string username)
            => createCustomerService.GetHeadlineSuggestionNames(username);

        [HttpGet]
        public async Task<dynamic> GetHeadlineCurrency(string username)
        {
            var currency = await createCustomerService.GetHeadlineCurrency(username);

            if(currency == null)
            {
                return null;
            }

            var betSettings = currency.Id > 0
                    ? await currencyBetSettingService.GetCurrencyBetSetting(currency.Id)
                    : Enumerable.Empty<CommBetSettingItem>();

            return new
            {
                currency,
                betSettings
            };
        }

        [HttpGet]
        public string GetTemplate()
        {
            string template = templateCached;

            if (string.IsNullOrEmpty(template))
            {
                var file = hostingEnvironment.WebRootFileProvider.GetFileInfo(templateFilePath);
                template = System.IO.File.ReadAllText(file.PhysicalPath);
#pragma warning disable S2696 // Instance members should not write to "static" fields
                templateCached = template;
#pragma warning restore S2696 // Instance members should not write to "static" fields
            }

            return template;
        }
    }
}