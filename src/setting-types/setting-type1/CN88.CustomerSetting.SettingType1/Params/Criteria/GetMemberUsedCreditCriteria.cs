namespace CN88.CustomerSetting.SettingType1.Params.Criteria
{
    using Fanex.Data.Repository;

    public class GetMemberUsedCreditCriteria : CriteriaBase
    {
        public GetMemberUsedCreditCriteria(int custId)
        {
            CustId = custId;
        }

        public int CustId { get; }

        public override string GetSettingKey()
            => "CS_MemberUsedCredit_Sel";

        public override bool IsValid()
            => CustId > 0;
    }
}