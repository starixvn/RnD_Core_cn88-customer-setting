namespace CN88.CustomerSetting.SettingType2.UnitTests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Services;
    using CN88.CustomerSetting.SettingType2.Models.Dtos;
    using CN88.CustomerSetting.SettingType2.Services;
    using CN88.CustomerSetting.Tests.Models;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using NSubstitute;
    using NUnit.Framework;

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

        //public Task

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

        private IEnumerable<CommBetSettingDto> StubCommBetSettingList()
        {
            return new List<CommBetSettingDto>
            {
                new CommBetSettingDto { SportTypeId = 1, BetTypeId = 1, IsLive = true, GroupId = 1, GroupName = "A", Comm = 0.0025m, MaxBet = 120, MaxPerMatch = 1000 },
                new CommBetSettingDto { SportTypeId = 1, BetTypeId = 1, IsLive = true, GroupId = 1, GroupName = "B", Comm = 0.0025m, MaxBet = 120, MaxPerMatch = 1000 },
                new CommBetSettingDto { SportTypeId = 1, BetTypeId = 1, IsLive = true, GroupId = 1, GroupName = "C", Comm = 0.0025m, MaxBet = 120, MaxPerMatch = 1000 },
                new CommBetSettingDto { SportTypeId = 1, BetTypeId = 1, IsLive = true, GroupId = 1, GroupName = "D", Comm = 0.0025m, MaxBet = 120, MaxPerMatch = 1000 },
                new CommBetSettingDto { SportTypeId = 1, BetTypeId = 1, IsLive = false, GroupId = 1, GroupName = "A", Comm = 0.0025m, MaxBet = 121, MaxPerMatch = 10001 },
                new CommBetSettingDto { SportTypeId = 1, BetTypeId = 1, IsLive = false, GroupId = 1, GroupName = "B", Comm = 0.0025m, MaxBet = 121, MaxPerMatch = 10001 },
                new CommBetSettingDto { SportTypeId = 1, BetTypeId = 1, IsLive = false, GroupId = 1, GroupName = "C", Comm = 0.0025m, MaxBet = 121, MaxPerMatch = 10001 },
                new CommBetSettingDto { SportTypeId = 1, BetTypeId = 2, IsLive = true, GroupId = 1, GroupName = "A", Comm = 0.0025m, MaxBet = 122, MaxPerMatch = 10002 },
                new CommBetSettingDto { SportTypeId = 1, BetTypeId = 2, IsLive = true, GroupId = 1, GroupName = "B", Comm = 0.0025m, MaxBet = 122, MaxPerMatch = 10002 },
                new CommBetSettingDto { SportTypeId = 1, BetTypeId = 2, IsLive = false, GroupId = 1, GroupName = "C", Comm = 0.0025m, MaxBet = 122, MaxPerMatch = 10002 },
                new CommBetSettingDto { SportTypeId = 1, BetTypeId = 2, IsLive = false, GroupId = 1, GroupName = "D", Comm = 0.0025m, MaxBet = 122, MaxPerMatch = 10002 },
                new CommBetSettingDto { SportTypeId = 2, BetTypeId = 1, IsLive = true, GroupId = 1, GroupName = "A", Comm = 0.0025m, MaxBet = 123, MaxPerMatch = 10004 },
                new CommBetSettingDto { SportTypeId = 2, BetTypeId = 1, IsLive = true, GroupId = 1, GroupName = "B", Comm = 0.0025m, MaxBet = 123, MaxPerMatch = 10004 },
                new CommBetSettingDto { SportTypeId = 2, BetTypeId = 1, IsLive = true, GroupId = 1, GroupName = "C", Comm = 0.0025m, MaxBet = 123, MaxPerMatch = 10004 },
                new CommBetSettingDto { SportTypeId = 2, BetTypeId = 1, IsLive = true, GroupId = 1, GroupName = "D", Comm = 0.0025m, MaxBet = 123, MaxPerMatch = 10004 },
                new CommBetSettingDto { SportTypeId = 3, BetTypeId = 1, IsLive = true, GroupId = 1, GroupName = "A", Comm = 0.0025m, MaxBet = 124, MaxPerMatch = 10005 }
            };
        }
    }
}
