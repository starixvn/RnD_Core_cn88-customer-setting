namespace CN88.CustomerSetting.SettingType2.Models.Dtos
{
    public class CommBetSettingDto : CommBetSettingConfigDto
    {
        private const int OneHundredPercent = 100;

        public string GroupName { get; set; }

        public decimal MaxMaxBet { get; set; }

        public decimal MinMaxBet { get; set; }

        public decimal MaxBet { get; set; }

        public decimal MaxPerMatch { get; set; }

        public decimal MaxMaxPerMatch { get; set; }

        public decimal MinMaxPerMatch { get; set; }

        public decimal MinComm { get; set; }

        public decimal MinCommPercent => MinComm * OneHundredPercent;

        public decimal MaxComm { get; set; }

        public decimal MaxCommPercent => MaxComm * OneHundredPercent;

        public decimal Comm { get; set; }

        public decimal CommPercent => Comm * OneHundredPercent;
    }
}