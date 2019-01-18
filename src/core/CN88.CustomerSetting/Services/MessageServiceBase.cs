namespace CN88.CustomerSetting.Services
{
    using System;
    using System.IO;
    using System.Resources;
    using System.Xml.Serialization;
    using Models.Constants;
    using Models.Messages;

    public interface MessageService
    {
        MessageContent TranslateMessage(int msgType, string rawMessage, byte settingLevel, string language);
    }

#pragma warning disable S1694 // An abstract class should have both abstract and concrete methods

    public abstract class MessageServiceBase : MessageService
#pragma warning restore S1694 // An abstract class should have both abstract and concrete methods
    {
        private const string NotFoundMessageFormat = "Undefined error message {0}";
        private const string MessageFormat = "Msg_{0}";

        protected MessageServiceBase(ResourceService resourceService)
        {
            ResourceService = resourceService;
        }

        protected ResourceService ResourceService { get; set; }

        public virtual MessageContent TranslateMessage(int msgType, string rawMessage, byte settingLevel, string language)
        {
            if (msgType == MessageTypes.Exception)
            {
                return TranslateSimpleMessage(rawMessage);
            }

            return new MessageContent(rawMessage);
        }

        private static MessageContent TranslateSimpleMessage(string rawMessage)
        {
            var mainMessage = rawMessage;
            var simpleMessage = Deserialize<SimpleMessage>(rawMessage);

            if (simpleMessage != null)
            {
                mainMessage = GetMessage(AppResources.Resources.ResourceManager, simpleMessage.MessageId);
            }

            return new MessageContent(mainMessage);
        }

        protected static T Deserialize<T>(string text) where T : SimpleMessage
        {
            var serializer = new XmlSerializer(typeof(T));

            using (var stringReader = new StringReader(text))
            {
                return (T)serializer.Deserialize(stringReader);
            }
        }

        protected static string GetMessage(ResourceManager resouceManager, int messsageId)
        {
            if (resouceManager == null)
            {
                throw new ArgumentNullException(nameof(resouceManager), "Resource Mananger cannot be null");
            }

            var key = string.Format(MessageFormat, messsageId);
            var message = resouceManager.GetString(key);

            return !string.IsNullOrEmpty(message)
                ? message
                : string.Format(NotFoundMessageFormat, messsageId);
        }

        protected MessageContent TranslateDefaultSettingTypeViolationMessage(
            ResourceManager resouceManager,
            string rawMessage,
            byte settingLevel,
            string language)
        {
            var mainMessage = Deserialize<MainMessage<DefaultSubMessage>>(rawMessage);
            var translatedSubMessages = mainMessage.TranslateSubMessages(ResourceService, language, settingLevel);
            var translatedMainMessage = GetMessage(resouceManager, mainMessage.MessageId);

            return new MessageContent(translatedMainMessage, translatedSubMessages);
        }
    }
}