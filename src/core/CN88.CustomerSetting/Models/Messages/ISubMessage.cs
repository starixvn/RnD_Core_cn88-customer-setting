namespace CN88.CustomerSetting.Models.Messages
{
    using Services;

    public interface ISubMessage
    {
        string ToString(ResourceService resourceService, string language, byte settingLevel);
    }
}