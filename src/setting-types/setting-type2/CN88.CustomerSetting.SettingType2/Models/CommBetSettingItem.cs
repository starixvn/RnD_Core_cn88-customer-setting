namespace CN88.CustomerSetting.SettingType2.Models
{
    using CN88.CustomerSetting.Models;

    public class CommBetSettingItem : SettingItemDefault
    {
        public decimal MaxBetMax { get; set; }

        public decimal MaxBetMin { get; set; }

        public decimal MaxBet { get; set; }

        public decimal MaxPerMatch { get; set; }

        public decimal MaxPerMatchMax { get; set; }

        public decimal MaxPerMatchMin { get; set; }
    }
}