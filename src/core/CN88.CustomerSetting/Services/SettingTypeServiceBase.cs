namespace CN88.CustomerSetting.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CN88.CustomerSetting.Exceptions;
    using CN88.CustomerSetting.SpParams.Criteria;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.RemotingRef;
    using Models;
    using Models.Constants;
    using Models.Enumerations;
    using Models.Requests;
    using Models.Results;

    public interface SettingTypeService
    {
        Task<SettingType> GetCustomerSettingType(
            ISettingContext settingContext,
            CustomerSettingQuery query);

        Task<SettingResult> UpdateCustomerSettingType(
            ISettingContext settingContext,
            UpdateCustomerSettingCommand command,
            SettingType settingType);

        IDictionary<string, object> BuildCreateCustomerParams(
            ISettingContext settingContext,
            CreateCustomerCommand command,
            SettingType settingType);

        SettingResult TranslateResultMessage(
                    SettingResult result,
                    string updateSuccessfulMessage,
                    byte settingLevel,
                    string language);
    }

    public abstract class SettingTypeServiceBase : SettingTypeService
    {
        private const int SettingTypeConfigCacheTimeInHours = 1;
        private const string SettingTypeConfigCacheKey = "SettingTypeConfigCacheKey_{0}_{1}";

        protected SettingTypeServiceBase()
            : this(null, null, null, null)
        {
        }

        protected SettingTypeServiceBase(
            ResourceService resourceService,
            ILogger logger,
            ICacheService cacheService,
            IDynamicRepository dynamicRepository)
        {
            ResourceService = resourceService ?? new ResourceServiceImpl(new RemotingRefService());
            Logger = logger ?? LogService.LoggerInstance;
            CacheService = cacheService ?? new CacheService();
            DynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        protected ResourceService ResourceService { get; set; }

        protected ILogger Logger { get; set; }

        protected MessageService MessageService { get; set; }

        protected ICacheService CacheService { get; set; }

        protected IDynamicRepository DynamicRepository { get; set; }

        /// <summary>
        /// Reads the update customer setting type message,
        /// after updated customer setting type.
        /// </summary>
        /// <param name="result">The result return from data access.</param>
        /// <param name="updateSuccessfulMessage">The update successful message for each setting type updated
        /// May be Position Taking, bet limit ... successful message.</param>
        /// <param name="settingLevel">The setting level for displaying message.</param>
        /// <param name="language">The language for globalization.</param>
        /// <returns>Result is processed.</returns>
        public virtual SettingResult TranslateResultMessage(
                    SettingResult result,
                    string updateSuccessfulMessage,
                    byte settingLevel,
                    string language)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (result.IsSuccessful)
            {
                return new SettingResult(ResultValues.Success, updateSuccessfulMessage, result);
            }

            var messageContent = MessageService.TranslateMessage(result.MsgType, result.Message, settingLevel, language);

            return new SettingResult(result, messageContent.MainMessage, messageContent.SubMessages);
        }

        /// <summary>
        /// Sets the name of the sport type bet type for setting item using sport type bet type for indentity.
        /// This function will use by post get setting task of setting type.
        /// </summary>
        /// <param name="settingType">Type of the setting, includes list of setting items.</param>
        /// <returns>Setting type with setting item have sport, bet type name.</returns>
        protected SettingType SetSportTypeBetTypeName(SettingType settingType, string language)
        {
            if (settingType != null && settingType.SettingItems.Any())
            {
                var defaultSettingItems = new List<SettingItemDefault>();

                foreach (ISettingItem settingItem in settingType.SettingItems)
                {
                    var defaultSettingItem = settingItem as SettingItemDefault;

                    if (defaultSettingItem != null)
                    {
                        defaultSettingItem.BetTypeName = ResourceService.GetBetTypeName(defaultSettingItem.BetTypeId, defaultSettingItem.IsLive);
                        defaultSettingItem.SportTypeName = ResourceService.GetSportTypeName(defaultSettingItem.SportTypeId, language);
                        defaultSettingItems.Add(defaultSettingItem);
                    }
                    else
                    {
                        throw new InvalidCastException("Invalid cast default setting item.");
                    }
                }

                settingType.SettingItems = defaultSettingItems;
            }

            return settingType;
        }

        protected SettingResult CreateSettingResult(IResult result, int customerId, SettingType settingType)
            => new SettingResult(
                result.Code,
                result.Message,
                result.MsgType,
                customerId,
                settingType.Id,
                settingType.Name);

        protected async Task<IEnumerable<T>> GetSettingTypeConfig<T>(int productId, int settingTypeId)
            => await CacheService.GetOrSetAsync(
                       string.Format(SettingTypeConfigCacheKey, productId, settingTypeId),
                       () => DynamicRepository.Fetch<T>(new GetSettingTypeConfigCriteria(productId, settingTypeId)),
                        new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(SettingTypeConfigCacheTimeInHours, 0, 0)));

        public abstract Task<SettingType> GetCustomerSettingType(
            ISettingContext settingContext,
            CustomerSettingQuery query);

        public abstract Task<SettingResult> UpdateCustomerSettingType(
            ISettingContext settingContext,
            UpdateCustomerSettingCommand command,
            SettingType settingType);

        private void WriteLog(
            ISettingContext settingContext,
            int customerId,
            Level customerLevel,
            byte productId,
            SettingType customerSettingType,
            Exception ex)
        {
            var customerSettingException = new CustomerSettingException(ex.Message, ex);
            customerSettingException.SetSettingContextInfo("Update Setting", settingContext);
            customerSettingException.SetCustomerInfo(customerId, customerLevel);
            customerSettingException.SetProductInfo(productId, customerSettingType);

            Logger.Error(customerSettingException.Message, customerSettingException);
        }

        public abstract IDictionary<string, object> BuildCreateCustomerParams(ISettingContext settingContext, CreateCustomerCommand command, SettingType settingType);
    }
}