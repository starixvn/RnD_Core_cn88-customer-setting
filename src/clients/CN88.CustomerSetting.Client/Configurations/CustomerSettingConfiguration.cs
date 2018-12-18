namespace CN88.CustomerSetting.Client.Configurations
{
    using System;
    using System.Configuration;

    public class CN88CustomerSettingConfiguration : ConfigurationSection
    {
        private static readonly CN88CustomerSettingConfiguration settings
            = ConfigurationManager.GetSection("CN88CustomerSettingConfiguration") as CN88CustomerSettingConfiguration;

        public static CN88CustomerSettingConfiguration Settings
        {
            get
            {
                return settings;
            }
        }

        [ConfigurationProperty("System", IsRequired = true)]
        public byte System
        {
            get { return (byte)this["System"]; }
            set { this["System"] = value; }
        }

        [ConfigurationProperty("ServiceUris", IsRequired = true)]
        public string ServiceUris
        {
            get { return (string)this["ServiceUris"]; }
            set { this["ServiceUris"] = value; }
        }

        [ConfigurationProperty("SecretKey", IsRequired = true, DefaultValue = "U6BUFHtVcq5SPQcTl7tHLBG/LF6d5YVRKHrCHD9M+Vw=")]
        public string SecretKey
        {
            get { return (string)this["SecretKey"]; }
            set { this["SecretKey"] = value; }
        }

        [ConfigurationProperty("CdnUris", IsRequired = true)]
        public string CdnUris
        {
            get { return (string)this["CdnUris"]; }
            set { this["CdnUris"] = value; }
        }

        [ConfigurationProperty("CustomCssUrl")]
        public Uri CustomCssUrl
        {
            get { return (Uri)this["CustomCssUrl"]; }
            set { this["CustomCssUrl"] = value; }
        }

        [ConfigurationProperty("ClientLogCategory", IsRequired = false, DefaultValue = "DCS-Setting-Client-Log")]
        public string ClientLogCategory
        {
            get { return (string)this["ClientLogCategory"]; }
            set { this["ClientLogCategory"] = value; }
        }

        [ConfigurationProperty("AccessDeniedAction", IsRequired = false, DefaultValue = "~/User/AccessDenied")]
        public string AccessDeniedAction
        {
            get { return (string)this["AccessDeniedAction"]; }
            set { this["AccessDeniedAction"] = value; }
        }

        [ConfigurationProperty("EnableRequestLogging", IsRequired = false, DefaultValue = true)]
        public bool EnableRequestLogging
        {
            get { return (bool)this["EnableRequestLogging"]; }
            set { this["EnableRequestLogging"] = value; }
        }

        [ConfigurationProperty("UpdateSettingTimeout", IsRequired = false, DefaultValue = 600)]
        public int UpdateSettingTimeout
        {
            get { return (int)this["UpdateSettingTimeout"] * 1000; }
            set { this["UpdateSettingTimeout"] = value; }
        }
    }
}