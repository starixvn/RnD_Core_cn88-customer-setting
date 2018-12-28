namespace CN88.CustomerSetting.SettingType2.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Enumerations;
    using CN88.CustomerSetting.Models.Requests;
    using CN88.CustomerSetting.Models.Results;
    using CN88.CustomerSetting.Services;
    using CN88.CustomerSetting.SettingType2.Models;
    using CN88.CustomerSetting.SettingType2.Models.Dtos;
    using CN88.CustomerSetting.SettingType2.Params.Criteria;
    using Fanex.Data.Repository;

    public class SettingTypeServiceImpl : SettingTypeServiceBase, SettingTypeService
    {
        private const int SettingTypeId = 2;
        private readonly IDynamicRepository dynamicRepository;

        public SettingTypeServiceImpl() : this(null)
        {
        }

        internal SettingTypeServiceImpl(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public override SettingType GetCustomerSettingType(
            ISettingContext settingContext, 
            CustomerSettingQuery query)
        {
            var settingTypeConfig = GetSettingTypeConfig<CommBetSettingConfigDto>(query.ProductId, SettingTypeId)
                                    .GroupBy(config => new
                                    {
                                        config.SportTypeId,
                                        config.BetTypeId,
                                        config.IsLive
                                    });

            var commBetSettingDtoList = query.IsCreateNew
                ? dynamicRepository.Fetch<CommBetSettingDto>(new GetCreateNewCommBetSettingCriteria
                    {
                        CustId = query.UplineId,
                        RoleId = query.CustomerLevel.Value
                    })
                : Enumerable.Empty<CommBetSettingDto>();
            var settingItems = new List<CommBetSettingItem>();

            foreach (var settingItemConfig in settingTypeConfig)
            {
                var settingItemDto = commBetSettingDtoList.Where(
                    item => item.SportTypeId == settingItemConfig.Key.SportTypeId
                            && item.BetTypeId == settingItemConfig.Key.BetTypeId
                            && item.IsLive == settingItemConfig.Key.IsLive);

                if(settingItemDto.Any())
                {
                    var firstSettingItemDto = settingItemDto.First();
                    var firstSettingItemConfig = settingItemConfig.ToList().FirstOrDefault();

                    settingItems.Add(new CommBetSettingItem
                    {
                        SportTypeName = $"SportTypeName_{firstSettingItemConfig.SportTypeId}",
                        BetTypeName = $"BetTypeName_{firstSettingItemConfig.BetTypeId}",

                        LayoutType = firstSettingItemConfig.LayoutType,
                        IntervalValue = firstSettingItemConfig.IntervalValue,
                        ChildOrder = firstSettingItemConfig.ChildOrder,

                        SportTypeId = firstSettingItemDto.SportTypeId,
                        BetTypeId = firstSettingItemDto.BetTypeId,
                        IsLive = firstSettingItemDto.IsLive,

                        MaxBet = firstSettingItemDto.MaxBet,
                        MaxMaxBet = firstSettingItemDto.MaxMaxBet,
                        MinMaxBet = firstSettingItemDto.MinMaxBet,

                        MaxPerMatch = firstSettingItemDto.MaxPerMatch,
                        MaxMaxPerMatch = firstSettingItemDto.MaxMaxPerMatch,
                        MinMaxPerMatch = firstSettingItemDto.MinMaxPerMatch,

                        CommissionGroups = settingItemDto.Select(dto =>
                        new CommissionGroup
                        {
                            Id = dto.GroupId,
                            Name = dto.GroupName,
                            MaxValue = dto.MaxComm,
                            MinValue = dto.MinComm,
                            Value = dto.Comm
                        })
                    });
                }
            }

            return new CommBetSetting
            {
                SettingItems = settingItems
            };
        }

        public override IDictionary<string, object> GetCreateCustomerParams(
            ISettingContext settingContext,
            CreateCustomerCommand command,
            SettingType settingType)
        {
            var commBetSetting = (settingType as CommBetSetting) ?? throw new InvalidOperationException("Invalid CommBetSetting object");

            return new Dictionary<string, object>
            {
                { "commsetting", commBetSetting.ToCommXml() },
                { "betsetting", commBetSetting.ToBetSettingXml() },
            };
        }

        public override SettingResult UpdateCustomerSettingType(
            ISettingContext settingContext, 
            int customerId, 
            Level customerLevel, 
            byte productId, 
            SettingType settingType)
        {
            throw new NotImplementedException();
        }
    }
}