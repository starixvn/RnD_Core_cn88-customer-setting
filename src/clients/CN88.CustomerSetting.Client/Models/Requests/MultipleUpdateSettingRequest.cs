namespace CN88.CustomerSetting.Client.Models.Requests
{
    public class MultipleUpdateSettingRequest : UpdateSettingRequest
    {
        public string CustomerIds { get; set; }

        public string CustomerNames { get; set; }
    }
}