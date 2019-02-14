namespace CN88.CustomerSetting.SpParams.Criteria
{
    using Fanex.Data.Repository;

    public class GetMainShareholderSuggestionNamesCriteria : CriteriaBase
    {
        public override string GetSettingKey()
            => "GetMainShareholderSuggestionNames";

        public override bool IsValid()
            => true;
    }
}