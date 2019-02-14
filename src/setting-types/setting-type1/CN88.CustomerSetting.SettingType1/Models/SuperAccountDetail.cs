using CN88.CustomerSetting.Models.Results;

namespace CN88.CustomerSetting.SettingType1.Models
{
    public class SuperAccountDetail : AccountDetailSettingItem
    {
        private const string PTXmlFormat
            = "<PT {0} MinPT=\"{1}\" MaxPT=\"{2}\" />";

        public decimal? SuperMinPT { get; set; }

        public decimal CurrentSuperMinPT { get; set; }

        public decimal MinSuperMinPT { get; set; }

        public decimal MaxSuperMinPT { get; set; }

        public decimal? SuperMaxPT { get; set; }

        public decimal CurrentSuperMaxPT { get; set; }

        public decimal MinSuperMaxPT { get; set; }

        public decimal MaxSuperMaxPT { get; set; }

        public override string ToXml()
            => string.Format(
                PTXmlFormat, 
                base.ToXml(), 
                SuperMinPT == 0 ? CurrentSuperMinPT : SuperMinPT.Value,
                SuperMaxPT == 0 ? CurrentSuperMaxPT : SuperMaxPT.Value);

        public override IResult Validate()
        {
            if (SuperMinPT > SuperMaxPT)
            {
                return Result.CreateErrorResult(AppResources.Resources.Msg_30009);
            }

            return base.Validate();
        }

        public override void AdjustPTForQuery(bool isCreateNew)
        {
            if (isCreateNew)
            {
                SuperMinPT = 0;
                SuperMaxPT = 0;
            }
            else
            {
                if (SuperMinPT.HasValue && CurrentSuperMinPT == SuperMinPT.Value)
                {
                    SuperMinPT = null;
                }

                if (SuperMaxPT.HasValue && CurrentSuperMaxPT == SuperMaxPT.Value)
                {
                    SuperMaxPT = null;
                }
            }
        }

        public override bool IsNoChangePT()
            => SuperMinPT == null && SuperMaxPT == null;
    }
}