namespace CN88.CustomerSetting.SpParams.Criteria
{
    using Fanex.Data.Repository;

    internal class ProductNamesQueryCriteria : CriteriaBase
    {
        public byte? ProductId { get; set; }

        public string LanguageKey { get; set; }

        public override string GetSettingKey() => "CustSetting_ProductNamesSelect";

        public override bool IsValid() => true;
    }
}