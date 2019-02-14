namespace CN88.CustomerSetting.UnitTests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CN88.CustomerSetting.Exceptions;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Requests;
    using CN88.CustomerSetting.Models.Results;
    using CN88.CustomerSetting.Services;
    using CN88.CustomerSetting.Tests;
    using CN88.CustomerSetting.Tests.Models;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture(Category = "Core")]
    public class QuerySettingServiceTests
    {
        private QuerySettingService querySettingService;
        private ISettingContext settingContext;
        private ValidationService validationService;
        private ProductService productService;
        private ILogger logger;
        private SettingTypeServiceFactory settingTypeServiceFactory;

        private SettingTypeService settingTypeService;

        [SetUp]
        public void Setup()
        {
            settingContext = new SettingContextStub();
            productService = Substitute.For<ProductService>();
            validationService = Substitute.For<ValidationService>();
            logger = Substitute.For<ILogger>();
            settingTypeServiceFactory = Substitute.For<SettingTypeServiceFactory>();

            settingTypeService = Substitute.For<SettingTypeService>();

            settingContext = new SettingContextStub();

            querySettingService = new QuerySettingServiceImpl(
                validationService,
                logger,
                productService,
                settingTypeServiceFactory);
        }

        [Test]
        public void GetCustomerSetting_QuerySettingParamIsNull_ThrowArgumentNullException()
        {
            //// Arranges
            CustomerSettingQuery querySettingParam = null;

            //// Act
            Assert.Throws<ArgumentNullException>(
                () => querySettingService.GetCustomerSetting(settingContext, querySettingParam).GetAwaiter().GetResult());
        }

        [Test]
        public void GetCustomerSetting_Always_CallValidateQuerySetting()
        {
            //// Arrange
            CustomerSettingQuery querySettingParam = InstanceFactory.CreateQuerySettingParam(false);
            StubProduct(querySettingParam.ProductId);
            SetupSettingTypeServiceStub();
            validationService
                .ValidateQuerySettingAction(settingContext, Arg.Any<CustomerSettingQuery>())
                .Returns(Task.FromResult(Result.Success as IResult));


            //// Act
            querySettingService
                .GetCustomerSetting(settingContext, querySettingParam)
                .GetAwaiter()
                .GetResult();

            //// Assert
            validationService.Received()
                .ValidateQuerySettingAction(
                settingContext, 
                Arg.Is<CustomerSettingQuery>(
                actualQuerySettingParam => actualQuerySettingParam.CustomerId == querySettingParam.CustomerId
                    && actualQuerySettingParam.CustomerLevel == querySettingParam.CustomerLevel
                    && actualQuerySettingParam.ProductId == querySettingParam.ProductId));
        }

        [Test]
        public void GetCustomerSetting_QuerySettingActionIsNotValid_ThrowAccessViolationException()
        {
            //// Arrange
            CustomerSettingQuery querySettingParam = InstanceFactory.CreateQuerySettingParam(false);
            validationService.ValidateQuerySettingAction(settingContext, Arg.Any<CustomerSettingQuery>()).Returns(Result.AccessDenied);

            //// Act
            Assert.ThrowsAsync<AccessViolationException>(
                async () => await querySettingService
                    .GetCustomerSetting(settingContext, querySettingParam));
        }

        private void AssumeQuerySettingActionIsValid(CustomerSettingQuery customerSettingQuery)
        {
            validationService.ValidateQuerySettingAction(settingContext, customerSettingQuery).Returns(Result.Success);
        }


        [Test]
        public async Task GetCustomerSetting_CustomerIsValid_ReturnCustomerId()
        {
            //// Arrange
            CustomerSettingQuery querySettingParam = InstanceFactory.CreateQuerySettingParam(false);
            AssumeQuerySettingActionIsValid(querySettingParam);
            StubProduct(querySettingParam.ProductId, false);

            //// Act
            var actualCustomerSetting = await querySettingService
                .GetCustomerSetting(settingContext, querySettingParam);

            //// Assert
            Assert.AreEqual(querySettingParam.CustomerId, actualCustomerSetting.CustomerId);
        }

        [Test]
        public async Task GetCustomerSetting_CustomerIsValid_ReturnCustomerLevel()
        {
            //// Arrange
            CustomerSettingQuery querySettingParam = InstanceFactory.CreateQuerySettingParam(false);
            AssumeQuerySettingActionIsValid(querySettingParam);
            StubProduct(querySettingParam.ProductId, false);

            //// Act
            var actualCustomerSetting = await querySettingService
                .GetCustomerSetting(settingContext, querySettingParam);

            //// Assert
            Assert.AreEqual(querySettingParam.CustomerLevel, actualCustomerSetting.CustomerLevel);
        }


        [Test]
        public async Task GetCustomerSetting_Always_CallGetProductSettingTypes()
        {
            //// Arrange
            CustomerSettingQuery querySettingParam = InstanceFactory.CreateQuerySettingParam(false);
            AssumeQuerySettingActionIsValid(querySettingParam);
            StubProduct(querySettingParam.ProductId, false);

            //// Act
            await querySettingService
                .GetCustomerSetting(settingContext, querySettingParam);

            //// Assert
            await productService.Received().GetProduct(
                settingContext,
                querySettingParam.ProductId,
                Arg.Any<byte>());
        }

        [Test]
        public async Task GetCustomerSetting_ProductHasSettingTypes_ReturnCustomerSettingTypesMatchedWithProduct()
        {
            //// Arrange
            CustomerSettingQuery querySettingParam = InstanceFactory.CreateQuerySettingParam(false);
            AssumeQuerySettingActionIsValid(querySettingParam);

            var productSettingTypes = InstanceFactory.CreateProduct().SettingTypes;
            StubProduct(querySettingParam.ProductId);

            productSettingTypes.First().SettingItems = new List<SettingItemStub> { new SettingItemStub() };
            SetupSettingTypeServiceStub(productSettingTypes.First());

            //// Act
            var actualCustomerSetting = await querySettingService
                .GetCustomerSetting(settingContext, querySettingParam);

            //// Assert
            Assert.AreEqual(productSettingTypes.First().Id, actualCustomerSetting.SettingTypes.First().Id);
        }

        [Test]
        public async Task GetCustomerSetting_Always_ReturnProductId()
        {
            //// Arrange
            CustomerSettingQuery querySettingParam = InstanceFactory.CreateQuerySettingParam(false);
            StubProduct(querySettingParam.ProductId, false);
      
            AssumeQuerySettingActionIsValid(querySettingParam);

            //// Act
            var actualCustomerSetting = await querySettingService
                .GetCustomerSetting(settingContext, querySettingParam);

            //// Assert
            Assert.AreEqual(querySettingParam.ProductId, actualCustomerSetting.ProductId);
        }

        private IEnumerable<SettingType> SetupProductSettingTypesStub(
            IEnumerable<SettingType> dummySettingTypes = null)
        {
            var productSettingTypes = dummySettingTypes ?? InstanceFactory.CreateProduct().SettingTypes;
            productService.GetProductSettingTypes(Arg.Any<byte>(), Arg.Any<byte>()).Returns(productSettingTypes);

            return productSettingTypes;
        }

        [Test]
        public async Task GetCustomerSetting_Always_CallResolveSettingTypeService()
        {
            //// Arrange
            CustomerSettingQuery querySettingParam = InstanceFactory.CreateQuerySettingParam(false);
            AssumeQuerySettingActionIsValid(querySettingParam);
            StubProduct(querySettingParam.ProductId);
            var dummySettingTypes = SetupProductSettingTypesStub();
            SetupSettingTypeServiceStub();

            //// Act
            await querySettingService
                .GetCustomerSetting(settingContext, querySettingParam);

            //// Assert
            settingTypeServiceFactory.Received(dummySettingTypes.Count()).GetInstance(Arg.Any<SettingType>());
        }

        [Test]
        public async Task GetCustomerSetting_Always_CallGetCustomerSettingType()
        {
            //// Arrange
            CustomerSettingQuery querySettingParam = InstanceFactory.CreateQuerySettingParam(false);
            AssumeQuerySettingActionIsValid(querySettingParam);
            StubProduct(querySettingParam.ProductId);
            var productSettingTypes = SetupProductSettingTypesStub();
            SetupSettingTypeServiceStub();

            //// Act
            await querySettingService
                .GetCustomerSetting(settingContext, querySettingParam);

            //// Assert
            await settingTypeService.Received(productSettingTypes.Count()).GetCustomerSettingType(settingContext, querySettingParam);
        }

        private void SetupSettingTypeServiceStub(SettingType dummySettingType = null)
        {
            var customerSettingType = dummySettingType ?? InstanceFactory.CreateCustomerSettingType(1);
            settingTypeService
                .GetCustomerSettingType(Arg.Any<ISettingContext>(), Arg.Any<CustomerSettingQuery>())
                .Returns(Task.FromResult(customerSettingType));
            settingTypeServiceFactory.GetInstance(Arg.Any<SettingType>()).Returns(settingTypeService);
        }

        [Test]
        public async Task GetCustomerSetting_RaisedException_AlwaysCallLogError()
        {
            //// Arrange
            CustomerSettingQuery querySettingParam = InstanceFactory.CreateQuerySettingParam(false);
            AssumeQuerySettingActionIsValid(querySettingParam);
            StubProduct(querySettingParam.ProductId);
            SetupProductSettingTypesStub();

            var expectedException = new DllNotFoundException("Dll not found");
            settingTypeServiceFactory
                .GetInstance(Arg.Any<SettingType>()).Returns(x => { throw expectedException; });

            //// Act
            await querySettingService
                .GetCustomerSetting(settingContext, querySettingParam);

            //// Assert
            logger.Received().Error(
                Arg.Is<string>(message => message.Contains(expectedException.Message)),
                Arg.Any<CustomerSettingException>());
        }

        private void StubProduct(byte productId, bool includedSettingTypes = true)
        {
            productService.GetProduct(
                settingContext, 
                productId, 
                Arg.Any<byte>())
                .Returns(Task.FromResult(InstanceFactory.CreateProduct(productId, includedSettingTypes)));
        }

    }
}
