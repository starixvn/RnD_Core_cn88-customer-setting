namespace CN88.CustomerSetting.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Enumerations;
    using Results;

    [Serializable]
    public class Setting : ISetting
    {
        private int customerId;
        private Level customerLevel;

        public Setting(
            int customerId,
            Level customerLevel,
            byte productId,
            IEnumerable<SettingType> settingTypes)
        {
            CustomerId = customerId;
            CustomerLevel = customerLevel;
            ProductId = productId;
            SettingTypes = settingTypes ?? Enumerable.Empty<SettingType>();
        }

        public Setting(
            int customerId,
            Level customerLevel,
            byte productId,
            string productName,
            IEnumerable<SettingType> settingTypes)
            : this(customerId, customerLevel, productId, settingTypes)
        {
            ProductName = productName;
        }

        public byte ProductId { get; }

        public string ProductName { get; protected set; }

        public int CustomerId
        {
            get
            {
                return customerId;
            }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Customer Id cannot be negative or zero.");
                }

                customerId = value;
            }
        }

        public Level CustomerLevel
        {
            get
            {
                return customerLevel;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (value < Level.Member || value > Level.Super)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Invalid customer level.");
                }

                customerLevel = value;
            }
        }

        public IEnumerable<SettingType> SettingTypes { get; }

        public IResult WarningMessage { get; set; }
    }
}