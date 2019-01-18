namespace CN88.CustomerSetting.WebApi.Configurations
{
    using System.Collections.Generic;

    public class CustomerSettingApiConfigs
    {
        public bool EnabledAuthentication { get; set; }

        public Dictionary<string, string> AllowedApps { get; set; }
    }
}