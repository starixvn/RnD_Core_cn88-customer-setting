namespace CN88.CustomerSetting.SettingType2.Models
{
    using System.Linq;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.SettingType2.AppResources;

    public class CommBetSetting : SettingType
    {
        public CommBetSetting()
        {
            Name = Resources.CommBetLimit;
        }

        public string ToCommXml()
        {
            var settingItemsXml = string.Join(string.Empty, SettingItems
                .Cast<CommBetSettingItem>()
                .Select(settingItem => settingItem.ToCommXml()));

            return string.Format(SettingTypeXmlFormat, settingItemsXml);
        }

        public string ToBetSettingXml()
        {
            var settingItemsXml = string.Join(string.Empty, SettingItems
                .Cast<CommBetSettingItem>()
                .Select(settingItem => settingItem.ToBetSettingXml()));

            return string.Format(SettingTypeXmlFormat, settingItemsXml);
        }
    }
}