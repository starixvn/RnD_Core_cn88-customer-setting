namespace CN88.CustomerSetting.SettingType2.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Requests;
    using CN88.CustomerSetting.Models.Results;
    using CN88.CustomerSetting.Services;
    using CN88.CustomerSetting.SettingType2.AppResources;
    using CN88.CustomerSetting.SettingType2.Models;
    using CN88.CustomerSetting.SettingType2.Models.Dtos;
    using CN88.CustomerSetting.SettingType2.Params.Commands;
    using CN88.CustomerSetting.SettingType2.Params.Criteria;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Fanex.Logging;

    public class SettingTypeServiceImpl : SettingTypeServiceBase, SettingTypeService
    {
        private const string CommTemplateCacheKey = "CN88.CustomerSetting.SettingType2.Services.CommTemplateCacheKey";
        private const int CommTemplateCacheInHours = 4;
        private const int SettingTypeId = 2;

        public SettingTypeServiceImpl() :
            this(null, null, null, null, null)
        { }

        internal SettingTypeServiceImpl(
            ResourceService resourceService,
            ILogger logger,
            ICacheService cacheService,
            IDynamicRepository dynamicRepository,
            MessageService messageService)
            : base(resourceService, logger, cacheService, dynamicRepository)
        {
            MessageService = messageService ?? new MessageServiceImpl(ResourceService);
        }

        public override async Task<SettingType> GetCustomerSettingType(
            ISettingContext settingContext,
            CustomerSettingQuery query)
        {
            var settingTypeConfig = (await GetSettingTypeConfig<CommBetSettingConfigDto>(query.ProductId, SettingTypeId))
                                    .GroupBy(config => new { config.SportTypeId, config.BetTypeId, config.IsLive});

            var commBetSettingDtoList = await GetCommBetSettingDtoList(query);
            var settingItems = new List<CommBetSettingItem>();

            foreach (var settingItemConfig in settingTypeConfig)
            {
                var settingItemDto = commBetSettingDtoList.Where(
                    item => item.SportTypeId == settingItemConfig.Key.SportTypeId
                            && item.BetTypeId == settingItemConfig.Key.BetTypeId
                            && item.IsLive == settingItemConfig.Key.IsLive);

                if (settingItemDto.Any())
                {
                    var settingItem = BuildSettingItem(settingItemDto, settingItemConfig, settingContext.Language);

                    settingItems.Add(settingItem);
                }
            }

            return new CommBetSetting
            {
                SettingItems = settingItems
            };
        }

        private async Task<IEnumerable<CommBetSettingDto>> GetCommBetSettingDtoList(CustomerSettingQuery query)
            => await DynamicRepository.FetchAsync<CommBetSettingDto>(
                new GetCommBetSettingCriteria
                {
                    IsCreateNew = query.IsCreateNew,
                    CustId = query.IsCreateNew ? query.UplineId : query.CustomerId,
                    RoleId = query.CustomerLevel.Value
                });

        private CommBetSettingItem BuildSettingItem(
            IEnumerable<CommBetSettingDto> settingItemDto,
            IEnumerable<CommBetSettingConfigDto> settingItemConfig,
            string language)
        {
            var firstSettingItemDto = settingItemDto.First();
            var firstSettingItemConfig = settingItemConfig.ToList().FirstOrDefault();
            var isLive = firstSettingItemConfig.IsLive ? 1 : 0;

            return new CommBetSettingItem
            {
                SportTypeName = ResourceService.GetSportTypeName(firstSettingItemConfig.SportTypeId, language),
                BetTypeName = Resources.ResourceManager.GetString($"BetTypeName_{firstSettingItemConfig.BetTypeId}_{isLive}"),

                LayoutType = firstSettingItemConfig.LayoutType,
                IntervalValue = firstSettingItemConfig.IntervalValuePercent,
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

                CommissionGroups = settingItemDto.Select(
                    dto => new CommissionGroup
                    {
                        Id = dto.GroupId,
                        Name = dto.GroupName,
                        MaxValue = dto.MaxCommPercent,
                        MinValue = dto.MinCommPercent,
                        Value = dto.CommPercent
                    })
            };
        }

        public override IDictionary<string, object> BuildCreateCustomerParams(
            ISettingContext settingContext,
            CreateCustomerCommand command,
            SettingType settingType)
        {
            var commBetSetting = (settingType as CommBetSetting)
                ?? throw new InvalidOperationException("Invalid CommBetSetting object");

            return new Dictionary<string, object>
            {
                { "commsetting", commBetSetting.ToCommXml(GetCommTemplateSetting().GetAwaiter().GetResult(), command.Setting.CustomerLevel) },
                { "betsetting", commBetSetting.ToBetSettingXml() },
            };
        }

        public override async Task<SettingResult> UpdateCustomerSettingType(
            ISettingContext settingContext,
            UpdateCustomerSettingCommand command,
            SettingType settingType)
        {
            var commBetSetting = settingType as CommBetSetting
                ?? throw new InvalidOperationException("CommBetSetting data is not valid");

            var updateCommResult = await UpdateCommSetting(settingContext, command.Setting, commBetSetting);

            if (updateCommResult.IsFailed)
            {
                return updateCommResult;
            }

            var updateBetSettingResult = await UpdateBetSetting(settingContext, command.Setting, commBetSetting);

            return new SettingResult(
                updateCommResult.Code,
                updateCommResult.IsSuccessful
                    ? Resources.UpdateCommBetSettingSuccess
                    : updateCommResult.Message,
                updateCommResult);
        }

        private async Task<SettingResult> UpdateCommSetting(
            ISettingContext settingContext,
            ISetting setting,
            CommBetSetting settingType)
        {
            var command = new UpdateCommSettingCommand
            {
                RoleId = setting.CustomerLevel.Value,
                CustId = setting.CustomerId,

                ProductId = setting.ProductId,
                Comm = settingType.ToCommXml(await GetCommTemplateSetting(), setting.CustomerLevel),

                IP = settingContext.IP,
                UserId = settingContext.Actor.Id,
                SubAccountId = settingContext.Actor.SubAccountId,
            };

            await DynamicRepository.ExecuteAsync(command);

            return new SettingResult(
                command.ErrCode,
                command.ErrMsg,
                command.MsgType,
                setting.CustomerId,
                settingType.Id,
                settingType.Name);
        }

        private async Task<SettingResult> UpdateBetSetting(
            ISettingContext settingContext,
            ISetting setting,
            CommBetSetting settingType)
        {
            var command = new UpdateBetSettingCommand
            {
                RoleId = setting.CustomerLevel.Value,
                CustId = setting.CustomerId,

                ProductId = setting.ProductId,
                BetSetting = settingType.ToBetSettingXml(),

                IP = settingContext.IP,
                UserId = settingContext.Actor.Id,
                SubAccountId = settingContext.Actor.SubAccountId,
            };

            await DynamicRepository.ExecuteAsync(command);

            return new SettingResult(
                command.ErrCode,
                command.ErrMsg,
                command.MsgType,
                setting.CustomerId,
                settingType.Id,
                settingType.Name);
        }

        private async Task<IEnumerable<CommTemplate>> GetCommTemplateSetting()
            => await CacheService.GetOrSetAsync(
                       CommTemplateCacheKey,
                       () => DynamicRepository.Fetch<CommTemplate>(new GetCommTemplateCriteria()),
                        new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(CommTemplateCacheInHours, 0, 0)));
    }
}