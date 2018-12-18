namespace CN88.CustomerSetting.WebApi.Models.Binders
{
    using System;
    using CN88.CustomerSetting.Models;
    using Newtonsoft.Json.Linq;

    public class SettingItemConverter : JsonCreationConverter<ISettingItem>
    {
        protected override Type GetTargetType(Type type, JObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "Object cannot be null");
            }

            return Type.GetType(obj.GetValue("typeName", StringComparison.OrdinalIgnoreCase).ToString());
        }
    }
}