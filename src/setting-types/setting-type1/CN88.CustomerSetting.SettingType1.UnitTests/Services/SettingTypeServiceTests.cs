namespace CN88.CustomerSetting.SettingType1.UnitTests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Enumerations;
    using CN88.CustomerSetting.Models.Results;
    using CN88.CustomerSetting.Services;
    using CN88.CustomerSetting.SettingType1.Models;
    using CN88.CustomerSetting.SettingType1.Models.Dtos;
    using CN88.CustomerSetting.SettingType1.Models.PTItems;
    using CN88.CustomerSetting.SettingType1.Params.Commands;
    using CN88.CustomerSetting.SettingType1.Params.Criteria;
    using CN88.CustomerSetting.SettingType1.Services;
    using CN88.CustomerSetting.Tests;
    using CN88.CustomerSetting.Tests.Models;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture(Category = "SettingType1-AccountDetail")]
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
                .FirstOrDefault() as AccountDetailSettingItem<SettingItemDefault>;

            Assert.AreEqual("UplineUsername", accountDetailItem.UplineUsername);
            Assert.AreEqual(1111, accountDetailItem.MinCredit);
            Assert.AreEqual(2222, accountDetailItem.MaxCredit);
            Assert.AreEqual("12", string.Join("", accountDetailItem.UsernameSuffixes));
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
                .FirstOrDefault() as AccountDetailSettingItem<SettingItemDefault>;

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
                .FirstOrDefault() as AccountDetailSettingItem<SettingItemDefault>;

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
                .FirstOrDefault() as AccountDetailSettingItem<SettingItemDefault>;

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
                .FirstOrDefault() as AccountDetailSettingItem<SettingItemDefault>;

            Assert.AreEqual(3, accountDetailItem.IntervalValue);
        }

        [Test]
        public void BuildCreateCustomerParams_AddNewSuper_ReturnCreateParams()
        {
            var command = InstanceFactory.CreateCreateCustomerCommand(customerLevel: Level.Super);
            var settingType = StubSuperAccountDetailSettingType();
            StubActorCanUpdateCredit();

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
            Assert.AreEqual("headlineusername", createParams["headlineusername"]);
            Assert.AreEqual("<ROOT><PT SportType=\"1\" BetType=\"2\" IsLive=\"1\" MinPT=\"11\" MaxPT=\"12\" /></ROOT>", createParams["ptsetting"]);
        }

        [Test]
        public void BuildCreateCustomerParams_AddNewMaster_ReturnCreateParams()
        {
            var command = InstanceFactory.CreateCreateCustomerCommand(customerLevel: Level.Master);
            var settingType = StubMasterAccountDetailSettingType();
            settingContext.Actor = new User
            {
                Level = Level.Super,
                Id = 3434,
                SubAccountId = 111
            };

            StubPasswordPassAvalidation();

            var createParams = settingTypeServiceImpl
                .BuildCreateCustomerParams(
                    settingContext,
                    command,
                    settingType);

            Assert.AreEqual("CustomerName", createParams["username"]);
            Assert.AreEqual("MasterName", createParams["firstname"]);
            Assert.AreEqual(3434, createParams["credit"]);
            Assert.AreEqual(settingContext.System, createParams["systemid"]);
            Assert.AreEqual(settingContext.IP, createParams["ip"]);
            Assert.AreEqual(3434, createParams["creator"]);
            Assert.AreEqual(111, createParams["subaccid"]);
            Assert.AreEqual(1, createParams["srecommend"]);
            Assert.AreEqual("<ROOT><PT SportType=\"2\" BetType=\"1\" IsLive=\"1\" SuperPT=\"20\" /></ROOT>", createParams["ptsetting"]);
        }

        [Test]
        public void BuildCreateCustomerParams_AddNewAgent_ReturnCreateParams()
        {
            var command = InstanceFactory.CreateCreateCustomerCommand(customerLevel: Level.Agent);
            var settingType = StubAgentAccountDetailSettingType();
            settingContext.Actor = new User
            {
                Level = Level.Master,
                Id = 34342,
                SubAccountId = 1112
            };

            StubPasswordPassAvalidation();

            var createParams = settingTypeServiceImpl
                .BuildCreateCustomerParams(
                    settingContext,
                    command,
                    settingType);

            Assert.AreEqual("CustomerName", createParams["username"]);
            Assert.AreEqual("AgentName", createParams["firstname"]);
            Assert.AreEqual(3434, createParams["credit"]);
            Assert.AreEqual(settingContext.System, createParams["systemid"]);
            Assert.AreEqual(settingContext.IP, createParams["ip"]);
            Assert.AreEqual(34342, createParams["creator"]);
            Assert.AreEqual(1112, createParams["subaccid"]);
            Assert.AreEqual(1, createParams["mrecommend"]);
            Assert.AreEqual("<ROOT><PT SportType=\"2\" BetType=\"1\" IsLive=\"1\" MasterPT=\"11\" AgentPT=\"23\" /></ROOT>", createParams["ptsetting"]);
        }

        [Test]
        public void BuildCreateCustomerParams_AddNewMember_ReturnCreateParams()
        {
            var command = InstanceFactory.CreateCreateCustomerCommand(customerLevel: Level.Member);
            var settingType = StubMemberAccountDetailSettingType();
            settingContext.Actor = new User
            {
                Level = Level.Master,
                Id = 34342,
                SubAccountId = 1112
            };

            StubPasswordPassAvalidation();

            var createParams = settingTypeServiceImpl
                .BuildCreateCustomerParams(
                    settingContext,
                    command,
                    settingType);

            Assert.AreEqual("CustomerName", createParams["username"]);
            Assert.AreEqual("AgentName", createParams["firstname"]);
            Assert.AreEqual(3434, createParams["credit"]);
            Assert.AreEqual(settingContext.System, createParams["systemid"]);
            Assert.AreEqual(settingContext.IP, createParams["ip"]);
            Assert.AreEqual(34342, createParams["creator"]);
            Assert.AreEqual(1112, createParams["subaccid"]);
            Assert.AreEqual(1, createParams["recommend"]);
            Assert.AreEqual("A", createParams["ugroup"]);
        }

        [Test]
        public void UpdateCustomerSettingType_SettingItemIsNull_ThrowInvalidOperationException()
        {
            var updateCommand = InstanceFactory.CreateUpdateSettingParam();
            var updateSettingType = new AccountDetailSettingType
            {
                SettingItems = Enumerable.Empty<ISettingItem>()
            };

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await settingTypeServiceImpl.UpdateCustomerSettingType(
                settingContext,
                updateCommand,
                updateSettingType));
        }

        [Test]
        public async Task UpdateCustomerSettingType_Always_UpdateCustomerInfo()
        {
            var updateCommand = InstanceFactory.CreateUpdateSettingParam(customerLevel: Level.Super);
            var updateSettingType = StubSuperAccountDetailSettingType();
            StubEditablePTTime();
            StubActorCanUpdateCredit();

            await settingTypeServiceImpl.UpdateCustomerSettingType(settingContext, updateCommand, updateSettingType);

            await dynamicRepository.Received().ExecuteAsync(Arg.Is<UpdateCustomerInfoCommand>(
                command => command.RoleId == Level.Super.Value
                            && command.Credit == 3434
                            && command.LastName == "LastName"
                            && command.Phone == "Phone"
                            && command.MobilePhone == "MobilePhone"
                            && command.FirstName == "SuperName"
                            && command.Language == "CT"
                            && command.CustId == 1
                            && command.LastName == "LastName"
                ));
        }

        [Test]
        public async Task UpdateCustomerSettingType_UpdateCustomerInfoFailed_ReturnFailedResult()
        {
            var updateCommand = InstanceFactory.CreateUpdateSettingParam(customerLevel: Level.Super);
            var updateSettingType = StubSuperAccountDetailSettingType();

            dynamicRepository
                .ExecuteAsync(Arg.Any<UpdateCustomerInfoCommand>())
                .Returns(command =>
                {
                    ((UpdateCustomerInfoCommand)command[0]).ErrCode = 1;

                    return Task.FromResult(1);
                });

            var result = await settingTypeServiceImpl.UpdateCustomerSettingType(settingContext, updateCommand, updateSettingType);

            Assert.IsTrue(result.IsFailed);
        }

        [Test]
        public async Task UpdateCustomerSettingType_UpdateCustomerInfoSuccess_ThenUpdatePTSetting()
        {
            var updateCommand = InstanceFactory.CreateUpdateSettingParam(customerLevel: Level.Super);
            var updateSettingType = StubSuperAccountDetailSettingType();
            StubEditablePTTime();
            StubActorCanUpdateCredit();

            await settingTypeServiceImpl.UpdateCustomerSettingType(settingContext, updateCommand, updateSettingType);

            await dynamicRepository.Received().ExecuteAsync(Arg.Is<UpdatePTSettingCommand>(
                command => command.RoleId == Level.Super.Value
                            && command.IP == settingContext.IP
                            && command.ProductId == 1
                            && command.UserId == 100
                            && command.PTSetting == "<ROOT><PT SportType=\"1\" BetType=\"2\" IsLive=\"1\" MinPT=\"11\" MaxPT=\"12\" /></ROOT>"
                ));
        }

        [Test]
        public async Task UpdateCustomerSettingType_UpdateSuccess_ReturnSuccessResult()
        {
            var updateCommand = InstanceFactory.CreateUpdateSettingParam(customerLevel: Level.Super);
            var updateSettingType = StubSuperAccountDetailSettingType();
            StubEditablePTTime();
            StubActorCanUpdateCredit();

            var result = await settingTypeServiceImpl.UpdateCustomerSettingType(settingContext, updateCommand, updateSettingType);

            Assert.IsTrue(result.IsSuccessful);
        }

        private static AccountDetailSettingType StubSuperAccountDetailSettingType()
            => new AccountDetailSettingType
            {
                SettingItems = new List<AccountDetailSettingItem<SettingItemDefault>>
                {
                     new AccountDetailSettingItem<SettingItemDefault>
                    {
                        Currency = new Currency
                        {
                            Id = 1,
                            Name = "UUS"
                        },
                        Fax = "1212",
                        FirstName = "SuperName",
                        Credit = 3434,
                        LastName = "LastName",
                        PTItems = new List<SuperPTItem>
                        {
                            new SuperPTItem
                            {
                                SportTypeId = 1,
                                BetTypeId = 2,
                                SuperMaxPT = 12,
                                SuperMinPT = 11,
                                IsLive = true
                            }
                        },
                        Username = "CustomerName",
                        Phone = "Phone",
                        MobilePhone = "MobilePhone",
                        Password = "Password",
                        UplineUsername = "headlineusername"
                    }
                }
            };

        private static AccountDetailSettingType StubMasterAccountDetailSettingType()
            => new AccountDetailSettingType
            {
                SettingItems = new List<AccountDetailSettingItem<SettingItemDefault>>
                {
                     new AccountDetailSettingItem<SettingItemDefault>
                    {
                        FirstName = "MasterName",
                        Credit = 3434,
                        Username = "CustomerName",
                        Password = "Password",
                        PTItems = new List<MasterPTItem>
                        {
                            new MasterPTItem
                            {
                                SuperPT = 20,
                                IsLive = true,
                                SportTypeId = 2,
                                BetTypeId = 1
                            }
                        }
                    }
                }
            };

        private static AccountDetailSettingType StubAgentAccountDetailSettingType()
            => new AccountDetailSettingType
            {
                SettingItems = new List<AccountDetailSettingItem<SettingItemDefault>>
                {
                     new AccountDetailSettingItem<SettingItemDefault>
                    {
                        FirstName = "AgentName",
                        Credit = 3434,
                        Username = "CustomerName",
                        Password = "Password",
                        PTItems = new List<AgentPTItem>
                        {
                            new AgentPTItem
                            {
                                SportTypeId = 2,
                                BetTypeId = 1,
                                AgentPT = 23,
                                MasterPT = 11,
                                IsLive = true
                            }
                        }
                    }
                }
            };

        private static AccountDetailSettingType StubMemberAccountDetailSettingType()
            => new AccountDetailSettingType
            {
                SettingItems = new List<AccountDetailSettingItem<SettingItemDefault>>
                {
                     new AccountDetailSettingItem<SettingItemDefault>
                    {
                        FirstName = "AgentName",
                        Credit = 3434,
                        IsLive = true,
                        SportTypeId = 2,
                        BetTypeId = 1,
                        Username = "CustomerName",
                        Password = "Password",
                        SpreadGroup = new SpreadGroup
                        {
                            UGroup = "A"
                        }
                    }
                }
            };

        private void StubAccountDetail(Level level)
        {
            if (level.IsSuperLevel())
            {
                dynamicRepository
                    .FetchAsync<SuperPTItem>(Arg.Any<GetPTSettingCriteria>())
                    .Returns(new List<SuperPTItem> { new SuperPTItem() });
            }

            if (level.IsMasterLevel())
            {
                dynamicRepository
                    .FetchAsync<MasterPTItem>(Arg.Any<GetPTSettingCriteria>())
                    .Returns(new List<MasterPTItem> { new MasterPTItem() });
            }

            if (level.IsAgentLevel())
            {
                dynamicRepository
                    .FetchAsync<AgentPTItem>(Arg.Any<GetPTSettingCriteria>())
                    .Returns(new List<AgentPTItem> { new AgentPTItem() });
            }

            StubEditablePTTime();
        }

        private void StubEditablePTTime(bool isEditable = true)
        {
            dynamicRepository.GetAsync<EditPTTimeDto>(Arg.Any<GetPTEditTimeCriteria>())
                .Returns(Task.FromResult(
                    new EditPTTimeDto { IsEditablePT = isEditable, EffectiveDate = new DateTime(2000, 11, 11) }));
        }

        private async Task AssertGetPTSetting(Level level, bool isCreateNew)
        {
            if (level.IsSuperLevel())
            {
                await dynamicRepository.Received().FetchAsync<SuperPTItem>(
                Arg.Is<GetPTSettingCriteria>(
                    criteria => criteria.RoleId == level.Value
                                && criteria.IsCreateNew == isCreateNew
                ));
            }

            if (level.IsMasterLevel())
            {
                await dynamicRepository.Received().FetchAsync<MasterPTItem>(
                Arg.Is<GetPTSettingCriteria>(
                    criteria => criteria.RoleId == level.Value
                                && criteria.IsCreateNew == isCreateNew
                ));
            }

            if (level.IsAgentLevel())
            {
                await dynamicRepository.Received().FetchAsync<AgentPTItem>(
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
            var accountDetailSettingItems = new List<AccountDetailSettingItem<SettingItemDefault>>
            {
                new AccountDetailSettingItem<SettingItemDefault> { IntervalValue = 3 }
            };

            cacheService.GetOrSetAsync(
                       Arg.Any<string>(),
                       Arg.Any<Func<IEnumerable<AccountDetailSettingItem<SettingItemDefault>>>>(),
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

        private void StubActorCanUpdateCredit()
        {
            dynamicRepository
                .When(dr => dr.ExecuteAsync(Arg.Any<CheckAdminPermissionCommand>()))
                .Do(args =>
                {
                    ((CheckAdminPermissionCommand)args[0]).ErrCode = 0;
                });
        }

        [TestCase(2, 12)]
        [TestCase(3, 13)]
        [TestCase(4, 14)]
        public async Task GetCustomerSettingType_WhenEditCustomer_GetEditPTTime(
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

            var settingType = await settingTypeServiceImpl.GetCustomerSettingType(
                settingContext,
                settingQuery);

            var settingItem = settingType.SettingItems.FirstOrDefault() as AccountDetailSettingItem<SettingItemDefault>;

            Assert.IsFalse(settingItem.IsPTReadOnly);
            Assert.AreEqual(new DateTime(2000, 11, 11), settingItem.PTEffectiveTime);
        }

        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public async Task GetCustomerSettingType_ActorIsNotCompany_DoesNotExecuteCheckAdminPermissionCommand(byte actorLevel)
        {
            settingContext.Actor.Level = Enumeration.FromValue<Level>(actorLevel);

            var downlineLevel = Enumeration.FromValue<Level>((byte)(actorLevel - 1));
            var settingQuery = InstanceFactory.CreateQuerySettingParam(
                false,
                1,
                downlineLevel,
                1,
                12);

            StubAccountDetail(downlineLevel);
            StubGetCustomerInfoForEdit();

            await settingTypeServiceImpl.GetCustomerSettingType(
                settingContext,
                settingQuery);

            await dynamicRepository.DidNotReceiveWithAnyArgs().ExecuteAsync(Arg.Any<CheckAdminPermissionCommand>());
        }

        [Test]
        public async Task GetCustomerSettingType_ActorIsCompany_ExecuteCheckAdminPermissionCommand()
        {
            var downlineLevel = Enumeration.FromValue<Level>(3);
            var settingQuery = InstanceFactory.CreateQuerySettingParam(
                false,
                1,
                downlineLevel,
                1,
                12);

            StubAccountDetail(downlineLevel);
            StubGetCustomerInfoForEdit();

            await settingTypeServiceImpl.GetCustomerSettingType(
                settingContext,
                settingQuery);

            await dynamicRepository.Received().ExecuteAsync(Arg.Is<CheckAdminPermissionCommand>(
                command => command.CustId == settingContext.Actor.Id));
        }

        [Test]
        public async Task UpdateCustomerSettingType_OutOfEditPTTime_ReturnErrorResult()
        {
            var updateCommand = InstanceFactory.CreateUpdateSettingParam(customerLevel: Level.Super);
            var updateSettingType = StubSuperAccountDetailSettingType();

            StubEditablePTTime(false);
            StubActorCanUpdateCredit();

            var result = await settingTypeServiceImpl.UpdateCustomerSettingType(settingContext, updateCommand, updateSettingType);

            Assert.AreEqual("You cannot update new possess value at this moment.", result.Message);
        }
    }
}