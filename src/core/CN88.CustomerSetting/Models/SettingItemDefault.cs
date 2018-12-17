namespace CN88.CustomerSetting.Models
{
    using Results;

    public class SettingItemDefault : ISettingItem
    {
        private const string SportTypeBetTypeXmlFormat = "SportType=\"{0}\" BetType=\"{1}\"";

        public short SportTypeId { get; set; }

        public string SportTypeName { get; set; }

        public short BetTypeId { get; set; }

        public string BetTypeName { get; set; }

        /// <summary>
        /// For Controller create exactly concrete class when get json from client.
        /// </summary>
        public string TypeName => GetType().AssemblyQualifiedName;

        public virtual IResult Validate()
            => SportTypeId <= 0 || BetTypeId <= 0
            ? new Result(Constants.ResultValues.DataIsNotValid, AppResources.Messages.SportTypeOrBetTypeIsInvalid)
            : Result.Success;

        public virtual string ToXml()
            => string.Format(SportTypeBetTypeXmlFormat, SportTypeId, BetTypeId);
    }
}
