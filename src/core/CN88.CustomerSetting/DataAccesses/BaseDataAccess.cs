namespace CN88.CustomerSetting.DataAccesses
{
    using System;
    using System.Collections;
    using CN88.CustomerSetting.Models.Enumerations;
    using CN88.CustomerSetting.Models.Requests;
    using CN88.CustomerSetting.Models.Results;
    using Fanex.Data;

    public class BaseDataAccess
    {
        protected IResult ExecuteNonQuery(string settingKey, Hashtable storedProcedureParam)
        {
            using (var objectDb = new ObjectDb(settingKey))
            {
                objectDb.ExecuteNonQuery(storedProcedureParam);

                return GetResult(storedProcedureParam);
            }
        }

        protected IResult GetResult(Hashtable param)
        {
            if (param == null || !param.ContainsKey("errcode") || !param.ContainsKey("errmsg"))
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

        protected Hashtable GetSPParamsForGettingSettingType(CustomerSettingQuery querySettingParam, byte settingType)
        {
            if (querySettingParam == null)
            {
                throw new ArgumentNullException(nameof(querySettingParam), "querySettingParam cannot be null.");
            }

            var param = new Hashtable
            {
                { "custid", querySettingParam.CustomerId },
                { "productid", querySettingParam.ProductId },
                { "settingtype", settingType }
            };

            if (querySettingParam.CustomerLevel == Level.Member)
            {
                param.Add("roleid", querySettingParam.UplineLevel.Value);
            }

            return param;
        }
    }
}