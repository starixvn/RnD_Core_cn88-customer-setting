namespace CN88.CustomerSetting.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Requests;
    using CN88.CustomerSetting.Models.Results;

    public interface InsertCustomerService
    {
        IResult Update(
            ISettingContext settingContext,
            CreateCustomerCommand command);
    }

    public class InsertCustomerServiceImpl : InsertCustomerService
    {
        public IResult Update(
            ISettingContext settingContext, 
            CreateCustomerCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
