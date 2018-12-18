namespace CN88.CustomerSetting.WebApi.Models
{
    using CN88.CustomerSetting.Models;

    public class SettingContext : ISettingContext
    {
        public User Actor { get; set; }

        public string Language { get; set; }

        public string IP { get; set; }

        public byte System { get; set; }

        public string Domain { get; set; }
    }
}