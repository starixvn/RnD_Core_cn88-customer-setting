namespace CN88.CustomerSetting.SettingType1.Params.Criteria
{
    using Fanex.Data.Repository;

    public class GetPTSettingCriteria : CriteriaBase
    {
        public GetPTSettingCriteria(byte roleId, int custId)
        {
            RoleId = roleId;
            CustId = custId;
        }

        public byte RoleId { get; }

        public int CustId { get; }

        public override string GetSettingKey()
            => "CS_CustSetting_PTSetting_Sel";

        public override bool IsValid()
            => CustId > 0 && RoleId > 0;
    }
}