namespace CN88.CustomerSetting.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CN88.CustomerSetting.Exceptions;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Constants;
    using CN88.CustomerSetting.Models.Requests;
    using Fanex.Data.Repository;
    using Fanex.Logging;

    public interface QuerySettingService
    {
        Task<ISetting> GetCustomerSetting(ISettingContext settingContext, CustomerSettingQuery query);
    }

    public class QuerySettingServiceImpl : QuerySettingService
    {
        private readonly ValidationService validationService;
        private readonly ProductService productService;
        private readonly ILogger logger;
        private readonly SettingTypeServiceFactory settingTypeServiceFactory;
        private readonly IDynamicRepository dynamicRepository;

        public QuerySettingServiceImpl(
            ValidationService validationService,
            IDynamicRepository dynamicRepository,
            ILogger logger,
            ProductService productService,
            SettingTypeServiceFactory settingTypeServiceFactory)
        {
            this.validationService = validationService;
            this.dynamicRepository = dynamicRepository;
            this.logger = logger;
            this.productService = productService;
            this.settingTypeServiceFactory = settingTypeServiceFactory;
        }

        public async Task<ISetting> GetCustomerSetting(ISettingContext settingContext, CustomerSettingQuery query)
        {
            await Validate(settingContext, query);

            var product = await productService.GetProduct(settingContext, query.ProductId, settingContext.System);
            var customerSettingTypes = await GetCustomerSettingTypes(settingContext, query, product.SettingTypes);

            var setting = new Setting(
                query.CustomerId,
                query.CustomerLevel,
                product.Id,
                product.Name,
                customerSettingTypes);

            return setting;
        }

        protected virtual async Task Validate(ISettingContext settingContext, CustomerSettingQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var result = await validationService.ValidateQuerySettingAction(settingContext, query);

            if (result.IsNotOk)
            {
                throw new AccessViolationException(result.Message);
            }
        }

        protected virtual async Task<IList<SettingType>> GetCustomerSettingTypes(
            ISettingContext settingContext,
            CustomerSettingQuery query,
            IEnumerable<SettingType> productSettingTypes)
        {
            var customerSettingTypes = await Task.WhenAll(
                productSettingTypes.Select(
                    productSettingType => GetCustomerSettingType(settingContext, query, productSettingType)));

            return customerSettingTypes
                .OrderBy(settingType => settingType.DisplayOrder)
                .ToList();
        }

        protected virtual async Task<SettingType> GetCustomerSettingType(
            ISettingContext settingContext,
            CustomerSettingQuery query,
            SettingType productSettingType)
        {
            SettingType customerSettingType = productSettingType;

            try
            {
                var settingTypeService = settingTypeServiceFactory.GetInstance(productSettingType);

                customerSettingType = await settingTypeService.GetCustomerSettingType(settingContext, query);
                customerSettingType.Id = productSettingType.Id;
                customerSettingType.SettingLevel = productSettingType.SettingLevel;
                customerSettingType.DisplayOrder = productSettingType.DisplayOrder;

                if (customerSettingType.DisplayMode != DisplayModes.ReadOnly)
                {
                    customerSettingType.DisplayMode = productSettingType.DisplayMode;
                }
            }
            catch (Exception ex)
            {
                WriteLog(settingContext, query, productSettingType, ex);

                customerSettingType.IsError = true;
                customerSettingType.SettingItems = new List<ISettingItem> { new SettingItemError() };
            }

            return customerSettingType;
        }

        protected virtual void WriteLog(
            ISettingContext settingContext,
            CustomerSettingQuery query,
            SettingType productSettingType,
            Exception ex)
        {
            var exception = new CustomerSettingException(ex.Message, ex);

            exception.SetSettingContextInfo("Query Setting", settingContext);
            exception.SetCustomerInfo(query.CustomerId, query.CustomerLevel);
            exception.SetProductInfo(query.ProductId, productSettingType);

            logger.Error(exception.Message, exception);
        }
    }
}