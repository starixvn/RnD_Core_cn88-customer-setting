namespace CN88.CustomerSetting.SettingType1.Models.Dtos
{
    public class CreateCustomerInfoDto
    {
        public string UplineUsername { get; set; }

        public string NewUsername { get; set; }

        public decimal MaxCredit { get; set; }

        public decimal MinCredit { get; set; }
    }
}