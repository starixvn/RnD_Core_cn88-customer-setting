namespace CN88.CustomerSetting.DataAccesses
{
    using System.Collections;
    using System.Threading.Tasks;
    using CN88.CustomerSetting.AppResources;
    using CN88.CustomerSetting.Models.Enumerations;
    using CN88.CustomerSetting.Models.Results;

    public interface CustomerDataAccess
    {
        Task<CreateNewResult> CreateNewCustomer(Level level, Hashtable parameters);
    }

    public class CustomerDataAccessImpl : BaseDataAccess, CustomerDataAccess
    {
        public async Task<CreateNewResult> CreateNewCustomer(Level level, Hashtable parameters)
        {
            var spSettingKey = GetSpSettingKey(level);
            parameters.Add("custId", 0);
            parameters.Add("errcode", 0);
            parameters.Add("errmsg", string.Empty);

            var result = await ExecuteNonQuery(spSettingKey, parameters);

            var newUsername = parameters["username"];

            return new CreateNewResult(
                result.Code,
                result.IsOk 
                    ? string.Format(Resources.CreateSuccessfully, newUsername) 
                    : result.Message, 
                int.Parse(parameters["custId"].ToString()));
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