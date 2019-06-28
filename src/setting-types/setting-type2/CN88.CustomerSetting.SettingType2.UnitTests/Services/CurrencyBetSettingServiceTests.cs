namespace CN88.CustomerSetting.SettingType2.UnitTests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CN88.CustomerSetting.SettingType2.Models.Dtos;
    using CN88.CustomerSetting.SettingType2.Params.Criteria;
    using CN88.CustomerSetting.SettingType2.Services;
    using Fanex.Data.Repository;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture(Category = "SettingType2-CurrencyBetSettingService")]
    public class CurrencyBetSettingServiceTests
    {
        private CurrencyBetSettingService currencyBetSettingService;
        private IDynamicRepository dynamicRepository;

        [SetUp]
        public void Setup()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            currencyBetSettingService = new CurrencyBetSettingServiceImpl(dynamicRepository);
        }

        [Test]
        public async Task GetCurrencyBetSetting_Always_ReturnBetSettingItems()
        {
            var currencyId = 11;
            var commBetSettingDtos = new List<CommBetSettingDto>
            {
                new CommBetSettingDto
                {
                    SportTypeId = 1,
                    BetTypeId = 2,
                    IsLive = true,

                    MaxBet = 1,
                    MaxMaxBet = 2,
                    MinMaxBet = 3,

                    MaxPerEvent = 4,
                    MaxMaxPerEvent = 5,
                    MinMaxPerEvent = 6
                }
            };
            dynamicRepository
                .FetchAsync<CommBetSettingDto>(Arg.Is<GetCommBetSettingCriteria>(criteria
                    => criteria.IsCreateNew && criteria.RoleId == 4 && criteria.Currency == currencyId))
                    .Returns(commBetSettingDtos);

            var currencyBetSettings = await currencyBetSettingService.GetCurrencyBetSetting(currencyId);

            Assert.AreEqual(1, currencyBetSettings.Count());
            Assert.AreEqual(1, currencyBetSettings.First().SportTypeId);
            Assert.AreEqual(2, currencyBetSettings.First().BetTypeId);
            Assert.IsTrue(currencyBetSettings.First().IsLive);
            Assert.AreEqual(1, currencyBetSettings.First().MaxBet);
            Assert.AreEqual(2, currencyBetSettings.First().MaxMaxBet);
            Assert.AreEqual(3, currencyBetSettings.First().MinMaxBet);
            Assert.AreEqual(4, currencyBetSettings.First().MaxPerEvent);
            Assert.AreEqual(5, currencyBetSettings.First().MaxMaxPerEvent);
            Assert.AreEqual(6, currencyBetSettings.First().MinMaxPerEvent);
        }
    }
}