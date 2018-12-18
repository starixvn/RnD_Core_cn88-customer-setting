namespace CN88.CustomerSetting.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using CN88.CustomerSetting.Exceptions;
    using Fanex.Logging;
    using Models;
    using Models.Constants;
    using Models.Enumerations;
    using Models.Requests;
    using Models.Results;

    public interface SettingTypeService
    {
        SettingType GetCustomerSettingType(
            ISettingContext settingContext,
            CustomerSettingQuery query);

        SettingResult UpdateCustomerSettingType(
            ISettingContext settingContext,
            int customerId,
            Level customerLevel,
            Level uplineLevel,
            byte productId,
            SettingType settingType);

        Hashtable GetInsertCustomerParams(
            ISettingContext settingContext,
            int customerId,
            Level customerLevel,
            Level uplineLevel,
            byte productId,
            SettingType settingType);
    }

    public abstract class SettingTypeServiceBase : SettingTypeService
    {
        protected SettingTypeServiceBase()
            : this(null, null)
        {
        }

        protected SettingTypeServiceBase(
            ResourceService resourceService,
            ILogger logger)
        {
            ResourceService = resourceService;
            Logger = logger;
        }

        protected ResourceService ResourceService { get; set; }

        protected ILogger Logger { get; set; }

        protected MessageService MessageService { get; set; }

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
        protected SettingResult TranslateUpdateResultMessage(
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
            else
            {
                var messageContent = MessageService.TranslateMessage(result.MsgType, result.Message, settingLevel, language);

                return new SettingResult(result, messageContent.MainMessage, messageContent.SubMessages);
            }
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
                        defaultSettingItem.BetTypeName = ResourceService.GetBetTypeName(defaultSettingItem.BetTypeId, language);
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
        {
            return new SettingResult(
                result.Code,
                result.Message,
                result.MsgType,
                customerId,
                settingType.Id,
                settingType.Name);
        }

        protected void SetSettingTypeReadOnlyWhenIndirectUplineEditMember(SettingType settingType, CustomerSettingQuery query)
        {
            if (query.CustomerLevel.IsMemberLevel()
                && query.UplineLevel.IsNotAgentLevel())
            {
                settingType.DisplayMode = DisplayModes.ReadOnly;
            }
        }

        public abstract SettingType GetCustomerSettingType(
            ISettingContext settingContext,
            CustomerSettingQuery query);

        public abstract SettingResult UpdateCustomerSettingType(
            ISettingContext settingContext,
            int customerId,
            Level customerLevel,
            Level uplineLevel,
            byte productId,
            SettingType settingType);

        private void WriteLog(
            ISettingContext settingContext,
            int customerId,
            Level customerLevel,
            Level uplineLevel,
            byte productId,
            SettingType customerSettingType,
            Exception ex)
        {
            var customerSettingException = new CustomerSettingException(ex.Message, ex);
            customerSettingException.SetSettingContextInfo("Update Setting", settingContext);
            customerSettingException.SetCustomerInfo(customerId, customerLevel, uplineLevel);
            customerSettingException.SetProductInfo(productId, customerSettingType);

            Logger.Error(customerSettingException.Message, customerSettingException);
        }

        public abstract Hashtable GetInsertCustomerParams(ISettingContext settingContext, int customerId, Level customerLevel, Level uplineLevel, byte productId, SettingType settingType);
    }
}