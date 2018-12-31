namespace CN88.CustomerSetting.SettingType2.Params.Commands
{
    using CN88.CustomerSetting.SpParams.Commands;

    public class UpdateCommSettingCommand : UpdateSettingTypeCommandBase
    {
        public string Comm { get; set; }

        public override string GetSettingKey()
            => "CS_CustSetting_Comm_Upd";

        public override bool IsValid()
            => base.IsValid() && !string.IsNullOrWhiteSpace(Comm);
    }
}