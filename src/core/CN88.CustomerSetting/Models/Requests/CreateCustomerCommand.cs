namespace CN88.CustomerSetting.Models.Requests
{
    using System;

    public class CreateCustomerCommand
    {
        public CreateCustomerCommand(
            ISetting setting,
            int uplineId = 0)
        {
            Setting = setting ?? throw new ArgumentNullException(nameof(setting), "Setting cannot be null when update customer setting.");
            UplineId = uplineId;
        }

        public ISetting Setting { get; }

        public int UplineId { get; }
    }
}