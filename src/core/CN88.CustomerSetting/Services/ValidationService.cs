namespace CN88.CustomerSetting.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CN88.CustomerSetting.AppResources;
    using CN88.CustomerSetting.DataAccesses;
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
        IResult ValidateQuerySettingAction(ISettingContext settingContext, CustomerSettingQuery query);

        IResult ValidateUpdateSettingAction(ISettingContext settingContext, UpdateCustomerSettingCommand command);

        IResult ValidateCreateCustomerAction(ISettingContext settingContext, CreateCustomerCommand command);
    }

    public class ValidationServiceImpl : ValidationService
    {
        private readonly ProductService productService;
        private readonly IValidationDataAccess validationDataAccess;
        private readonly IDynamicRepository dynamicRepository;
        private readonly ILogger logger;

        public ValidationServiceImpl(
            ProductService productService = null,
            IValidationDataAccess validationDataAccess = null,
            IDynamicRepository dynamicRepository = null,
            ILogger logger = null)
        {
            this.productService = productService ?? new ProductServiceImpl();
            this.validationDataAccess = validationDataAccess ?? new ValidationDataAccess();
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
            this.logger = logger ?? Logger.Log;
        }

        /// <summary>
        /// Validate Query action is valid or not, this function also using by
        /// update action, this function mark virtual for testable update action validation.
        /// </summary>
        /// <param name="query">Query information for querying customer setting.</param>
        /// <returns>Query action is valid or not.</returns>
        public virtual IResult ValidateQuerySettingAction(
            ISettingContext settingContext,
            CustomerSettingQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var customerId = query.CustomerId;
            var customerLevel = query.CustomerLevel;
            var uplineLevel = query.UplineLevel;
            var actor = settingContext.Actor;

            try
            {
                CheckActorStatus(actor);

                if (query.IsNotCreateNew)
                {
                    CheckCustomerLevel(customerId, customerLevel);

                    CheckActorAndCustomerLevelsConstrain(actor, customerLevel);

                    CheckActorLevelAndCustomerUplineLevelConstrain(actor, customerLevel, uplineLevel);

                    CheckBelonging(actor, customerId);
                }
            }
            catch (CustomerSettingException customerSettingException)
            {
                WriteQuerySettingLog(settingContext, customerId, customerLevel, uplineLevel, customerSettingException);

                return new Result(ResultValues.AccessDenied, customerSettingException.OriginalMessage);
            }

            return Result.Success;
        }

        private void CheckActorStatus(User actor)
        {
            var enabledStatus = dynamicRepository.Get<bool>(new ActorStatusQueryCriteria
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
                || actor.Level.IsDirectUplineOf(customerLevel);

            if (!isValidConstrain)
            {
                throw new CustomerSettingException(Result.AccessDenied.Message);
            }
        }

        private void CheckActorLevelAndCustomerUplineLevelConstrain(User actor, Level customerLevel, Level uplineLevel)
        {
            //// If actor is not company when edit member, actor level should equal with upline level.
            if (actor.IsNotCompany
                && customerLevel.IsMemberLevel()
                && uplineLevel.IsNot(null)
                && actor.Level.IsNot(uplineLevel))
            {
                throw new CustomerSettingException(Messages.UplineLevelIsNotValid);
            }
        }

        private void CheckBelonging(User actor, int customerId)
        {
            if (actor.IsNotCompany)
            {
                var isBelonging = dynamicRepository.Get<bool>(new CheckBelongingCriteria
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
        private void CheckCustomerLevel(int customerId, Level customerLevel)
        {
            var actualCustomerLevel = dynamicRepository.Get<byte>(new CustomerLevelQueryCriteria
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
            Level uplineLevel,
            CustomerSettingException ex)
        {
            ex.SetSettingContextInfo("Validate Query Setting", settingContext);
            ex.SetCustomerInfo(customerId, customerLevel, uplineLevel);

            logger.Warn(ex.Message, ex);
        }

        /// <summary>
        /// Validating update action, run before update, if result is valid then run update
        /// customer setting to database.
        /// This function was marked virtual in order to make it testable when test update multiple action.
        /// </summary>
        /// <param name="command">Customer setting information to update to database.</param>
        /// <returns>Customer setting is valid for update to database or not.</returns>
        public virtual IResult ValidateUpdateSettingAction(
            ISettingContext settingContext,
            UpdateCustomerSettingCommand command)
        {
            var actorLevel = settingContext.Actor.Level;
            var customerId = command.Setting.CustomerId;
            var customerLevel = command.Setting.CustomerLevel;
            var productId = command.Setting.ProductId;
            var uplineLevel = command.UplineLevel;
            var settingTypes = command.Setting.SettingTypes;

            try
            {
                CheckQueryPermission(settingContext, customerId, customerLevel, productId, uplineLevel, false);

                CheckCustomerSettingTypesIsValidWithProductProfile(settingContext, productId, settingTypes);

                CheckUpdatableCustomerSettingTypes(actorLevel, customerLevel, settingTypes);
            }
            catch (CustomerSettingException customerSettingException)
            {
                WriteUpdateSettingLog(settingContext, command, customerSettingException);

                return new Result(ResultValues.AccessDenied, customerSettingException.OriginalMessage);
            }

            return Result.Success;
        }

        private void CheckQueryPermission(
            ISettingContext settingContext,
            int customerId,
            Level customerLevel,
            byte productId,
            Level uplineLevel,
            bool isCreateNew)
        {
            var querySettingParam = new CustomerSettingQuery(isCreateNew, customerId, customerLevel, productId, uplineLevel);
            var result = ValidateQuerySettingAction(settingContext, querySettingParam);

            if (result.IsNotOk)
            {
                throw new CustomerSettingException(result.Message);
            }
        }

        private void CheckCustomerSettingTypesIsValidWithProductProfile(ISettingContext settingContext, byte productId, IEnumerable<SettingType> customerSettingTypes)
        {
            var productSettingTypes = productService.GetProductSettingTypes(productId, settingContext.System)
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
            UpdateCustomerSettingCommand command,
            CustomerSettingException ex)
        {
            ex.SetSettingContextInfo("Validate Update Setting", settingContext);
            ex.SetCustomerInfo(command.Setting.CustomerId, command.Setting.CustomerLevel, command.UplineLevel);
            ex.SetProductInfo(command.Setting.ProductId);

            logger.Warn(ex.Message, ex);
        }

        public IResult ValidateCreateCustomerAction(ISettingContext settingContext, CreateCustomerCommand command)
        {
            throw new NotImplementedException();
        }
    }
}