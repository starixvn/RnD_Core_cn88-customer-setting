namespace CN88.CustomerSetting.Models.Messages
{
    using System;
    using System.Xml.Serialization;
    using CN88.CustomerSetting.Models.Enumerations;
    using Services;

    public class DefaultSubMessage : ISubMessage
    {
        [XmlAttribute]
        public short SportType { get; set; }

        [XmlAttribute]
        public short BetType { get; set; }

        [XmlAttribute]
        public bool IsLive { get; set; }

        public virtual string ToString(ResourceService resourceService, string language, byte settingLevel)
        {
            if (resourceService == null)
            {
                throw new ArgumentNullException(nameof(resourceService));
            }

            switch ((SettingLevel)settingLevel)
            {
                case SettingLevel.Product:
                    return string.Empty;

                case SettingLevel.SportType:
                    return resourceService.GetSportTypeName(SportType, language);

                case SettingLevel.BetType:
                    return resourceService.GetBetTypeName(BetType, IsLive);

                case SettingLevel.SportTypeBetType:
                    return string.Format(
                        "{0} - {1}",
                        resourceService.GetSportTypeName(SportType, language),
                        resourceService.GetBetTypeName(BetType, IsLive));

                default:
                    throw new InvalidOperationException("Not implement setting level");
            }
        }
    }
}