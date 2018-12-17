namespace CN88.CustomerSetting.Models
{
    using Results;

    public interface ISettingItem
    {
        /// <summary>
        /// For Controller create exactly concrete instance when getting json from client.
        /// </summary>
        string TypeName { get; }

        IResult Validate();

        string ToXml();
    }
}
