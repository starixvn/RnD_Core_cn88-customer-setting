namespace CN88.CustomerSetting.SettingType1.Models.Messages
{
    using System.Xml.Serialization;
    using CN88.CustomerSetting.Models.Messages;
    using CustomerSetting.Services;

    public class MinMaxPTViolationSubMessage : DefaultSubMessage
    {
        [XmlAttribute]
        public byte MinPT { get; set; }

        [XmlAttribute]
        public byte MaxPT { get; set; }

        public override string ToString(ResourceService resourceService, string language, byte settingLevel)
        {
            return string.Format(
                "{0} - {1}: {2} - {3}: {4}",
                base.ToString(resourceService, language, settingLevel),
                IsLive ? AppResources.Resources.MinLive : AppResources.Resources.Min,
                MinPT,
                IsLive ? AppResources.Resources.MaxLive : AppResources.Resources.Max,
                MaxPT);
        }
    }
}