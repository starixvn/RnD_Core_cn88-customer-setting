namespace CN88.CustomerSetting.DemoSite.Configurations
{
    using System.Configuration;

    public class DemoSiteConfiguration : ConfigurationSection
    {
        private static readonly DemoSiteConfiguration settings
            = ConfigurationManager.GetSection("DemoSiteConfiguration") as DemoSiteConfiguration;

        public static DemoSiteConfiguration Settings
        {
            get
            {
                return settings;
            }
        }

        [ConfigurationProperty("EnableLogin", IsRequired = false, DefaultValue = false)]
        public bool EnableLogin
        {
            get { return (bool)this["EnableLogin"]; }
            set { this["EnableLogin"] = value; }
        }

        [ConfigurationProperty("SecurityCode", IsRequired = false, DefaultValue = "123457aaAA")]
        public string SecurityCode
        {
            get { return (string)this["SecurityCode"]; }
            set { this["SecurityCode"] = value; }
        }
    }
}