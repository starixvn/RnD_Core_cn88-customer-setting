namespace CN88.CustomerSetting.SettingType1.Params.Criteria
{
    using CN88.CustomerSetting.Models.Enumerations;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetPTSettingCriteria : CriteriaBase
    {
        public GetPTSettingCriteria(
            byte roleId,
            int custId,
            bool isCreateNew)
        {
            RoleId = roleId;
            CustId = custId;
            IsCreateNew = isCreateNew;
        }

        public byte RoleId { get; }

        public int CustId { get; }

        [SpParam(Ignored = true)]
        public bool IsCreateNew { get; }

        public override string GetSettingKey()
            => IsCreateNew
                ? "CS_PTSetting_CustomerIns_Sel"
                : "CS_CustSetting_PTSetting_Sel";

        public override bool IsValid()
            => RoleId > Level.MemberLevelId
                && CustId >= 0;
    }
}