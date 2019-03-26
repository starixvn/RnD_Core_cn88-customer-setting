namespace CN88.CustomerSetting.SettingType2.Models.Dtos
{
    public class CommBetSettingConfigDto
    {
        private const int OneHundredPercent = 100;

        public decimal MaxDiscount { get; set; }

        public int GroupId { get; set; }

        public short SportTypeId { get; set; }

        public short BetTypeId { get; set; }

        public decimal IntervalValue { get; set; }

        public decimal IntervalValuePercent => IntervalValue * OneHundredPercent;

        public int ChildOrder { get; set; }

        public int LayoutType { get; set; }

        public bool IsLive { get; set; }
    }
}