namespace CN88.CustomerSetting.WebApi.Models.Binders
{
    using System;
    using CN88.CustomerSetting.Models.Enumerations;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class LevelConverter : JsonCreationConverter<Level>
    {
        protected override Level Create(Type type, JObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "object cannot be null");
            }

            return Enumeration.FromValue<Level>(Convert.ToByte(obj["value"]));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken token = JToken.FromObject(value);

            token.WriteTo(writer);
        }
    }
}