namespace CN88.CustomerSetting.SettingType1.Params.Criteria
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data.Repository;

    public class GetGeneralInfoCriteria : CriteriaBase
    {
        public GetGeneralInfoCriteria(byte roleId, int custId)
        {
            RoleId = roleId;
            CustId = custId;
        }

        public byte RoleId { get; }

        public int CustId { get; }

        public override string GetSettingKey()
            => "CS_CustSetting_GeneralInfo_Sel";

        public override bool IsValid()
            => RoleId > 0 && CustId > 0;
    }
}
