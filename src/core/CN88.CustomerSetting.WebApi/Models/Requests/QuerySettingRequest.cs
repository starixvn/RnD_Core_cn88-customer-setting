namespace CN88.CustomerSetting.WebApi.Models.Requests
{
    using System.ComponentModel.DataAnnotations;
    using CN88.CustomerSetting.Models.Requests;
#pragma warning disable S109 // Magic numbers should not be used
    public class QuerySettingRequest : SettingRequest
    {

        [Range(1, 255)]

        public byte ProductId { get; set; }

        [Range(0, int.MaxValue)]
        public int CustomerId { get; set; }

        [Range(1, 4)]
        public byte CustomerLevel { get; set; }

        public bool IsCreateNew { get; set; }

        internal CustomerSettingQuery GetCustomerSettingQuery()
            => new CustomerSettingQuery(
             IsCreateNew,
             CustomerId,
             CustomerLevel,
             ProductId,
             UplineId);
    }
#pragma warning restore S109 // Magic numbers should not be used
}