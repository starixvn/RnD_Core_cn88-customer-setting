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
        private byte productId;
        private Level customerLevel;

        public Setting()
        {
        }

        public Setting(
            int customerId,
            Level customerLevel,
            bool isCreateNew,
            byte productId,
            IEnumerable<SettingType> settingTypes)
        {
            CustomerId = customerId;
            CustomerLevel = customerLevel;
            ProductId = productId;
            IsCreateNew = isCreateNew;
            SettingTypes = settingTypes ?? Enumerable.Empty<SettingType>();
        }

        public Setting(
            int customerId,
            Level customerLevel,
            bool isCreateNew,
            byte productId,
            string productName,
            IEnumerable<SettingType> settingTypes)
            : this(customerId, customerLevel, isCreateNew, productId, settingTypes)
        {
            ProductName = productName;
        }

        public byte ProductId
        {
            get
            {
                return productId;
            }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Product Id cannot be negative or zero.");
                }

                productId = value;
            }
        }

        public string ProductName { get; protected set; }

        public int CustomerId { get; set; }

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

        public IEnumerable<SettingType> SettingTypes { get; set; } = Enumerable.Empty<SettingType>();

        public IResult WarningMessage { get; set; }

        public bool IsCreateNew { get; set; }
    }
}