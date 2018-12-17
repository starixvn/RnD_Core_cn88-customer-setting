namespace CN88.CustomerSetting.SettingType1.Models
{
    using CN88.CustomerSetting.Models;

    public class AccountDetail : SettingItemDefault
    {
        public CustomerInfo CustomerInfo { get; set; }

        public PTItem PTItem { get; set; }
    }
}