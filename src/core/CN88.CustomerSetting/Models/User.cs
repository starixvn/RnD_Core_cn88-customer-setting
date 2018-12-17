namespace CN88.CustomerSetting.Models
{
    using System;
    using Enumerations;

    [Serializable]
    public class User
    {
        private int id;
        private string name;
        private int subAccountId;
        private Level level;

        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "User Id cannot be negative or zero.");
                }

                id = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Username cannot be null or empty.");
                }

                name = value;
            }
        }

        public int SubAccountId
        {
            get
            {
                return subAccountId;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Sub account id cannot be negative.");
                }

                subAccountId = value;
            }
        }

        public Level Level
        {
            get
            {
                return level;
            }

            set
            {
                if (ReferenceEquals(value, null))
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (value == Level.Member)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Cannot set level Member to user");
                }

                level = value;
            }
        }

        public bool IsCompany => Level.IsCompanyLevel();

        public bool IsNotCompany => !IsCompany;
    }
}
