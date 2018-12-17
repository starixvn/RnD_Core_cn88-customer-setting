namespace CN88.CustomerSetting.DataAccesses
{
    using System.Collections;
    using Models.Results;

    public interface IValidationDataAccess
    {
        IResult ValidatePermissionMultiple(int actorId, string memberIds);
    }

    public class ValidationDataAccess : BaseDataAccess, IValidationDataAccess
    {
        public IResult ValidatePermissionMultiple(int actorId, string memberIds)
        {
            var param = new Hashtable
            {
                { "custid", actorId },
                { "targetcustidlst", memberIds },
                { "errcode", 0 },
                { "errmsg", string.Empty }
            };

            return ExecuteNonQuery("CustSetting_MultiCustPermissionValidate", param);
        }
    }
}
