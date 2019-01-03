namespace CN88.CustomerSetting.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CN88.CustomerSetting.Exceptions;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Requests;
    using CN88.CustomerSetting.Models.Results;
    using Fanex.Logging;

    public interface UpdateSettingService
    {
        Task<IResult> Update(
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

        public virtual async Task<IResult> Update(
            ISettingContext settingContext,
            UpdateCustomerSettingCommand command)
        {
            var validationResult = await Validate(settingContext, command);

            if (validationResult.IsNotOk)
            {
                return validationResult;
            }

            var result = new UpdateSettingResult(await UpdateCustomerSettingTypes(settingContext, command));

            return result;
        }

        protected virtual async Task<IResult> Validate(
            ISettingContext settingContext,
            UpdateCustomerSettingCommand command)
            => await validationService.ValidateUpdateSettingAction(settingContext, command);

        protected virtual async Task<IEnumerable<SettingResult>> UpdateCustomerSettingTypes(
            ISettingContext settingContext,
            UpdateCustomerSettingCommand command)
        {
            var settingTypes = command.Setting.SettingTypes;
            var settingTypeResults = new List<SettingResult>();

            return await Task.WhenAll(
                settingTypes.Select(
                    settingType =>
                        UpdateCustomerSettingType(
                        settingContext,
                        command,
                        settingType)));
        }

        protected virtual async Task<SettingResult> UpdateCustomerSettingType(
            ISettingContext settingContext,
            UpdateCustomerSettingCommand command,
            SettingType settingType)
        {
            var setting = command?.Setting;

            try
            {
                CheckSettingTypeIsValid(settingType);

                var settingTypeService = settingTypeServiceFactory.GetInstance(settingType);
                var settingResult = await settingTypeService.UpdateCustomerSettingType(settingContext, command, settingType);

                return settingResult;
            }
            catch (CustomerSettingException customerSettingException)
            {
                WriteLog(settingContext, setting, settingType, customerSettingException);

                return await Task.FromResult(
                    new SettingResult(
                        Result.AccessDenied.Code,
                        customerSettingException.OriginalMessage,
                        0,
                        setting.CustomerId,
                        settingType.Id,
                        settingType.Name));
            }
            catch (Exception ex)
            {
                WriteLog(settingContext, setting, settingType, new CustomerSettingException(ex.Message, ex));

                return await Task.FromResult(new SettingResult(
                    Result.SystemError,
                    setting.CustomerId,
                    settingType.Id,
                    settingType.Name));
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