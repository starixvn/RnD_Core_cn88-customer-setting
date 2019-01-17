namespace CN88.CustomerSetting.SettingType1.Params.Commands
{
    using CN88.CustomerSetting.SpParams.Commands;

    public class UpdatePTSettingCommand : UpdateSettingTypeCommandBase
    {
        public string PTSetting { get; set; }

        public override string GetSettingKey()
            => "CS_CustSetting_PTSetting_Upd";

        public override bool IsValid()
            => base.IsValid()
                && !string.IsNullOrWhiteSpace(PTSetting);
    }
}