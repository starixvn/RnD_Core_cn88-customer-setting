namespace CN88.CustomerSetting.SettingType1.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Results;

    public class AccountDetailSettingItem<T> : SettingItemDefault
        where T : SettingItemDefault
    {
        public AccountDetailSettingItem()
        {
            SportTypeId = 1;
            BetTypeId = 1;
        }

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

        public bool IsPTReadOnly { get; set; }

        public DateTime PTEffectiveTime { get; set; }

        public IEnumerable<T> PTItems { get; set; }

        public override IResult Validate()
        {
            foreach (var item in PTItems)
            {
                var validateResult = item.Validate();

                if (validateResult.IsNotOk)
                {
                    return validateResult;
                }
            }

            return base.Validate();
        }

        public override string ToXml()
            => string.Join(string.Empty, PTItems.Select(item => item.ToXml()));
    }

    public class MemberAccountDetail : AccountDetailSettingItem<SettingItemDefault>
    {
        public override IResult Validate()
            => Result.Success;
    }
}