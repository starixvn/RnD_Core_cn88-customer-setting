namespace CN88.CustomerSetting.Client.Models
{
    using System.Collections.Generic;
    using CN88.CustomerSetting.Client.Helpers;

    public class SettingFormQuery
    {
        public string Language { get; set; }

        public byte ProductId { get; set; }

        public byte CustomerLevel { get; set; }

        public int? CustomerId { get; set; }

        public bool IsReadOnly { get; set; }

        internal byte System { get; set; }

        public bool IsCreateNew { get; set; }

        public int UplineId { get; set; }

        public string HashKey { get; set; }

        public byte ActorLevel { get; set; }

        public BrowserInfo BrowserInfo { get; set; }

        internal string ToJson()
        {
            var properties = new List<string>();

            properties.Add("{");
            properties.Add(string.Format("\"system\": {0},", System));
            properties.Add(string.Format("\"hashKey\": \"{0}\",", HashKey));
            properties.Add(string.Format("\"language\": \"{0}\",", Language));
            properties.Add(string.Format("\"productId\": {0},", ProductId));
            properties.Add(string.Format("\"customerLevel\": {0},", CustomerLevel));
            properties.Add(string.Format("\"actorLevel\": {0},", ActorLevel));
            properties.Add(string.Format("\"uplineId\": {0},", UplineId));
            properties.Add(string.Format("\"isCreateNew\": {0},", IsCreateNew.ToString().ToLowerInvariant()));
            properties.Add(string.Format("\"editable\": {0},", (!IsReadOnly).ToString().ToLowerInvariant()));

            properties.Add(string.Format("\"customerId\": {0},", CustomerId.HasValue ? CustomerId.Value : 0));

            properties.Add(string.Format("\"isMobile\": {0},", BrowserInfo.IsMobile.ToString().ToLowerInvariant()));
            properties.Add(string.Format("\"isPhone\": {0},", BrowserInfo.IsPhone.ToString().ToLowerInvariant()));
            properties.Add(string.Format("\"isTablet\": {0}", BrowserInfo.IsTablet.ToString().ToLowerInvariant()));

            properties.Add("}");

            return string.Join(string.Empty, properties);
        }
    }
}