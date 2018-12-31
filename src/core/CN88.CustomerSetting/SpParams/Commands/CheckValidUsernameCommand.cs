namespace CN88.CustomerSetting.SpParams.Commands
{
    using Fanex.Data.Repository;

    public class CheckValidUsernameCommand : NonQueryCommand
    {
        public CheckValidUsernameCommand(string username)
        {
            Username = username;
        }

        public string Username { get; }

        public bool IsExists { get; set; }

        public override string GetSettingKey()
            => "Age_CustomerCheckExists";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(Username);
    }
}