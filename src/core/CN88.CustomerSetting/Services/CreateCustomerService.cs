namespace CN88.CustomerSetting.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using CN88.CustomerSetting.DataAccesses;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Requests;
    using CN88.CustomerSetting.Models.Results;
    using CN88.CustomerSetting.SpParams.Commands;
    using Fanex.Data.Repository;

    public interface CreateCustomerService
    {
        IResult Create(
            ISettingContext settingContext,
            CreateCustomerCommand command);

        IResult IsUsernameExisted(string username);
    }

    public class CreateCustomerServiceImpl : CreateCustomerService
    {
        private readonly ProductService productService;
        private readonly CustomerDataAccess customerDataAccess;
        private readonly SettingTypeServiceFactory settingTypeServiceFactory;
        private readonly ValidationService validationService;
        private readonly IDynamicRepository dynamicRepository;

        public CreateCustomerServiceImpl(
            ProductService productService,
            CustomerDataAccess customerDataAccess,
            SettingTypeServiceFactory settingTypeServiceFactory,
            ValidationService validationService,
            IDynamicRepository dynamicRepository)
        {
            this.productService = productService;
            this.customerDataAccess = customerDataAccess;
            this.settingTypeServiceFactory = settingTypeServiceFactory;
            this.validationService = validationService;
            this.dynamicRepository = dynamicRepository;
        }

        public IResult Create(
            ISettingContext settingContext,
            CreateCustomerCommand command)
        {
            var validationResult = Validate(settingContext, command);

            if (validationResult.IsNotOk)
            {
                return validationResult;
            }

            var product = productService.GetProduct(settingContext, command.Setting.ProductId, settingContext.System);
            var insertCustomerParams = new Dictionary<string, object>();

            var settingTypes = command.Setting.SettingTypes;
            var setting = command.Setting;

            foreach (var settingType in settingTypes)
            {
                var settingTypeService = settingTypeServiceFactory.GetInstance(settingType);

                var insertSettingTypeParams = settingTypeService.GetCreateCustomerParams(
                    settingContext,
                    command,
                    settingType);

                insertCustomerParams = insertCustomerParams
                                            .Concat(insertSettingTypeParams)
                                            .ToDictionary(pair => pair.Key, pair => pair.Value);
            }

            return customerDataAccess.InsertCustomer(
                setting.CustomerLevel, 
                new Hashtable(insertCustomerParams, StringComparer.OrdinalIgnoreCase));
        }

        public IResult IsUsernameExisted(string username)
        {
            var command = new CheckValidUsernameCommand { Username = username };

            dynamicRepository.Execute(command);

            return new Result(command.IsExists ? 1 : 0);
        }

        protected virtual IResult Validate(
           ISettingContext settingContext,
           CreateCustomerCommand command)
        {
            return validationService.ValidateCreateCustomerAction(settingContext, command);
        }
    }
}