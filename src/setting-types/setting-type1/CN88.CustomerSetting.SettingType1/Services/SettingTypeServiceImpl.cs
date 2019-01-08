namespace CN88.CustomerSetting.SettingType1.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Enumerations;
    using CN88.CustomerSetting.Models.Requests;
    using CN88.CustomerSetting.Models.Results;
    using CN88.CustomerSetting.Services;
    using CN88.CustomerSetting.SettingType1.AppResources;
    using CN88.CustomerSetting.SettingType1.Models;
    using CN88.CustomerSetting.SettingType1.Models.Dtos;
    using CN88.CustomerSetting.SettingType1.Params.Commands;
    using CN88.CustomerSetting.SettingType1.Params.Criteria;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Fanex.Logging;

    public class SettingTypeServiceImpl : SettingTypeServiceBase, SettingTypeService
    {
        private const string CN88Site = "CN88";
        private const int SettingTypeId = 1;
        private const int CurrencyCacheTimeInMinutes = 5;
        private const string CurrencyCacheKey = "CN88.CustomerSetting.CurrencyCacheKey";
        private const int SpreadGroupCacheTimeInHours = 5;
        private const string SpreadGroupCacheKey = "CN88.CustomerSetting.SpreadGroupCacheKey";

        private readonly PasswordService passwordService;

        public SettingTypeServiceImpl() :
            this(null, null, null, null, null)
        { }

        internal SettingTypeServiceImpl(
            ResourceService resourceService,
            ILogger logger,
            ICacheService cacheService,
            IDynamicRepository dynamicRepository,
            MessageService messageService,
            PasswordService passwordService = null)
            : base(resourceService, logger, cacheService, dynamicRepository)
        {
            this.passwordService = passwordService ?? new PasswordServiceImpl();
            MessageService = messageService ?? new MessageServiceImpl(ResourceService);
        }

        public override async Task<SettingType> GetCustomerSettingType(
            ISettingContext settingContext,
            CustomerSettingQuery query)
        {
            if (query.IsCreateNew)
            {
                return await GetDefaultSettingForCreateNew(query);
            }

            return await GetAccountDetail(query);
        }

        private async Task<SettingType> GetAccountDetail(CustomerSettingQuery query)
        {
            var settingItem = await GetAccountDetailSettingItem(query.CustomerLevel, query.CustomerId, query.ProductId, false);
            var customerInfo = (await DynamicRepository.GetAsync<CustomerInfoDto>(new GetGeneralInfoCriteria(query.CustomerLevel.Value, query.CustomerId)))
                ?? throw new InvalidOperationException($"Not found customer id {query.CustomerId} level {query.CustomerLevel.DisplayName}"); ;

            settingItem.Username = customerInfo.Username;
            settingItem.FirstName = customerInfo.FirstName;
            settingItem.LastName = customerInfo.LastName;
            settingItem.Phone = customerInfo.Phone;
            settingItem.MobilePhone = customerInfo.MobilePhone;
            settingItem.Fax = customerInfo.Fax;
            settingItem.Currency = new Currency { Name = customerInfo.Currency };
            settingItem.Credit = customerInfo.Credit;
            settingItem.AvailableCredit = customerInfo.AvailableCredit;

            if(query.CustomerLevel.IsMemberLevel()
                && !string.IsNullOrEmpty(customerInfo.UGroup))
            {
                settingItem.SpreadGroup = (await GetSpreadGroups())
                    .FirstOrDefault(group => 
                        string.Compare(customerInfo.UGroup, group.UGroup, true) == 0);
            }

            return CreateSettingType(settingItem);
        }

        private async Task<SettingType> GetDefaultSettingForCreateNew(CustomerSettingQuery query)
        {
            var settingItem = await GetAccountDetailSettingItem(query.CustomerLevel, query.UplineId, query.ProductId, true);
            var createCustomerInfo = await GetDefaultCreateCustomerInfo(query.CustomerLevel, query.UplineId);

            settingItem.AvailableCredit = createCustomerInfo.AvailableCredit;
            settingItem.UplineUsername = createCustomerInfo.UplineUsername;
            settingItem.UsernameSuffixes = createCustomerInfo.NewUsername?.ToCharArray().Select(c => c.ToString());
            settingItem.Currencies = query.CustomerLevel.IsSuperLevel()
                                ? await GetCurrencies()
                                : Enumerable.Empty<Currency>();

            if (query.CustomerLevel.IsMemberLevel())
            {
                settingItem.SpreadGroups = await GetSpreadGroups();
                settingItem.SpreadGroup = settingItem.SpreadGroups.FirstOrDefault();
            }

            return CreateSettingType(settingItem);
        }

        private SettingType CreateSettingType(ISettingItem settingItem)
            => new AccountDetailSettingType
            {
                SettingItems = new List<ISettingItem> { settingItem }
            };

        private async Task<AccountDetailSettingItem> GetAccountDetailSettingItem(
            Level level, int custId, int productId, bool isCreateNew)
        {
            if (level.IsMemberLevel())
            {
                return new MemberAccountDetail();
            }

            var settingItem = await GetAccountDetailSettingItem(level, custId, isCreateNew);
            var settingTypeConfig = (await GetSettingTypeConfig<AccountDetailSettingItem>(productId, SettingTypeId)).FirstOrDefault();
            if (settingTypeConfig != null)
            {
                settingItem.IntervalValue = settingTypeConfig.IntervalValue;
            }

            return settingItem;
        }

        private async Task<AccountDetailSettingItem> GetAccountDetailSettingItem(
            Level level,
            int custId,
            bool isCreateNew)
        {
            AccountDetailSettingItem settingItem;

            switch (level.Value)
            {
                case Level.SuperLevelId:
                    settingItem = await GetSuperAccountDetail(custId, isCreateNew);
                    break;

                case Level.MasterLevelId:
                    settingItem = await GetMasterAccountDetail(custId, isCreateNew);
                    break;

                default:
                    settingItem = await GetAgentAccountDetail(custId, isCreateNew);
                    break;
            }

            return settingItem
                ?? throw new InvalidOperationException($"Cannot get default setting to create new downline");
        }

        private async Task<AccountDetailSettingItem> GetSuperAccountDetail(int custId, bool isCreateNew)
            => await DynamicRepository.GetAsync<SuperAccountDetail>(new GetPTSettingCriteria(Level.Super.Value, custId, isCreateNew));

        private async Task<AccountDetailSettingItem> GetMasterAccountDetail(int custId, bool isCreateNew)
            => await DynamicRepository.GetAsync<MasterAccountDetail>(new GetPTSettingCriteria(Level.Master.Value, custId, isCreateNew));

        private async Task<AccountDetailSettingItem> GetAgentAccountDetail(int custId, bool isCreateNew)
            => await DynamicRepository.GetAsync<AgentAccountDetail>(new GetPTSettingCriteria(Level.Agent.Value, custId, isCreateNew));

        private async Task<CreateCustomerInfoDto> GetDefaultCreateCustomerInfo(Level level, int uplineId)
            => level.IsNotSuperLevel()
                ? await DynamicRepository.GetAsync<CreateCustomerInfoDto>(new GetCustomerInfoForCreateCriteria(level.Value, uplineId))
                : new CreateCustomerInfoDto();

        private async Task<IEnumerable<Currency>> GetCurrencies()
            => await CacheService.GetOrSetAsync(
                       CurrencyCacheKey,
                       () => DynamicRepository.Fetch<Currency>(new GetCurrencyListCriteria()),
                        new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(0, CurrencyCacheTimeInMinutes, 0)));

        private async Task<IEnumerable<SpreadGroup>> GetSpreadGroups()
            => await CacheService.GetOrSetAsync(
                       SpreadGroupCacheKey,
                       () => DynamicRepository.Fetch<SpreadGroup>(new GetSpreadGroupCriteria()),
                        new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(SpreadGroupCacheTimeInHours, 0, 0)));

        public override IDictionary<string, object> BuildCreateCustomerParams(
            ISettingContext settingContext,
            CreateCustomerCommand command,
            SettingType settingType)
        {
            var settingItem = settingType.SettingItems?.FirstOrDefault() as AccountDetailSettingItem
                ?? throw new InvalidOperationException("Account Detail Item cannot be null");

            settingItem.Password = ValidateAndEncodePassword(command, settingItem);

            var createNewParams = BuildCreateNewParams(settingContext, settingItem);

            if (command.Setting.CustomerLevel.IsNotMemberLevel())
            {
                createNewParams.Add("ptsetting", settingType.ToXml());
            }

            switch (command.Setting.CustomerLevel.Value)
            {
                case Level.SuperLevelId:
                    return AddParamsForAddNewSuper(settingItem, createNewParams);

                case Level.MasterLevelId:
                    return AddParamsForAddNewMaster(settingContext, command, createNewParams);

                case Level.AgentLevelId:
                    return AddParamsForAddNewAgent(settingContext, command, createNewParams);

                default:
                    return AddParamsForAddNewMember(settingContext, command, settingItem, createNewParams);
            }
        }

        private string ValidateAndEncodePassword(
            CreateCustomerCommand command,
            AccountDetailSettingItem settingItem)
        {
            var passwordResult = passwordService.ValidateAndHashPassword(
                settingItem.Password,
                command.HashKey,
                command.Setting.CustomerLevel);

            if (passwordResult.IsNotOk)
            {
                throw new InvalidOperationException(passwordResult.Message);
            }

            return passwordResult.Message;
        }

        private static Dictionary<string, object> BuildCreateNewParams(
            ISettingContext settingContext,
            AccountDetailSettingItem settingItem)
            => new Dictionary<string, object>
            {
                { "username", settingItem.Username ?? throw new InvalidOperationException("Username cannot be null") },
                { "userpwd", settingItem.Password ?? throw new InvalidOperationException(Resources.PasswordCannotBeNullOrEmpty) },
                { "firstname", settingItem.FirstName ?? string.Empty },
                { "credit", settingItem.Credit },
                { "systemid", settingContext.System },
                { "ip", settingContext.IP },
                { "language", LanguageNameConverter.GetLanguageKey(settingContext.Language) },
                { "creator", settingContext.Actor.Id }
            };

        private static Dictionary<string, object> AddParamsForAddNewMember(
            ISettingContext settingContext,
            CreateCustomerCommand command,
            AccountDetailSettingItem settingItem,
            Dictionary<string, object> createNewParams)
        {
            createNewParams.Add("subaccid", settingContext.Actor.SubAccountId);
            createNewParams.Add("recommend", command.UplineId);
            createNewParams.Add("ugroup", settingItem.SpreadGroup.UGroup);

            return createNewParams;
        }

        private static Dictionary<string, object> AddParamsForAddNewAgent(
            ISettingContext settingContext,
            CreateCustomerCommand command,
            Dictionary<string, object> createNewParams)
        {
            createNewParams.Add("subaccid", settingContext.Actor.SubAccountId);
            createNewParams.Add("mrecommend", command.UplineId);

            return createNewParams;
        }

        private static Dictionary<string, object> AddParamsForAddNewMaster(
            ISettingContext settingContext,
            CreateCustomerCommand command,
            Dictionary<string, object> createNewParams)
        {
            createNewParams.Add("subaccid", settingContext.Actor.SubAccountId);
            createNewParams.Add("srecommend", command.UplineId);

            return createNewParams;
        }

        private static Dictionary<string, object> AddParamsForAddNewSuper(
            AccountDetailSettingItem settingItem,
            Dictionary<string, object> createNewParams)
        {
            createNewParams.Add("site", CN88Site);
            createNewParams.Add("currency", settingItem.Currency?.Id);
            createNewParams.Add("lastname", settingItem.LastName ?? string.Empty);
            createNewParams.Add("phone", settingItem.Phone ?? string.Empty);
            createNewParams.Add("mobilephone", settingItem.MobilePhone ?? string.Empty);
            createNewParams.Add("fax", settingItem.Fax ?? string.Empty);

            return createNewParams;
        }

        public override async Task<SettingResult> UpdateCustomerSettingType(
            ISettingContext settingContext,
            UpdateCustomerSettingCommand command,
            SettingType settingType)
        {
            var settingItem = settingType.SettingItems.FirstOrDefault() as AccountDetailSettingItem
                ?? throw new InvalidOperationException("AccountDetail cannot be null");

            var updateCustomerInfoResult = await UpdateCustomerInfo(command, settingItem, settingContext.Language);
            if (updateCustomerInfoResult.IsFailed)
            {
                return updateCustomerInfoResult;
            }

            return await UpdatePTSetting(settingContext, command, settingType);
        }

        private async Task<SettingResult> UpdatePTSetting(
            ISettingContext settingContext,
            UpdateCustomerSettingCommand command,
            SettingType settingType)
        {
            var updatePTSettingCommand = new UpdatePTSettingCommand
            {
                RoleId = command.Setting.CustomerLevel.Value,
                CustId = command.Setting.CustomerId,

                ProductId = command.Setting.ProductId,
                PTSetting = settingType.ToXml(),

                IP = settingContext.IP,
                UserId = settingContext.Actor.Id,
                SubAccountId = settingContext.Actor.SubAccountId,
            };

            await DynamicRepository.ExecuteAsync(updatePTSettingCommand);

            var settingResult = new SettingResult(
                updatePTSettingCommand.ErrCode,
                updatePTSettingCommand.ErrMsg,
                updatePTSettingCommand.MsgType,
                command.Setting.CustomerId,
                settingType.Id,
                settingType.Name);

            return TranslateResultMessage(
                settingResult, 
                Resources.UpdateAccountDetailSuccess, 
                settingType.SettingLevel, 
                settingContext.Language);
        }

        private async Task<SettingResult> UpdateCustomerInfo(
            UpdateCustomerSettingCommand command,
            AccountDetailSettingItem settingItem,
            string language)
        {
            var updateCustomerInfoCommand = new UpdateCustomerInfoCommand
            {
                RoleId = command.Setting.CustomerLevel.Value,
                Credit = settingItem.Credit,
                CustId = command.Setting.CustomerId,
                Fax = settingItem.Fax,
                FirstName = settingItem.FirstName,
                LastName = settingItem.LastName,
                MobilePhone = settingItem.MobilePhone,
                Phone = settingItem.Phone,
                Language = LanguageNameConverter.GetLanguageKey(language)
            };

            await DynamicRepository.ExecuteAsync(updateCustomerInfoCommand);

            return updateCustomerInfoCommand.ErrCode > 0
                ? new SettingResult(Result.CreateErrorResult(updateCustomerInfoCommand.ErrMsg))
                : new SettingResult(Result.CreateSuccessResult(updateCustomerInfoCommand.ErrMsg));
        }
    }
}