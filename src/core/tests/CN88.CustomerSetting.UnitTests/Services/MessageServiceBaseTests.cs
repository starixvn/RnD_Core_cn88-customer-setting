namespace CN88.CustomerSetting.UnitTests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using CN88.CustomerSetting.AppResources;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Services;
    using NUnit.Framework;

    public abstract class MessageServiceBaseTests
    {
        private const int ExceptionMessageType = 100;
        private const int OtherMessageType = 101;

        protected ISettingContext SettingContext { get; set; }

        protected ResourceService ResourceService { get; set; }

        protected MessageService MessageService { get; set; }

        ////[Test]
        ////public virtual void TranslateMessage_MessageTypeIsException_ReturnMatchedMessageContent()
        ////{
        ////    //// Arrange
        ////    int msgType = OtherMessageType;
        ////    string rawMessage = "<Root MsgID=\"90001\" Msg=\"Update process is unsuccessful!\"/>";
        ////    byte settingLevel = 1;

        ////    //// Act
        ////    var actualMessageContent = MessageService.TranslateMessage(msgType, rawMessage, settingLevel, "en-US");

        ////    //// Assert
        ////    Assert.AreEqual(Resources.Msg_90001, actualMessageContent.MainMessage);
        ////}

        [Test]
        public virtual void TranslateMessage_MessageIdIsNotFound_ReturnUndefinedMessage()
        {
            //// Arrange
            int msgType = ExceptionMessageType;
            string rawMessage = "<Root MsgID=\"121212\"/>";
            byte settingLevel = 1;

            //// Act
            var actualMessageContent = MessageService.TranslateMessage(msgType, rawMessage, settingLevel, "en-US");

            //// Assert
            Assert.AreEqual("Undefined error message 121212", actualMessageContent.MainMessage);
        }

        [Test]
        public virtual void TranslateMessage_MessageTypeIsOther_ReturnRawMessage()
        {
            //// Arrange
            int msgType = OtherMessageType;
            string rawMessage = "Raw Message";
            byte settingLevel = 1;

            //// Act
            var actualMessageContent = MessageService.TranslateMessage(msgType, rawMessage, settingLevel, "en-US");

            //// Assert
            Assert.AreEqual(rawMessage, actualMessageContent.MainMessage);
        }
    }
}
