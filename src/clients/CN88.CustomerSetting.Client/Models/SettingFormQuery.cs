namespace CN88.CustomerSetting.Client.Models
{
    using System.Collections.Generic;

    public class SettingFormQuery
    {
        public string Language { get; set; }

        public byte ProductId { get; set; }

        public byte CustomerLevel { get; set; }

        public byte UplineLevel { get; set; }

        public int? CustomerId { get; set; }

        public string CustomerIds { get; set; }

        public string CustomerNames { get; set; }

        public bool IsReadOnly { get; set; }

        public bool IsForcingMode { get; set; }

        internal byte System { get; set; }

        public bool IsCreateNew { get; set; }

        internal string ToJson()
        {
            var properties = new List<string>();

            properties.Add("{");
            properties.Add(string.Format("\"system\": {0},", System));
            properties.Add(string.Format("\"language\": \"{0}\",", Language));
            properties.Add(string.Format("\"productId\": {0},", ProductId));
            properties.Add(string.Format("\"customerLevel\": {0},", CustomerLevel));
            properties.Add(string.Format("\"uplineLevel\": {0},", UplineLevel));
            properties.Add(string.Format("\"isCreateNew\": {0},", IsCreateNew.ToString().ToLowerInvariant()));
            properties.Add(string.Format("\"isForcingMode\": {0},", IsForcingMode.ToString().ToLowerInvariant()));

            properties.Add(string.Format("\"customerId\": {0}", CustomerId));

            properties.Add("}");

            return string.Join(string.Empty, properties);
        }
    }
}