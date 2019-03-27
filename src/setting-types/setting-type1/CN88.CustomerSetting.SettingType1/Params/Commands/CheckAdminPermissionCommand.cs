namespace CN88.CustomerSetting.SettingType1.Params.Commands
{
    using Fanex.Data.Repository;

    public class CheckAdminPermissionCommand : NonQueryCommand
    {
        public CheckAdminPermissionCommand(int custId)
        {
            CustId = custId;
        }

        public int CustId { get; }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
        public string FuncName => "[Credit]";
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static

        public int? ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey()
            => "Acc_Con_CheckPermission";

        public override bool IsValid()
            => CustId > 0;
    }
}