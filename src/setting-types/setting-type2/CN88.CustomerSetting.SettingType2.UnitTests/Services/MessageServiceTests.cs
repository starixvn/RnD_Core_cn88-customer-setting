namespace CN88.CustomerSetting.SettingType2.UnitTests.Services
{
    using System.Linq;
    using CN88.CustomerSetting.Services;
    using CN88.CustomerSetting.SettingType2.AppResources;
    using CN88.CustomerSetting.SettingType2.Services;
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

        [TestCase(1, "", "", "")]
        [TestCase(2, "SportType1", "", "SportType1")]
        [TestCase(3, "", "BetType1", "BetType1")]
        [TestCase(4, "SportType1", "BetType1", "SportType1 - BetType1")]
        public void TranslateMessage_MessageTypeIsWrongBetLimit_ReturnMatchedMessageContent(
            byte settingLevel,
            string sportTypeName,
            string betTypeName,
            string expectedSubMessage)
        {
            /// Arrange
            int msgType = 71; //// Wrong Bet Limit
            var rawMessage = "<Root MsgID=\"10003\" Msg=\"\">"
                                + "<Row SportType=\"1\" BetType=\"1\" />"
                                + "</Root>";
            ResourceService.GetSportTypeName(1, Arg.Any<string>()).Returns(sportTypeName);
            ResourceService.GetBetTypeName(1, Arg.Any<bool>(), Arg.Any<string>()).Returns(betTypeName);

            //// Act
            var actualMessageContent = MessageService.TranslateMessage(msgType, rawMessage, settingLevel, SettingContext.Language);

            //// Assert
            Assert.AreEqual(Resources.Msg_10003, actualMessageContent.MainMessage);
            Assert.AreEqual(expectedSubMessage, actualMessageContent.SubMessages.First());
        }

        [TestCase(1, "", "", "")]
        [TestCase(2, "SportType1", "", "SportType1")]
        [TestCase(3, "", "BetType1", "BetType1")]
        [TestCase(4, "SportType1", "BetType1", "SportType1 - BetType1")]
        public void TranslateMessage_MessageTypeIsWrongCommission_ReturnMatchedMessageContent(
            byte settingLevel,
            string sportTypeName,
            string betTypeName,
            string expectedSubMessage)
        {
            /// Arrange
            int msgType = 51; //// Wrong Commission Rate
            var rawMessage = "<Root MsgID=\"1\" Msg=\"\">"
                                + "<Row SportType=\"1\" BetType=\"1\" />"
                                + "</Root>";
            ResourceService.GetSportTypeName(1, Arg.Any<string>()).Returns(sportTypeName);
            ResourceService.GetBetTypeName(1, Arg.Any<bool>(), Arg.Any<string>()).Returns(betTypeName);

            //// Act
            var actualMessageContent = MessageService.TranslateMessage(msgType, rawMessage, settingLevel, SettingContext.Language);

            //// Assert
            Assert.AreEqual(Resources.Msg_1, actualMessageContent.MainMessage);
            Assert.AreEqual(expectedSubMessage, actualMessageContent.SubMessages.First());
        }
    }
}