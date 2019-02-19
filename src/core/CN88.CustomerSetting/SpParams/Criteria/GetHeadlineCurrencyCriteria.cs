namespace CN88.CustomerSetting.SpParams.Criteria
{
    using Fanex.Data.Repository;

    public class GetHeadlineCurrencyCriteria : CriteriaBase
    {
        public GetHeadlineCurrencyCriteria(string username)
        {
            Username = username;
        }

        public string Username { get; }

        public override string GetSettingKey()
            => "CS_GetHeadlineCurrency";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(Username);
    }
}