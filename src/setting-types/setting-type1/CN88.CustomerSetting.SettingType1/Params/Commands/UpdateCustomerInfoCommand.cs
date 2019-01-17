namespace CN88.CustomerSetting.SettingType1.Params.Commands
{
    using Fanex.Data.Repository;

    public class UpdateCustomerInfoCommand : NonQueryCommand
    {
        public byte RoleId { get; set; }

        public int CustId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string MobilePhone { get; set; }

        public string Fax { get; set; }

        public decimal Credit { get; set; }

        public string Language { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey()
            => "CS_CustSetting_GeneralInfo_Upd";

        public override bool IsValid()
            => CustId > 0
                && RoleId > 0;
    }
}