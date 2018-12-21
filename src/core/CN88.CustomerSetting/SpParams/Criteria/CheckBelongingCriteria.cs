namespace CN88.CustomerSetting.SpParams.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    internal class CheckBelongingCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "CustId")]
        public int ActorId { get; set; }

        [SpParam(SqlParamName = "TargetCustId")]
        public int CustomerId { get; set; }

        public override string GetSettingKey() => "CustSetting_SameLineCheck";

        public override bool IsValid() => ActorId > 0 && CustomerId > 0;
    }
}