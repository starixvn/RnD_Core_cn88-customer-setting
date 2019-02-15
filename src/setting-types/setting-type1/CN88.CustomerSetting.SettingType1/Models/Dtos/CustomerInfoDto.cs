namespace CN88.CustomerSetting.SettingType1.Models.Dtos
{
    public class CustomerInfoDto
    {
        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string MobilePhone { get; set; }

        public string Fax { get; set; }

        public string Currency { get; set; }

        public decimal Credit { get; set; }

        public decimal MaxCredit { get; set; }

        public decimal MinCredit { get; set; }

        public string UGroup { get; set; }

        public string MainShareholderUsername { get; set; }
    }
}