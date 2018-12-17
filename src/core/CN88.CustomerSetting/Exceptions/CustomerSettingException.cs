namespace CN88.CustomerSetting.Exceptions
{
    using System;
    using System.Runtime.Serialization;
    using System.Text;
    using Models;
    using Models.Enumerations;

    [Serializable]
    public class CustomerSettingException : Exception
    {
        public CustomerSettingException()
        {
        }

        public CustomerSettingException(string message)
            : base(message)
        {
        }

        public CustomerSettingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CustomerSettingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info != null)
            {
                ActionName = info.GetString("ActionName");

                SystemId = info.GetInt32("SystemId");
                Language = info.GetString("Language");
                IP = info.GetString("IP");
                Domain = info.GetString("Domain");

                ActorId = info.GetInt32("ActorId");
                ActorName = info.GetString("ActorName");
                ActorLevel = info.GetString("ActorLevel");
                SubAccountId = info.GetInt32("SubAccountId");

                ProductId = info.GetInt32("ProductId");
                SettingTypeId = info.GetInt16("SettingTypeId");
                SettingTypeName = info.GetString("SettingTypeName");
                SettingTypeLevel = info.GetByte("SettingTypeLevel");

                CustomerId = info.GetInt32("CustomerId");
                CustomerIds = info.GetString("CustomerIds");
                CustomerLevel = info.GetString("CustomerLevel");
                UplineLevel = info.GetString("UplineLevel");
            }
        }

        public string OriginalMessage
        {
            get
            {
                return base.Message;
            }
        }

        public string ActionName { get; set; }

        //// System information

        public int SystemId { get; set; }

        public string Language { get; set; }

        public string IP { get; set; }

        public string Domain { get; set; }

        //// Actor information

        public int ActorId { get; set; }

        public string ActorName { get; set; }

        public string ActorLevel { get; set; }

        public int SubAccountId { get; set; }

        //// Product information

        public int ProductId { get; set; }

        public short SettingTypeId { get; set; }

        public string SettingTypeName { get; set; }

        public byte SettingTypeLevel { get; set; }

        //// Customer information

        public int CustomerId { get; set; }

        public string CustomerIds { get; set; }

        public string CustomerLevel { get; set; }

        public string UplineLevel { get; set; }

        public override string Message
        {
            get
            {
                return BuildMessage();
            }
        }

        public void SetSettingContextInfo(string actionName, ISettingContext settingContext)
        {
            if (settingContext == null)
            {
                throw new ArgumentNullException(nameof(settingContext), "Setting Context cannot be null");
            }

            ActionName = actionName;

            SystemId = settingContext.System;
            Language = settingContext.Language;
            IP = settingContext.IP;
            Domain = settingContext.Domain;

            ActorId = settingContext.Actor.Id;
            ActorName = settingContext.Actor.Name;
            ActorLevel = settingContext.Actor.Level.DisplayName;
            SubAccountId = settingContext.Actor.SubAccountId;
        }

        public void SetProductInfo(byte productId, SettingType settingType = null)
        {
            ProductId = productId;

            if (settingType != null)
            {
                SettingTypeId = settingType.Id;
                SettingTypeName = settingType.Name;
                SettingTypeLevel = settingType.SettingLevel;
            }
        }

        public void SetCustomerInfo(int customerId, Level customerLevel, Level uplineLevel = null)
        {
            CustomerId = customerId;

            SetCustomerLevelAndUplineLevel(customerLevel, uplineLevel);
        }

        public void SetCustomerInfo(string customerIds, Level customerLevel, Level uplineLevel = null)
        {
            CustomerIds = customerIds;

            SetCustomerLevelAndUplineLevel(customerLevel, uplineLevel);
        }

        private void SetCustomerLevelAndUplineLevel(Level customerLevel, Level uplineLevel)
        {
            CustomerLevel = customerLevel.DisplayName;

            if (uplineLevel != null)
            {
                UplineLevel = uplineLevel.DisplayName;
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info != null)
            {
                info.AddValue("ActionName", ActionName);

                info.AddValue("SystemId", SystemId);
                info.AddValue("Language", Language);
                info.AddValue("Domain", Domain);
                info.AddValue("IP", IP);

                info.AddValue("ActorId", ActorId);
                info.AddValue("ActorName", ActorName);
                info.AddValue("ActorLevel", ActorLevel);
                info.AddValue("SubAccountId", SubAccountId);

                info.AddValue("ProductId", ProductId);
                info.AddValue("SettingTypeId", SettingTypeId);
                info.AddValue("SettingTypeName", SettingTypeName);
                info.AddValue("SettingTypeLevel", SettingTypeLevel);

                info.AddValue("CustomerId", CustomerId);
                info.AddValue("CustomerIds", CustomerIds);
                info.AddValue("CustomerLevel", CustomerLevel);
                info.AddValue("UplineLevel", UplineLevel);
            }
        }

        private string BuildMessage()
        {
            var messageBuilder = new StringBuilder();

            messageBuilder.AppendLine(base.Message);

            messageBuilder.AppendLine($"Action: {ActionName}");
            messageBuilder.AppendLine($"Language: {Language}");
            messageBuilder.AppendLine($"IP: {IP}");
            messageBuilder.AppendLine($"System: {SystemId}");
            messageBuilder.AppendLine($"Domain: {Domain}");

            messageBuilder.AppendLine($"Actor Id: {ActorId}");
            messageBuilder.AppendLine($"Actor Name: {ActorName}");
            messageBuilder.AppendLine($"Actor Level: {ActorLevel}");
            messageBuilder.AppendLine($"Sub Account Id: {SubAccountId}");

            messageBuilder.AppendLine($"Customer Id(s): {(CustomerId > 0 ? CustomerId.ToString() : CustomerIds)}");
            messageBuilder.AppendLine($"Customer Level: {CustomerLevel}");
            messageBuilder.AppendLine($"Upline Level: {UplineLevel}");

            messageBuilder.AppendLine($"ProductId: {ProductId}");

            messageBuilder.AppendLine($"Setting Type Id: {SettingTypeId}");
            messageBuilder.AppendLine($"Setting Type Name: {SettingTypeName}");
            messageBuilder.AppendLine($"Setting Type Level: {SettingTypeLevel}");

            return messageBuilder.ToString();
        }
    }
}