namespace CN88.CustomerSetting.DataAccesses
{
    using System.Collections;
    using CN88.CustomerSetting.Models.Enumerations;
    using CN88.CustomerSetting.Models.Results;

    public interface CustomerDataAccess
    {
        CreateNewResult InsertCustomer(Level level, Hashtable parameters);
    }

    public class CustomerDataAccessImpl : BaseDataAccess, CustomerDataAccess
    {
        public CreateNewResult InsertCustomer(Level level, Hashtable parameters)
        {
            var spSettingKey = GetSpSettingKey(level);
            parameters.Add("custId", 0);
            parameters.Add("errcode", 0);
            parameters.Add("errmsg", string.Empty);

            var result = ExecuteNonQuery(spSettingKey, parameters);

            return new CreateNewResult(result.Code, result.Message, int.Parse(parameters["custId"].ToString()));
        }

        private string GetSpSettingKey(Level level)
        {
            switch (level.Value)
            {
                case Level.SuperLevelId:
                    return "CS_CustomerIns_Com";

                case Level.MasterLevelId:
                    return "CS_CustomerIns_SAg";

                case Level.AgentLevelId:
                    return "CS_CustomerIns_MAg";

                default:
                    return "CS_CustomerIns_Ag";
            }
        }
    }
}