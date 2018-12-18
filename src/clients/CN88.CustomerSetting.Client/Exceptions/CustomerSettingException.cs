namespace CN88.CustomerSetting.Client.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class CustomerSettingException : Exception, ISerializable
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