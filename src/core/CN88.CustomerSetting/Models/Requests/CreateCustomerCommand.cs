namespace CN88.CustomerSetting.Models.Requests
{
    using System;

    public class CreateCustomerCommand
    {
        public CreateCustomerCommand(
            ISetting setting,
            int uplineId,
            string hashKey)
        {
            Setting = setting ?? throw new ArgumentNullException(nameof(setting), "Setting cannot be null when create customer.");
            UplineId = uplineId;
            HashKey = hashKey ?? throw new ArgumentNullException(nameof(setting), "Hashkey cannot be null when create customer.");
        }

        public ISetting Setting { get; }

        public int UplineId { get; }

        public string HashKey { get; }
    }
}