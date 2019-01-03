namespace CN88.CustomerSetting.SettingType2.Params.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCommBetSettingCriteria : CriteriaBase
    {
        public int RoleId { get; set; }

        public int CustId { get; set; }

        [SpParam(Ignored = true)]
        public bool IsCreateNew { get; set; }

        public override string GetSettingKey()
            => IsCreateNew 
                ? "CS_CustomerIns_CommBetSetting_Sel"
                : "CS_CustSetting_CommBetSetting_Sel";

        public override bool IsValid()
            => RoleId > 0 && CustId >= 0;
    }
}