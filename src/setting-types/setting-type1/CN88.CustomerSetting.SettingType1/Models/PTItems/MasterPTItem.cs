namespace CN88.CustomerSetting.SettingType1.Models.PTItems
{
    public class MasterPTItem : BasePTItem
    {
        private const string PTXmlFormat
            = "<PT {0} SuperPT=\"{1}\" />";

        public decimal? SuperPT { get; set; }

        public decimal CurrentSuperPT { get; set; }

        public decimal MinSuperPT { get; set; }

        public decimal MaxSuperPT { get; set; }

        public override string ToXml()
            => string.Format(
                PTXmlFormat,
                base.ToXml(),
                SuperPT.HasValue ? SuperPT.Value : CurrentSuperPT);

        public override void AdjustPTForQuery(bool isCreateNew)
        {
            if (isCreateNew)
            {
                SuperPT = 0;
            }
            else
            {
                if (SuperPT.HasValue && CurrentSuperPT == SuperPT.Value)
                {
                    SuperPT = null;
                }
            }
        }

        public override bool IsNoChangePT()
            => SuperPT == null;
    }
}