namespace CN88.CustomerSetting.UnitTests.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CN88.CustomerSetting.DataAccesses;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Requests;
    using CN88.CustomerSetting.Models.Results;
    using CN88.CustomerSetting.Services;
    using CN88.CustomerSetting.SpParams.Commands;
    using CN88.CustomerSetting.SpParams.Criteria;
    using CN88.CustomerSetting.Tests;
    using CN88.CustomerSetting.Tests.Models;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture(Category = "Core")]
    public class CreateCustomerServiceTests
    {
        private ISettingContext settingContext;
        private CustomerDataAccess customerDataAccess;
        private SettingTypeService settingTypeService;
        private SettingTypeServiceFactory settingTypeServiceFactory;
        private ValidationService validationService;
        private IDynamicRepository dynamicRepository;
        private ILogger logger;
        private CreateCustomerService createCustomerService;

        [SetUp]
        public void SetUp()
        {
            settingContext = new SettingContextStub();
            validationService = Substitute.For<ValidationService>();
            logger = Substitute.For<ILogger>();
            settingTypeServiceFactory = Substitute.For<SettingTypeServiceFactory>();
            dynamicRepository = Substitute.For<IDynamicRepository>();
            settingTypeService = Substitute.For<SettingTypeService>();
            customerDataAccess = Substitute.For<CustomerDataAccess>();

            createCustomerService = new CreateCustomerServiceImpl(
                customerDataAccess,
                settingTypeServiceFactory,
                validationService,
                dynamicRepository,
                logger);
        }

        [Test]
        public async Task IsUsernameExisted_UsernameExisted_ReturnErrorResult()
        {
            var isExisted = true;
            var username = "ExistedUsername";
            dynamicRepository
                .ExecuteAsync(Arg.Is<CheckValidUsernameCommand>(criteria => criteria.Username == username))
                .Returns(command =>
                {
                    ((CheckValidUsernameCommand)command[0]).IsExists = isExisted;

                    return Task.FromResult(1);
                });

            var result = await createCustomerService.IsUsernameExisted(username);

            Assert.True(result.IsNotOk);
        }

        [Test]
        public async Task IsUsernameExisted_UsernameAvailable_ReturnSuccessResult()
        {
            var isExisted = false;
            var username = "AvailableUsername";
            dynamicRepository
                .ExecuteAsync(Arg.Is<CheckValidUsernameCommand>(criteria => criteria.Username == username))
                .Returns(command =>
                {
                    ((CheckValidUsernameCommand)command[0]).IsExists = isExisted;

                    return Task.FromResult(1);
                });

            var result = await createCustomerService.IsUsernameExisted(username);

            Assert.True(result.IsOk);
        }

        [Test]
        public async Task Create_FailedValiation_ReturnFailedResult()
        {
            var createCustomerCommand = InstanceFactory.CreateCreateCustomerCommand();
            validationService.ValidateCreateCustomerAction(
                settingContext,
                createCustomerCommand).Returns(Task.FromResult(Result.DataError as IResult));

            var result = await createCustomerService.Create(settingContext, createCustomerCommand);

            Assert.True(result.IsNotOk);
        }

        [Test]
        public async Task Create_Always_CombineNewCustomerSettingTypeParams()
        {
            var command = InstanceFactory.CreateCreateCustomerCommand();

            AssumePassValidation(command);
            SetupSettingTypeServiceStub();

            await createCustomerService.Create(settingContext, command);

            await customerDataAccess
                .Received()
                .CreateNewCustomer(
                    command.Setting.CustomerLevel,
                    Arg.Is<Hashtable>(hash
                        => (int)hash["param1"] == 1
                            && (int)hash["param2"] == 2
                            && (string)hash["param3"] == "test"));
        }

        [Test]
        public async Task Create_CreateFailed_ReturnFailedResult()
        {
            var command = InstanceFactory.CreateCreateCustomerCommand();

            AssumePassValidation(command);
            SetupSettingTypeServiceStub();

            var failedResult = new CreateNewResult(1, "failed", 1, 1, 1);

            settingTypeService.TranslateResultMessage(
                Arg.Any<SettingResult>(),
                string.Empty,
                Arg.Any<byte>(),
                settingContext.Language)
                .Returns(new SettingResult(failedResult));

            var createResult = await createCustomerService.Create(settingContext, command);

            Assert.True(createResult.IsFailed);
        }

        [Test]
        public async Task Create_CreateSuccess_ReturnSuccessResult()
        {
            var command = InstanceFactory.CreateCreateCustomerCommand();

            AssumePassValidation(command);
            SetupSettingTypeServiceStub();

            settingTypeService.TranslateResultMessage(
                Arg.Any<SettingResult>(),
                string.Empty,
                Arg.Any<byte>(),
                settingContext.Language)
                .Returns(new SettingResult(Result.Success));

            customerDataAccess
            .CreateNewCustomer(
                command.Setting.CustomerLevel,
                Arg.Any<Hashtable>())
                    .Returns(Task.FromResult(new CreateNewResult(1, string.Empty, 1, 1, 0)));

            var createResult = await createCustomerService.Create(settingContext, command);

            Assert.True(createResult.IsSuccessful);
        }

        [Test]
        public async Task Create_ThrowException_ReturnFailedResult()
        {
            var command = InstanceFactory.CreateCreateCustomerCommand();
            
            AssumePassValidation(command);
            settingTypeServiceFactory
                .When(fac => fac.GetInstance(Arg.Any<SettingType>()))
                .Do(fac => { throw new Exception("Factory Exception"); });

            var createResult = await createCustomerService.Create(settingContext, command);

            Assert.True(createResult.IsFailed);
        }

        public void AssumePassValidation(CreateCustomerCommand createCustomerCommand)
        {
            validationService.ValidateCreateCustomerAction(
               settingContext,
               createCustomerCommand).Returns(Task.FromResult(Result.Success as IResult));
        }

        private void SetupSettingTypeServiceStub()
        {
            settingTypeService
                .BuildCreateCustomerParams(
                settingContext,
                Arg.Any<CreateCustomerCommand>(),
                Arg.Any<SettingType>())
                .Returns(
                new Dictionary<string, object>
                {
                    { "param1", 1 },
                    { "param2", 2 },
                    { "param3", "test" },
                });

            settingTypeServiceFactory.GetInstance(Arg.Any<SettingType>()).Returns(settingTypeService);
        }

        [Test]
        public async Task GetHeadlineSuggestionNames_Always_CallDynamicRepositoryFetch()
        {
            var username = "mshusername";

            await createCustomerService.GetHeadlineSuggestionNames(username);

            await dynamicRepository
                .Received()
                .FetchAsync<string>(Arg.Is<GetHeadlineSuggestionNamesCriteria>(
                    criteria => criteria.Username == username));
        }

        [Test]
        public async Task GetHeadlineSuggestionNames_Always_ReturnDataFromDatabase()
        {
            var username = "mshusername";

            dynamicRepository
                .FetchAsync<string>(Arg.Is<GetHeadlineSuggestionNamesCriteria>(
                    criteria => criteria.Username == username))
                .Returns(new List<string> { "MSH1" });

            var headlineNames = await createCustomerService.GetHeadlineSuggestionNames(username);

            Assert.AreEqual(1, headlineNames.Count());
            Assert.AreEqual("MSH1", headlineNames.FirstOrDefault());
        }
    }
}