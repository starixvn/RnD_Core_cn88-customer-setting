namespace CN88.CustomerSetting.SettingType2.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.SettingType2.AppResources;

    public class CommBetSetting : SettingType
    {
        public CommBetSetting()
        {
            Name = Resources.CommBetLimit;
        }

        public string ToCommXml(IEnumerable<CommTemplate> commTemplates)
        {
            return string.Format(
                SettingTypeXmlFormat,
                string.Join(
                    string.Empty,
                    SettingItems
                        .Cast<CommBetSettingItem>()
                        .Select(settingItem => settingItem.ToCommXml(commTemplates))));
        }

        public string ToBetSettingXml()
            => string.Format(
                SettingTypeXmlFormat, 
                string.Join(
                    string.Empty, 
                    SettingItems
                        .Cast<CommBetSettingItem>()
                        .Select(settingItem => settingItem.ToBetSettingXml())));
    }
}