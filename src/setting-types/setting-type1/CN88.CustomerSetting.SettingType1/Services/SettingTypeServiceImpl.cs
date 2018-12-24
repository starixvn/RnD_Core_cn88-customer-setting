namespace CN88.CustomerSetting.SettingType1.Services
{
    using System;
    using System.Collections;
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

        public SettingTypeServiceImpl() :
            this(null, null, null, null)
        { }

        internal SettingTypeServiceImpl(
            ResourceService resourceService,
            ILogger logger,
            ICacheService cacheService,
            IDynamicRepository dynamicRepository)
            : base(resourceService, logger, cacheService, dynamicRepository)
        {
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
            var currencies = query.CustomerLevel.IsSuperLevel() 
                ? GetCurrencies()
                : Enumerable.Empty<Currency>();
            var createCustomerInfo = GetCreateCustomerInfo(query.CustomerLevel, query.CustomerId);
            var settingItem = GetDefaultValueForInsertCustomer(query.CustomerLevel, query.CustomerId, query.ProductId);

            settingItem.Currency = currencies.FirstOrDefault();
            settingItem.AvailableCredit = createCustomerInfo.AvailableCredit;
            settingItem.UplineUsername = createCustomerInfo.UplineUsername;
            settingItem.UsernameSuffix = createCustomerInfo.NewUsername?.ToCharArray().Select(c => c.ToString());
            settingItem.UGroup = query.CustomerLevel.IsMemberLevel() ? "CNA" : null;
            settingItem.UGroups = query.CustomerLevel.IsMemberLevel() ? new List<string> { "CNA", "CNB", "CNC", "CND" } : null;
            settingItem.Currencies = currencies;

            var settingType = new SettingType
            {
                SettingItems = new List<ISettingItem> { settingItem }
            };

            return settingType;
        }

        private AccountDetail GetDefaultValueForInsertCustomer(
            Level level, int custId, int productId)
        {
            if(level.IsMemberLevel())
            {
                return default(AccountDetail);
            }

            AccountDetail accountDetail;

            switch(level.Value)
            {
                case Level.SuperLevelId:
                    accountDetail = GetDefaultSuperAccountDetail(custId, true);
                    break;

                case Level.MasterLevelId:
                    accountDetail = GetDefaultMasterAccountDetail(custId, true);
                    break;

                default:
                    accountDetail = GetDefaultAgentAccountDetail(custId, true);
                    break;
            }

            var settingTypeConfig = GetSettingTypeConfig<AccountDetail>(productId, SettingTypeId).FirstOrDefault();
            if(settingTypeConfig != null)
            {
                accountDetail.IntervalValue = settingTypeConfig.IntervalValue;
            }

            return accountDetail;
        }

        private AccountDetail GetDefaultSuperAccountDetail(int custId, bool isCreateNew)
            => isCreateNew
                ? DynamicRepository.Get<SuperAccountDetail>(new GetDefaultPTSettingCriteria(Level.Super.Value, custId))
                : null;

        private AccountDetail GetDefaultMasterAccountDetail(int custId, bool isCreateNew)
            => isCreateNew
                ? DynamicRepository.Get<MasterAccountDetail>(new GetDefaultPTSettingCriteria(Level.Master.Value, custId))
                : null;

        private AccountDetail GetDefaultAgentAccountDetail(int custId, bool isCreateNew)
            => isCreateNew
                ? DynamicRepository.Get<AgentAccountDetail>(new GetDefaultPTSettingCriteria(Level.Agent.Value, custId))
                : null;

        private CreateCustomerInfoDto GetCreateCustomerInfo(Level level, int custId)
            => level.IsNotSuperLevel()
                ? DynamicRepository.Get<CreateCustomerInfoDto>(new GetInsertCustomerInfoCriteria(level.Value, custId))
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
                        MaxPT = 90,
                        SuperMaxPT = isCreateNew ? 0 : 80,
                        SuperMinPT = isCreateNew ? 0 : 20,
                    };

                    break;

                case Level.MasterLevelId:
                    ptItem = new MasterAccountDetail
                    {
                        MaxPT = 80,
                        SuperPT = isCreateNew ? 0 : 10,
                    };

                    break;

                case Level.AgentLevelId:
                    ptItem = new AgentAccountDetail
                    {
                        MaxPT = 70,
                        MinPT = 10,
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

        public override IDictionary<string, object> GetInsertCustomerParams(
            ISettingContext settingContext,
            CreateCustomerCommand command,
            SettingType settingType)
        {
            var settingItem = settingType.SettingItems.FirstOrDefault() as AccountDetail
                ?? throw new InvalidOperationException("First Setting Item cannot be null");

            return new Dictionary<string, object>
            {
                { "username", settingItem.Username ?? throw new InvalidOperationException("Username cannot be null") },
                { "userpwd", settingItem.Password ?? "56E8A2D78170E87C928E480797CC73" },
                { "firstname", settingItem.FirstName ?? string.Empty },
                { "lastname", settingItem.LastName ?? string.Empty},
                { "phone", settingItem.Phone ?? string.Empty},
                { "mobilephone", settingItem.MobilePhone ?? string.Empty},
                { "fax", settingItem.Fax?? string.Empty },
                { "currency", settingItem.Currency?.Id },
                { "credit", settingItem.Credit },
                { "site", "CN88" },
                { "systemid", settingContext.System },
                { "ip", settingContext.IP },
                { "language", settingContext.Language },
                { "creator", settingContext.Actor.Id },
                { "ptsetting", settingItem.ToXml() }
            };
        }

        public override SettingResult UpdateCustomerSettingType(
            ISettingContext settingContext,
            int customerId,
            Level customerLevel,
            Level uplineLevel,
            byte productId,
            SettingType settingType)
        {
            throw new NotImplementedException();
        }
    }
}