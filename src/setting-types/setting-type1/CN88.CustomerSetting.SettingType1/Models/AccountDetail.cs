namespace CN88.CustomerSetting.SettingType1.Models
{
    using System.Collections.Generic;
    using CN88.CustomerSetting.Models;

    public class AccountDetail : SettingItemDefault
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string MobilePhone { get; set; }

        public string Fax { get; set; }

        public Currency Currency { get; set; }

        public decimal Credit { get; set; }

        public decimal AvailableCredit { get; set; }

        public string UGroup { get; set; }

        public IEnumerable<string> UsernameSuffix { get; set; }

        public string UplineUsername { get; set; }

        public IEnumerable<string> UGroups { get; set; }

        public IEnumerable<Currency> Currencies { get; set; }

        public decimal MaxPT { get; set; }

        public decimal MinPT { get; set; }
    }
}