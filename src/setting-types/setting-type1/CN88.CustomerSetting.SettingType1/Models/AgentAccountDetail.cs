namespace CN88.CustomerSetting.SettingType1.Models
{
    public class AgentAccountDetail : AccountDetail
    {
        private const string PTXmlFormat
            = "<PT {0} MasterPT=\"{1}\" AgentPT=\"{1}\"/>";

        public decimal MasterPT { get; set; }

        public decimal AgentPT { get; set; }

        public override string ToXml()
            => string.Format(PTXmlFormat, base.ToXml(), MasterPT, AgentPT);
    }
}