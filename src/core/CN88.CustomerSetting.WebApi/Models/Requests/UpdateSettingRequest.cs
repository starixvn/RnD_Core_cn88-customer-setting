namespace CN88.CustomerSetting.WebApi.Models.Requests
{
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Requests;

    public class UpdateSettingRequest : SettingRequest
    {
        public Setting Setting { get; set; }

        internal UpdateCustomerSettingCommand GetUpdateCustomerSettingCommand()
            => new UpdateCustomerSettingCommand(Setting);
    }
}