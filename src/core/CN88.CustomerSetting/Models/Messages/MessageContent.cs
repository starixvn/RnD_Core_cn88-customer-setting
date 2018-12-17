namespace CN88.CustomerSetting.Models.Messages
{
    using System.Collections.Generic;

    /// <summary>
    /// Store message information,
    /// Includes main message and list of sub messages for more detail about error message.
    /// </summary>
    public class MessageContent
    {
        public MessageContent(string mainMessage)
        {
            MainMessage = mainMessage;
        }

        public MessageContent(string mainMessage, IEnumerable<string> subMessages)
        {
            MainMessage = mainMessage;
            SubMessages = subMessages;
        }

        public string MainMessage { get; private set; }

        public IEnumerable<string> SubMessages { get; private set; }
    }
}