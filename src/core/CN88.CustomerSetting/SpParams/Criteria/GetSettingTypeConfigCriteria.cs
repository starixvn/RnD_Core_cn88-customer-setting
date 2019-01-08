namespace CN88.CustomerSetting.SpParams.Criteria
{
    using Fanex.Data.Repository;

    public class GetSettingTypeConfigCriteria : CriteriaBase
    {
        public GetSettingTypeConfigCriteria(int productId, int settingType)
        {
            ProductId = productId;
            SettingType = settingType;
        }

        public int ProductId { get; }

        public int SettingType { get; }

        public override string GetSettingKey()
            => "CS_SettingTypeConfigSel";

        public override bool IsValid()
            => ProductId > 0 && SettingType > 0;
    }
}