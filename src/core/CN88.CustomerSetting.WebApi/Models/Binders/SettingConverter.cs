namespace CN88.CustomerSetting.WebApi.Models.Binders
{
    using System;
    using CN88.CustomerSetting.Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class SettingConverter : JsonCreationConverter<Setting>
    {
        protected override JObject CreateJObject(JsonReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader), "Reader cannot be null");
            }

            return JObject.Parse((string)reader.Value);
        }
    }
}