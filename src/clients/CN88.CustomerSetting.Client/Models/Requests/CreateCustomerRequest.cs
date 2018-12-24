namespace CN88.CustomerSetting.Client.Models.Requests
{
    public class CreateCustomerRequest : SettingRequest
    {
        public string Setting { get; set; }

        public int UplineId { get; set; }

        public string HashKey { get; set; }
    }
}