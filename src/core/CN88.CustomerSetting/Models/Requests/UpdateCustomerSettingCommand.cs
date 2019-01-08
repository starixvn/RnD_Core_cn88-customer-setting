namespace CN88.CustomerSetting.Models.Requests
{
    using System;
    using Models;

    public class UpdateCustomerSettingCommand
    {
        public UpdateCustomerSettingCommand(
            ISetting setting)
        {
            Setting = setting ?? throw new ArgumentNullException(nameof(setting), "Setting cannot be null when update customer setting.");
        }

        public ISetting Setting { get; }
    }
}