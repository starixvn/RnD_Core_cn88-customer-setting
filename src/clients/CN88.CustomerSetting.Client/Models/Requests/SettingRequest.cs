namespace CN88.CustomerSetting.Client.Models.Requests
{
    using System.ComponentModel.DataAnnotations;

    public abstract class SettingRequest
    {
        [Range(2, 4)]
        public byte? UplineLevel { get; set; }

        [Range(1, int.MaxValue)]
        public int ActorId { get; set; }

        [Required]
        public string ActorName { get; set; }

        [Range(2, 5)]
        public byte ActorLevel { get; set; }

        [Range(0, int.MaxValue)]
        public int SubAccountId { get; set; }

        public string Domain { get; set; }

        public string Language { get; set; }

        public string ClientIP { get; set; }

        public byte System { get; set; }
    }
}