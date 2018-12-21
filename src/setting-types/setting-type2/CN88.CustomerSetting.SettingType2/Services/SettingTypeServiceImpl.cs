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

        public override IDictionary<string, object> GetInsertCustomerParams(
            ISettingContext settingContext,
            CreateCustomerCommand command,
            SettingType settingType)
        {
            throw new NotImplementedException();
        }

        public override SettingResult UpdateCustomerSettingType(ISettingContext settingContext, int customerId, Level customerLevel, Level uplineLevel, byte productId, SettingType settingType)
        {
            throw new NotImplementedException();
        }
    }
}