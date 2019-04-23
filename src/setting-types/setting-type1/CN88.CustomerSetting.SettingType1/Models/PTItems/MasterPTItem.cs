namespace CN88.CustomerSetting.SettingType1.Models.PTItems
{
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Results;

    public class MasterPTItem : SettingItemDefault, IPTItem
    {
        private const string PTXmlFormat
            = "<PT {0} SuperPT=\"{1}\" />";

        public decimal SuperPT { get; set; }

        public decimal CurrentSuperPT { get; set; }

        public decimal MinSuperPT { get; set; }

        public decimal MaxSuperPT { get; set; }

        public override IResult Validate()
        {
            if (SuperPT < 0)
            {
                return Result.CreateErrorResult(CustomerSetting.AppResources.Resources.ValuePositive);
            }

            return base.Validate();
        }

        public override string ToXml()
            => string.Format(
                PTXmlFormat,
                base.ToXml(),
                SuperPT);

        public void ResetMinMax()
        {
            MinSuperPT = 0;
            MaxSuperPT = 0;
        }
    }
}