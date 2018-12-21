namespace CN88.CustomerSetting.SettingType1.Params.Criteria
{
    using CN88.CustomerSetting.Models.Enumerations;
    using Fanex.Data.Repository;

    public class GetDefaultPTSettingCriteria : CriteriaBase
    {
        public GetDefaultPTSettingCriteria(byte roleId, int custId)
        {
            RoleId = roleId;
            CustId = custId;
        }

        public byte RoleId { get; }

        public int CustId { get; }

        public override string GetSettingKey()
            => "CS_PTSetting_CustomerIns_Sel";

        public override bool IsValid()
            => RoleId > Level.MemberLevelId
                && CustId >= 0;
    }
}