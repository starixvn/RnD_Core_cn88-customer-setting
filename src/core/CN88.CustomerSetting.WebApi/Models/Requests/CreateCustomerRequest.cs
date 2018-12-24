namespace CN88.CustomerSetting.WebApi.Models.Requests
{
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Requests;

    public class CreateCustomerRequest : SettingRequest
    {
        public Setting Setting { get; set; }

        internal CreateCustomerCommand GetCreateCustomerCommand()
            => new CreateCustomerCommand(
                setting: Setting,
                uplineId: UplineId);
    }
}