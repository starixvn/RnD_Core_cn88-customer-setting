namespace CN88.CustomerSetting.SettingType1.Params.Criteria
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data.Repository;

    public class GetCurrencyListCriteria : CriteriaBase
    {
        public override string GetSettingKey()
            => "CS_CurrencyListSel";

        public override bool IsValid()
            => true;
    }
}
