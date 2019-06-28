namespace CN88.CustomerSetting.SettingType2.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CN88.CustomerSetting.Models.Enumerations;
    using CN88.CustomerSetting.SettingType2.Models;
    using CN88.CustomerSetting.SettingType2.Models.Dtos;
    using CN88.CustomerSetting.SettingType2.Params.Criteria;
    using Fanex.Data.Repository;

    public interface CurrencyBetSettingService
    {
        Task<IEnumerable<CommBetSettingItem>> GetCurrencyBetSetting(int currencyId);
    }

    public class CurrencyBetSettingServiceImpl : CurrencyBetSettingService
    {
        private readonly IDynamicRepository dynamicRepository;

        public CurrencyBetSettingServiceImpl(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task<IEnumerable<CommBetSettingItem>> GetCurrencyBetSetting(int currencyId)
        {
            var comBetSettingDtoItems = await dynamicRepository.FetchAsync<CommBetSettingDto>(
                new GetCommBetSettingCriteria
                {
                    IsCreateNew = true,
                    RoleId = Level.Super.Value,
                    Currency = currencyId
                }).ConfigureAwait(false);

            return comBetSettingDtoItems.Select(item => new CommBetSettingItem
            {
                SportTypeId = item.SportTypeId,
                BetTypeId = item.BetTypeId,
                IsLive = item.IsLive,

                MaxBet = item.MaxBet,
                MaxMaxBet = item.MaxMaxBet,
                MinMaxBet = item.MinMaxBet,

                MaxPerEvent = item.MaxPerEvent,
                MaxMaxPerEvent = item.MaxMaxPerEvent,
                MinMaxPerEvent = item.MinMaxPerEvent,
            });
        }
    }
}