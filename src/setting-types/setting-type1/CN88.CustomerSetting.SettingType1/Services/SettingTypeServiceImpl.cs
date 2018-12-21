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

        private AccountDetail GetCreateNewAccountDetail(CustomerSettingQuery query)
        {
            var currencies = query.CustomerLevel.IsSuperLevel() 
                ? GetCurrencies()
                : Enumerable.Empty<Currency>();
            var createCustomerInfo = GetCreateCustomerInfo(query.CustomerLevel, query.CustomerId);

            var settingType = new AccountDetail
            {
                SettingItems = new List<ISettingItem>
                {
                    GetDefaultValueForInsertCustomer(query.CustomerLevel, query.CustomerId, query.ProductId)
                },
                Currencies = currencies,
                CustomerInfo = new CustomerInfo
                {
                    Currency = currencies.FirstOrDefault(),
                    AvailableCredit = createCustomerInfo.AvailableCredit,
                    UplineUsername = createCustomerInfo.UplineUsername,
                    UsernameSuffix = createCustomerInfo.NewUsername?.ToCharArray().Select(c => c.ToString())
                },
            };

            return settingType;
        }

        private PTItem GetDefaultValueForInsertCustomer(
            Level level, int custId, int productId)
        {
            if(level.IsMemberLevel())
            {
                return default(PTItem);
            }

            PTItem ptItem;

            switch(level.Value)
            {
                case Level.SuperLevelId:
                    ptItem = GetSuperPT(custId, true);
                    break;

                case Level.MasterLevelId:
                    ptItem = GetMasterPT(custId, true);
                    break;

                default:
                    ptItem = GetAgentPT(custId, true);
                    break;
            }

            var settingTypeConfig = GetSettingTypeConfig<PTItem>(productId, SettingTypeId).FirstOrDefault();
            if(settingTypeConfig != null)
            {
                ptItem.IntervalValue = settingTypeConfig.IntervalValue;
            }

            return ptItem;
        }

        private PTItem GetSuperPT(int custId, bool isCreateNew)
            => isCreateNew
                ? DynamicRepository.Get<SuperPTItem>(new GetDefaultPTSettingCriteria(Level.Super.Value, custId))
                : null;

        private PTItem GetMasterPT(int custId, bool isCreateNew)
            => isCreateNew
                ? DynamicRepository.Get<MasterPTItem>(new GetDefaultPTSettingCriteria(Level.Master.Value, custId))
                : null;

        private PTItem GetAgentPT(int custId, bool isCreateNew)
            => isCreateNew
                ? DynamicRepository.Get<AgentPTItem>(new GetDefaultPTSettingCriteria(Level.Agent.Value, custId))
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
            PTItem ptItem = null;
            var isCreateNew = query.IsCreateNew;

            switch (query.CustomerLevel.Value)
            {
                case Level.SuperLevelId:
                    ptItem = new SuperPTItem
                    {
                        MaxPT = 90,
                        SuperMaxPT = isCreateNew ? 0 : 80,
                        SuperMinPT = isCreateNew ? 0 : 20,
                    };

                    break;

                case Level.MasterLevelId:
                    ptItem = new MasterPTItem
                    {
                        MaxPT = 80,
                        SuperPT = isCreateNew ? 0 : 10,
                    };

                    break;

                case Level.AgentLevelId:
                    ptItem = new AgentPTItem
                    {
                        MaxPT = 70,
                        MinPT = 10,
                        MasterPT = isCreateNew ? 0 : 40,
                        AgentPT = isCreateNew ? 0 : 20,
                    };

                    break;
            }

            return new AccountDetail
            {
                CustomerInfo = new CustomerInfo
                {
                    Username = isCreateNew ? string.Empty : "AccountTest",
                    Currency = new Currency
                    {
                        Id = 1,
                        Name = "UUS"
                    },
                    Credit = isCreateNew ? 0 : 100,
                    AvailableCredit = 1000,
                    UGroup = query.CustomerLevel.IsMemberLevel() ? "CNA" : null,
                    UGroups = query.CustomerLevel.IsMemberLevel() ? new List<string> { "CNA", "CNB", "CNC", "CND" } : null,
                    UsernameSuffix = query.CustomerLevel.IsSuperLevel() || !isCreateNew
                                ? Enumerable.Empty<string>()
                                : query.CustomerLevel.IsMemberLevel()
                                    ? new List<string> { "2", "B", "E" }
                                    : new List<string> { "4", "C" }
                },
                PTItem = ptItem
            };
        }

        public override Hashtable GetInsertCustomerParams(
            ISettingContext settingContext,
            int customerId,
            Level customerLevel,
            Level uplineLevel,
            byte productId,
            SettingType settingType)
        {
            throw new NotImplementedException();
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