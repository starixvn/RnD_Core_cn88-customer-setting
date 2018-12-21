namespace CN88.CustomerSetting.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CN88.CustomerSetting.SpParams.Criteria;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Models;

    public interface ProductService
    {
        IEnumerable<SettingType> GetProductSettingTypes(byte productId, byte systemId);

        SettingType GetProductSettingType(byte productId, short settingTypeId, byte systemId);

        IEnumerable<Product> GetAllProducts();
    }

    public class ProductServiceImpl : ProductService
    {
        private const int CacheDuration = 1; //// 1 hour
        private const string SettingTypesCacheKey = "CN88.CustomerSetting.SettingTypesCacheKey_{0}_{1}";
        private const string AllProductNamesCacheKey = "CN88.CustomerSetting.AllProductNamesCacheKey";

        private readonly IDynamicRepository _productRepository;
        private readonly ICacheService _cachingService;

        public ProductServiceImpl(IDynamicRepository productRepository = null, ICacheService cachingService = null)
        {
            _productRepository = productRepository ?? new DynamicRepository();
            _cachingService = cachingService ?? new CacheService();
        }

        public IEnumerable<SettingType> GetProductSettingTypes(byte productId, byte systemId)
        {
            var settingTypes = _cachingService.GetOrSet(
                   string.Format(SettingTypesCacheKey, productId, systemId),
                   () =>
                   {
                       return _productRepository.Fetch<SettingType>(
                           new ProductSettingTypesQueryCriteria
                           {
                               ProductId = productId,
                               SystemId = systemId
                           });
                   },
                    new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(CacheDuration, 0, 0)));

            return settingTypes;
        }

        public SettingType GetProductSettingType(byte productId, short settingTypeId, byte systemId)
        {
            var productSettingTypes = GetProductSettingTypes(productId, systemId);

            if (productSettingTypes == null)
            {
                return null;
            }

            return productSettingTypes.FirstOrDefault(settingType => settingType.Id == settingTypeId);
        }

        public IEnumerable<Product> GetAllProducts()
        {
            var products = _cachingService.GetOrSet(
                  AllProductNamesCacheKey,
                  () =>
                  {
                      return _productRepository
                          .Fetch<ProductName>(new ProductNamesQueryCriteria { LanguageKey = LanguageNameConverter.GetRefLanguageName() })
                          .Select(productName => new Product(productName.ProductId, productName.Name));
                  },
                  new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(CacheDuration, 0, 0)));

            return products;
        }
    }
}