namespace CN88.CustomerSetting.Services
{
    using System;
    using System.Collections.Concurrent;
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
        ISetting GetCustomerSetting(ISettingContext settingContext, CustomerSettingQuery query);
    }

    public class QuerySettingServiceImpl : QuerySettingService
    {
        public QuerySettingServiceImpl(
            ValidationService validationService,
            IDynamicRepository dynamicRepository,
            ILogger logger,
            ProductService productService,
            SettingTypeServiceFactory settingTypeServiceFactory)
        {
            ValidationService = validationService;
            DynamicRepository = dynamicRepository;
            Logger = logger;
            ProductService = productService;
            SettingTypeServiceFactory = settingTypeServiceFactory;
        }

        public ISetting GetCustomerSetting(ISettingContext settingContext, CustomerSettingQuery query)
        {
            //Validate(settingContext, query);

            var product = ProductService.GetProduct(settingContext, query.ProductId, settingContext.System);
            var customerSettingTypes = GetCustomerSettingTypes(settingContext, query, product.SettingTypes);

            var setting = new Setting(
                query.CustomerId,
                query.CustomerLevel,
                product.Id,
                product.Name,
                customerSettingTypes);

            return setting;
        }

        protected ValidationService ValidationService { get; }

        protected ProductService ProductService { get; }

        protected ILogger Logger { get; }

        protected SettingTypeServiceFactory SettingTypeServiceFactory { get; }

        protected IDynamicRepository DynamicRepository { get; }

        protected virtual void Validate(ISettingContext settingContext, CustomerSettingQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var validationResult = ValidationService.ValidateQuerySettingAction(settingContext, query);

            if (validationResult.IsNotOk)
            {
                throw new AccessViolationException(validationResult.Message);
            }
        }

        protected virtual IList<SettingType> GetCustomerSettingTypes(
            ISettingContext settingContext,
            CustomerSettingQuery query, 
            IEnumerable<SettingType> productSettingTypes)
        {
            var customerSettingTypes = new ConcurrentBag<SettingType>();

            Parallel.ForEach(
                productSettingTypes,
                productSettingType =>
                {
                    SettingType customerSettingType = GetCustomerSettingType(settingContext, query, productSettingType);

                    customerSettingTypes.Add(customerSettingType);
                });

            return customerSettingTypes
                .OrderBy(settingType => settingType.DisplayOrder)
                .ToList();
        }

        protected virtual SettingType GetCustomerSettingType(
            ISettingContext settingContext,
            CustomerSettingQuery query, 
            SettingType productSettingType)
        {
            SettingType customerSettingType = productSettingType;

            try
            {
                var settingTypeService = SettingTypeServiceFactory.GetInstance(productSettingType);

                customerSettingType = settingTypeService.GetCustomerSettingType(settingContext, query);
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

            Logger.Error(exception.Message, exception);
        }
    }
}