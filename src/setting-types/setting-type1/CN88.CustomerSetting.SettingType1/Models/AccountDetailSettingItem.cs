namespace CN88.CustomerSetting.SettingType1.Models
{
    using System;
    using System.Collections.Generic;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Results;

    public class AccountDetailSettingItem : SettingItemDefault
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

        public decimal MaxCredit { get; set; }

        public decimal MinCredit { get; set; }

        public SpreadGroup SpreadGroup { get; set; }

        public IEnumerable<string> UsernameSuffixes { get; set; }

        public string UplineUsername { get; set; }

        public IEnumerable<SpreadGroup> SpreadGroups { get; set; }

        public IEnumerable<Currency> Currencies { get; set; }

        public bool IsPTReadOnly { get; set; }

        public DateTime PTEffectiveTime { get; set; }
    }

    public class MemberAccountDetail : AccountDetailSettingItem
    {
        public override IResult Validate()
            => Result.Success;
    }
}