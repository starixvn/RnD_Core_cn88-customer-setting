namespace CN88.CustomerSetting.SpParams.Criteria
{
    using Fanex.Data.Repository;

    internal class ProductSettingTypesQueryCriteria : CriteriaBase
    {
        public byte ProductId { get; set; }

        public byte SystemId { get; set; }

        public override string GetSettingKey()
            => "CustSetting_ProductProfileSelect";

        public override bool IsValid()
            => ProductId > 0;
    }
}