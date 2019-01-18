namespace CN88.CustomerSetting.SpParams.Commands
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public abstract class UpdateSettingTypeCommandBase : NonQueryCommand
    {
        public byte RoleId { get; set; }

        public int CustId { get; set; }

        public int ProductId { get; set; }

        [SpParam(SqlParamName = "subaccid")]
        public int SubAccountId { get; set; }

        public int UserId { get; set; }

        public string IP { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public int MsgType { get; set; }

        public override bool IsValid()
#pragma warning disable S1067 // Expressions should not be too complex
            => RoleId > 0
                && CustId > 0
                && UserId > 0
                && ProductId > 0
                && !string.IsNullOrWhiteSpace(IP);
#pragma warning restore S1067 // Expressions should not be too complex
    }
}