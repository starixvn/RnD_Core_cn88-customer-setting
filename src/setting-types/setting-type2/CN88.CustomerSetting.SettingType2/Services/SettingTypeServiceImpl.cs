namespace CN88.CustomerSetting.SettingType2.Services
{
    using System;
    using System.Collections.Generic;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Enumerations;
    using CN88.CustomerSetting.Models.Requests;
    using CN88.CustomerSetting.Models.Results;
    using CN88.CustomerSetting.Services;
    using CN88.CustomerSetting.SettingType2.Models;

    public class SettingTypeServiceImpl : SettingTypeServiceBase, SettingTypeService
    {
        public override SettingType GetCustomerSettingType(ISettingContext settingContext, CustomerSettingQuery query)
            => new CommBetSetting();

        public override IDictionary<string, object> GetCreateCustomerParams(
            ISettingContext settingContext,
            CreateCustomerCommand command,
            SettingType settingType)
        {
            return new Dictionary<string, object>
            {
                { "commsetting", "<ROOT><Comm SportType=\"1\" BetType=\"1\" IsLive=\"0\" GroupId = \"1\" ValueId = \"6\"/></ROOT>" },
                { "betsetting", "<ROOT><BetSetting SportType=\"1\" BetType=\"1\" IsLive=\"0\" MaxBet = \"8000\" MaxPerMatch = \"80000\"/></ROOT>" }
            };
        }

        public override SettingResult UpdateCustomerSettingType(ISettingContext settingContext, int customerId, Level customerLevel, byte productId, SettingType settingType)
        {
            throw new NotImplementedException();
        }
    }
}