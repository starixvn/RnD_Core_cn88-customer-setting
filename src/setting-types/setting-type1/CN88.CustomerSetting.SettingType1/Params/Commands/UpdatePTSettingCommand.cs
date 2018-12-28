namespace CN88.CustomerSetting.SettingType1.Params.Commands
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdatePTSettingCommand : NonQueryCommand
    {
        public byte RoleId { get; set; }

        public int CustId { get; set; }

        public string PTSetting { get; set; }

        public int ProductId { get; set; }

        [SpParam(Ignored = true)]
        public int SubAccountId { get; set; }

        public int UserId { get; set; }

        public string IP { get; set; }

        public string Language { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public int MsgType { get; set; }

        public override string GetSettingKey()
            => "CS_CustSetting_PTSetting_Upd";

        public override bool IsValid()
            => RoleId > 0
                && CustId > 0
                && UserId > 0
                && ProductId > 0
                && !string.IsNullOrWhiteSpace(PTSetting)
                && !string.IsNullOrWhiteSpace(IP)
                && !string.IsNullOrWhiteSpace(Language);
    }
}