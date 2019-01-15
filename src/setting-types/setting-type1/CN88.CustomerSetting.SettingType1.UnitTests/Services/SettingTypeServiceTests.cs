namespace CN88.CustomerSetting.SettingType1.UnitTests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;
    using System.Threading.Tasks;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Enumerations;
    using CN88.CustomerSetting.Services;
    using CN88.CustomerSetting.SettingType1.Models;
    using CN88.CustomerSetting.SettingType1.Models.Dtos;
    using CN88.CustomerSetting.SettingType1.Params.Criteria;
    using CN88.CustomerSetting.SettingType1.Services;
    using CN88.CustomerSetting.Tests;
    using CN88.CustomerSetting.Tests.Models;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using NSubstitute;
    using NUnit.Framework;
    using CN88.CustomerSetting.Models.Results;

    public class SettingTypeServiceTests
    {
        private ISettingContext settingContext;
        private ResourceService resourceService;
        private ILogger logger;
        private ICacheService cacheService;
        private IDynamicRepository dynamicRepository;
        private MessageService messageService;
        private PasswordService passwordService;
        private SettingTypeServiceImpl settingTypeServiceImpl;

        [SetUp]
        public void SetUp()
        {
            resourceService = Substitute.For<ResourceService>();
            logger = Substitute.For<ILogger>();
            cacheService = Substitute.For<ICacheService>();
            dynamicRepository = Substitute.For<IDynamicRepository>();
            messageService = Substitute.For<MessageService>();
            passwordService = Substitute.For<PasswordService>();
            settingContext = new SettingContextStub();

            settingTypeServiceImpl = new SettingTypeServiceImpl(
                resourceService,
                logger,
                cacheService,
                dynamicRepository,
                messageService,
                passwordService);
        }

        [TestCase(2, 12)]
        [TestCase(3, 13)]
        [TestCase(4, 14)]
        public async Task GetCustomerSettingType_WhenCreateNew_GetDefaultPTSettingForDownline(
            byte downlineLevelId,
            int uplineId)
        {
            var downlineLevel = Enumeration.FromValue<Level>(downlineLevelId);
            var settingQuery = InstanceFactory.CreateQuerySettingParam(
                true,
                1,
                downlineLevel,
                1,
                uplineId);

            StubAccountDetail(downlineLevel);
            StubGetCustomerInfoForCreate();

            await settingTypeServiceImpl.GetCustomerSettingType(
                settingContext,
                settingQuery);

            await AssertGetPTSetting(downlineLevel, true);
        }

        [TestCase(2, 12)]
        [TestCase(3, 13)]
        [TestCase(1, 11)]
        public async Task GetCustomerSettingType_WhenCreateNewCustomerIsNotSuper_GetDefaultCustomerInforation(
            byte downlineLevelId,
            int uplineId)
        {
            var isCreateNew = true;
            var downlineLevel = Enumeration.FromValue<Level>(downlineLevelId);
            var settingQuery = InstanceFactory.CreateQuerySettingParam(
                isCreateNew,
                1,
                downlineLevel,
                1,
                uplineId);
            StubAccountDetail(downlineLevel);
            StubGetCustomerInfoForCreate();

            var accountDetailItem = (await settingTypeServiceImpl
                .GetCustomerSettingType(settingContext, settingQuery))
                .SettingItems
                .FirstOrDefault() as AccountDetailSettingItem;

            Assert.AreEqual("UplineUsername", accountDetailItem.UplineUsername);
            Assert.AreEqual(1111, accountDetailItem.MinCredit);
            Assert.AreEqual(2222, accountDetailItem.MaxCredit);
            Assert.AreEqual("12", string.Join("", accountDetailItem.UsernameSuffixes));
        }

        [Test]
        public async Task GetCustomerSettingType_WhenCreateNewSuper_GetDefaultCustomerInforation()
        {
            var isCreateNew = true;
            var downlineLevel = Level.Super;
            var settingQuery = InstanceFactory.CreateQuerySettingParam(
                isCreateNew,
                1,
                downlineLevel,
                1);
            StubAccountDetail(downlineLevel);
            StubGetCustomerInfoForCreate();
            StubCurrencyCached();

            var accountDetailItem = (await settingTypeServiceImpl
                .GetCustomerSettingType(settingContext, settingQuery))
                .SettingItems
                .FirstOrDefault() as AccountDetailSettingItem;

            Assert.AreEqual(2, accountDetailItem.Currencies.Count());
            Assert.AreEqual("UUS", accountDetailItem.Currencies.ElementAt(0).Name);
            Assert.AreEqual("RM", accountDetailItem.Currencies.ElementAt(1).Name);
        }

        [Test]
        public async Task GetCustomerSettingType_WhenCreateNewMember_GetSpreadGroups()
        {
            var isCreateNew = true;
            var downlineLevel = Level.Member;
            var settingQuery = InstanceFactory.CreateQuerySettingParam(
                isCreateNew,
                1,
                downlineLevel,
                1);
            StubAccountDetail(downlineLevel);
            StubGetCustomerInfoForCreate();
            StubSpreadGroupsCached();

            var accountDetailItem = (await settingTypeServiceImpl
                .GetCustomerSettingType(settingContext, settingQuery))
                .SettingItems
                .FirstOrDefault() as AccountDetailSettingItem;


            Assert.AreEqual(2, accountDetailItem.SpreadGroups.Count());
            Assert.AreEqual("CNA", accountDetailItem.SpreadGroups.ElementAt(0).GroupName);
            Assert.AreEqual("CNB", accountDetailItem.SpreadGroups.ElementAt(1).GroupName);
            Assert.AreEqual("A", accountDetailItem.SpreadGroup.UGroup);
        }

        [TestCase(2, 12)]
        [TestCase(3, 13)]
        [TestCase(4, 14)]
        public async Task GetCustomerSettingType_WhenEditCustomer_GetPTSetting(
            byte downlineLevelId,
            int uplineId)
        {
            var downlineLevel = Enumeration.FromValue<Level>(downlineLevelId);
            var settingQuery = InstanceFactory.CreateQuerySettingParam(
                false,
                1,
                downlineLevel,
                1,
                uplineId);

            StubAccountDetail(downlineLevel);
            StubGetCustomerInfoForEdit();

            await settingTypeServiceImpl.GetCustomerSettingType(
                settingContext,
                settingQuery);

            await AssertGetPTSetting(downlineLevel, false);
        }

        [TestCase(2, 12)]
        [TestCase(3, 13)]
        [TestCase(4, 14)]
        public async Task GetCustomerSettingType_WhenEditCustomer_ReturnAccountDetail(
            byte downlineLevelId,
            int uplineId)
        {
            var downlineLevel = Enumeration.FromValue<Level>(downlineLevelId);
            var settingQuery = InstanceFactory.CreateQuerySettingParam(
                false,
                1,
                downlineLevel,
                1,
                uplineId);

            StubAccountDetail(downlineLevel);
            StubGetCustomerInfoForEdit();

            var accountDetailItem = (await settingTypeServiceImpl.GetCustomerSettingType(
                settingContext,
                settingQuery))
                .SettingItems
                .FirstOrDefault() as AccountDetailSettingItem;

            Assert.AreEqual(123, accountDetailItem.Credit);
            Assert.AreEqual(1111, accountDetailItem.MinCredit);
            Assert.AreEqual(2222, accountDetailItem.MaxCredit);
            Assert.AreEqual("22323", accountDetailItem.Fax);
            Assert.AreEqual("FirstName", accountDetailItem.FirstName);
            Assert.AreEqual("LastName", accountDetailItem.LastName);
            Assert.AreEqual("MobilePhone", accountDetailItem.MobilePhone);
            Assert.AreEqual("Phone", accountDetailItem.Phone);
            Assert.AreEqual("Username", accountDetailItem.Username);
        }

        [Test]
        public async Task GetCustomerSettingType_WhenEditMember_GetSpreadGroups()
        {
            var isCreateNew = false;
            var downlineLevel = Level.Member;
            var settingQuery = InstanceFactory.CreateQuerySettingParam(
                isCreateNew,
                1,
                downlineLevel,
                1);
            StubAccountDetail(downlineLevel);
            StubGetCustomerInfoForEdit();
            StubSpreadGroupsCached();

            var accountDetailItem = (await settingTypeServiceImpl
                .GetCustomerSettingType(settingContext, settingQuery))
                .SettingItems
                .FirstOrDefault() as AccountDetailSettingItem;

            Assert.AreEqual("CNB", accountDetailItem.SpreadGroup.GroupName);
        }

        [Test]
        public async Task GetCustomerSettingType_Always_ReturnIntervalValueForSettingItem()
        {
            var isCreateNew = false;
            var downlineLevel = Level.Super;
            var settingQuery = InstanceFactory.CreateQuerySettingParam(
                isCreateNew,
                1,
                downlineLevel,
                1);
            StubAccountDetail(downlineLevel);
            StubGetCustomerInfoForEdit();
            StubIntervalValue();

            var accountDetailItem = (await settingTypeServiceImpl
                .GetCustomerSettingType(settingContext, settingQuery))
                .SettingItems
                .FirstOrDefault() as AccountDetailSettingItem;

            Assert.AreEqual(3, accountDetailItem.IntervalValue);
        }

        [Test]
        public void BuildCreateCustomerParams_AddNewSuper_ReturnCreateParams()
        {
            var command = InstanceFactory.CreateCreateCustomerCommand(customerLevel: Level.Super);
            var settingType = StubSuperAccountDetailSettingType();
            var settingItem = settingType.SettingItems.FirstOrDefault() as SuperAccountDetail;

            StubPasswordPassAvalidation();

            var createParams = settingTypeServiceImpl
                .BuildCreateCustomerParams(
                    settingContext,
                    command,
                    settingType);

            Assert.AreEqual("CustomerName", createParams["username"]);
            Assert.AreEqual("HashedPassword", createParams["userpwd"]);
            Assert.AreEqual("SuperName", createParams["firstname"]);
            Assert.AreEqual(3434, createParams["credit"]);
            Assert.AreEqual("LastName", createParams["lastname"]);
            Assert.AreEqual("Phone", createParams["phone"]);
            Assert.AreEqual("MobilePhone", createParams["mobilephone"]);
            Assert.AreEqual(settingContext.System, createParams["systemid"]);
            Assert.AreEqual(settingContext.IP, createParams["ip"]);
            Assert.AreEqual(settingContext.Actor.Id, createParams["creator"]);
            Assert.AreEqual("CN88", createParams["site"]);
            Assert.AreEqual(1, createParams["currency"]);
            Assert.AreEqual("<ROOT><PT SportType=\"1\" BetType=\"2\" IsLive=\"1\" MinPT=\"11\" MaxPT=\"12\" /></ROOT>", createParams["ptsetting"]);
        }

        private static AccountDetailSettingType StubSuperAccountDetailSettingType()
        {
            return new AccountDetailSettingType
            {
                SettingItems = new List<SuperAccountDetail>
                {
                     new SuperAccountDetail
                    {
                        Currency = new Currency
                        {
                            Id = 1,
                            Name = "UUS"
                        },
                        Fax = "1212",
                        FirstName = "SuperName",
                        Credit = 3434,
                        IsLive = true,
                        LastName = "LastName",
                        SportTypeId = 1,
                        BetTypeId = 2,
                        SuperMaxPT = 12,
                        SuperMinPT = 11,
                        Username = "CustomerName",
                        Phone = "Phone",
                        MobilePhone = "MobilePhone",
                        Password = "Password"
                    }
                }

            };
        }

        private void StubAccountDetail(Level level)
        {
            if (level.IsSuperLevel())
            {
                dynamicRepository
                    .GetAsync<SuperAccountDetail>(Arg.Any<GetPTSettingCriteria>())
                    .Returns(Task.FromResult(new SuperAccountDetail()));
            }

            if (level.IsMasterLevel())
            {
                dynamicRepository
                    .GetAsync<MasterAccountDetail>(Arg.Any<GetPTSettingCriteria>())
                    .Returns(Task.FromResult(new MasterAccountDetail()));
            }

            if (level.IsAgentLevel())
            {
                dynamicRepository
                    .GetAsync<AgentAccountDetail>(Arg.Any<GetPTSettingCriteria>())
                    .Returns(Task.FromResult(new AgentAccountDetail()));
            }
        }

        private async Task AssertGetPTSetting(Level level, bool isCreateNew)
        {
            if (level.IsSuperLevel())
            {
                await dynamicRepository.Received().GetAsync<SuperAccountDetail>(
                Arg.Is<GetPTSettingCriteria>(
                    criteria => criteria.RoleId == level.Value
                                && criteria.IsCreateNew == isCreateNew
                ));
            }

            if (level.IsMasterLevel())
            {
                await dynamicRepository.Received().GetAsync<MasterAccountDetail>(
                Arg.Is<GetPTSettingCriteria>(
                    criteria => criteria.RoleId == level.Value
                                && criteria.IsCreateNew == isCreateNew
                ));
            }

            if (level.IsAgentLevel())
            {
                await dynamicRepository.Received().GetAsync<AgentAccountDetail>(
                Arg.Is<GetPTSettingCriteria>(
                    criteria => criteria.RoleId == level.Value
                                && criteria.IsCreateNew == isCreateNew
                ));
            }
        }


        private void StubGetCustomerInfoForCreate()
        {
            var customerInfo = new CreateCustomerInfoDto
            {
                UplineUsername = "UplineUsername",
                MinCredit = 1111,
                MaxCredit = 2222,
                NewUsername = "12"
            };

            dynamicRepository
                .GetAsync<CreateCustomerInfoDto>(Arg.Any<GetCustomerInfoForCreateCriteria>())
                .Returns(Task.FromResult(customerInfo));


        }

        private void StubGetCustomerInfoForEdit()
        {
            var customerInfo = new CustomerInfoDto
            {
                MinCredit = 1111,
                MaxCredit = 2222,
                Credit = 123,
                Currency = "UUS",
                Fax = "22323",
                FirstName = "FirstName",
                LastName = "LastName",
                MobilePhone = "MobilePhone",
                Phone = "Phone",
                UGroup = "B",
                Username = "Username"
            };

            dynamicRepository
                .GetAsync<CustomerInfoDto>(Arg.Any<GetGeneralInfoCriteria>())
                .Returns(Task.FromResult(customerInfo));


        }

        private void StubCurrencyCached()
        {
            var currencies = new List<Currency>
            {
                new Currency { Id = 1, Code = "UUS", Name = "UUS" },
                new Currency { Id = 2, Code = "RM", Name = "RM" }
            };

            cacheService.GetOrSetAsync(
                       Arg.Any<string>(),
                       Arg.Any<Func<IEnumerable<Currency>>>(),
                       Arg.Any<CacheItemOptions>())
                       .Returns(currencies);
        }

        private void StubSpreadGroupsCached()
        {
            IEnumerable<SpreadGroup> spreadGroups = new List<SpreadGroup>
            {
                new SpreadGroup { GroupName = "CNA", UGroup = "A" },
                new SpreadGroup { GroupName = "CNB", UGroup = "B" },
            };

            cacheService.GetOrSetAsync(
                       Arg.Any<string>(),
                       Arg.Any<Func<IEnumerable<SpreadGroup>>>(),
                       Arg.Any<CacheItemOptions>())
                       .Returns(Task.FromResult(spreadGroups));
        }

        private void StubIntervalValue()
        {
            IEnumerable<AccountDetailSettingItem> accountDetailSettingItems = new List<AccountDetailSettingItem>
            {
                new AccountDetailSettingItem { IntervalValue = 3 }
            };

            cacheService.GetOrSetAsync(
                       Arg.Any<string>(),
                       Arg.Any<Func<IEnumerable<AccountDetailSettingItem>>>(),
                       Arg.Any<CacheItemOptions>())
                       .Returns(accountDetailSettingItems);
        }

        private void StubPasswordPassAvalidation()
        {
            passwordService.ValidateAndHashPassword(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<Level>())
                .Returns(Result.CreateSuccessResult("HashedPassword"));
        }
    }
}
