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
            ValidationService validationService,
            ILogger logger,
            SettingTypeServiceFactory settingTypeServiceFactory)
        {
            this.validationService = validationService;
            this.logger = logger;
            this.settingTypeServiceFactory = settingTypeServiceFactory;
        }
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
            => validationService.ValidateUpdateSettingAction(settingContext, command);

        protected virtual IEnumerable<SettingResult> UpdateCustomerSettingTypes(
            ISettingContext settingContext,
            UpdateCustomerSettingCommand command)
        {
            var settingTypes = command.Setting.SettingTypes;
            var settingTypeResults = new List<SettingResult>();

            Parallel.ForEach(
                settingTypes,
                settingType =>
                {
                    var updateSettingTypeResult = UpdateCustomerSettingType(
                    settingContext,
                    command,
                    settingType);

                    settingTypeResults.Add(updateSettingTypeResult);
                });

            return settingTypeResults;
        }

        protected virtual SettingResult UpdateCustomerSettingType(
            ISettingContext settingContext,
            UpdateCustomerSettingCommand command,
            SettingType settingType)
        {
            var setting = command?.Setting;

            try
            {
                CheckSettingTypeIsValid(settingType);

                var settingTypeService = settingTypeServiceFactory.GetInstance(settingType);
                var settingResult = settingTypeService.UpdateCustomerSettingType(settingContext, command, settingType);

                return settingResult;
            }
            catch (CustomerSettingException customerSettingException)
            {
                WriteLog(settingContext, setting, settingType, customerSettingException);

                return new SettingResult(Result.AccessDenied.Code, customerSettingException.OriginalMessage, 0, setting.CustomerId, settingType.Id, settingType.Name);
            }
            catch (Exception ex)
            {
                WriteLog(settingContext, setting, settingType, new CustomerSettingException(ex.Message, ex));

                return new SettingResult(
                    Result.SystemError,
                    setting.CustomerId,
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
            ISetting setting,
            SettingType customerSettingType,
            CustomerSettingException ex)
        {
            ex.SetSettingContextInfo("Update Setting", settingContext);
            ex.SetCustomerInfo(setting.CustomerId, setting.CustomerLevel);
            ex.SetProductInfo(setting.ProductId, customerSettingType);

            logger.Error(ex.Message, ex);
        }
    }
}