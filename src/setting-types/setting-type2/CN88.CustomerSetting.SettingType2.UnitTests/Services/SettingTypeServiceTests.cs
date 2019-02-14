namespace CN88.CustomerSetting.SettingType2.UnitTests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Enumerations;
    using CN88.CustomerSetting.Services;
    using CN88.CustomerSetting.SettingType2.Models;
    using CN88.CustomerSetting.SettingType2.Models.Dtos;
    using CN88.CustomerSetting.SettingType2.Params.Commands;
    using CN88.CustomerSetting.SettingType2.Params.Criteria;
    using CN88.CustomerSetting.SettingType2.Services;
    using CN88.CustomerSetting.Tests;
    using CN88.CustomerSetting.Tests.Models;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture(Category = "SettingType2-CommBetSetting")]
    public class SettingTypeServiceTests
    {
        private ISettingContext settingContext;
        private ResourceService resourceService;
        private ILogger logger;
        private ICacheService cacheService;
        private IDynamicRepository dynamicRepository;
        private MessageService messageService;
        private SettingTypeServiceImpl settingTypeService;

        [SetUp]
        public void SetUp()
        {
            resourceService = Substitute.For<ResourceService>();
            logger = Substitute.For<ILogger>();
            cacheService = Substitute.For<ICacheService>();
            dynamicRepository = Substitute.For<IDynamicRepository>();
            messageService = Substitute.For<MessageService>();
            settingContext = new SettingContextStub();

            settingTypeService = new SettingTypeServiceImpl(
                resourceService,
                logger,
                cacheService,
                dynamicRepository,
                messageService);
        }

        [Test]
        public async Task GetCustomerSettingType_Always_ReturnCommBetSetting()
        {
            var query = InstanceFactory.CreateQuerySettingParam(true);
            StubCommBetSettingConfigs();
            StubCommBetSettingList(
                query.IsCreateNew,
                query.IsCreateNew ? query.UplineId : query.CustomerId,
                query.CustomerLevel.Value);

            var commBetSetting = (await settingTypeService.GetCustomerSettingType(settingContext, query)) as CommBetSetting;

            var firstSettingItem = commBetSetting.SettingItems.ElementAt(0) as CommBetSettingItem;
            var secondSettingItem = commBetSetting.SettingItems.ElementAt(1) as CommBetSettingItem;
            Assert.AreEqual(6, commBetSetting.SettingItems.Count());

            Assert.AreEqual(4, firstSettingItem.CommissionGroups.Count());
            Assert.AreEqual("D", firstSettingItem.CommissionGroups.ElementAt(3).Name);
            Assert.AreEqual(0.55m, firstSettingItem.CommissionGroups.ElementAt(3).Value);
            Assert.AreEqual(120, firstSettingItem.MaxBet);
            Assert.AreEqual(1000, firstSettingItem.MaxPerEvent);
            Assert.AreEqual(1, firstSettingItem.SportTypeId);
            Assert.AreEqual(1, firstSettingItem.BetTypeId);
            Assert.AreEqual(true, firstSettingItem.IsLive);

            Assert.AreEqual(3, secondSettingItem.CommissionGroups.Count());
            Assert.AreEqual("C", secondSettingItem.CommissionGroups.ElementAt(2).Name);
            Assert.AreEqual(0.25m, secondSettingItem.CommissionGroups.ElementAt(2).Value);
            Assert.AreEqual(121, secondSettingItem.MaxBet);
            Assert.AreEqual(10001, secondSettingItem.MaxPerEvent);
            Assert.AreEqual(1, secondSettingItem.SportTypeId);
            Assert.AreEqual(1, secondSettingItem.BetTypeId);
            Assert.AreEqual(false, secondSettingItem.IsLive);
        }

        [Test]
        public void BuildCreateCustomerParams_SettingTypeIsNotValid_ThrowInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(()
                => settingTypeService.BuildCreateCustomerParams(
                    settingContext,
                    InstanceFactory.CreateCreateCustomerCommand(),
                    new SettingType()));
        }

        [Test]
        public void BuildCreateCustomerParams_Always_ReturnCreateParams()
        {
            var createCommand = InstanceFactory.CreateCreateCustomerCommand();
            StubCommTemplateCache();
            var commBetSetting = StubCommBetSetting();

            var createParams = settingTypeService.BuildCreateCustomerParams(
                settingContext,
                createCommand,
                commBetSetting);

            var expectedCommXlm = "<ROOT>"
                + "<Comm SportType=\"1\" BetType=\"2\" IsLive=\"1\" GroupId=\"1\" ValueId=\"1\"/>"
                + "<Comm SportType=\"1\" BetType=\"2\" IsLive=\"1\" GroupId=\"2\" ValueId=\"5\"/>"
                + "<Comm SportType=\"1\" BetType=\"2\" IsLive=\"1\" GroupId=\"3\" ValueId=\"4\"/>"
                + "<Comm SportType=\"1\" BetType=\"1\" IsLive=\"0\" GroupId=\"1\" ValueId=\"4\"/>"
                + "<Comm SportType=\"1\" BetType=\"1\" IsLive=\"0\" GroupId=\"2\" ValueId=\"7\"/>"
                + "</ROOT>";

            var expectedBetSettingXlm = "<ROOT>"
                + "<BetSetting SportType=\"1\" BetType=\"2\" IsLive=\"1\" MaxBet=\"111\" MaxPerEvent=\"2222\"/>"
                + "<BetSetting SportType=\"1\" BetType=\"1\" IsLive=\"0\" MaxBet=\"222\" MaxPerEvent=\"3333\"/>"
                + "</ROOT>";

            Assert.AreEqual(expectedCommXlm, createParams["commsetting"]);
            Assert.AreEqual(expectedBetSettingXlm, createParams["betsetting"]);
        }

        [Test]
        public void UpdateCustomerSettingType_SettingItemIsNull_ThrowInvalidOperationException()
        {
            var updateCommand = InstanceFactory.CreateUpdateSettingParam();
            var updateSettingType = new SettingType();

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await settingTypeService.UpdateCustomerSettingType(
                settingContext,
                updateCommand,
                updateSettingType));
        }

        [Test]
        public async Task UpdateCustomerSettingType_Always_CallExecuteUpdate()
        {
            var updateCommand = InstanceFactory.CreateUpdateSettingParam();
            var commBetSetting = StubCommBetSetting();
            StubCommTemplateCache();

            await settingTypeService.UpdateCustomerSettingType(
               settingContext,
               updateCommand,
               commBetSetting);

            var expectedCommXlm =
                  "<ROOT>"
                    + "<Comm SportType=\"1\" BetType=\"2\" IsLive=\"1\" GroupId=\"1\" ValueId=\"1\"/>"
                    + "<Comm SportType=\"1\" BetType=\"2\" IsLive=\"1\" GroupId=\"2\" ValueId=\"5\"/>"
                    + "<Comm SportType=\"1\" BetType=\"2\" IsLive=\"1\" GroupId=\"3\" ValueId=\"4\"/>"
                    + "<Comm SportType=\"1\" BetType=\"1\" IsLive=\"0\" GroupId=\"1\" ValueId=\"4\"/>"
                    + "<Comm SportType=\"1\" BetType=\"1\" IsLive=\"0\" GroupId=\"2\" ValueId=\"7\"/>"
                + "</ROOT>";

            var expectedBetSettingXlm =
                  "<ROOT>"
                    + "<BetSetting SportType=\"1\" BetType=\"2\" IsLive=\"1\" MaxBet=\"111\" MaxPerEvent=\"2222\"/>"
                    + "<BetSetting SportType=\"1\" BetType=\"1\" IsLive=\"0\" MaxBet=\"222\" MaxPerEvent=\"3333\"/>"
                + "</ROOT>";

            await dynamicRepository.Received().ExecuteAsync(Arg.Is<UpdateCommBetSettingCommand>(
                command => command.RoleId == Level.Master.Value
                            && command.IP == settingContext.IP
                            && command.ProductId == 1
                            && command.UserId == 100
                            && command.CommSetting == expectedCommXlm
                            && command.BetSetting == expectedBetSettingXlm));
        }

        [Test]
        public async Task UpdateCustomerSettingType_UpdateSuccess_ReturnSuccessResult()
        {
            var updateCommand = InstanceFactory.CreateUpdateSettingParam(customerLevel: Level.Super);
            var commBetSetting = StubCommBetSetting();

            var result = await settingTypeService.UpdateCustomerSettingType(settingContext, updateCommand, commBetSetting);

            Assert.IsTrue(result.IsSuccessful);
        }

        private static CommBetSetting StubCommBetSetting()
            => new CommBetSetting
            {
                SettingItems = new List<CommBetSettingItem>
                {
                    new CommBetSettingItem
                    {
                        BetTypeId = 2,
                        SportTypeId = 1,
                        IsLive = true,
                        CommissionGroups = new List<CommissionGroup>
                        {
                            new CommissionGroup { Id = 1, Value = 0.00m },
                            new CommissionGroup { Id = 2, Value = 0.2m },
                            new CommissionGroup { Id = 3, Value = 0.15m }
                        },
                        MaxBet = 111,
                        MaxPerEvent = 2222
                    },
                    new CommBetSettingItem
                    {
                        BetTypeId = 1,
                        SportTypeId = 1,
                        IsLive = false,
                        CommissionGroups = new List<CommissionGroup>
                        {
                            new CommissionGroup { Id = 1, Value = 0.15m },
                            new CommissionGroup { Id = 2, Value = 0.3m }
                        },
                        MaxBet = 222,
                        MaxPerEvent = 3333
                    }
                }
            };

        private void StubCommBetSettingConfigs()
        {
            var commBetSettingConfigs = new List<CommBetSettingConfigDto>
            {
                new CommBetSettingConfigDto { SportTypeId = 1, BetTypeId = 1, IsLive = true, GroupId = 1, IntervalValue = 1, MaxDiscount = 0.5m },
                new CommBetSettingConfigDto { SportTypeId = 1, BetTypeId = 1, IsLive = false, GroupId = 1, IntervalValue = 1, MaxDiscount = 0.5m },
                new CommBetSettingConfigDto { SportTypeId = 1, BetTypeId = 2, IsLive = true, GroupId = 1, IntervalValue = 1, MaxDiscount = 0.5m },
                new CommBetSettingConfigDto { SportTypeId = 1, BetTypeId = 2, IsLive = false, GroupId = 1, IntervalValue = 1, MaxDiscount = 0.5m },
                new CommBetSettingConfigDto { SportTypeId = 2, BetTypeId = 1, IsLive = true, GroupId = 1, IntervalValue = 1, MaxDiscount = 0.5m },
                new CommBetSettingConfigDto { SportTypeId = 2, BetTypeId = 1, IsLive = false, GroupId = 1, IntervalValue = 1, MaxDiscount = 0.5m },
                new CommBetSettingConfigDto { SportTypeId = 3, BetTypeId = 1, IsLive = true, GroupId = 1, IntervalValue = 1, MaxDiscount = 0.5m },
                new CommBetSettingConfigDto { SportTypeId = 3, BetTypeId = 1, IsLive = false, GroupId = 1, IntervalValue = 1, MaxDiscount = 0.5m },
                new CommBetSettingConfigDto { SportTypeId = 3, BetTypeId = 2, IsLive = true, GroupId = 1, IntervalValue = 1, MaxDiscount = 0.5m }
            };

            cacheService.GetOrSetAsync(
                        Arg.Any<string>(),
                        Arg.Any<Func<IEnumerable<CommBetSettingConfigDto>>>(),
                        Arg.Any<CacheItemOptions>())
                        .Returns(commBetSettingConfigs);
        }

        private IEnumerable<CommBetSettingDto> StubCommBetSettingList(
            bool isCreateNew = true,
            int customerId = 1,
            byte level = 4)
        {
            IEnumerable<CommBetSettingDto> commBetSettings = new List<CommBetSettingDto>
            {
                new CommBetSettingDto { SportTypeId = 1, BetTypeId = 1, IsLive = true, GroupId = 1, GroupName = "A", Comm = 0.0025m, MaxBet = 120, MaxPerEvent = 1000 },
                new CommBetSettingDto { SportTypeId = 1, BetTypeId = 1, IsLive = true, GroupId = 2, GroupName = "B", Comm = 0.0035m, MaxBet = 120, MaxPerEvent = 1000 },
                new CommBetSettingDto { SportTypeId = 1, BetTypeId = 1, IsLive = true, GroupId = 3, GroupName = "C", Comm = 0.0045m, MaxBet = 120, MaxPerEvent = 1000 },
                new CommBetSettingDto { SportTypeId = 1, BetTypeId = 1, IsLive = true, GroupId = 4, GroupName = "D", Comm = 0.0055m, MaxBet = 120, MaxPerEvent = 1000 },
                new CommBetSettingDto { SportTypeId = 1, BetTypeId = 1, IsLive = false, GroupId = 5, GroupName = "A", Comm = 0.0025m, MaxBet = 121, MaxPerEvent = 10001 },
                new CommBetSettingDto { SportTypeId = 1, BetTypeId = 1, IsLive = false, GroupId = 6, GroupName = "B", Comm = 0.0035m, MaxBet = 121, MaxPerEvent = 10001 },
                new CommBetSettingDto { SportTypeId = 1, BetTypeId = 1, IsLive = false, GroupId = 7, GroupName = "C", Comm = 0.0025m, MaxBet = 121, MaxPerEvent = 10001 },
                new CommBetSettingDto { SportTypeId = 1, BetTypeId = 2, IsLive = true, GroupId = 8, GroupName = "A", Comm = 0.0025m, MaxBet = 122, MaxPerEvent = 10002 },
                new CommBetSettingDto { SportTypeId = 1, BetTypeId = 2, IsLive = true, GroupId = 9, GroupName = "B", Comm = 0.0025m, MaxBet = 122, MaxPerEvent = 10002 },
                new CommBetSettingDto { SportTypeId = 1, BetTypeId = 2, IsLive = false, GroupId = 10, GroupName = "C", Comm = 0.0025m, MaxBet = 122, MaxPerEvent = 10002 },
                new CommBetSettingDto { SportTypeId = 1, BetTypeId = 2, IsLive = false, GroupId = 11, GroupName = "D", Comm = 0.0025m, MaxBet = 122, MaxPerEvent = 10002 },
                new CommBetSettingDto { SportTypeId = 2, BetTypeId = 1, IsLive = true, GroupId = 12, GroupName = "A", Comm = 0.0025m, MaxBet = 123, MaxPerEvent = 10004 },
                new CommBetSettingDto { SportTypeId = 2, BetTypeId = 1, IsLive = true, GroupId = 13, GroupName = "B", Comm = 0.0025m, MaxBet = 123, MaxPerEvent = 10004 },
                new CommBetSettingDto { SportTypeId = 2, BetTypeId = 1, IsLive = true, GroupId = 14, GroupName = "C", Comm = 0.0025m, MaxBet = 123, MaxPerEvent = 10004 },
                new CommBetSettingDto { SportTypeId = 2, BetTypeId = 1, IsLive = true, GroupId = 15, GroupName = "D", Comm = 0.0025m, MaxBet = 123, MaxPerEvent = 10004 },
                new CommBetSettingDto { SportTypeId = 3, BetTypeId = 1, IsLive = true, GroupId = 16, GroupName = "A", Comm = 0.0025m, MaxBet = 124, MaxPerEvent = 10005 }
            };

            dynamicRepository.FetchAsync<CommBetSettingDto>(
                Arg.Is<GetCommBetSettingCriteria>(
                    criteria => criteria.IsCreateNew == isCreateNew
                                && criteria.CustId == customerId
                                && criteria.RoleId == level))
                                .Returns(Task.FromResult(commBetSettings));

            return commBetSettings;
        }

        private void StubCommTemplateCache()
        {
            IEnumerable<CommTemplate> commTemplates = new List<CommTemplate>
            {
                new CommTemplate { Id = 1, Value = 0.0000m },
                new CommTemplate { Id = 2, Value = 0.0005m },
                new CommTemplate { Id = 3, Value = 0.001m },
                new CommTemplate { Id = 4, Value = 0.0015m },
                new CommTemplate { Id = 5, Value = 0.002m },
                new CommTemplate { Id = 6, Value = 0.0025m },
                new CommTemplate { Id = 7, Value = 0.003m }
            };

            cacheService.GetOrSetAsync(
                       Arg.Any<string>(),
                       Arg.Any<Func<IEnumerable<CommTemplate>>>(),
                       Arg.Any<CacheItemOptions>())
                       .Returns(commTemplates);
        }
    }
}