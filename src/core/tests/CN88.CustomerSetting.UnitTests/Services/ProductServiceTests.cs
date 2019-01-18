namespace CN88.CustomerSetting.UnitTests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Enumerations;
    using CN88.CustomerSetting.Services;
    using CN88.CustomerSetting.SpParams.Criteria;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using NSubstitute;
    using NUnit.Framework;

    public class ProductServiceTests
    {
        private ProductService productService;
        private IDynamicRepository dynamicRepository;
        private ICacheService cachingService;
        private ResourceService resourceService;

        [SetUp]
        public void SetUp()
        {
            resourceService = Substitute.For<ResourceService>();
            dynamicRepository = Substitute.For<IDynamicRepository>();
            cachingService = Substitute.For<ICacheService>();
            productService = new ProductServiceImpl(dynamicRepository, cachingService, resourceService);
        }

        [Test]
        public void GetProductSettingTypes_ProductIdIsZero_ThrowArgumentOutOfRangeException()
        {
            //// Arrange
            byte productId = 0;

            //// Act
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                async () => await productService.GetProductSettingTypes(productId, Arg.Any<byte>()));
        }

        [Test]
        public async Task GetProductSettingTypes_Always_ReturnSettingTypesInCache()
        {
            //// Arrange
            byte productId = 1;
            IEnumerable<SettingType> settingTypes = new List<SettingType> { new SettingType() };
            cachingService.GetOrSetAsync(
              Arg.Any<string>(),
              Arg.Any<Func<IEnumerable<SettingType>>>(),
              Arg.Any<CacheItemOptions>())
              .Returns(settingTypes);

            //// Act
            var actualSettingTypes = await productService
                .GetProductSettingTypes(productId, (byte)SystemType.Nap);

            //// Assert
            Assert.AreSame(settingTypes, actualSettingTypes);
        }

        [Test]
        public async Task GetProductSettingTypes_NoDataInCache_ReturnSettingTypesFromDatabase()
        {
            //// Arrange
            byte productId = 1;
            IEnumerable<SettingType> settingTypesFromDatabase = new List<SettingType> { new SettingType() };
            var cachingServiceStub = new CacheService();
            cachingServiceStub.Remove("Fanex.CustomerSetting.SettingTypesCacheKey_1_1");

            var productServiceImpl = new ProductServiceImpl(dynamicRepository, cachingServiceStub, resourceService);

            dynamicRepository
                .Fetch<SettingType>(Arg.Any<ProductSettingTypesQueryCriteria>())
                .Returns(settingTypesFromDatabase);

            //// Act
            var actualSettingTypes = await productServiceImpl
                .GetProductSettingTypes(productId, (byte)SystemType.Nap);

            //// Assert
            Assert.AreSame(settingTypesFromDatabase, actualSettingTypes);
        }

        /// <summary>
        /// Get single setting types
        /// </summary>
        [Test]
        public async Task GetProductSettingType_Always_ReturnSettingTypeInCache()
        {
            //// Arrange
            byte productId = 1;
            byte settingType = 1;
            IEnumerable<SettingType> settingTypes = new List<SettingType> { new SettingType { Id = 1, SettingLevel = 1 } };
            cachingService.GetOrSetAsync(
                Arg.Any<string>(),
                Arg.Any<Func<IEnumerable<SettingType>>>(),
                Arg.Any<CacheItemOptions>())
                .Returns(settingTypes);

            //// Act
            var actualSettingType = await productService
                .GetProductSettingType(productId, settingType, (byte)SystemType.Nap);

            //// Assert
            Assert.AreSame(settingTypes.First(), actualSettingType);
        }

        [Test]
        public async Task GetProductSettingType_NoData_ReturnNull()
        {
            //// Arrange
            byte productId = 1;
            byte settingType = 1;
            IEnumerable<SettingType> settingTypesInCache = null;
            IEnumerable<SettingType> settingTypesFromDatabase = new List<SettingType>();

            cachingService.GetOrSetAsync<IEnumerable<SettingType>>(
                Arg.Any<string>(),
                Arg.Any<Func<IEnumerable<SettingType>>>(),
                Arg.Any<CacheItemOptions>())
               .Returns(settingTypesInCache);

            dynamicRepository
                .FetchAsync<SettingType>(Arg.Any<ICriteria>())
                .Returns(Task.FromResult(settingTypesFromDatabase));

            //// Act
            var actualSettingType = await productService
                .GetProductSettingType(productId, settingType, (byte)SystemType.Nap);

            //// Assert
            Assert.Null(actualSettingType);
        }
    }
}