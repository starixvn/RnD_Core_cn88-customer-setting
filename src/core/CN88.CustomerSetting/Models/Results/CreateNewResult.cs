namespace CN88.CustomerSetting.Models.Results
{
    public class CreateNewResult : Result
    {
        public CreateNewResult(
            int code,
            string message,
            int custId,
            int settingTypeId,
            int msgType)
            : base(code, message, msgType)
        {
            CustId = custId;
            SettingTypeId = settingTypeId;
        }

        public int CustId { get; }

        public int SettingTypeId { get; }
    }
}