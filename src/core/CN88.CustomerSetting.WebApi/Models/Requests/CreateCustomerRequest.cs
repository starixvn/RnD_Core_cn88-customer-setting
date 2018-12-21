namespace CN88.CustomerSetting.WebApi.Models.Requests
{
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Enumerations;
    using CN88.CustomerSetting.Models.Requests;

    public class CreateCustomerRequest : SettingRequest
    {
        public Setting Setting { get; set; }

        internal CreateCustomerCommand GetCreateCustomerCommand()
        {
            return new CreateCustomerCommand(
                setting: Setting,
                uplineLevel: UplineLevel == null ? null : Enumeration.FromValue<Level>(UplineLevel.Value));
        }
    }
}