namespace CN88.CustomerSetting.Models.Results
{
    using System.Collections.Generic;

    public class SettingResult : Result
    {
        public SettingResult(
            int code,
            string message,
            int msgType,
            int customerId,
            int settingTypeId,
            string settingTypeName)
            : base(code, message, msgType)
        {
            CustomerId = customerId;
            SettingTypeId = settingTypeId;
            SettingTypeName = settingTypeName;
        }

        public SettingResult(
            int code,
            string message,
            int msgType,
            int customerId,
            int settingTypeId,
            string settingTypeName,
            IEnumerable<string> subMessages)
            : this(
            code,
            message,
            msgType,
            customerId,
            settingTypeId,
            settingTypeName)
        {
            SubMessages = subMessages;
        }

        public SettingResult(
            int code,
            string message,
            SettingResult settingResult)
            : this(
            code,
            message,
            settingResult.MsgType,
            settingResult.CustomerId,
            settingResult.SettingTypeId,
            settingResult.SettingTypeName,
            settingResult.SubMessages)
        {
        }

        public SettingResult(
            SettingResult settingResult,
            string mainMessage,
            IEnumerable<string> subMessages)
            : this(
            settingResult.Code,
            mainMessage,
            settingResult.MsgType,
            settingResult.CustomerId,
            settingResult.SettingTypeId,
            settingResult.SettingTypeName,
            subMessages)
        {
        }

        public SettingResult(IResult result)
            : base(result.Code, result.Message)
        {
        }

        public SettingResult(
            IResult result,
            int customerId,
            int settingTypeId,
            string settingTypeName)
            : this(
            result.Code,
            result.Message,
            0,
            customerId,
            settingTypeId,
            settingTypeName)
        {
        }

        public SettingResult(
            int code,
            string mainMessage,
            IEnumerable<string> subMessages)
            : base(code, mainMessage)
        {
            SubMessages = subMessages;
        }

        public int CustomerId { get; private set; }

        public string CustomerName { get; set; }

        public int SettingTypeId { get; private set; }

        public string SettingTypeName { get; private set; }

        public IEnumerable<string> SubMessages { get; private set; }
    }
}