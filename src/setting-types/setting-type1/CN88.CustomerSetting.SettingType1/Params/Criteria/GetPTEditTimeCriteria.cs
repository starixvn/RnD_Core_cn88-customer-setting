namespace CN88.CustomerSetting.SettingType1.Params.Criteria
{
    using Fanex.Data.Repository;

    public class GetPTEditTimeCriteria : CriteriaBase
    {
        public override string GetSettingKey()
            => "CS_EditablePTDateSel";

        public override bool IsValid()
            => true;
    }
}