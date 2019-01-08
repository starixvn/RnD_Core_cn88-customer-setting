namespace CN88.CustomerSetting.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CN88.CustomerSetting.Helpers;
    using CN88.CustomerSetting.Models.Constants;
    using CN88.CustomerSetting.SpParams.Criteria;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Models;

    public interface ProductService
    {
        Task<IEnumerable<SettingType>> GetProductSettingTypes(byte productId, byte systemId);

        Task<SettingType> GetProductSettingType(byte productId, short settingTypeId, byte systemId);

        Task<IEnumerable<Product>> GetAllProducts();

        Task<Product> GetProduct(ISettingContext settingContext, byte productId, byte systemId);
    }

    public class ProductServiceImpl : ProductService
    {
        private const int CacheDuration = 1; //// 1 hour
        private const string SettingTypesCacheKey = "CN88.CustomerSetting.SettingTypesCacheKey_{0}_{1}";
        private const string AllProductNamesCacheKey = "CN88.CustomerSetting.AllProductNamesCacheKey";

        private readonly IDynamicRepository productRepository;
        private readonly ICacheService cachingService;
        private readonly ResourceService resourceService;

        public ProductServiceImpl(
            IDynamicRepository productRepository,
            ICacheService cachingService,
            ResourceService resourceService)
        {
            this.productRepository = productRepository;
            this.cachingService = cachingService;
            this.resourceService = resourceService;
        }

        public async Task<IEnumerable<SettingType>> GetProductSettingTypes(byte productId, byte systemId)
        {
            InputValidator.Validate(nameof(productId), productId);
            InputValidator.Validate(nameof(systemId), systemId);

            var settingTypes = await cachingService.GetOrSetAsync(
                   string.Format(SettingTypesCacheKey, productId, systemId),
                   () =>
                   {
                       return productRepository.Fetch<SettingType>(
                           new ProductSettingTypesQueryCriteria
                           {
                               ProductId = productId,
                               SystemId = systemId
                           });
                   },
                    new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(CacheDuration, 0, 0)));

            return settingTypes;
        }

        public async Task<SettingType> GetProductSettingType(byte productId, short settingTypeId, byte systemId)
        {
            var productSettingTypes = await GetProductSettingTypes(productId, systemId);

            if (productSettingTypes == null)
            {
                return null;
            }

            return productSettingTypes.FirstOrDefault(settingType => settingType.Id == settingTypeId);
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            var products = await cachingService.GetOrSetAsync(
                  AllProductNamesCacheKey,
                  () =>
                  {
                      return productRepository
                          .Fetch<ProductName>(new ProductNamesQueryCriteria { LanguageKey = LanguageNameConverter.GetRefLanguageName() })
                          .Select(productName => new Product(productName.ProductId, productName.Name));
                  },
                  new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(CacheDuration, 0, 0)));

            return products;
        }

        public async Task<Product> GetProduct(ISettingContext settingContext, byte productId, byte systemId)
        {
            var productSettingTypes = (await GetProductSettingTypes(productId, systemId))
               .Where(product => product.DisplayMode != DisplayModes.Hidden);
            var productName = resourceService.GetProductName(productId, settingContext.Language);

            return new Product(productId, productName, productSettingTypes);
        }
    }
}