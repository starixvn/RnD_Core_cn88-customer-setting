namespace CN88.CustomerSetting.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CustomerSetting.Models;
    using CustomerSetting.Models.Constants;
    using CustomerSetting.Models.Enumerations;
    using CustomerSetting.Models.Requests;
    using CustomerSetting.Models.Results;
    using CustomerSetting.Services;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Models;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class ValidationServiceTestsValidateUpdateSettingAction
    {
        private ISettingContext settingContext;
        private ValidationService validationService;
        private IDynamicRepository dynamicRepository;
        private ProductService productService;
        private ILogger logger;

        [SetUp]
        public void Setup()
        {
            settingContext = new SettingContextStub();
            productService = Substitute.For<ProductService>();
            dynamicRepository = Substitute.For<IDynamicRepository>();
            logger = Substitute.For<ILogger>();

            validationService = Substitute.ForPartsOf<ValidationServiceImpl>(
                productService,
                dynamicRepository,
                logger);
        }

        private void AssumeValidateQuerySettingIsSuccessful()
        {
            validationService
                .ValidateQuerySettingAction(settingContext, Arg.Any<CustomerSettingQuery>())
                .Returns(Result.Success);
        }

        [Test]
        public void ValidateUpdateSetting_Always_CallValidateQuerySetting()
        {
            //// Arrange
            var updateParam = InstanceFactory.CreateUpdateSettingParam();
            settingContext.Actor.Level = Level.Super;

            //// Act
            validationService.ValidateUpdateSettingAction(settingContext, updateParam);

            //// Assert

            validationService.Received().ValidateQuerySettingAction(
                settingContext,
                Arg.Is<CustomerSettingQuery>(query =>
                    query.CustomerId == updateParam.Setting.CustomerId
                    && query.CustomerLevel == updateParam.Setting.CustomerLevel
                    && query.ProductId == updateParam.Setting.ProductId));
        }

        [Test]
        public async Task ValidateUpdateSetting_Always_CallGetProductSettingTypes()
        {
            //// Arrange
            var updateParam = InstanceFactory.CreateUpdateSettingParam();

            AssumeValidateQuerySettingIsSuccessful();

            //// Act
            await validationService
                .ValidateUpdateSettingAction(settingContext, updateParam);

            //// Assert
            productService.Received().GetProductSettingTypes(
                Arg.Is<byte>(productId => productId == updateParam.Setting.ProductId),
                Arg.Any<byte>());
        }

        [Test]
        public async Task ValidateUpdateSetting_CustomerSettingTypesDontMatchWithProductSettingTypes_ReturnFailedResult()
        {
            //// Arrange
            var updateParam = InstanceFactory.CreateUpdateSettingParam();

            AssumeValidateQuerySettingIsSuccessful();

            ////updateParam.Setting.SettingTypes = new List<SettingType>
            ////{
            ////    InstanceFactory.CreateCustomerSettingType(1)
            ////};
            productService.GetProductSettingTypes(updateParam.Setting.ProductId, (byte)SystemType.Nap)
                .Returns(Task.FromResult(new List<SettingType>
                {
                    InstanceFactory.CreateCustomerSettingType(2)
                } as IEnumerable<SettingType>));

            //// Act
            var actualResult = await validationService
                .ValidateUpdateSettingAction(settingContext, updateParam);

            //// Assert
            Assert.IsFalse(actualResult.IsSuccessful);
        }

        [Test]
        public async Task ValidateUpdateSetting_CustomerSettingTypesMatchWithProductSettingTypes_ReturnSuccessResult()
        {
            //// Arrange
            var updateParam = InstanceFactory.CreateUpdateSettingParam();
            
            AssumeValidateQuerySettingIsSuccessful();

            ////updateParam.Setting.SettingTypes = new List<SettingType>
            ////{
            ////    InstanceFactory.CreateCustomerSettingType(1)
            ////};
            //productService.GetProduct(
            //    settingContext, 
            //    updateParam.Setting.ProductId, 
            //    (byte)SystemType.Nap)
            //    .Returns(Task.FromResult(InstanceFactory.CreateProduct(updateParam.Setting.ProductId)));
            productService.GetProductSettingTypes(updateParam.Setting.ProductId, (byte)SystemType.Nap)
                .Returns(Task.FromResult(updateParam.Setting.SettingTypes));

            //// Act
            var actualResult = await validationService
                .ValidateUpdateSettingAction(settingContext, updateParam);

            //// Assert
            Assert.IsTrue(actualResult.IsSuccessful);
        }

        [Test]
        public async Task ValidateUpdateSetting_AnySettingTypeIsNotEditable_ReturnFailedResult()
        {
            // Arrange.
            var updateParam = InstanceFactory.CreateUpdateSettingParam();

            updateParam.Setting.SettingTypes.First().DisplayMode = DisplayModes.ReadOnly;
            AssumeValidateQuerySettingIsSuccessful();

            // Act.
            var actualResult = await validationService.ValidateUpdateSettingAction(settingContext, updateParam);

            // Assert.
            Assert.IsFalse(actualResult.IsSuccessful);
        }

        [Test]
        public async Task ValidateUpdateSetting_AnySettingTypeDoesNotMeetCustomerLevelConstraint_ReturnFailedResult()
        {
            // Arrange.
            var updateParam = InstanceFactory.CreateUpdateSettingParam(customerLevel: Level.Agent);

            settingContext.Actor.Level = Level.Super;
            AssumeValidateQuerySettingIsSuccessful();

            // Act.
            var actualResult = await validationService.ValidateUpdateSettingAction(settingContext, updateParam);

            // Assert.
            Assert.IsFalse(actualResult.IsSuccessful);
        }
    }
}