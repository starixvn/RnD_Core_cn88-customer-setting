namespace CN88.CustomerSetting.WebApi.Models.Requests
{
    using System.ComponentModel.DataAnnotations;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Enumerations;

    public class SettingRequest
    {
        private SettingContext settingContext;

        [Range(1, int.MaxValue)]
        public int ActorId { get; set; }

        [Required]
        public string ActorName { get; set; }

        [Range(2, 5)]
        public byte ActorLevel { get; set; }

        [Range(0, int.MaxValue)]
        public int SubAccountId { get; set; }

        [Range(1, 5)]
        public byte System { get; set; }

        public string Domain { get; set; }

        public string ClientIP { get; set; }

        public string Language { get; set; }

        public int UplineId { get; set; }

        internal ISettingContext GetSettingContext()
        {
            if (settingContext == null)
            {
                var actor = new User
                {
                    Id = ActorId,
                    Name = ActorName,
                    Level = Enumeration.FromValue<Level>(ActorLevel),
                    SubAccountId = SubAccountId
                };

                settingContext = new SettingContext
                {
                    Actor = actor,
                    Language = Language,
                    IP = ClientIP,
                    System = System,
                    Domain = Domain
                };
            }

            return settingContext;
        }
    }
}