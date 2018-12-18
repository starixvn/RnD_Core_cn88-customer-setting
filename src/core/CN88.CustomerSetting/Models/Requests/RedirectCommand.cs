namespace CN88.CustomerSetting.Models.Requests
{
    using System;
    using Enumerations;

    public class RedirectCommand : CustomerSettingQuery
    {
        private short _settingTypeId;
        private byte _actionId;

        public RedirectCommand(
            int customerId,
            Level customerLevel,
            byte productId,
            Level uplineLevel,
            short settingTypeId,
            byte actionId,
            ActionType actionType,
            string jsonParameters)
            : base(false, customerId, customerLevel, productId, uplineLevel)
        {
            SettingTypeId = settingTypeId;
            ActionId = actionId;
            ActionType = actionType;
            JsonParameters = jsonParameters;
        }

        public RedirectCommand(
            int customerId,
            byte customerLevel,
            byte productId,
            byte? uplineLevel,
            short settingTypeId,
            byte actionId,
            ActionType actionType,
            string jsonParameters)
            : base(false, customerId, customerLevel, productId, uplineLevel)
        {
            SettingTypeId = settingTypeId;
            ActionId = actionId;
            ActionType = actionType;
            JsonParameters = jsonParameters;
        }

        public short SettingTypeId
        {
            get
            {
                return _settingTypeId;
            }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(SettingTypeId)} cannot be negative or zero.");
                }

                _settingTypeId = value;
            }
        }

        public byte ActionId
        {
            get
            {
                return _actionId;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(ActionId)} cannot be negative.");
                }

                _actionId = value;
            }
        }

        public ActionType ActionType { get; set; }

        public string JsonParameters { get; set; }
    }
}