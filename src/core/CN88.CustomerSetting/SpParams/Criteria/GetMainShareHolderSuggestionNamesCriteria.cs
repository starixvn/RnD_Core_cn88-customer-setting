namespace CN88.CustomerSetting.SpParams.Criteria
{
    using Fanex.Data.Repository;

    public class GetMainShareholderSuggestionNamesCriteria : CriteriaBase
    {
        public GetMainShareholderSuggestionNamesCriteria(string username)
        {
            Username = username;
        }

        public string Username { get; }

        public override string GetSettingKey()
            => "CS_MainShareholderUsernameSuggestion";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(Username);
    }
}