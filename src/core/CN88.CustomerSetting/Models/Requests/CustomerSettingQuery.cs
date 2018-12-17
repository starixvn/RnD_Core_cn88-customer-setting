namespace CN88.CustomerSetting.Models.Requests
{
    using System;
    using Enumerations;

    public class CustomerSettingQuery
    {
        private int customerId;
        private Level customerLevel;
        private byte productId;
        private Level uplineLevel;


        public CustomerSettingQuery(
          bool isCreateNew,
          int customerId,
          Level customerLevel,
          byte productId,
          Level uplineLevel = null)
        {
            IsCreateNew = isCreateNew;
            CustomerId = customerId;
            CustomerLevel = customerLevel;
            ProductId = productId;
            UplineLevel = uplineLevel;
        }

        public CustomerSettingQuery(
           int customerId,
           byte customerLevel,
           byte productId,
           byte? uplineLevel)
        {
            CustomerId = customerId;
            CustomerLevel = Enumeration.FromValue<Level>(customerLevel);
            ProductId = productId;
            UplineLevel = uplineLevel.HasValue 
                ? Enumeration.FromValue<Level>(uplineLevel.Value) 
                : null;
        }

        public bool IsCreateNew { get; }

        public bool IsNotCreateNew => !IsCreateNew;

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

        public Level UplineLevel
        {
            get
            {
                return uplineLevel;
            }

            set
            {
                if (value < Level.Agent || value > Level.Super)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Upline Level is out of value range.");
                }

                uplineLevel = value;
            }
        }
    }
}