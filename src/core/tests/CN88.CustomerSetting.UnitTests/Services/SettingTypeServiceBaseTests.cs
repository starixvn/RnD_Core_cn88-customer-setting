//namespace CN88.CustomerSetting.UnitTests.Services
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Text;
//    using System.Threading.Tasks;
//    using CN88.CustomerSetting.Models;
//    using CN88.CustomerSetting.Models.Enumerations;
//    using CN88.CustomerSetting.Models.Messages;
//    using CN88.CustomerSetting.Models.Requests;
//    using CN88.CustomerSetting.Models.Results;
//    using CN88.CustomerSetting.Services;
//    using CN88.CustomerSetting.Tests.Models;
//    using Fanex.Logging;
//    using NSubstitute;
//    using NUnit.Framework;

//    public class SettingTypeServiceBaseTests : SettingTypeServiceBase
//    {
//        private const string DefaultLanguage = "en-US";

//        public SettingTypeServiceBaseTests()
//        {
//            ResourceService = Substitute.For<ResourceService>();
//            Logger = Substitute.For<ILogger>();
//            MessageService = Substitute.For<MessageService>();
//        }

//        [SetUp]
//        public void SetUp()
//        {
//            ResourceService = Substitute.For<ResourceService>();
//            Logger = Substitute.For<ILogger>();
//            MessageService = Substitute.For<MessageService>();
//        }

//        [Test]
//        public void SetSportTypeBetTypeName_WhenCalled_SetSportTypeBetTypeNameForSettingItems()
//        {
//            //// Arrange
//            var settingType = new SettingType
//            {
//                SettingItems = new List<ISettingItem>
//                    {
//                        new SettingItemDefault
//                        {
//                            BetTypeId = 1,
//                            SportTypeId = 1
//                        },
//                        new SettingItemDefault()
//                    }
//            };
//            var sportTypeName = "SportTypeName";
//            var betTypeName = "BetTypeName";
//            ResourceService.GetSportTypeName(1, Arg.Any<string>()).Returns(sportTypeName);
//            ResourceService.GetBetTypeName(1, Arg.Any<bool>()).Returns(betTypeName);

//            //// Act
//            var actualSettingType = SetSportTypeBetTypeName(settingType, DefaultLanguage);

//            //// Assert
//            var settingItem = actualSettingType.SettingItems.First() as SettingItemDefault;
//            Assert.AreEqual(sportTypeName, settingItem.SportTypeName);
//            Assert.AreEqual(betTypeName, settingItem.BetTypeName);
//        }

//        [Test]
//        public void GetCustomerSettingType_TypeOfSettingItemIsNotDefaultSettingItem_ThrowInvalidCastException()
//        {
//            //// Arrange
//            var settingType = new SettingType
//            {
//                SettingItems = new List<ISettingItem>
//                    {
//                        new SettingItemStub()
//                    }
//            };

//            //// Act
//            Assert.Throws<InvalidCastException>(() => SetSportTypeBetTypeName(settingType, DefaultLanguage));
//        }

//        [Test]
//        public void TranslateResultMessage_SettingResultIsNull_ThrowArgumentNullException()
//        {
//            //// Arrange
//            SettingResult settingResult = null;
//            string successMessage = "Success Message";
//            byte settingLevel = 1;

//            //// Act
//            Assert.Throws<ArgumentNullException>(() 
//                => TranslateResultMessage(settingResult, successMessage, settingLevel, DefaultLanguage));
//        }

//        [Test]
//        public void TranslateResultMessage_SettingResultIsSuccess_ReturnSuccessMessage()
//        {
//            //// Arrange
//            var settingResult = new SettingResult(Result.Success);
//            string successMessage = "Success Message";
//            byte settingLevel = 1;

//            //// Act
//            var actualSettingResult = TranslateResultMessage(settingResult, successMessage, settingLevel, DefaultLanguage);

//            //// Assert
//            Assert.AreEqual(successMessage, actualSettingResult.Message);
//        }

//        [Test]
//        public void TranslateResultMessage_SettingResultIsFailed_AlwaysCallTranslateMessage()
//        {
//            //// Arrange
//            var settingResult = new SettingResult(code: 1, message: "Failed", msgType: 1, customerId: 1, settingTypeId: 1, settingTypeName: "SettingTypeName");
//            string successMessage = "Success Message";
//            byte settingLevel = 1;
//            var messageContent = new MessageContent("MainMessage");

//            MessageService.TranslateMessage(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<byte>(), Arg.Any<string>()).Returns(messageContent);

//            //// Act
//            TranslateResultMessage(settingResult, successMessage, settingLevel, DefaultLanguage);

//            //// Assert
//            MessageService.Received().TranslateMessage(
//                Arg.Is(settingResult.MsgType),
//                Arg.Is(settingResult.Message),
//                Arg.Is(settingLevel),
//                Arg.Is(DefaultLanguage));
//        }

//        [Test]
//        public void TranslateResultMessage_SettingResultIsFailed_AlwaysReturnTranslatedMessage()
//        {
//            //// Arrange
//            var settingResult = new SettingResult(code: 1, message: "Failed", msgType: 1, customerId: 1, settingTypeId: 1, settingTypeName: "SettingTypeName");
//            string successMessage = "Success Message";
//            byte settingLevel = 1;
//            var messageContent = new MessageContent("MainMessage");

//            MessageService.TranslateMessage(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<byte>(), Arg.Any<string>()).Returns(messageContent);

//            //// Act
//            var actualSettingResult = TranslateResultMessage(settingResult, successMessage, settingLevel, DefaultLanguage);

//            //// Assert
//            Assert.AreEqual(messageContent.MainMessage, actualSettingResult.Message);
//        }


//        public override Task<SettingType> GetCustomerSettingType(ISettingContext settingContext, CustomerSettingQuery query)
//        {
//            throw new NotImplementedException();
//        }

//        public override Task<SettingResult> UpdateCustomerSettingType(
//            ISettingContext settingContext, 
//            UpdateCustomerSettingCommand command, 
//            SettingType settingType)
//        {
//            if (command.Setting.CustomerId == 0)
//            {
//                throw new ArgumentOutOfRangeException(nameof(command.Setting.CustomerId));
//            }

//            return Task.FromResult(new SettingResult(Result.Success));
//        }

//        public override IDictionary<string, object> BuildCreateCustomerParams(ISettingContext settingContext, CreateCustomerCommand command, SettingType settingType)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
