namespace CN88.CustomerSetting.SettingType2.Params.Criteria
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data.Repository;

    public class GetCreateNewCommBetSettingCriteria : CriteriaBase
    {
        public int RoleId { get; set; }

        public int CustId { get; set; }

        public string UGroup { get; set; }

        public override string GetSettingKey()
            => "CS_CustSetting_CommBetSetting_Sel";

        public override bool IsValid()
            => RoleId > 0 && CustId >= 0;
    }
}
