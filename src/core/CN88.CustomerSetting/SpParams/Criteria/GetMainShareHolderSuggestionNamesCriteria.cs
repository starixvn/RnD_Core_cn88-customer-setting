namespace CN88.CustomerSetting.SpParams.Criteria
{
    using Fanex.Data.Repository;

    public class GetMainShareHolderSuggestionNamesCriteria : CriteriaBase
    {
        public override string GetSettingKey()
            => "GetMainShareHolderSuggestionNames";

        public override bool IsValid()
            => true;
    }
}