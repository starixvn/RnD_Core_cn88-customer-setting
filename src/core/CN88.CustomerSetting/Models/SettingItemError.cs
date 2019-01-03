namespace CN88.CustomerSetting.Models
{
    using Results;

    public class SettingItemError : ISettingItem
    {
        public SettingItemError()
        {
            //// Workaround for multiple language, if private set, cannot use any other languages
            ErrorMessage = AppResources.Resources.SettingTypeError;
        }

        public string ErrorMessage { get; set; }

        public string TypeName
            => GetType().AssemblyQualifiedName;

        public IResult Validate()
            => new Result(Constants.ResultValues.Error, AppResources.Resources.SettingTypeError);

        public string ToXml()
            => string.Format("<Error>{0}</Error>", AppResources.Resources.SettingTypeError);
    }
}