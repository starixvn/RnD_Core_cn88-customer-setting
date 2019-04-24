namespace CN88.CustomerSetting.SettingType1.Models.PTItems
{
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Results;

    public class AgentPTItem : SettingItemDefault
    {
        private const string PTXmlFormat
            = "<PT {0} MasterPT=\"{1}\" AgentPT=\"{2}\" />";

        public decimal MinAllowedPT { get; set; }

        public decimal MaxAllowedPT { get; set; }

        public decimal MasterPT { get; set; }

        public decimal CurrentMasterPT { get; set; }

        public decimal MinMasterPT { get; set; }

        public decimal MaxMasterPT { get; set; }

        public decimal AgentPT { get; set; }

        public decimal CurrentAgentPT { get; set; }

        public decimal MinAgentPT { get; set; }

        public decimal MaxAgentPT { get; set; }

        public override IResult Validate()
        {
            if (MasterPT < 0 || AgentPT < 0)
            {
                return Result.CreateErrorResult(CustomerSetting.AppResources.Resources.ValuePositive);
            }

            return base.Validate();
        }

        public override string ToXml()
            => string.Format(
                PTXmlFormat,
                base.ToXml(),
                MasterPT,
                AgentPT);
    }
}