namespace CN88.CustomerSetting.SettingType2.Services
{
    using CN88.CustomerSetting.Models.Messages;
    using CN88.CustomerSetting.Services;
    using CN88.CustomerSetting.SettingType2.AppResources;
    using CN88.CustomerSetting.SettingType2.Constants;

    public class MessageServiceImpl : MessageServiceBase, MessageService
    {
        public MessageServiceImpl(ResourceService resourceService = null)
            : base(resourceService)
        {
        }

        public override MessageContent TranslateMessage(int msgType, string rawMessage, byte settingLevel, string language)
        {
            switch (msgType)
            {
                case MessageTypes.CommissionViolation:
                case MessageTypes.BetLimitViolation:
                    return TranslateDefaultSettingTypeViolationMessage(Resources.ResourceManager, rawMessage, settingLevel, language);

                default:
                    return base.TranslateMessage(msgType, rawMessage, settingLevel, language);
            }
        }
    }
}