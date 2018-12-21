namespace CN88.CustomerSetting.Services
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using CN88.CustomerSetting.DataAccesses;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Requests;
    using CN88.CustomerSetting.Models.Results;

    public interface CreateCustomerService
    {
        IResult Create(
            ISettingContext settingContext,
            CreateCustomerCommand command);
    }

    public class CreateCustomerServiceImpl : CreateCustomerService
    {
        private readonly ProductService productService;
        private readonly CustomerDataAccess customerDataAccess;
        private readonly SettingTypeServiceFactory settingTypeServiceFactory;

        public CreateCustomerServiceImpl(
            ProductService productService,
            CustomerDataAccess customerDataAccess,
            SettingTypeServiceFactory settingTypeServiceFactory)
        {
            this.productService = productService;
            this.customerDataAccess = customerDataAccess;
            this.settingTypeServiceFactory = settingTypeServiceFactory;
        }

        public IResult Create(
            ISettingContext settingContext,
            CreateCustomerCommand command)
        {
            var product = productService.GetProduct(settingContext, command.Setting.ProductId, settingContext.System);
            var insertCustomerParams = new Dictionary<string, object>();

            var settingTypes = command.Setting.SettingTypes;
            var setting = command.Setting;

            foreach (var settingType in settingTypes)
            {
                var settingTypeService = settingTypeServiceFactory.GetInstance(settingType);

                var insertSettingTypeParams = settingTypeService.GetInsertCustomerParams(
                    settingContext,
                    command,
                    settingType);

                insertCustomerParams.Concat(insertSettingTypeParams);
            }

            return customerDataAccess.InsertCustomer(setting.CustomerLevel, new Hashtable(insertCustomerParams));
        }
    }
}