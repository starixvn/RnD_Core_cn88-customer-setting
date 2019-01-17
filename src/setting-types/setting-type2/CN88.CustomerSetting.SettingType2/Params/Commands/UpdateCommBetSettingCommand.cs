namespace CN88.CustomerSetting.SettingType2.Params.Commands
{
    using CN88.CustomerSetting.SpParams.Commands;

    public class UpdateCommBetSettingCommand : UpdateSettingTypeCommandBase
    {
        public string BetSetting { get; set; }

        public string CommSetting { get; set; }

        public override string GetSettingKey()
            => "CS_CustSetting_CommBetSetting_Upd";

        public override bool IsValid()
            => base.IsValid()
                && !string.IsNullOrWhiteSpace(BetSetting)
                && !string.IsNullOrWhiteSpace(CommSetting);
    }
}