namespace CN88.CustomerSetting.SettingType1.Params.Criteria
{
    using Fanex.Data.Repository;

    public class GetCustomerInfoForCreateCriteria : CriteriaBase
    {
        public GetCustomerInfoForCreateCriteria(byte levelId, int custId)
        {
            RoleId = levelId;
            CustId = custId;
        }

        public byte RoleId { get; }

        public int CustId { get; }

        public override string GetSettingKey()
            => "CS_CustomerIns_Sel";

        public override bool IsValid()
            => CustId > 0 && RoleId > 0;
    }
}