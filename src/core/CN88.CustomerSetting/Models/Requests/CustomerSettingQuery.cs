namespace CN88.CustomerSetting.Models.Requests
{
    using System;
    using Enumerations;

    public class CustomerSettingQuery
    {
        private Level customerLevel;
        private byte productId;

        public CustomerSettingQuery(
           bool isCreateNew,
           int customerId,
           byte customerLevel,
           byte productId,
           int uplineId = 0)
        {
            IsCreateNew = isCreateNew;
            CustomerId = customerId;
            CustomerLevel = Enumeration.FromValue<Level>(customerLevel);
            ProductId = productId;
            UplineId = uplineId;
        }

        public bool IsCreateNew { get; }

        public bool IsNotCreateNew => !IsCreateNew;

        public int CustomerId { get; }

        public int UplineId { get; }

        public Level CustomerLevel
        {
            get
            {
                return customerLevel;
            }

            set
            {
                if (value < Level.Member || value > Level.Super)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Invalid customer level.");
                }

                customerLevel = value;
            }
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
    }
}