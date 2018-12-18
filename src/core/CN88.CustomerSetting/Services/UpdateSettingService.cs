namespace CN88.CustomerSetting.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CN88.CustomerSetting.Exceptions;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Enumerations;
    using CN88.CustomerSetting.Models.Requests;
    using CN88.CustomerSetting.Models.Results;
    using Fanex.Logging;

    public interface UpdateSettingService
    {
        IResult Update(
            ISettingContext settingContext,
            UpdateCustomerSettingCommand command);
    }

    public class UpdateSettingServiceImpl : UpdateSettingService
    {
        private readonly ValidationService validationService;
        private readonly ILogger logger;
        private readonly SettingTypeServiceFactory settingTypeServiceFactory;

        public UpdateSettingServiceImpl(
            ValidationService validationService = null,
            ILogger logger = null,
            SettingTypeServiceFactory settingTypeServiceFactory = null)
        {
            this.validationService = validationService ?? new ValidationServiceImpl();
            this.logger = logger ?? Fanex.Logging.Logger.Log;
            this.settingTypeServiceFactory = settingTypeServiceFactory ?? new SettingTypeServiceFactoryImpl();
        }

        protected ValidationService ValidationService { get; }

        protected ILogger Logger { get; }

        protected SettingTypeServiceFactory SettingTypeServiceFactory { get; }

        public virtual IResult Update(
            ISettingContext settingContext,
            UpdateCustomerSettingCommand command)
        {
            var validationResult = Validate(settingContext, command);

            if (validationResult.IsNotOk)
            {
                return validationResult;
            }

            IResult result = new UpdateSettingResult(UpdateCustomerSettingTypes(settingContext, command));

            return result;
        }

        protected virtual IResult Validate(
            ISettingContext settingContext,
            UpdateCustomerSettingCommand command)
        {
            return ValidationService.ValidateUpdateSettingAction(settingContext, command);
        }

        protected virtual IEnumerable<SettingResult> UpdateCustomerSettingTypes(
            ISettingContext settingContext,
            UpdateCustomerSettingCommand command)
        {
            var settingTypes = command.Setting.SettingTypes;
            var customerId = command.Setting.CustomerId;
            var customerLevel = command.Setting.CustomerLevel;
            var uplineLevel = command.UplineLevel;
            var productId = command.Setting.ProductId;
            var settingTypeResults = new List<SettingResult>();

            Parallel.ForEach(
                settingTypes,
                settingType =>
                {
                    var updateSettingTypeResult = UpdateCustomerSettingType(
                    settingContext,
                    customerId,
                    customerLevel,
                    settingType,
                    productId,
                    uplineLevel);

                    settingTypeResults.Add(updateSettingTypeResult);
                });

            return settingTypeResults;
        }

        protected virtual SettingResult UpdateCustomerSettingType(
            ISettingContext settingContext,
            int customerId,
            Level customerLevel,
            SettingType settingType,
            byte productId,
            Level uplineLevel)
        {
            try
            {
                CheckSettingTypeIsValid(settingType);

                var settingTypeService = SettingTypeServiceFactory.GetInstance(settingType);
                var settingResult = settingTypeService.UpdateCustomerSettingType(settingContext, customerId, customerLevel, uplineLevel, productId, settingType);

                return settingResult;
            }
            catch (CustomerSettingException customerSettingException)
            {
                WriteLog(settingContext, customerId, customerLevel, uplineLevel, productId, settingType, customerSettingException);

                return new SettingResult(Result.AccessDenied.Code, customerSettingException.OriginalMessage, 0, customerId, settingType.Id, settingType.Name);
            }
            catch (Exception ex)
            {
                WriteLog(settingContext, customerId, customerLevel, uplineLevel, productId, settingType, new CustomerSettingException(ex.Message, ex));

                return new SettingResult(
                    Result.SystemError,
                    customerId,
                    settingType.Id,
                    settingType.Name);
            }
        }

        protected virtual void CheckSettingTypeIsValid(SettingType settingType)
        {
            var result = settingType.Validate();

            if (result.IsNotOk)
            {
                throw new CustomerSettingException(result.Message);
            }
        }

        protected virtual void WriteLog(
            ISettingContext settingContext,
            int customerId,
            Level customerLevel,
            Level uplineLevel,
            byte productId,
            SettingType customerSettingType,
            CustomerSettingException ex)
        {
            ex.SetSettingContextInfo("Update Setting", settingContext);
            ex.SetCustomerInfo(customerId, customerLevel, uplineLevel);
            ex.SetProductInfo(productId, customerSettingType);

            Logger.Error(ex.Message, ex);
        }
    }
}