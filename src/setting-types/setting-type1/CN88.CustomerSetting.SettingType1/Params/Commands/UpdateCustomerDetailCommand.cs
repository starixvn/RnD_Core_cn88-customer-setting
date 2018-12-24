namespace CN88.CustomerSetting.SettingType1.Params.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data.Repository;

    public class UpdateCustomerDetailCommand : NonQueryCommand
    {
        public int CustId { get; set; }

        public string FirstName { get; set; }

        public decimal Credit { get; set; }

        public string PTSetting { get; set; }

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
