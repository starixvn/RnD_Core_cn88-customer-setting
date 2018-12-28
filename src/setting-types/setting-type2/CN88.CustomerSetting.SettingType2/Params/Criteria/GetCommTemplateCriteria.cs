namespace CN88.CustomerSetting.SettingType2.Params.Criteria
{
    using Fanex.Data.Repository;

    public class GetCommTemplateCriteria : CriteriaBase
    {
        public override string GetSettingKey()
            => "CS_CommTemplateSel";

        public override bool IsValid()
            => true;
    }
}