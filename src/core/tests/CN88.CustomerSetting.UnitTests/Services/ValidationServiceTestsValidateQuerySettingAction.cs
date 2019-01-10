namespace CN88.CustomerSetting.Tests.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using CN88.CustomerSetting.AppResources;
    using CN88.CustomerSetting.SpParams.Criteria;
    using CustomerSetting.Models;
    using CustomerSetting.Models.Enumerations;
    using CustomerSetting.Models.Requests;
    using CustomerSetting.Models.Results;
    using CustomerSetting.Services;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Models;
    using NSubstitute;
    using NUnit.Framework;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "Reviewed.")]
    [TestFixture]
    public class ValidationServiceTestsValidateQuerySettingAction
    {
        private ValidationService validationService;
        private IDynamicRepository dynamicRepository;
        private ILogger logger;
        private ProductService productService;
        private ISettingContext settingContext;

        [SetUp]
        public void Setup()
        {
            productService = Substitute.For<ProductService>();
            logger = Substitute.For<ILogger>();
            dynamicRepository = Substitute.For<IDynamicRepository>();
            settingContext = new SettingContextStub();

            validationService = new ValidationServiceImpl(
                productService,
                dynamicRepository,
                logger);
        }

        [Test]
        public void ValidateQuerySetting_QuerySettingParamIsNull_ThrowArgumentNullException()
        {
            //// Arrange
            CustomerSettingQuery query = null;

            //// Act
            Assert.Throws<ArgumentNullException>(
                new TestDelegate(
                    () => validationService.ValidateQuerySettingAction(settingContext, query)));
        }

        private void AssumeCutomerIsDownlineOfActor()
        {
            dynamicRepository.Get<bool>(Arg.Any<CheckBelongingCriteria>()).Returns(true);
        }

        private void AssumeActorStatusIsEnabled()
        {
            dynamicRepository.Get<bool>(Arg.Any<ActorStatusQueryCriteria>()).Returns(true);
        }

        private void AssumeCustomerLevelIsEqualWithValueStoreInDatabase(Level customerLevel)
        {
            dynamicRepository.Get<byte>(Arg.Any<CustomerLevelQueryCriteria>()).Returns(customerLevel.Value);
        }

        [Test, TestCaseSource(nameof(ValidateQuerySetting_ActorHasPermission_Cases))]
        public void ValidateQuerySetting_ActorHasPermission_ReturnSuccessResult(
            Level userLevel,
            Level customerLevel,
            Level uplineLevel)
        {
            //// Arrange
            var query = new CustomerSettingQuery(
                false,
                customerId: 1,
                customerLevel: customerLevel.Value,
                productId: 1);

            settingContext.Actor.Level = userLevel;

            AssumeCutomerIsDownlineOfActor();
            AssumeActorStatusIsEnabled();
            AssumeCustomerLevelIsEqualWithValueStoreInDatabase(query.CustomerLevel);

            //// Act
            var actualResult = validationService
                .ValidateQuerySettingAction(settingContext, query)
                .GetAwaiter()
                .GetResult();

            //// Assert
            Assert.IsTrue(actualResult.IsSuccessful);
        }

        private static readonly object[] ValidateQuerySetting_ActorHasPermission_Cases =
        {
            new object[] { Level.Company, Level.Super },
            new object[] { Level.Company, Level.Master },
            new object[] { Level.Company, Level.Agent },
            new object[] { Level.Company, Level.Member },
            new object[] { Level.Super, Level.Master },
            new object[] { Level.Super, Level.Member },
            new object[] { Level.Master, Level.Agent },
            new object[] { Level.Master, Level.Member },
            new object[] { Level.Agent, Level.Member }
        };

        [Test, TestCaseSource(nameof(ValidateQuerySetting_ActorDoesNotHavePermission_Cases))]
        public void ValidateQuerySetting_ActorDoesNotHavePermission_ReturnFailedResult(
            Level userLevel,
            Level customerLevel)
        {
            //// Arrange
            var query = new CustomerSettingQuery(
                false,
                customerId: 1,
                customerLevel: customerLevel.Value,
                productId: 1);

            settingContext.Actor.Level = userLevel;

            AssumeActorStatusIsEnabled();
            AssumeCustomerLevelIsEqualWithValueStoreInDatabase(query.CustomerLevel);
            AssumeCutomerIsDownlineOfActor();

            //// Act
            var actualResult = validationService
                .ValidateQuerySettingAction(settingContext, query)
                .GetAwaiter()
                .GetResult();

            //// Assert
            Assert.IsFalse(actualResult.IsSuccessful);
        }

        private static readonly object[] ValidateQuerySetting_ActorDoesNotHavePermission_Cases =
        {
            new object[] { Level.Super, Level.Super },
            new object[] { Level.Super, Level.Agent },
            new object[] { Level.Master, Level.Super },
            new object[] { Level.Master, Level.Master },
            new object[] { Level.Agent, Level.Super },
            new object[] { Level.Agent, Level.Master },
            new object[] { Level.Agent, Level.Agent }
        };

        [Test]
        public void ValidateQuerySetting_Always_CallGetActorStatus()
        {
            //// Arrange
            var query = new CustomerSettingQuery(
                false,
                customerId: 1,
                customerLevel: Level.Member.Value,
                productId: 1);

            settingContext.Actor.Level = Level.Agent;

            //// Act
            validationService
                .ValidateQuerySettingAction(settingContext, query)
                .GetAwaiter()
                .GetResult();

            //// Assert
            dynamicRepository.Received().Get<bool>(Arg.Is<ActorStatusQueryCriteria>(criteria => criteria.CustId == settingContext.Actor.Id));
        }

        [Test]
        public void ValidateQuerySetting_ActorIsDisabled_ReturnFailedResult()
        {
            //// Arrange
            var query = new CustomerSettingQuery(
                false,
                customerId: 1,
                customerLevel: Level.Member.Value,
                productId: 1);

            settingContext.Actor.Level = Level.Agent;
            dynamicRepository.Get<bool>(Arg.Any<ActorStatusQueryCriteria>()).Returns(false);

            //// Act
            var actualResult = validationService
                .ValidateQuerySettingAction(settingContext, query)
                .GetAwaiter()
                .GetResult();

            //// Assert
            Assert.IsFalse(actualResult.IsSuccessful);
            Assert.AreEqual(Resources.AccountWasDisabled, actualResult.Message);
        }

        [Test]
        public void ValidateQuerySetting_Always_CallGetCustomerLevel()
        {
            //// Arrange
            var query = new CustomerSettingQuery(
                false,
                customerId: 1,
                customerLevel: Level.Member.Value,
                productId: 1);
            settingContext.Actor.Level = Level.Agent;

            AssumeActorStatusIsEnabled();

            //// Act
            validationService
                .ValidateQuerySettingAction(settingContext, query)
                .GetAwaiter()
                .GetResult();

            //// Assert
            dynamicRepository.Received().Get<byte>(Arg.Is<CustomerLevelQueryCriteria>(criteria => criteria.CustId == query.CustomerId));
        }

        private static readonly object[] ValidateQuerySetting_ActorIsNotCompany_AlwaysCallCheckBelonging_Cases =
        {
            new object[] { Level.Super },
            new object[] { Level.Master },
            new object[] { Level.Agent }
        };

        [Test, TestCaseSource(nameof(ValidateQuerySetting_ActorIsNotCompany_AlwaysCallCheckBelonging_Cases))]
        public void ValidateQuerySetting_ActorIsNotCompany_AlwaysCallCheckBelonging(
            Level actorLevel)
        {
            //// Arrange
            var query = new CustomerSettingQuery(
                false,
                customerId: 1,
                customerLevel: Level.Member.Value,
                productId: 1);
            settingContext.Actor.Level = actorLevel;

            AssumeActorStatusIsEnabled();
            AssumeCustomerLevelIsEqualWithValueStoreInDatabase(query.CustomerLevel);

            //// Act
            validationService
                .ValidateQuerySettingAction(settingContext, query)
                .GetAwaiter()
                .GetResult();

            //// Assert
            dynamicRepository.Received().Get<bool>(Arg.Is<CheckBelongingCriteria>(
                criteria => criteria.ActorId == settingContext.Actor.Id
                    && criteria.CustomerId == query.CustomerId));
        }

        [Test]
        public void ValidateQuerySetting_IsNotSameLine_ReturnAccessDeninedResult()
        {
            //// Arrange
            var query = new CustomerSettingQuery(
                false,
                customerId: 1,
                customerLevel: Level.Member.Value,
                productId: 1);
            settingContext.Actor.Level = Level.Agent;

            AssumeActorStatusIsEnabled();
            AssumeCustomerLevelIsEqualWithValueStoreInDatabase(query.CustomerLevel);

            dynamicRepository.Get<bool>(Arg.Any<CheckBelongingCriteria>()).Returns(false);

            //// Act
            var actualResult = validationService
                .ValidateQuerySettingAction(settingContext, query)
                .GetAwaiter()
                .GetResult();

            //// Assert
            Assert.AreEqual(Result.AccessDenied.Code, actualResult.Code);
            Assert.AreEqual("Customers are not in the same line!", actualResult.Message);
        }

        [Test]
        public void ValidateQuerySetting_ActorIsCompany_DontCallValidateBelongIn()
        {
            //// Arrange
            var query = new CustomerSettingQuery(
                false,
                customerId: 1,
                customerLevel: Level.Member.Value,
                productId: 1);
            settingContext.Actor.Level = Level.Company;

            AssumeActorStatusIsEnabled();
            AssumeCustomerLevelIsEqualWithValueStoreInDatabase(query.CustomerLevel);

            //// Act
            validationService
                .ValidateQuerySettingAction(settingContext, query)
                .GetAwaiter()
                .GetResult();

            //// Assert
            dynamicRepository.DidNotReceive().Get<bool>(Arg.Is<CheckBelongingCriteria>(
                criteria => criteria.ActorId == settingContext.Actor.Id
                    && criteria.CustomerId == query.CustomerId));
        }

        [Test, TestCaseSource(nameof(ValidateQuerySetting_EditingMemberAndActorLevelIsDifferentWithUplineLevel_Cases))]
        public void ValidateQuerySetting_EditingMemberAndActorLevelIsDifferentWithUplineLevel_ReturnAccessDeniedResult(
            Level actorLevel)
        {
            //// Arrange
            var query = new CustomerSettingQuery(
                false,
                customerId: 1,
                customerLevel: Level.Member.Value,
                productId: 1);
            settingContext.Actor.Level = actorLevel;

            AssumeActorStatusIsEnabled();
            AssumeCustomerLevelIsEqualWithValueStoreInDatabase(query.CustomerLevel);
            AssumeCutomerIsDownlineOfActor();

            //// Act
            var actualResult = validationService
                .ValidateQuerySettingAction(settingContext, query)
                .GetAwaiter()
                .GetResult();

            //// Assert
            Assert.AreEqual(Result.AccessDenied.Code, actualResult.Code);
            Assert.AreEqual("Upline Level is not valid.", actualResult.Message);
        }

        private static readonly object[] ValidateQuerySetting_EditingMemberAndActorLevelIsDifferentWithUplineLevel_Cases =
        {
            new object[] { Level.Super },
            new object[] { Level.Master },
            new object[] { Level.Agent }
        };

        private static readonly Level[] ValidateQuerySetting_CompanyEditsCustomerIsNotMemberAndUplineLevelIsNull_Cases =
        {
            Level.Super,
            Level.Master,
            Level.Agent
        };
    }
}