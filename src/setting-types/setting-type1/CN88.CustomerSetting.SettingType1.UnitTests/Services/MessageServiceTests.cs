namespace CN88.CustomerSetting.SettingType1.UnitTests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using CN88.CustomerSetting.Services;
    using CN88.CustomerSetting.SettingType1.AppResources;
    using CN88.CustomerSetting.SettingType1.Services;
    using CN88.CustomerSetting.Tests.Models;
    using CN88.CustomerSetting.UnitTests.Services;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class MessageServiceTests : MessageServiceBaseTests
    {
        [SetUp]
        public void Setup()
        {
            SettingContext = new SettingContextStub();
            ResourceService = Substitute.For<ResourceService>();
            MessageService = new MessageServiceImpl(ResourceService);
        }

        [Test]
        public override void TranslateMessage_MessageIdIsNotFound_ReturnUndefinedMessage()
        {
            base.TranslateMessage_MessageIdIsNotFound_ReturnUndefinedMessage();
        }

        [Test]
        public override void TranslateMessage_MessageTypeIsOther_ReturnRawMessage()
        {
            base.TranslateMessage_MessageTypeIsOther_ReturnRawMessage();
        }

        [TestCase(1, false, "", "", "")]
        [TestCase(2, false, "SportType1", "", "SportType1")]
        [TestCase(3, false, "", "BetType1", "BetType1")]
        [TestCase(4, false, "SportType1", "BetType1", "SportType1 - BetType1")]
        public void TranslateMessage_MessageTypeIsWrongPositionTaking_ReturnMatchedMessageContent(
            byte settingLevel,
            bool isLive,
            string sportTypeName,
            string betTypeName,
            string expectedSubMessage)
        {
            /// Arrange
            int msgType = 5; //// WrongPositionTaking
            var rawMessage = "<Root MsgID=\"20001\" Msg=\"Wrong position taking.\">"
                                + "<Row SportType=\"1\" BetType=\"1\" IsLive=\""
                                + (isLive ? "1" : "0")
                                + "\"/>"
                                + "</Root>";
            ResourceService.GetSportTypeName(1, Arg.Any<string>()).Returns(sportTypeName);
            ResourceService.GetBetTypeName(1, Arg.Any<bool>()).Returns(betTypeName);

            //// Act
            var actualMessageContent = MessageService.TranslateMessage(msgType, rawMessage, settingLevel, SettingContext.Language);

            //// Assert
            Assert.AreEqual(Resources.Msg_20001, actualMessageContent.MainMessage);
            Assert.AreEqual(expectedSubMessage, actualMessageContent.SubMessages.First());
        }

        [TestCase(1, false, "", "", " - Min: 12")]
        [TestCase(2, false, "SportType1", "", "SportType1 - Min: 12")]
        [TestCase(3, false, "", "BetType1", "BetType1 - Min: 12")]
        [TestCase(4, false, "SportType1", "BetType1", "SportType1 - BetType1 - Min: 12")]
        public void TranslateMessage_MessageTypeIsMinPTViolation_ReturnMatchedMessageContent(
            byte settingLevel,
            bool isLive,
            string sportTypeName,
            string betTypeName,
            string expectedSubMessage)
        {
            /// Arrange
            int msgType = 2; //// MinPTViolation
            var rawMessage = "<Root MsgID=\"20029\" Msg=\"Wrong position taking.\">"
                                + "<Row SportType=\"1\" BetType=\"1\" IsLive=\""
                                + (isLive ? "1" : "0")
                                + "\" MinPT=\"12\"/>"
                                + "</Root>";
            ResourceService.GetSportTypeName(1, Arg.Any<string>()).Returns(sportTypeName);
            ResourceService.GetBetTypeName(1, Arg.Any<bool>()).Returns(betTypeName);

            //// Act
            var actualMessageContent = MessageService.TranslateMessage(msgType, rawMessage, settingLevel, SettingContext.Language);

            //// Assert
            Assert.AreEqual(Resources.Msg_20029, actualMessageContent.MainMessage);
            Assert.AreEqual(expectedSubMessage, actualMessageContent.SubMessages.First());
        }

        [TestCase(1, false, "", "", " - Max: 12")]
        [TestCase(2, false, "SportType1", "", "SportType1 - Max: 12")]
        [TestCase(3, false, "", "BetType1", "BetType1 - Max: 12")]
        [TestCase(4, false, "SportType1", "BetType1", "SportType1 - BetType1 - Max: 12")]
        public void TranslateMessage_MessageTypeIsMaxPTViolation_ReturnMatchedMessageContent(
            byte settingLevel,
            bool isLive,
            string sportTypeName,
            string betTypeName,
            string expectedSubMessage)
        {
            /// Arrange
            int msgType = 3; //// MaxPTViolation
            var rawMessage = "<Root MsgID=\"20028\" Msg=\"Wrong position taking.\">"
                                + "<Row SportType=\"1\" BetType=\"1\" IsLive=\""
                                + (isLive ? "1" : "0")
                                + "\" MaxPT=\"12\"/>"
                                + "</Root>";
            ResourceService.GetSportTypeName(1, Arg.Any<string>()).Returns(sportTypeName);
            ResourceService.GetBetTypeName(1, Arg.Any<bool>()).Returns(betTypeName);

            //// Act
            var actualMessageContent = MessageService.TranslateMessage(msgType, rawMessage, settingLevel, SettingContext.Language);

            //// Assert
            Assert.AreEqual(Resources.Msg_20028, actualMessageContent.MainMessage);
            Assert.AreEqual(expectedSubMessage, actualMessageContent.SubMessages.First());
        }

        [TestCase(1, false, "", "", " - Min: 11 - Max: 13")]
        [TestCase(2, false, "SportType1", "", "SportType1 - Min: 11 - Max: 13")]
        [TestCase(3, false, "", "BetType1", "BetType1 - Min: 11 - Max: 13")]
        [TestCase(4, false, "SportType1", "BetType1", "SportType1 - BetType1 - Min: 11 - Max: 13")]
        public void TranslateMessage_MessageTypeIsMinMaxPTViolation_ReturnMatchedMessageContent(
            byte settingLevel,
            bool isLive,
            string sportTypeName,
            string betTypeName,
            string expectedSubMessage)
        {
            /// Arrange
            int msgType = 4; //// MinMaxPTViolation
            var rawMessage = "<Root MsgID=\"20033\" Msg=\"Wrong position taking.\">"
                                + "<Row SportType=\"1\" BetType=\"1\" IsLive=\""
                                + (isLive ? "1" : "0")
                                + "\" MinPT=\"11\" MaxPT=\"13\"/>"
                                + "</Root>";
            ResourceService.GetSportTypeName(1, Arg.Any<string>()).Returns(sportTypeName);
            ResourceService.GetBetTypeName(1, Arg.Any<bool>()).Returns(betTypeName);

            //// Act
            var actualMessageContent = MessageService.TranslateMessage(msgType, rawMessage, settingLevel, SettingContext.Language);

            //// Assert
            Assert.AreEqual(Resources.Msg_20033, actualMessageContent.MainMessage);
            Assert.AreEqual(expectedSubMessage, actualMessageContent.SubMessages.First());
        }
    }
}
