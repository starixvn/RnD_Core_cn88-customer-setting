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

    public class SettingTypeServiceImpl : SettingTypeServiceBase, SettingTypeService
    {
        public override SettingType GetCustomerSettingType(
            ISettingContext settingContext,
            CustomerSettingQuery query)
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
                    CurrencyId = 1,
                    CurrencyName = "UUS",
                    Credit = isCreateNew ? 0 : 100,
                    AvailableCredit = 1000,
                    UGroup = query.CustomerLevel.IsMemberLevel() ? "CNA" : null,
                    UGroups = query.CustomerLevel.IsMemberLevel() ? new List<string> { "CNA", "CNB", "CNC", "CND" } : null,
                    UsernameSuffixSuggestion = query.CustomerLevel.IsSuperLevel() || !isCreateNew
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