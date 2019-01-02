namespace CN88.CustomerSetting.SettingType1.Params.Criteria
{
    using Fanex.Data.Repository;

    public class GetSpreadGroupCriteria : CriteriaBase
    {
        public override string GetSettingKey()
            => "CS_SpreadGroupSel";

        public override bool IsValid()
            => true;
    }
}