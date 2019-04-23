namespace CN88.CustomerSetting.Client.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
#pragma warning disable S1939 // Inheritance list should not be redundant
    public class CustomerSettingException : Exception, ISerializable
#pragma warning restore S1939 // Inheritance list should not be redundant
    {
        public CustomerSettingException()
            : base()
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
            // Add implementation.
        }
    }
}