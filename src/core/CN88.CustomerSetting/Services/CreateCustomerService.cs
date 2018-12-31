namespace CN88.CustomerSetting.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using CN88.CustomerSetting.DataAccesses;
    using CN88.CustomerSetting.Exceptions;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Enumerations;
    using CN88.CustomerSetting.Models.Requests;
    using CN88.CustomerSetting.Models.Results;
    using CN88.CustomerSetting.SpParams.Commands;
    using Fanex.Data.Repository;
    using Fanex.Logging;

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
        private readonly ILogger logger;

        public CreateCustomerServiceImpl(
            ProductService productService,
            CustomerDataAccess customerDataAccess,
            SettingTypeServiceFactory settingTypeServiceFactory,
            ValidationService validationService,
            IDynamicRepository dynamicRepository,
            ILogger logger)
        {
            this.productService = productService;
            this.customerDataAccess = customerDataAccess;
            this.settingTypeServiceFactory = settingTypeServiceFactory;
            this.validationService = validationService;
            this.dynamicRepository = dynamicRepository;
            this.logger = logger;
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

            try
            {
                var createNewParams = GetCreateNewCustomerParams(settingContext, command);

                return customerDataAccess.CreateNewCustomer(
                    command.Setting.CustomerLevel,
                    new Hashtable(createNewParams, StringComparer.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                WriteLog(settingContext, command.Setting, new CustomerSettingException(ex.Message, ex));

                return Result.CreateErrorResult(ex.Message);
            }
        }

        private Dictionary<string, object> GetCreateNewCustomerParams(
            ISettingContext settingContext, 
            CreateCustomerCommand command)
        {
            var createNewParams = new Dictionary<string, object>();
            var settingTypes = command.Setting.SettingTypes;

            foreach (var settingType in settingTypes)
            {
                var settingTypeService = settingTypeServiceFactory.GetInstance(settingType);

                var insertSettingTypeParams = settingTypeService.BuildCreateCustomerParams(
                    settingContext,
                    command,
                    settingType);

                createNewParams = createNewParams
                                            .Concat(insertSettingTypeParams)
                                            .ToDictionary(pair => pair.Key, pair => pair.Value);
            }

            return createNewParams;
        }

        public IResult IsUsernameExisted(string username)
        {
            var command = new CheckValidUsernameCommand(username);

            dynamicRepository.Execute(command);

            return command.IsExists
                ? Result.CreateErrorResult(string.Empty)
                : Result.CreateSuccessResult(string.Empty);
        }

        protected virtual IResult Validate(
           ISettingContext settingContext,
           CreateCustomerCommand command)
            => validationService.ValidateCreateCustomerAction(settingContext, command);

        protected virtual void WriteLog(
            ISettingContext settingContext,
            ISetting setting,
            CustomerSettingException ex)
        {
            ex.SetSettingContextInfo("Create Customer", settingContext);
            ex.SetCustomerInfo(setting.CustomerId, setting.CustomerLevel);
            ex.SetProductInfo(setting.ProductId);

            logger.Error(ex.Message, ex);
        }
    }
}