namespace CN88.CustomerSetting.SettingType1.Services
{
    using CN88.CustomerSetting.Models.Messages;
    using CN88.CustomerSetting.Services;
    using CN88.CustomerSetting.SettingType1.Constants;
    using CN88.CustomerSetting.SettingType1.Models.Messages;

    internal class MessageServiceImpl : MessageServiceBase, MessageService
    {
        public MessageServiceImpl(ResourceService resourceService = null)
            : base(resourceService)
        {
        }

        public override MessageContent TranslateMessage(int msgType, string rawMessage, byte settingLevel, string language)
        {
            switch (msgType)
            {
                case MessageTypes.InvalidCredit:
                case MessageTypes.WrongPT:
                    return TranslateMessage<DefaultSubMessage>(rawMessage, settingLevel, language);

                case MessageTypes.MinPTViolation:
                    return TranslateMessage<MinPTViolationSubMessage>(rawMessage, settingLevel, language);

                case MessageTypes.MaxPTViolation:
                    return TranslateMessage<MaxPTViolationSubMessage>(rawMessage, settingLevel, language);

                case MessageTypes.MinMaxPTViolation:
                    return TranslateMessage<MinMaxPTViolationSubMessage>(rawMessage, settingLevel, language);

                default:
                    return base.TranslateMessage(msgType, rawMessage, settingLevel, language);
            }
        }

        private MessageContent TranslateMessage<T>(
            string rawMessage,
            byte settingLevel,
            string language)
            where T : ISubMessage
        {
            var mainMessage = Deserialize<MainMessage<T>>(rawMessage);

            return GetMessageContent<T>(mainMessage, settingLevel, language);
        }

        private MessageContent GetMessageContent<T>(
            MainMessage<T> positionTakingMainMessage,
            byte settingLevel,
            string language)
            where T : ISubMessage
        {
            var subMessage = positionTakingMainMessage.TranslateSubMessages(ResourceService, language, settingLevel);
            var mainMessage = GetMessage(AppResources.Resources.ResourceManager, positionTakingMainMessage.MessageId);

            return new MessageContent(mainMessage, subMessage);
        }
    }
}