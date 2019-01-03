namespace CN88.CustomerSetting.SettingType1.Models.Messages
{
    using System.Xml.Serialization;
    using CN88.CustomerSetting.Models.Messages;
    using CustomerSetting.Services;

    public class MinPTViolationSubMessage : DefaultSubMessage
    {
        [XmlAttribute]
        public byte MinPT { get; set; }

        public override string ToString(ResourceService resourceService, string language, byte settingLevel)
        {
            return string.Format(
                "{0}: {2}",
                base.ToString(resourceService, language, settingLevel),
                MinPT);
        }
    }
}