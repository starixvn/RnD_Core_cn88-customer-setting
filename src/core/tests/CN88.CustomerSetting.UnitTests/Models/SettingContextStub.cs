namespace CN88.CustomerSetting.Tests.Models
{
    using CustomerSetting.Models;
    using CustomerSetting.Models.Enumerations;

    public class SettingContextStub : ISettingContext
    {
        private User _actor = new User
        {
            Id = 100,
            Level = Level.Company,
            Name = "Admin"
        };

        private byte _systemId = 1;

        private string _language = "en-US";

        public User Actor
        {
            get => _actor;

            set => _actor = value;
        }

        public string Language
        {
            get => _language;

            set => _language = value;
        }

        public string IP => "192.168.1.1";

        public byte System
        {
            get => _systemId;

            set => _systemId = value;
        }

        public string Domain => "Domain Name";
    }
}