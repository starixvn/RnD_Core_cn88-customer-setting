namespace CN88.CustomerSetting.WebApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Enumerations;
    using CN88.CustomerSetting.Services;
    using CN88.CustomerSetting.SettingType1.Models;
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
        private readonly IHostingEnvironment hostingEnvironment;

        public CustomerSettingController(
            QuerySettingService querySettingService,
            IHostingEnvironment hostingEnvironment)
        {
            this.querySettingService = querySettingService;
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public ISetting SampleQuery(byte levelId, bool isCreateNew, int customerId, byte productId = 1)
        {
            PTItem ptItem = null;

            switch (levelId)
            {
                case Level.SuperLevelId:
                    ptItem = new SuperPTItem
                    {
                        MaxPT = 90,
                        SuperMaxPT = isCreateNew ? 0 : 80,
                        SuperMinPT = isCreateNew ? 0 : 20,
                    };

                    break;

                case Level.MasterLevelId:
                    ptItem = new MasterPTItem
                    {
                        MaxPT = 80,
                        SuperPT = isCreateNew ? 0 : 10,
                    };

                    break;

                case Level.AgentLevelId:
                    ptItem = new AgentPTItem
                    {
                        MaxPT = 70,
                        MinPT = 10,
                        MasterPT = isCreateNew ? 0 : 40,
                        AgentPT = isCreateNew ? 0 : 20,
                    };

                    break;
            }

            var custLevel = Enumeration.FromValue<Level>(levelId);

            return new Setting(
                customerId,
                custLevel,
                isCreateNew,
                productId,
                new List<SettingType>
                {
                    new AccountDetail
                    {
                        CustomerInfo = new CustomerInfo
                        {
                            Username = isCreateNew ? string.Empty : "AccountTest",
                            Currency = new Currency
                            {
                                Id = 1,
                                Code = "11"
                            },
                            Credit = isCreateNew ? 0 : 100,
                            AvailableCredit = 1000,
                            UGroup = custLevel.IsMemberLevel() ? "CNA" : null,
                            UGroups = custLevel.IsMemberLevel() ? new List<string> { "CNA", "CNB", "CNC", "CND" } : null,
                            UsernameSuffix = custLevel.IsSuperLevel() || !isCreateNew
                                ? Enumerable.Empty<string>()
                                : custLevel.IsMemberLevel()
                                    ? new List<string> { "2", "B", "E" }
                                    : new List<string> { "4", "C" }
                        },
                        PTItem = ptItem,
                        SettingLevel = 1,
                        Id = 1,
                        DisplayOrder = 1,
                        DisplayMode = 1
                    }
                });
        }

        [HttpPost]
        public ISetting QuerySetting([FromBody]QuerySettingRequest request)
            => querySettingService.GetCustomerSetting(request.GetSettingContext(), request.GetCustomerSettingQuery());

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