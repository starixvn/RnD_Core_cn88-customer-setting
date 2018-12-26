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
    using CN88.CustomerSetting.SettingType1.Models;
    using CN88.CustomerSetting.SettingType1.Models.Dtos;
    using CN88.CustomerSetting.SettingType1.Params.Criteria;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Fanex.Logging;

    public class SettingTypeServiceImpl : SettingTypeServiceBase, SettingTypeService
    {
        private const int SettingTypeId = 1;
        private const int CurrencyCacheTimeInMinutes = 5;
        private const string CurrencyCacheKey = "CN88.CustomerSetting.CurrencyCacheKey";
        private static readonly List<string> defaultGroups = new List<string> { "CNA", "CNB", "CNC", "CND" };
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
                return GetCreateNewAccountDetail(query);
            }

            return StubData(query);
        }

        private SettingType GetCreateNewAccountDetail(CustomerSettingQuery query)
        {
            var settingItem = GetDefaultValueForInsertCustomer(query.CustomerLevel, query.UplineId, query.ProductId);
            var createCustomerInfo = GetCreateCustomerInfo(query.CustomerLevel, query.UplineId);

            settingItem.AvailableCredit = createCustomerInfo.AvailableCredit;
            settingItem.UplineUsername = createCustomerInfo.UplineUsername;
            settingItem.UsernameSuffixes = createCustomerInfo.NewUsername?.ToCharArray().Select(c => c.ToString());

            if (query.CustomerLevel.IsMemberLevel())
            {
                settingItem.UGroups = defaultGroups;
            }

            if (query.CustomerLevel.IsSuperLevel())
            {
                var currencies = query.CustomerLevel.IsSuperLevel()
                                ? GetCurrencies()
                                : Enumerable.Empty<Currency>();
                settingItem.Currency = currencies.FirstOrDefault();
                settingItem.Currencies = currencies;
            }

            return new SettingType
            {
                SettingItems = new List<ISettingItem> { settingItem }
            };
        }

        private AccountDetail GetDefaultValueForInsertCustomer(
            Level level, int uplineId, int productId)
        {
            if (level.IsMemberLevel())
            {
                return new AccountDetail();
            }

            AccountDetail accountDetail;

            switch (level.Value)
            {
                case Level.SuperLevelId:
                    accountDetail = GetSuperAccountDetail(uplineId, true);
                    break;

                case Level.MasterLevelId:
                    accountDetail = GetMasterAccountDetail(uplineId, true);
                    break;

                default:
                    accountDetail = GetAgentAccountDetail(uplineId, true);
                    break;
            }

            if (accountDetail == null)
            {
                throw new InvalidOperationException($"Cannot get default setting for upline {uplineId} to create new downline");
            }

            var settingTypeConfig = GetSettingTypeConfig<AccountDetail>(productId, SettingTypeId).FirstOrDefault();
            if (settingTypeConfig != null)
            {
                accountDetail.IntervalValue = settingTypeConfig.IntervalValue;
            }

            return accountDetail;
        }

        private AccountDetail GetSuperAccountDetail(int custId, bool isCreateNew)
            => isCreateNew
                ? DynamicRepository.Get<SuperAccountDetail>(new GetDefaultPTSettingCriteria(Level.Super.Value, custId))
                : DynamicRepository.Get<SuperAccountDetail>(new GetPTSettingCriteria(Level.Super.Value, custId));

        private AccountDetail GetMasterAccountDetail(int custId, bool isCreateNew)
            => isCreateNew
                ? DynamicRepository.Get<MasterAccountDetail>(new GetDefaultPTSettingCriteria(Level.Master.Value, custId))
                : DynamicRepository.Get<MasterAccountDetail>(new GetPTSettingCriteria(Level.Master.Value, custId));

        private AccountDetail GetAgentAccountDetail(int custId, bool isCreateNew)
            => isCreateNew
                ? DynamicRepository.Get<AgentAccountDetail>(new GetDefaultPTSettingCriteria(Level.Agent.Value, custId))
                : DynamicRepository.Get<AgentAccountDetail>(new GetPTSettingCriteria(Level.Agent.Value, custId));

        private CreateCustomerInfoDto GetCreateCustomerInfo(Level level, int uplineId)
            => level.IsNotSuperLevel()
                ? DynamicRepository.Get<CreateCustomerInfoDto>(new GetInsertCustomerInfoCriteria(level.Value, uplineId))
                : new CreateCustomerInfoDto();

        private IEnumerable<Currency> GetCurrencies()
            => CacheService.GetOrSet(
                       CurrencyCacheKey,
                       () => DynamicRepository.Fetch<Currency>(new GetCurrencyListCriteria()),
                        new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(0, CurrencyCacheTimeInMinutes, 0)));

        private static SettingType StubData(CustomerSettingQuery query)
        {
            AccountDetail ptItem = null;
            var isCreateNew = query.IsCreateNew;

            switch (query.CustomerLevel.Value)
            {
                case Level.SuperLevelId:
                    ptItem = new SuperAccountDetail
                    {
                        MinSuperMinPT = 0,
                        MaxSuperMinPT = 80,
                        MinSuperMaxPT = 80,
                        MaxSuperMaxPT = 90,
                        SuperMaxPT = isCreateNew ? 0 : 80,
                        SuperMinPT = isCreateNew ? 0 : 20,
                    };

                    break;

                case Level.MasterLevelId:
                    ptItem = new MasterAccountDetail
                    {
                        MinSuperPT = 0,
                        MaxSuperPT = 80,
                        SuperPT = isCreateNew ? 0 : 10,
                    };

                    break;

                case Level.AgentLevelId:
                    ptItem = new AgentAccountDetail
                    {
                        MinAllowedPT = 70,
                        MaxAllowedPT = 10,
                        MinAgentPT = 0,
                        MaxAgentPT = 70,
                        MinMasterPT = 0,
                        MaxMasterPT = 70,
                        MasterPT = isCreateNew ? 0 : 40,
                        AgentPT = isCreateNew ? 0 : 20,
                    };

                    break;
            }

            //Username = isCreateNew ? string.Empty : "AccountTest",
            //        Currency = new Currency
            //        {
            //            Id = 1,
            //            Name = "UUS"
            //        },
            //        Credit = isCreateNew ? 0 : 100,
            //        AvailableCredit = 1000,
            //        UGroup = query.CustomerLevel.IsMemberLevel() ? "CNA" : null,
            //        UGroups = query.CustomerLevel.IsMemberLevel() ? new List<string> { "CNA", "CNB", "CNC", "CND" } : null,
            //        UsernameSuffix = query.CustomerLevel.IsSuperLevel() || !isCreateNew
            //                    ? Enumerable.Empty<string>()
            //                    : query.CustomerLevel.IsMemberLevel()
            //                        ? new List<string> { "2", "B", "E" }
            //                        : new List<string> { "4", "C" },

            return new SettingType
            {
                SettingItems = new List<ISettingItem> { ptItem }
            };
        }

        public override IDictionary<string, object> GetCreateCustomerParams(
            ISettingContext settingContext,
            CreateCustomerCommand command,
            SettingType settingType)
        {
            var settingItem = settingType.SettingItems.FirstOrDefault() as AccountDetail
                ?? throw new InvalidOperationException("AccountDetail Item cannot be null");

            var passwordResult = passwordService.ValidateAndHashPassword(
                settingItem.Password,
                command.HashKey,
                command.Setting.CustomerLevel.IsMemberLevel());

            if (passwordResult.IsNotOk)
            {
                throw new InvalidOperationException(passwordResult.Message);
            }

            settingItem.Password = passwordResult.Message;

            var createNewParams = CreateAddNewParams(settingContext, settingItem);

            switch (command.Setting.CustomerLevel.Value)
            {
                case Level.SuperLevelId:
                    AddParamsForAddNewSuper(settingItem, createNewParams);
                    break;

                case Level.MasterLevelId:
                    AddParamsForAddNewMaster(settingContext, command, settingItem, createNewParams);
                    break;

                case Level.AgentLevelId:
                    AddParamsForAddNewAgent(settingContext, command, settingItem, createNewParams);
                    break;

                case Level.MemberLevelId:
                    AddParamsForAddNewMember(settingContext, command, settingItem, createNewParams);
                    break;
            }

            return createNewParams;
        }

        private static Dictionary<string, object> CreateAddNewParams(ISettingContext settingContext, AccountDetail settingItem)
        {
            return new Dictionary<string, object>
            {
                { "username", settingItem.Username ?? throw new InvalidOperationException("Username cannot be null") },
                { "userpwd", settingItem.Password ?? "56E8A2D78170E87C928E480797CC73" },
                { "firstname", settingItem.FirstName ?? string.Empty },
                { "credit", settingItem.Credit },
                { "systemid", settingContext.System },
                { "ip", settingContext.IP },
                { "language", settingContext.Language },
                { "creator", settingContext.Actor.Id }
            };
        }

        private static void AddParamsForAddNewMember(ISettingContext settingContext, CreateCustomerCommand command, AccountDetail settingItem, Dictionary<string, object> createNewParams)
        {
            createNewParams.Add("subaccid", settingContext.Actor.SubAccountId);
            createNewParams.Add("recommend", command.UplineId);
            createNewParams.Add("ugroup", settingItem.UGroup);
        }

        private static void AddParamsForAddNewAgent(ISettingContext settingContext, CreateCustomerCommand command, AccountDetail settingItem, Dictionary<string, object> createNewParams)
        {
            createNewParams.Add("subaccid", settingContext.Actor.SubAccountId);
            createNewParams.Add("mrecommend", command.UplineId);
            createNewParams.Add("ptsetting", settingItem.ToXml());
        }

        private static void AddParamsForAddNewMaster(ISettingContext settingContext, CreateCustomerCommand command, AccountDetail settingItem, Dictionary<string, object> createNewParams)
        {
            createNewParams.Add("subaccid", settingContext.Actor.SubAccountId);
            createNewParams.Add("srecommend", command.UplineId);
            createNewParams.Add("ptsetting", settingItem.ToXml());
        }

        private static void AddParamsForAddNewSuper(AccountDetail settingItem, Dictionary<string, object> createNewParams)
        {
            createNewParams.Add("site", "CN88");
            createNewParams.Add("currency", settingItem.Currency?.Id);
            createNewParams.Add("lastname", settingItem.LastName ?? string.Empty);
            createNewParams.Add("phone", settingItem.Phone ?? string.Empty);
            createNewParams.Add("mobilephone", settingItem.MobilePhone ?? string.Empty);
            createNewParams.Add("fax", settingItem.Fax ?? string.Empty);
            createNewParams.Add("ptsetting", settingItem.ToXml());
        }

        public override SettingResult UpdateCustomerSettingType(
            ISettingContext settingContext,
            int customerId,
            Level customerLevel,
            byte productId,
            SettingType settingType)
        {
            var settingItem = settingType.SettingItems.FirstOrDefault() as AccountDetail
                ?? throw new InvalidOperationException("AccountDetail cannot be null");

            var createNewParams = new Dictionary<string, object>
            {
                { "username", settingItem.Username ?? throw new InvalidOperationException("Username cannot be null") },
                { "userpwd", settingItem.Password ?? "56E8A2D78170E87C928E480797CC73" },
                { "firstname", settingItem.FirstName ?? string.Empty },
                { "credit", settingItem.Credit },
                { "systemid", settingContext.System },
                { "ip", settingContext.IP },
                { "language", settingContext.Language },
                { "creator", settingContext.Actor.Id }
            };

            switch (customerLevel.Value)
            {
                case Level.SuperLevelId:
                    createNewParams.Add("site", "CN88");
                    createNewParams.Add("currency", settingItem.Currency?.Id);
                    createNewParams.Add("lastname", settingItem.LastName ?? string.Empty);
                    createNewParams.Add("phone", settingItem.Phone ?? string.Empty);
                    createNewParams.Add("mobilephone", settingItem.MobilePhone ?? string.Empty);
                    createNewParams.Add("fax", settingItem.Fax ?? string.Empty);
                    createNewParams.Add("ptsetting", settingItem.ToXml());
                    break;

                case Level.MasterLevelId:
                    createNewParams.Add("subaccid", settingContext.Actor.SubAccountId);
                    createNewParams.Add("ptsetting", settingItem.ToXml());
                    break;

                case Level.AgentLevelId:
                    createNewParams.Add("subaccid", settingContext.Actor.SubAccountId);
                    createNewParams.Add("ptsetting", settingItem.ToXml());
                    break;

                case Level.MemberLevelId:
                    createNewParams.Add("subaccid", settingContext.Actor.SubAccountId);
                    createNewParams.Add("ugroup", settingItem.UGroup);
                    break;
            }

            throw new NotImplementedException();
        }
    }
}