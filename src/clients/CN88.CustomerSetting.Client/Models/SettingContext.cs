namespace CN88.CustomerSetting.Client.Models
{
    using Configurations;

    public class SettingContext
    {
        public SettingContext()
        {
            System = CN88CustomerSettingConfiguration.Settings.System;
        }

        public int ActorId { get; set; }

        public string ActorName { get; set; }

        public byte ActorLevel { get; set; }

        public int SubAccountId { get; set; }

        public string Language { get; set; }

        public string Domain { get; set; }

        public string IP { get; set; }

        public byte System { get; set; }

        public string HashKey { get; set; }
    }
}