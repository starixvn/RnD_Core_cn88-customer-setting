namespace CN88.CustomerSetting.SpParams.Criteria
{
    using Fanex.Data.Repository;


    internal class CustomerLevelQueryCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public override string GetSettingKey() => "CustSetting_CustomerLevelSelect";

        public override bool IsValid() => CustId > 0;
    }
}