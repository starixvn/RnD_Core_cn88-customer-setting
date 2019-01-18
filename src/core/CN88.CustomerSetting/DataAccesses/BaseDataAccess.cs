namespace CN88.CustomerSetting.DataAccesses
{
    using System;
    using System.Collections;
    using System.Threading.Tasks;
    using CN88.CustomerSetting.Models.Results;
    using Fanex.Data;

    public class BaseDataAccess
    {
        protected async Task<IResult> ExecuteNonQuery(string settingKey, Hashtable storedProcedureParam)
        {
            using (var objectDb = new ObjectDb(settingKey))
            {
                await objectDb.ExecuteNonQueryAsync(storedProcedureParam).ConfigureAwait(false);

                return GetResult(storedProcedureParam);
            }
        }

        protected static IResult GetResult(Hashtable param)
        {
            if (param == null
                || !param.ContainsKey("errcode")
                || !param.ContainsKey("errmsg"))
            {
                throw new ArgumentException(
                    "result output param is nulll or does not contain neither errcode nor errmsg.");
            }

            var errorCode = Convert.ToInt32(param["errcode"]);
            var errMsg = Convert.ToString(param["errmsg"]);
            if (param.ContainsKey("msgtype"))
            {
                return new Result(
                    errorCode,
                    errMsg,
                    Convert.ToInt32(param["msgtype"]));
            }

            return new Result(errorCode, errMsg);
        }
    }
}