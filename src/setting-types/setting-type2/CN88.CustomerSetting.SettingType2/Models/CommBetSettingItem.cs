namespace CN88.CustomerSetting.SettingType2.Models
{
    using System.Linq;
    using System.Collections.Generic;
    using CN88.CustomerSetting.Models;

    public class CommBetSettingItem : SettingItemDefault
    {
        private const string CommXmlFormat
            = "<Comm {0} GroupId=\"{1}\" ValueId=\"{1}\"/>";

        private const string BetSettingXmlFormat
            = "<BetSetting {0} MaxBet=\"{1}\" MaxPerMatch=\"{1}\"/>";

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

        public string ToCommXml()
            => string.Join(string.Empty,
                CommissionGroups.Select(comm => string.Format(CommXmlFormat, base.ToXml(), comm.Id, comm.Value)));

        public string ToBetSettingXml() => string.Format(BetSettingXmlFormat, base.ToXml(), MaxBet, MaxPerMatch);
    }
}