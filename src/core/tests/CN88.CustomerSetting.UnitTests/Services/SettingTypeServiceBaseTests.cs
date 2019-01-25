namespace CN88.CustomerSetting.UnitTests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Messages;
    using CN88.CustomerSetting.Models.Requests;
    using CN88.CustomerSetting.Models.Results;
    using CN88.CustomerSetting.Services;
    using CN88.CustomerSetting.Tests.Models;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using NSubstitute;
    using NUnit.Framework;

    public class SettingTypeServiceMock : SettingTypeServiceBase
    {
        public SettingTypeServiceMock(
            ResourceService resourceService,
            ILogger logger,
            ICacheService cacheService,
            IDynamicRepository dynamicRepository,
            MessageService messageService)
            : base(resourceService, logger, cacheService, dynamicRepository)
        {
            this.MessageService = messageService;
        }

        public override Task<SettingType> GetCustomerSettingType(
            ISettingContext settingContext,
            CustomerSettingQuery query)
        {
            throw new NotImplementedException();
        }

        public override Task<SettingResult> UpdateCustomerSettingType(
            ISettingContext settingContext,
            UpdateCustomerSettingCommand command,
            SettingType settingType)
        {
            if (command.Setting.CustomerId == 0)
            {
#pragma warning disable S3928 // Parameter names used into ArgumentException constructors should match an existing one 
                throw new ArgumentOutOfRangeException(nameof(command.Setting.CustomerId));
#pragma warning restore S3928 // Parameter names used into ArgumentException constructors should match an existing one 
            }

            return Task.FromResult(new SettingResult(Result.Success));
        }

        public override IDictionary<string, object> BuildCreateCustomerParams(ISettingContext settingContext, CreateCustomerCommand command, SettingType settingType)
        {
            throw new NotImplementedException();
        }
    }

    public class SettingTypeServiceBaseTests
    {
        private const string DefaultLanguage = "en-US";
        private ResourceService resourceService;
        private ILogger logger;
        private ICacheService cacheService;
        private IDynamicRepository dynamicRepository;
        private MessageService messageService;

        private SettingTypeServiceMock settingTypeServiceMock;

        [SetUp]
        public void SetUp()
        {
            resourceService = Substitute.For<ResourceService>();
            logger = Substitute.For<ILogger>();
            dynamicRepository = Substitute.For<IDynamicRepository>();
            cacheService = Substitute.For<ICacheService>();
            messageService = Substitute.For<MessageService>();

            settingTypeServiceMock = new SettingTypeServiceMock(
                resourceService,
                logger,
                cacheService,
                dynamicRepository,
                messageService);
        }

        [Test]
        public void SetSportTypeBetTypeName_WhenCalled_SetSportTypeBetTypeNameForSettingItems()
        {
            //// Arrange
            var settingType = new SettingType
            {
                SettingItems = new List<ISettingItem>
                    {
                        new SettingItemDefault
                        {
                            BetTypeId = 1,
                            SportTypeId = 1
                        },
                        new SettingItemDefault()
                    }
            };
            var sportTypeName = "SportTypeName";
            var betTypeName = "BetTypeName";
            resourceService.GetSportTypeName(1, Arg.Any<string>()).Returns(sportTypeName);
            resourceService.GetBetTypeName(1, Arg.Any<bool>(), Arg.Any<string>()).Returns(betTypeName);

            //// Act
            var actualSettingType = settingTypeServiceMock.SetSportTypeBetTypeName(settingType, DefaultLanguage);

            //// Assert
            var settingItem = actualSettingType.SettingItems.First() as SettingItemDefault;
            Assert.AreEqual(sportTypeName, settingItem.SportTypeName);
            Assert.AreEqual(betTypeName, settingItem.BetTypeName);
        }

        [Test]
        public void GetCustomerSettingType_TypeOfSettingItemIsNotDefaultSettingItem_ThrowInvalidCastException()
        {
            //// Arrange
            var settingType = new SettingType
            {
                SettingItems = new List<ISettingItem>
                    {
                        new SettingItemStub()
                    }
            };

            //// Act
            Assert.Throws<InvalidCastException>(() => settingTypeServiceMock.SetSportTypeBetTypeName(settingType, DefaultLanguage));
        }

        [Test]
        public void TranslateResultMessage_SettingResultIsNull_ThrowArgumentNullException()
        {
            //// Arrange
            SettingResult settingResult = null;
            string successMessage = "Success Message";
            byte settingLevel = 1;

            //// Act
            Assert.Throws<ArgumentNullException>(()
                => settingTypeServiceMock.TranslateResultMessage(settingResult, successMessage, settingLevel, DefaultLanguage));
        }

        [Test]
        public void TranslateResultMessage_SettingResultIsSuccess_ReturnSuccessMessage()
        {
            //// Arrange
            var settingResult = new SettingResult(Result.Success);
            string successMessage = "Success Message";
            byte settingLevel = 1;

            //// Act
            var actualSettingResult = settingTypeServiceMock.TranslateResultMessage(settingResult, successMessage, settingLevel, DefaultLanguage);

            //// Assert
            Assert.AreEqual(successMessage, actualSettingResult.Message);
        }

        [Test]
        public void TranslateResultMessage_SettingResultIsFailed_AlwaysCallTranslateMessage()
        {
            //// Arrange
            var settingResult = new SettingResult(code: 1, message: "Failed", msgType: 1, customerId: 1, settingTypeId: 1, settingTypeName: "SettingTypeName");
            string successMessage = "Success Message";
            byte settingLevel = 1;
            var messageContent = new MessageContent("MainMessage");

            messageService.TranslateMessage(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<byte>(), Arg.Any<string>()).Returns(messageContent);

            //// Act
            settingTypeServiceMock.TranslateResultMessage(settingResult, successMessage, settingLevel, DefaultLanguage);

            //// Assert
            messageService.Received().TranslateMessage(
                Arg.Is(settingResult.MsgType),
                Arg.Is(settingResult.Message),
                Arg.Is(settingLevel),
                Arg.Is(DefaultLanguage));
        }

        [Test]
        public void TranslateResultMessage_SettingResultIsFailed_AlwaysReturnTranslatedMessage()
        {
            //// Arrange
            var settingResult = new SettingResult(code: 1, message: "Failed", msgType: 1, customerId: 1, settingTypeId: 1, settingTypeName: "SettingTypeName");
            string successMessage = "Success Message";
            byte settingLevel = 1;
            var messageContent = new MessageContent("MainMessage");

            messageService.TranslateMessage(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<byte>(), Arg.Any<string>()).Returns(messageContent);

            //// Act
            var actualSettingResult = settingTypeServiceMock.TranslateResultMessage(settingResult, successMessage, settingLevel, DefaultLanguage);

            //// Assert
            Assert.AreEqual(messageContent.MainMessage, actualSettingResult.Message);
        }
    }
}