namespace CN88.CustomerSetting.SettingType1.Models
{
    public class AgentAccountDetail : AccountDetailSettingItem
    {
        private const string PTXmlFormat
            = "<PT {0} MasterPT=\"{1}\" AgentPT=\"{2}\" />";

        public decimal MinAllowedPT { get; set; }

        public decimal MaxAllowedPT { get; set; }

        public decimal? MasterPT { get; set; }

        public decimal CurrentMasterPT { get; set; }

        public decimal MinMasterPT { get; set; }

        public decimal MaxMasterPT { get; set; }

        public decimal? AgentPT { get; set; }

        public decimal CurrentAgentPT { get; set; }

        public decimal MinAgentPT { get; set; }

        public decimal MaxAgentPT { get; set; }

        public override string ToXml()
            => string.Format(
                PTXmlFormat, 
                base.ToXml(), 
                MasterPT == null ? CurrentMasterPT : MasterPT.Value, 
                AgentPT == null ? CurrentAgentPT : AgentPT.Value);

        public override void AdjustPTForQuery()
        {
            if (CurrentAgentPT == AgentPT.Value)
            {
                AgentPT = null;
            }

            if (CurrentMasterPT == MasterPT.Value)
            {
                MasterPT = null;
            }
        }

        public override bool IsNoChangePT()
            => AgentPT == null && MasterPT == null;
    }
}