namespace CN88.CustomerSetting.Models
{
    public class ProductName
    {
        internal ProductName(
            byte productId,
            string language,
            string name)
        {
            ProductId = productId;
            Language = language;
            Name = name;
        }

        public byte ProductId { get; private set; }

        public string Language { get; private set; }

        public string Name { get; private set; }
    }
}