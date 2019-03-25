namespace CN88.CustomerSetting.SettingType1.Params.Criteria
{
    using Fanex.Data.Repository;

    public class GetMemberCreditCriteria : CriteriaBase
    {
        public GetMemberCreditCriteria(int custId)
        {
            CustId = custId;
        }

        public int CustId { get; }

        public override string GetSettingKey()
            => "CS_Customer_Credit_Sel";

        public override bool IsValid()
            => CustId > 0;
    }
}