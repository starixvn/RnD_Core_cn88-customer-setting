namespace CN88.CustomerSetting.SettingType1.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        private const int SettingTypeId = 1;
        private const int CurrencyCacheTimeInMinutes = 5;
        private const string CurrencyCacheKey = "CN88.CustomerSetting.CurrencyCacheKey";
        private static readonly List<string> defaultGroups = new List<string> { "cna", "cnb", "cnc", "cnd" };
        private readonly PasswordService passwordService;

        public SettingTypeServiceImpl() :
            this(null, null, null, null)
        { }

        internal SettingTypeServiceImpl(
            ResourceService resourceService,
            ILogger logger,
            ICacheService cacheService,
            IDynamicRepository dynamicRepository,
            PasswordService passwordService = null)
            : base(resourceService, logger, cacheService, dynamicRepository)
        {
            this.passwordService = passwordService ?? new PasswordServiceImpl();
        }

        public override SettingType GetCustomerSettingType(
            ISettingContext settingContext,
            CustomerSettingQuery query)
        {
            if (query.IsCreateNew)
            {
                return GetDefaultSettingForCreateNew(query);
            }

            return GetAccountDetail(query);
        }

        private SettingType GetAccountDetail(CustomerSettingQuery query)
        {
            var settingItem = GetAccountDetailSettingItem(query.CustomerLevel, query.CustomerId, query.ProductId, false);

            var customerInfo = DynamicRepository.Get<CustomerInfoDto>(new GetGeneralInfoCriteria(query.CustomerLevel.Value, query.CustomerId));

            if(customerInfo == null)
            {
                throw new InvalidOperationException($"Not found customer id {query.CustomerId} level {query.CustomerLevel.DisplayName}");
            }

            settingItem.Username = customerInfo.Username;
            settingItem.FirstName = customerInfo.FirstName;
            settingItem.LastName = customerInfo.LastName;
            settingItem.Phone = customerInfo.Phone;
            settingItem.MobilePhone = customerInfo.MobilePhone;
            settingItem.Fax = customerInfo.Fax;
            settingItem.Currency = new Currency { Name = customerInfo.Currency };
            settingItem.Credit = customerInfo.Credit;
            settingItem.AvailableCredit = customerInfo.AvailableCredit;

            return CreateSettingType(settingItem);
        }

        private SettingType GetDefaultSettingForCreateNew(CustomerSettingQuery query)
        {
            var settingItem = GetAccountDetailSettingItem(query.CustomerLevel, query.UplineId, query.ProductId, true);
            var createCustomerInfo = GetDefaultCreateCustomerInfo(query.CustomerLevel, query.UplineId);

            settingItem.AvailableCredit = createCustomerInfo.AvailableCredit;
            settingItem.UplineUsername = createCustomerInfo.UplineUsername;
            settingItem.UsernameSuffixes = createCustomerInfo.NewUsername?.ToCharArray().Select(c => c.ToString());
            settingItem.Currencies = query.CustomerLevel.IsSuperLevel()
                                ? GetCurrencies()
                                : Enumerable.Empty<Currency>();

            if (query.CustomerLevel.IsMemberLevel())
            {
                settingItem.UGroup = defaultGroups.FirstOrDefault();
                settingItem.UGroups = defaultGroups;
            }

            return CreateSettingType(settingItem);
        }

        private SettingType CreateSettingType(ISettingItem settingItem)
            => new AccountDetailSettingType
            {
                SettingItems = new List<ISettingItem> { settingItem }
            };

        private AccountDetailSettingItem GetAccountDetailSettingItem(
            Level level, int custId, int productId, bool isCreateNew)
        {
            if (level.IsMemberLevel())
            {
                return new MemberAccountDetail();
            }

            var settingItem = GetAccountDetailSettingItem(level, custId, isCreateNew);
            var settingTypeConfig = GetSettingTypeConfig<AccountDetailSettingItem>(productId, SettingTypeId).FirstOrDefault();
            if (settingTypeConfig != null)
            {
                settingItem.IntervalValue = settingTypeConfig.IntervalValue;
            }

            return settingItem;
        }

        private AccountDetailSettingItem GetAccountDetailSettingItem(
            Level level,
            int custId,
            bool isCreateNew)
        {
            AccountDetailSettingItem settingItem;

            switch (level.Value)
            {
                case Level.SuperLevelId:
                    settingItem = GetSuperAccountDetail(custId, isCreateNew);
                    break;

                case Level.MasterLevelId:
                    settingItem = GetMasterAccountDetail(custId, isCreateNew);
                    break;

                default:
                    settingItem = GetAgentAccountDetail(custId, isCreateNew);
                    break;
            }

            if (settingItem == null)
            {
                throw new InvalidOperationException($"Cannot get default setting for upline {custId} to create new downline");
            }

            return settingItem;
        }

        private AccountDetailSettingItem GetSuperAccountDetail(int custId, bool isCreateNew)
            => isCreateNew
                ? DynamicRepository.Get<SuperAccountDetail>(new GetDefaultPTSettingCriteria(Level.Super.Value, custId))
                : DynamicRepository.Get<SuperAccountDetail>(new GetPTSettingCriteria(Level.Super.Value, custId));

        private AccountDetailSettingItem GetMasterAccountDetail(int custId, bool isCreateNew)
            => isCreateNew
                ? DynamicRepository.Get<MasterAccountDetail>(new GetDefaultPTSettingCriteria(Level.Master.Value, custId))
                : DynamicRepository.Get<MasterAccountDetail>(new GetPTSettingCriteria(Level.Master.Value, custId));

        private AccountDetailSettingItem GetAgentAccountDetail(int custId, bool isCreateNew)
            => isCreateNew
                ? DynamicRepository.Get<AgentAccountDetail>(new GetDefaultPTSettingCriteria(Level.Agent.Value, custId))
                : DynamicRepository.Get<AgentAccountDetail>(new GetPTSettingCriteria(Level.Agent.Value, custId));

        private CreateCustomerInfoDto GetDefaultCreateCustomerInfo(Level level, int uplineId)
            => level.IsNotSuperLevel()
                ? DynamicRepository.Get<CreateCustomerInfoDto>(new GetInsertCustomerInfoCriteria(level.Value, uplineId))
                : new CreateCustomerInfoDto();

        private IEnumerable<Currency> GetCurrencies()
            => CacheService.GetOrSet(
                       CurrencyCacheKey,
                       () => DynamicRepository.Fetch<Currency>(new GetCurrencyListCriteria()),
                        new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(0, CurrencyCacheTimeInMinutes, 0)));

        public override IDictionary<string, object> BuildCreateCustomerParams(
            ISettingContext settingContext,
            CreateCustomerCommand command,
            SettingType settingType)
        {
            var settingItem = settingType.SettingItems.FirstOrDefault() as AccountDetailSettingItem
                ?? throw new InvalidOperationException("AccountDetail Item cannot be null");

            settingItem.Password = ValidateAndEncodePassword(command, settingItem);

            var createNewParams = BuildCreateNewParams(settingContext, settingItem);

            switch (command.Setting.CustomerLevel.Value)
            {
                case Level.SuperLevelId:
                    return AddParamsForAddNewSuper(settingItem, createNewParams);

                case Level.MasterLevelId:
                    return AddParamsForAddNewMaster(settingContext, command, settingItem, createNewParams);

                case Level.AgentLevelId:
                    return AddParamsForAddNewAgent(settingContext, command, settingItem, createNewParams);

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
                { "language", settingContext.Language },
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
            createNewParams.Add("ugroup", settingItem.UGroup);

            return createNewParams;
        }

        private static Dictionary<string, object> AddParamsForAddNewAgent(
            ISettingContext settingContext,
            CreateCustomerCommand command,
            AccountDetailSettingItem settingItem,
            Dictionary<string, object> createNewParams)
        {
            createNewParams.Add("subaccid", settingContext.Actor.SubAccountId);
            createNewParams.Add("mrecommend", command.UplineId);
            createNewParams.Add("ptsetting", settingItem.ToXml());

            return createNewParams;
        }

        private static Dictionary<string, object> AddParamsForAddNewMaster(
            ISettingContext settingContext,
            CreateCustomerCommand command,
            AccountDetailSettingItem settingItem,
            Dictionary<string, object> createNewParams)
        {
            createNewParams.Add("subaccid", settingContext.Actor.SubAccountId);
            createNewParams.Add("srecommend", command.UplineId);
            createNewParams.Add("ptsetting", settingItem.ToXml());

            return createNewParams;
        }

        private static Dictionary<string, object> AddParamsForAddNewSuper(
            AccountDetailSettingItem settingItem,
            Dictionary<string, object> createNewParams)
        {
            createNewParams.Add("site", "CN88");
            createNewParams.Add("currency", settingItem.Currency?.Id);
            createNewParams.Add("lastname", settingItem.LastName ?? string.Empty);
            createNewParams.Add("phone", settingItem.Phone ?? string.Empty);
            createNewParams.Add("mobilephone", settingItem.MobilePhone ?? string.Empty);
            createNewParams.Add("fax", settingItem.Fax ?? string.Empty);
            createNewParams.Add("ptsetting", settingItem.ToXml());

            return createNewParams;
        }

        public override SettingResult UpdateCustomerSettingType(
            ISettingContext settingContext,
            UpdateCustomerSettingCommand command,
            SettingType settingType)
        {
            var settingItem = settingType.SettingItems.FirstOrDefault() as AccountDetailSettingItem
                ?? throw new InvalidOperationException("AccountDetail cannot be null");

            var updateCustomerInfoResult = UpdateCustomerInfo(command, settingItem);
            if (updateCustomerInfoResult.IsFailed)
            {
                return updateCustomerInfoResult;
            }

            return UpdatePTSetting(settingContext, command, settingType);
        }

        private SettingResult UpdatePTSetting(
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
                Language = settingContext.Language,
                SubAccountId = settingContext.Actor.SubAccountId,
            };

            DynamicRepository.Execute(updatePTSettingCommand);

            return new SettingResult(
                updatePTSettingCommand.ErrCode,
                updatePTSettingCommand.ErrCode == Result.Success.Code
                    ? Resources.UpdateAccountDetailSuccess
                    : updatePTSettingCommand.ErrMsg,
                updatePTSettingCommand.MsgType,
                command.Setting.CustomerId,
                settingType.Id,
                settingType.Name);
        }

        private SettingResult UpdateCustomerInfo(
            UpdateCustomerSettingCommand command,
            AccountDetailSettingItem settingItem)
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
                Phone = settingItem.Phone
            };

            DynamicRepository.Execute(updateCustomerInfoCommand);

            return updateCustomerInfoCommand.ErrCode > 0
                ? new SettingResult(Result.CreateErrorResult(updateCustomerInfoCommand.ErrMsg))
                : new SettingResult(Result.CreateSuccessResult(updateCustomerInfoCommand.ErrMsg));
        }
    }
}