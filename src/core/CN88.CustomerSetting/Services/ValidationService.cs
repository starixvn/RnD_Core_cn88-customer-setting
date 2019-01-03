namespace CN88.CustomerSetting.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CN88.CustomerSetting.AppResources;
    using CN88.CustomerSetting.Exceptions;
    using CN88.CustomerSetting.SpParams.Criteria;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Models;
    using Models.Constants;
    using Models.Enumerations;
    using Models.Requests;
    using Models.Results;

    public interface ValidationService
    {
        Task<IResult> ValidateQuerySettingAction(ISettingContext settingContext, CustomerSettingQuery query);

        Task<IResult> ValidateUpdateSettingAction(ISettingContext settingContext, UpdateCustomerSettingCommand command);

        Task<IResult> ValidateCreateCustomerAction(ISettingContext settingContext, CreateCustomerCommand command);
    }

    public class ValidationServiceImpl : ValidationService
    {
        private readonly ProductService productService;
        private readonly IDynamicRepository dynamicRepository;
        private readonly ILogger logger;

        public ValidationServiceImpl(
            ProductService productService,
            IDynamicRepository dynamicRepository,
            ILogger logger)
        {
            this.productService = productService;
            this.dynamicRepository = dynamicRepository;
            this.logger = logger;
        }

        /// <summary>
        /// Validate Query action is valid or not, this function also using by
        /// update action, this function mark virtual for testable update action validation.
        /// </summary>
        /// <param name="query">Query information for querying customer setting.</param>
        /// <returns>Query action is valid or not.</returns>
        public virtual async Task<IResult> ValidateQuerySettingAction(
            ISettingContext settingContext,
            CustomerSettingQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var customerId = query.CustomerId;
            var customerLevel = query.CustomerLevel;
            var actor = settingContext.Actor;
            var uplineId = query.UplineId;

            try
            {
                await CheckActorStatus(actor);

                if (query.IsNotCreateNew)
                {
                    await CheckCustomerLevel(customerId, customerLevel);

                    CheckActorAndCustomerLevelsConstrain(actor, customerLevel);

                    await CheckBelonging(actor, customerId);
                }
                else
                {
                    await CheckBelonging(actor, uplineId);
                }
            }
            catch (CustomerSettingException customerSettingException)
            {
                WriteQuerySettingLog(settingContext, customerId, customerLevel, customerSettingException);

                return new Result(ResultValues.AccessDenied, customerSettingException.OriginalMessage);
            }

            return Result.Success;
        }

        private async Task CheckActorStatus(User actor)
        {
            var enabledStatus = await dynamicRepository.GetAsync<bool>(new ActorStatusQueryCriteria
            {
                CustId = actor.Id
            });

            if (!enabledStatus)
            {
                throw new CustomerSettingException(Messages.AccountWasDisabled);
            }
        }

        private void CheckActorAndCustomerLevelsConstrain(User actor, Level customerLevel)
        {
            var isValidConstrain = actor.IsCompany
                || customerLevel.IsMemberLevel()
                || actor.Level.IsUplineOf(customerLevel);

            if (!isValidConstrain)
            {
                throw new CustomerSettingException(Result.AccessDenied.Message);
            }
        }

        private async Task CheckBelonging(User actor, int customerId)
        {
            if (actor.IsNotCompany && actor.Id != customerId)
            {
                var isBelonging = await dynamicRepository.GetAsync<bool>(new CheckBelongingCriteria
                {
                    ActorId = actor.Id,
                    CustomerId = customerId
                });

                if (!isBelonging)
                {
                    throw new CustomerSettingException(Messages.CustomersAreNotSameLine);
                }
            }
        }

        /// <summary>
        /// Check whether input customer level is equal to value stored in database
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="customerLevel"></param>
        private async Task CheckCustomerLevel(int customerId, Level customerLevel)
        {
            var actualCustomerLevel = await dynamicRepository.GetAsync<byte>(new CustomerLevelQueryCriteria
            {
                CustId = customerId
            });

            if (actualCustomerLevel != customerLevel.Value)
            {
                throw new CustomerSettingException(Result.AccessDenied.Message);
            }
        }

        private void WriteQuerySettingLog(
            ISettingContext settingContext,
            int customerId,
            Level customerLevel,
            CustomerSettingException ex)
        {
            ex.SetSettingContextInfo("Validate Query Setting", settingContext);
            ex.SetCustomerInfo(customerId, customerLevel);

            logger.Warn(ex.Message, ex);
        }

        /// <summary>
        /// Validating update action, run before update, if result is valid then run update
        /// customer setting to database.
        /// This function was marked virtual in order to make it testable when test update multiple action.
        /// </summary>
        /// <param name="command">Customer setting information to update to database.</param>
        /// <returns>Customer setting is valid for update to database or not.</returns>
        public virtual async Task<IResult> ValidateUpdateSettingAction(
            ISettingContext settingContext,
            UpdateCustomerSettingCommand command)
        {
            var actorLevel = settingContext.Actor.Level;
            var customerId = command.Setting.CustomerId;
            var customerLevel = command.Setting.CustomerLevel;
            var productId = command.Setting.ProductId;
            var settingTypes = command.Setting.SettingTypes;

            try
            {
                await CheckQueryPermission(settingContext, customerId, customerLevel, productId, false, 0);

                await CheckCustomerSettingTypesIsValidWithProductProfile(settingContext, productId, settingTypes);

                CheckUpdatableCustomerSettingTypes(actorLevel, customerLevel, settingTypes);
            }
            catch (CustomerSettingException customerSettingException)
            {
                WriteUpdateSettingLog(settingContext, command.Setting, customerSettingException);

                return new Result(ResultValues.AccessDenied, customerSettingException.OriginalMessage);
            }

            return Result.Success;
        }

        private async Task CheckQueryPermission(
            ISettingContext settingContext,
            int customerId,
            Level customerLevel,
            byte productId,
            bool isCreateNew,
            int uplineId)
        {
            var querySettingParam = new CustomerSettingQuery(isCreateNew, customerId, customerLevel.Value, productId, uplineId);
            var result = await ValidateQuerySettingAction(settingContext, querySettingParam);

            if (result.IsNotOk)
            {
                throw new CustomerSettingException(result.Message);
            }
        }

        private async Task CheckCustomerSettingTypesIsValidWithProductProfile(
            ISettingContext settingContext,
            byte productId,
            IEnumerable<SettingType> customerSettingTypes)
        {
            var productSettingTypes = (await productService
                .GetProductSettingTypes(productId, settingContext.System))
                .Where(product => product.DisplayMode == DisplayModes.Editable);

            foreach (var customerSettingType in customerSettingTypes)
            {
                if (!productSettingTypes.Any(stt => stt.Id == customerSettingType.Id))
                {
                    throw new CustomerSettingException(Result.AccessDenied.Message);
                }
            }
        }

        private void CheckUpdatableCustomerSettingTypes(Level actorLevel, Level customerLevel, IEnumerable<SettingType> customerSettingTypes)
        {
            foreach (var customerSettingType in customerSettingTypes)
            {
                if (customerSettingType.DisplayMode != DisplayModes.Editable
                    || !customerSettingType.SatisfyCustomerLevelConstraint(actorLevel, customerLevel))
                {
                    throw new CustomerSettingException(Result.AccessDenied.Message);
                }
            }
        }

        private void WriteUpdateSettingLog(
            ISettingContext settingContext,
            ISetting setting,
            CustomerSettingException ex)
        {
            ex.SetSettingContextInfo("Validate Update Setting", settingContext);
            ex.SetCustomerInfo(setting.CustomerId, setting.CustomerLevel);
            ex.SetProductInfo(setting.ProductId);

            logger.Warn(ex.Message, ex);
        }

        public async Task<IResult> ValidateCreateCustomerAction(
            ISettingContext settingContext,
            CreateCustomerCommand command)
        {
            var actorLevel = settingContext.Actor.Level;
            var customerId = command.Setting.CustomerId;
            var customerLevel = command.Setting.CustomerLevel;
            var productId = command.Setting.ProductId;
            var settingTypes = command.Setting.SettingTypes;

            try
            {
                await CheckQueryPermission(settingContext, customerId, customerLevel, productId, true, command.UplineId);

                await CheckCustomerSettingTypesIsValidWithProductProfile(settingContext, productId, settingTypes);

                //CheckUpdatableCustomerSettingTypes(actorLevel, customerLevel, settingTypes);
            }
            catch (CustomerSettingException customerSettingException)
            {
                WriteUpdateSettingLog(settingContext, command.Setting, customerSettingException);

                return new Result(ResultValues.AccessDenied, customerSettingException.OriginalMessage);
            }

            return Result.Success;
        }
    }
}