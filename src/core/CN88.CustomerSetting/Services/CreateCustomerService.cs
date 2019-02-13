namespace CN88.CustomerSetting.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CN88.CustomerSetting.DataAccesses;
    using CN88.CustomerSetting.Exceptions;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Constants;
    using CN88.CustomerSetting.Models.Requests;
    using CN88.CustomerSetting.Models.Results;
    using CN88.CustomerSetting.SpParams.Commands;
    using CN88.CustomerSetting.SpParams.Criteria;
    using Fanex.Data.Repository;
    using Fanex.Logging;

    public interface CreateCustomerService
    {
        Task<IResult> Create(
            ISettingContext settingContext,
            CreateCustomerCommand command);

        Task<IResult> IsUsernameExisted(string username);

        Task<IEnumerable<string>> GetMainShareHolderSuggestionNames();
    }

    public class CreateCustomerServiceImpl : CreateCustomerService
    {
        private readonly CustomerDataAccess customerDataAccess;
        private readonly SettingTypeServiceFactory settingTypeServiceFactory;
        private readonly ValidationService validationService;
        private readonly IDynamicRepository dynamicRepository;
        private readonly ILogger logger;

        public CreateCustomerServiceImpl(
            CustomerDataAccess customerDataAccess,
            SettingTypeServiceFactory settingTypeServiceFactory,
            ValidationService validationService,
            IDynamicRepository dynamicRepository,
            ILogger logger)
        {
            this.customerDataAccess = customerDataAccess;
            this.settingTypeServiceFactory = settingTypeServiceFactory;
            this.validationService = validationService;
            this.dynamicRepository = dynamicRepository;
            this.logger = logger;
        }

        public async Task<IResult> Create(
            ISettingContext settingContext,
            CreateCustomerCommand command)
        {
            var validationResult = await Validate(settingContext, command).ConfigureAwait(false);

            if (validationResult.IsNotOk)
            {
                return validationResult;
            }

            try
            {
                var createNewParams = GetCreateNewCustomerParams(settingContext, command);

                var result = await customerDataAccess
                    .CreateNewCustomer(
                        command.Setting.CustomerLevel,
                        new Hashtable(createNewParams, StringComparer.OrdinalIgnoreCase))
                    .ConfigureAwait(false);

                if (result.IsFailed
                    && result.SettingTypeId > 0)
                {
                    return BuildSettingTypeResult(settingContext, command, result);
                }

                return result;
            }
            catch (Exception ex)
            {
                WriteLog(settingContext, command.Setting, new CustomerSettingException(ex.Message, ex));

                return Result.CreateErrorResult(ex.Message);
            }
        }

        private IResult BuildSettingTypeResult(ISettingContext settingContext, CreateCustomerCommand command, CreateNewResult result)
        {
            var settingType = command.Setting.SettingTypes.FirstOrDefault(st => st.Id == result.SettingTypeId);

            if (settingType == null)
            {
                return result;
            }

            var settingTypeService = settingTypeServiceFactory.GetInstance(settingType);
            var settingResult = settingTypeService.TranslateResultMessage(
                new SettingResult(
                    result.Code,
                    result.Message,
                    result.IsFailed && result.MsgType == 0
                        ? MessageTypes.Error
                        : result.MsgType,
                    command.Setting.CustomerId,
                    settingType.Id,
                    settingType.Name),
                string.Empty,
                settingType.SettingLevel,
                settingContext.Language);

            return new UpdateSettingResult(new List<SettingResult> { settingResult });
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

        public async Task<IResult> IsUsernameExisted(string username)
        {
            var command = new CheckValidUsernameCommand(username);

            await dynamicRepository.ExecuteAsync(command).ConfigureAwait(false);

            return command.IsExists
                ? Result.CreateErrorResult(string.Empty)
                : Result.CreateSuccessResult(string.Empty);
        }

        protected virtual async Task<IResult> Validate(
           ISettingContext settingContext,
           CreateCustomerCommand command)
            => await validationService.ValidateCreateCustomerAction(settingContext, command).ConfigureAwait(false);

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

        public async Task<IEnumerable<string>> GetMainShareHolderSuggestionNames()
            => await Task.FromResult(
                new List<string>
                {
                    "ShareHolderName1",
                    "ShareHolderName2",
                    "ShareHolderName3"
                })
            .ConfigureAwait(false);
    }
}