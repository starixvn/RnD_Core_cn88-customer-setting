namespace CN88.CustomerSetting.SpParams.Criteria
{
    using Fanex.Data.Repository;

    public class GetHeadlineSuggestionNamesCriteria : CriteriaBase
    {
        public GetHeadlineSuggestionNamesCriteria(string username)
        {
            Username = username;
        }

        public string Username { get; }

        public override string GetSettingKey()
            => "CS_HeadlineUsernameSuggestion";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(Username);
    }
}