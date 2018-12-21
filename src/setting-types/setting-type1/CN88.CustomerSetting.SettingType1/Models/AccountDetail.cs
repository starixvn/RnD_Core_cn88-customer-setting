namespace CN88.CustomerSetting.SettingType1.Models
{
    using System.Collections.Generic;
    using CN88.CustomerSetting.Models;

    public class AccountDetail : SettingType
    {
        public CustomerInfo CustomerInfo { get; set; }

        public PTItem PTItem { get; set; }

        public IEnumerable<Currency> Currencies { get; set; }
    }
}