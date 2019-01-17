namespace CN88.CustomerSetting.SettingType2.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Enumerations;

    public class CommBetSettingItem : SettingItemDefault
    {
        private const int OneHundredPercent = 100;

        private const string CommXmlFormat
            = "<Comm {0} GroupId=\"{1}\" ValueId=\"{2}\"/>";

        private const string MemberCommXmlFormat
            = "<Comm {0} ValueId=\"{1}\"/>";

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

        public string ToCommXml(IEnumerable<CommTemplate> commTemplates, Level level)
            => level.IsMemberLevel()
                ? string.Join(string.Empty,
                        CommissionGroups.Select(comm =>
                                FormatMemberComXml(commTemplates, comm)))
                : string.Join(string.Empty,
                        CommissionGroups.Select(comm =>
                                FormatComXml(commTemplates, comm)));

        private string FormatMemberComXml(IEnumerable<CommTemplate> commTemplates, CommissionGroup comm)
            => string.Format(MemberCommXmlFormat,
                base.ToXml(),
                GetCommValueId(comm.Value > 0
                    ? comm.Value / OneHundredPercent
                    : 0,
                    commTemplates));

        private string FormatComXml(IEnumerable<CommTemplate> commTemplates, CommissionGroup comm)
            => string.Format(CommXmlFormat,
                base.ToXml(),
                comm.Id,
                GetCommValueId(comm.Value > 0
                    ? comm.Value / OneHundredPercent
                    : 0,
                    commTemplates));

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