namespace CN88.CustomerSetting.SettingType1.Models
{
    public class MasterAccountDetail : AccountDetail
    {
        private const string PTXmlFormat
            = "<PT {0} SuperPT=\"{1}\"/>";

        public decimal SuperPT { get; set; }

        public override string ToXml()
            => string.Format(PTXmlFormat, base.ToXml(), SuperPT);
    }
}