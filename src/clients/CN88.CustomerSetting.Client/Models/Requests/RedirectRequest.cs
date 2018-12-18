namespace CN88.CustomerSetting.Client.Models.Requests
{
    using System.ComponentModel.DataAnnotations;

    public class RedirectRequest : QuerySettingRequest
    {
        [Range(1, short.MaxValue)]
        public short SettingTypeId { get; set; }

        [Range(0, byte.MaxValue)]
        public byte ActionId { get; set; }

        [Range(0, 2)]
        public byte ActionType { get; set; }

        public string JsonParameters { get; set; }
    }
}