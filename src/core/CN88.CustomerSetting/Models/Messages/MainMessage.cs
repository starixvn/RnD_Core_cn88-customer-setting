namespace CN88.CustomerSetting.Models.Messages
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;
    using Services;

    /// <summary>
    /// Contains Xml data of Raw Message,
    /// Has message id to get main message
    /// Has list of sub message for more detail about error.
    /// </summary>
    /// <typeparam name="T">Implement of sub message interface.</typeparam>
    [XmlRoot("Root")]
    public class MainMessage<T> : SimpleMessage
        where T : ISubMessage
    {
        [XmlElement("Row")]
        public T[] SubMessages { get; set; }

        public IEnumerable<string> TranslateSubMessages(ResourceService resourceService, string language, byte settingLevel)
        {
            if (SubMessages != null && SubMessages.Any())
            {
                var subTranslatedMessages = new List<string>();
                foreach (ISubMessage subMessage in SubMessages)
                {
                    subTranslatedMessages.Add(subMessage.ToString(resourceService, language, settingLevel));
                }

                return subTranslatedMessages;
            }

            return Enumerable.Empty<string>();
        }
    }
}