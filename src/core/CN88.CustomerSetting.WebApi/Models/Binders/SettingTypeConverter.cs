namespace CN88.CustomerSetting.WebApi.Models.Binders
{
    using System;
    using CN88.CustomerSetting.Models;
    using Newtonsoft.Json.Linq;

    public class SettingTypeConverter : JsonCreationConverter<SettingType>
    {
        protected override Type GetTargetType(Type type, JObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return Type.GetType(obj.GetValue("typeName", StringComparison.OrdinalIgnoreCase).ToString());
        }
    }
}