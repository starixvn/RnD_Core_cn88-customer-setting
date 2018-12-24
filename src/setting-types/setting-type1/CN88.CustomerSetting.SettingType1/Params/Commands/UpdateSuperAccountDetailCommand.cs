namespace CN88.CustomerSetting.SettingType1.Params.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data.Repository;

    public class UpdateSuperAccountDetailCommand : UpdateCustomerDetailCommand
    {
        public string LastName { get; set; }

        public string Phone { get; set; }

        public string MobilePhone { get; set; }

        public string Fax { get; set; }

        public decimal Currency { get; set; }

        public override string GetSettingKey()
        {
            throw new NotImplementedException();
        }

        public override bool IsValid()
        {
            throw new NotImplementedException();
        }
    }
}
