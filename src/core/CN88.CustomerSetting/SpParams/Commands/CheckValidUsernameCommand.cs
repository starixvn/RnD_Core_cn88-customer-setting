namespace CN88.CustomerSetting.SpParams.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data.Repository;

    public class CheckValidUsernameCommand : NonQueryCommand
    {
        public string Username { get; set; }

        public bool IsExists { get; set; }

        public override string GetSettingKey()
            => "Age_CustomerCheckExists";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(Username);
    }
}
