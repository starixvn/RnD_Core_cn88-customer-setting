namespace CN88.CustomerSetting.SettingType2.Models
{
    using CN88.CustomerSetting.Models;

    public class CommBetSettingItem : SettingItemDefault
    {
        public decimal MaxMaxBet { get; set; }

        public decimal MinMaxBet { get; set; }

        public decimal MaxBet { get; set; }

        public decimal MaxPerMatch { get; set; }

        public decimal MaxMaxPerMatch { get; set; }

        public decimal MinMaxPerMatch { get; set; }

        public decimal MinComm { get; set; }

        public decimal MaxComm { get; set; }
    }
}