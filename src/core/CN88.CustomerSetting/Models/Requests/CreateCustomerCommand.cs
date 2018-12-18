namespace CN88.CustomerSetting.Models.Requests
{
    using System;
    using CN88.CustomerSetting.Models.Enumerations;

    public class CreateCustomerCommand
    {
        private Level uplineLevel;

        public CreateCustomerCommand(
            ISetting setting,
            Level uplineLevel = null)
        {
            Setting = setting ?? throw new ArgumentNullException(nameof(setting), "Setting cannot be null when update customer setting.");
            UplineLevel = uplineLevel;
        }

        public ISetting Setting { get; }

        public Level UplineLevel
        {
            get
            {
                return uplineLevel;
            }

            set
            {
                if (value != null)
                {
                    if (value < Level.Agent || value > Level.Super)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), "Upline Level is out of value range.");
                    }

                    if (Setting.CustomerLevel.IsNotMemberLevel())
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), "Upline should be null when getting customer setting is not member");
                    }
                }
                else if (Setting.CustomerLevel.IsMemberLevel())
                {
                    throw new ArgumentNullException(nameof(value), "Upline cannot be null when update member setting.");
                }

                uplineLevel = value;
            }
        }
    }
}