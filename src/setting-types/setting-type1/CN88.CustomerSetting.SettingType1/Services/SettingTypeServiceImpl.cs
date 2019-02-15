namespace CN88.CustomerSetting.SettingType1.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Constants;
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
            => query.IsCreateNew
                ? await GetAccountDetailForCreateNew(query).ConfigureAwait(false)
                : await GetAccountDetailForEdit(query).ConfigureAwait(false);

        private async Task<SettingType> GetAccountDetailForEdit(CustomerSettingQuery query)
        {
            var settingItem = await GetAccountDetailSettingItem(query.CustomerLevel, query.CustomerId, query.ProductId, false).ConfigureAwait(false);
            var customerInfo = (await DynamicRepository.GetAsync<CustomerInfoDto>(new GetGeneralInfoCriteria(query.CustomerLevel.Value, query.CustomerId)).ConfigureAwait(false))
                ?? throw new InvalidOperationException($"Not found customer id {query.CustomerId} level {query.CustomerLevel.DisplayName}");

            settingItem.Username = customerInfo.Username;
            settingItem.FirstName = DecodeHtml(customerInfo.FirstName);
            settingItem.LastName = DecodeHtml(customerInfo.LastName);
            settingItem.Phone = DecodeHtml(customerInfo.Phone);
            settingItem.MobilePhone = DecodeHtml(customerInfo.MobilePhone);
            settingItem.Fax = DecodeHtml(customerInfo.Fax);
            settingItem.Currency = new Currency { Name = customerInfo.Currency };
            settingItem.Credit = customerInfo.Credit;
            settingItem.MaxCredit = customerInfo.MaxCredit;
            settingItem.MinCredit = customerInfo.MinCredit;
            settingItem.UplineUsername = DecodeHtml(customerInfo.MainShareholderUsername);

            if (query.CustomerLevel.IsMemberLevel()
                && !string.IsNullOrEmpty(customerInfo.UGroup))
            {
                settingItem.SpreadGroup = (await GetSpreadGroups().ConfigureAwait(false))
                    .FirstOrDefault(group =>
#pragma warning disable S1449 // Culture should be specified for "string" operations
                        string.Compare(customerInfo.UGroup, group.UGroup, true) == 0);
#pragma warning restore S1449 // Culture should be specified for "string" operations
            }

            return CreateSettingType(settingItem);
        }

        private async Task<SettingType> GetAccountDetailForCreateNew(CustomerSettingQuery query)
        {
            var settingItem = await GetAccountDetailSettingItem(query.CustomerLevel, query.UplineId, query.ProductId, true).ConfigureAwait(false);
            var createCustomerInfo = await GetDefaultCreateCustomerInfo(query.CustomerLevel, query.UplineId).ConfigureAwait(false);

            settingItem.MaxCredit = createCustomerInfo.MaxCredit;
            settingItem.MinCredit = createCustomerInfo.MinCredit;
            settingItem.UplineUsername = createCustomerInfo.UplineUsername;
            settingItem.UsernameSuffixes = createCustomerInfo.NewUsername?.ToCharArray().Select(c => c.ToString());
            settingItem.Currencies = query.CustomerLevel.IsSuperLevel()
                                ? await GetCurrencies().ConfigureAwait(false)
                                : Enumerable.Empty<Currency>();

            if (query.CustomerLevel.IsMemberLevel())
            {
                settingItem.SpreadGroups = await GetSpreadGroups().ConfigureAwait(false);
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

            var settingItem = await GetAccountDetailSettingItem(level, custId, isCreateNew).ConfigureAwait(false);
            var settingTypeConfig = (await GetSettingTypeConfig<AccountDetailSettingItem>(productId, SettingTypeId).ConfigureAwait(false)).FirstOrDefault();
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
                    settingItem = await GetSuperAccountDetail(custId, isCreateNew).ConfigureAwait(false);
                    break;

                case Level.MasterLevelId:
                    settingItem = await GetMasterAccountDetail(custId, isCreateNew).ConfigureAwait(false);
                    break;

                default:
                    settingItem = await GetAgentAccountDetail(custId, isCreateNew).ConfigureAwait(false);
                    break;
            }

            if(settingItem == null)
            {
                throw new InvalidOperationException($"Cannot get default setting to create new downline");
            }

            if (!isCreateNew)
            {
                var editPTTime = await GetEditPTTime().ConfigureAwait(false);
                settingItem.IsPTReadOnly = !editPTTime.IsEditablePT;
                settingItem.PTEffectiveTime = editPTTime.EffectiveDate;
            }

            settingItem.AdjustPTForQuery(isCreateNew);

            return settingItem;
        }

        private async Task<AccountDetailSettingItem> GetSuperAccountDetail(int custId, bool isCreateNew)
            => await DynamicRepository.GetAsync<SuperAccountDetail>(new GetPTSettingCriteria(Level.Super.Value, custId, isCreateNew)).ConfigureAwait(false);

        private async Task<AccountDetailSettingItem> GetMasterAccountDetail(int custId, bool isCreateNew)
            => await DynamicRepository.GetAsync<MasterAccountDetail>(new GetPTSettingCriteria(Level.Master.Value, custId, isCreateNew)).ConfigureAwait(false);

        private async Task<AccountDetailSettingItem> GetAgentAccountDetail(int custId, bool isCreateNew)
            => await DynamicRepository.GetAsync<AgentAccountDetail>(new GetPTSettingCriteria(Level.Agent.Value, custId, isCreateNew)).ConfigureAwait(false);

        private async Task<CreateCustomerInfoDto> GetDefaultCreateCustomerInfo(Level level, int uplineId)
            => level.IsNotSuperLevel()
                ? await DynamicRepository.GetAsync<CreateCustomerInfoDto>(new GetCustomerInfoForCreateCriteria(level.Value, uplineId)).ConfigureAwait(false)
                : new CreateCustomerInfoDto();

        private async Task<IEnumerable<Currency>> GetCurrencies()
            => await CacheService.GetOrSetAsync(
                       CurrencyCacheKey,
                       () => DynamicRepository.Fetch<Currency>(new GetCurrencyListCriteria()),
                        new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(0, CurrencyCacheTimeInMinutes, 0))).ConfigureAwait(false);

        private async Task<IEnumerable<SpreadGroup>> GetSpreadGroups()
            => await CacheService.GetOrSetAsync(
                       SpreadGroupCacheKey,
                       () => DynamicRepository.Fetch<SpreadGroup>(new GetSpreadGroupCriteria()),
                        new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(SpreadGroupCacheTimeInHours, 0, 0))).ConfigureAwait(false);

        public override IDictionary<string, object> BuildCreateCustomerParams(
            ISettingContext settingContext,
            CreateCustomerCommand command,
            SettingType settingType)
        {
#pragma warning disable S3215 // "interface" instances should not be cast to concrete types
            var settingItem = settingType.SettingItems?.FirstOrDefault() as AccountDetailSettingItem
#pragma warning restore S3215 // "interface" instances should not be cast to concrete types
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
                { "username", settingItem.Username ?? throw new InvalidOperationException(Resources.UsernameCannotBeNullOrEmpty) },
                { "userpwd", settingItem.Password ?? throw new InvalidOperationException(Resources.PasswordCannotBeNullOrEmpty) },
                { "firstname", EncodeHtml(settingItem.FirstName) },
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
            createNewParams.Add("lastname", EncodeHtml(settingItem.LastName));
            createNewParams.Add("phone", EncodeHtml(settingItem.Phone));
            createNewParams.Add("mobilephone", EncodeHtml(settingItem.MobilePhone));
            createNewParams.Add("fax", EncodeHtml(settingItem.Fax));
            createNewParams.Add("mainshareholderusername",
                EncodeHtml(
                    settingItem.UplineUsername
                    ?? throw new InvalidOperationException(Resources.MainShareholderUsernameCannotBeNullOrEmpty)));

            return createNewParams;
        }

        public override async Task<SettingResult> UpdateCustomerSettingType(
            ISettingContext settingContext,
            UpdateCustomerSettingCommand command,
            SettingType settingType)
        {
#pragma warning disable S3215 // "interface" instances should not be cast to concrete types
            var settingItem = settingType.SettingItems.FirstOrDefault() as AccountDetailSettingItem
#pragma warning restore S3215 // "interface" instances should not be cast to concrete types
                ?? throw new InvalidOperationException("AccountDetail cannot be null");

            var updateCustomerInfoResult = await UpdateCustomerInfo(command, settingItem, settingType, settingContext.Language).ConfigureAwait(false);

            if (updateCustomerInfoResult.IsFailed
                || settingItem.IsNoChangePT())
            {
                return updateCustomerInfoResult;
            }

            return await UpdatePTSetting(settingContext, command, settingType).ConfigureAwait(false);
        }

        private async Task<SettingResult> UpdatePTSetting(
            ISettingContext settingContext,
            UpdateCustomerSettingCommand command,
            SettingType settingType)
        {
            var editPTTime = await GetEditPTTime().ConfigureAwait(false);

            if(!editPTTime.IsEditablePT)
            {
                return new SettingResult(Result.CreateErrorResult(Resources.EditPossessTimeInvalid));
            }

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

            await DynamicRepository.ExecuteAsync(updatePTSettingCommand).ConfigureAwait(false);

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
            SettingType settingType,
            string language)
        {
            var updateCommand = new UpdateCustomerInfoCommand
            {
                RoleId = command.Setting.CustomerLevel.Value,
                Credit = settingItem.Credit,
                CustId = command.Setting.CustomerId,
                Fax = EncodeHtml(settingItem.Fax),
                FirstName = EncodeHtml(settingItem.FirstName),
                LastName = EncodeHtml(settingItem.LastName),
                MobilePhone = EncodeHtml(settingItem.MobilePhone),
                Phone = EncodeHtml(settingItem.Phone),
                Language = LanguageNameConverter.GetLanguageKey(language)
            };

            await DynamicRepository
                .ExecuteAsync(updateCommand)
                .ConfigureAwait(false);

            var isSuccess = updateCommand.ErrCode == 0;

            return new SettingResult(
                updateCommand.ErrCode,
                isSuccess 
                    ? Resources.UpdateAccountDetailSuccess
                    : updateCommand.ErrMsg,
                0,
                command.Setting.CustomerId,
                settingType.Id,
                settingType.Name);
        }

        private static string EncodeHtml(string data)
            => WebUtility.HtmlEncode(data ?? string.Empty);

        private static string DecodeHtml(string data)
            => WebUtility.HtmlDecode(data);

        private async Task<EditPTTimeDto> GetEditPTTime()
            => await DynamicRepository.GetAsync<EditPTTimeDto>(new GetPTEditTimeCriteria()).ConfigureAwait(false);
    }
}