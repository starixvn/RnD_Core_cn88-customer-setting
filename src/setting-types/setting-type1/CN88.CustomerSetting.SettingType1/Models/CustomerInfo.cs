namespace CN88.CustomerSetting.SettingType1.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class CustomerInfo
    {
        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string MobilePhone { get; set; }

        public string Fax { get; set; }

        public int CurrencyId { get; set; }

        public decimal Credit { get; set; }

        public decimal AvailableCredit { get; set; }
    }
}
