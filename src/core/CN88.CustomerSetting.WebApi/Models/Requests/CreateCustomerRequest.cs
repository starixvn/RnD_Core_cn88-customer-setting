namespace CN88.CustomerSetting.WebApi.Models.Requests
{
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Requests;

    public class CreateCustomerRequest : SettingRequest
    {
        public Setting Setting { get; set; }

        public string HashKey { get; set; }

        internal CreateCustomerCommand GetCreateCustomerCommand()
            => new CreateCustomerCommand(
                Setting,
                UplineId,
                HashKey);
    }
}