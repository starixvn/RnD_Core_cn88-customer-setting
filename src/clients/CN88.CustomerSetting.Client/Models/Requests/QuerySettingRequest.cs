namespace CN88.CustomerSetting.Client.Models.Requests
{
    using System.ComponentModel.DataAnnotations;

    public class QuerySettingRequest : SettingRequest
    {
        [Range(1, 255)]
        public byte ProductId { get; set; }

        [Range(0, int.MaxValue)]
        public int CustomerId { get; set; }

        [Range(1, 4)]
        public byte CustomerLevel { get; set; }

        public bool IsCreateNew { get; set; }
    }
}