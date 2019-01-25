namespace CN88.CustomerSetting.Services
{
    using CN88.CustomerSetting.AppResources;
    using Fanex.RemotingRef;

    public interface ResourceService
    {
        string GetProductName(byte productId, string language);

        string GetSportTypeName(int sportTypeId, string language);

        string GetBetTypeName(int betTypeId, bool isLive, string language);

        string GetResourceData(string refId, string resourceId, string language);
    }

    public class ResourceServiceImpl : ResourceService
    {
        private const string SportTypeDefaultFormat = "Sport Type {0}";
        private const string ProductDefaultFormat = "Product {0}";
        private const string ProductRefId = "Product";
        private const string RefResourceDefaultFormat = "Ref Resource: {0} - Key: {1}";

        private readonly IRemotingRefService remotingRef;

        public ResourceServiceImpl(IRemotingRefService remotingRef)
        {
            this.remotingRef = remotingRef ?? new RemotingRefService();
        }

        public string GetProductName(byte productId, string language)
        {
            if (productId <= 0)
            {
                return string.Empty;
            }

            var resourceData = remotingRef.GetResourceDataByRefIdAndResourceId(
                ProductRefId,
                productId.ToString(),
                language,
                string.Empty);

            return string.IsNullOrEmpty(resourceData)
                ? string.Format(ProductDefaultFormat, productId)
                : resourceData;
        }

        public string GetSportTypeName(int sportTypeId, string language)
        {
            if (sportTypeId <= 0)
            {
                return string.Empty;
            }

            var sportTypeName = remotingRef.GetSportName(sportTypeId.ToString(), language);

            if (!string.IsNullOrEmpty(sportTypeName))
            {
                return sportTypeName;
            }

            sportTypeName = Resources.ResourceManager.GetString($"SportTypeName_{sportTypeId}");

            return string.IsNullOrEmpty(sportTypeName)
                ? string.Format(SportTypeDefaultFormat, sportTypeId)
                : sportTypeName;
        }

        public string GetBetTypeName(int betTypeId, bool isLive, string language)
        {
            if (betTypeId <= 0)
            {
                return string.Empty;
            }

            var live = isLive ? 1 : 0;
            var betTypeKey = $"BetTypeName_{betTypeId}_{live}";
            var betTypeName = Resources.ResourceManager.GetString(betTypeKey);

            if (!string.IsNullOrEmpty(betTypeName))
            {
                return betTypeName;
            }

            betTypeName = remotingRef.GetBetTypeName(betTypeId.ToString(), language);
            var liveText = isLive ? $"{Resources.Live} " : string.Empty;

            return string.IsNullOrEmpty(betTypeName)
                ? betTypeKey
                : $"{liveText}{betTypeName}";
        }

        public string GetResourceData(string refId, string resourceId, string language)
        {
            if (string.IsNullOrEmpty(refId) || string.IsNullOrEmpty(resourceId))
            {
                return string.Empty;
            }

            var resourceData = remotingRef.GetResourceDataByRefIdAndResourceId(
                refId,
                resourceId,
                language,
                string.Empty);

            return string.IsNullOrEmpty(resourceData)
                ? string.Format(RefResourceDefaultFormat, refId, resourceId)
                : resourceData;
        }
    }
}