namespace CN88.CustomerSetting.SpParams.Criteria
{
    using Fanex.Data.Repository;

    internal class ActorStatusQueryCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public override string GetSettingKey() => "CustSetting_ActorStatusSelect";

        public override bool IsValid() => CustId > 0;
    }
}