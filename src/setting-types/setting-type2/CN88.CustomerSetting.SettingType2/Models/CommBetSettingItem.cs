namespace CN88.CustomerSetting.SettingType2.Models
{
    using System.Linq;
    using System.Collections.Generic;
    using CN88.CustomerSetting.Models;

    public class CommBetSettingItem : SettingItemDefault
    {
        private const int OneHundredPercent = 100;

        private const string CommXmlFormat
            = "<Comm {0} GroupId=\"{1}\" ValueId=\"{2}\"/>";

        private const string BetSettingXmlFormat
            = "<BetSetting {0} MaxBet=\"{1}\" MaxPerMatch=\"{2}\"/>";

        public IEnumerable<CommissionGroup> CommissionGroups { get; set; }

        public decimal MaxMaxBet { get; set; }

        public decimal MinMaxBet { get; set; }

        public decimal MaxBet { get; set; }

        public decimal MaxPerMatch { get; set; }

        public decimal MaxMaxPerMatch { get; set; }

        public decimal MinMaxPerMatch { get; set; }

        public decimal MinComm { get; set; }

        public decimal MaxComm { get; set; }

        public decimal MaxDiscount { get; set; }

        public string ToCommXml(IEnumerable<CommTemplate> commTemplates)
            => string.Join(string.Empty,
                        CommissionGroups.Select(comm => 
                                string.Format(
                                    CommXmlFormat, 
                                    base.ToXml(), 
                                    comm.Id, 
                                    GetCommValueId(comm.Value > 0 
                                        ? comm.Value/OneHundredPercent 
                                        : 0, 
                                        commTemplates))));

        private int? GetCommValueId(decimal value, IEnumerable<CommTemplate> commTemplates)
            => commTemplates
                    .FirstOrDefault(commTemplate => commTemplate.Value == value)?
                    .Id;

        public string ToBetSettingXml() 
            => string.Format(
                BetSettingXmlFormat, 
                base.ToXml(), 
                MaxBet, 
                MaxPerMatch);
    }
}