namespace CN88.CustomerSetting.SettingType1.Models
{
    public class SuperAccountDetail : AccountDetailSettingItem
    {
        private const string PTXmlFormat
            = "<PT {0} MinPT=\"{1}\" MaxPT=\"{2}\" />";

        public decimal SuperMinPT { get; set; }

        public decimal MinSuperMinPT { get; set; }

        public decimal MaxSuperMinPT { get; set; }

        public decimal SuperMaxPT { get; set; }

        public decimal MinSuperMaxPT { get; set; }

        public decimal MaxSuperMaxPT { get; set; }

        public override string ToXml()
            => string.Format(PTXmlFormat, base.ToXml(), SuperMinPT, SuperMaxPT);
    }
}