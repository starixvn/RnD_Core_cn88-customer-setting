namespace CN88.CustomerSetting.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;


    public interface ResourceService
    {
        string GetProductName(byte productId, string language);

        string GetSportTypeName(int sportTypeId, string language);

        string GetBetTypeName(int betTypeId, string language);

        string GetResourceData(string refId, string resourceId, string language);
    }

    public class ResourceServiceImpl : ResourceService
    {
        //private const string SportTypeDefaultFormat = "Sport Type {0}";
        //private const string BetTypeDefaultFormat = "Bet Type {0}";
        //private const string ProductDefaultFormat = "Product {0}";
        //private const string ProductRefId = "Product";
        //private const string RefResourceDefaultFormat = "Ref Resource: {0} - Key: {1}";

        //private readonly IRemotingRefService _remotingRef;

        //public ResourceService(IRemotingRefService remotingRef = null)
        //{
        //    _remotingRef = remotingRef ?? new RemotingRefService();
        //}

        //public string GetProductName(byte productId, string language)
        //{
        //    if (productId <= 0)
        //    {
        //        return string.Empty;
        //    }

        //    var resourceData = _remotingRef.GetResourceDataByRefIdAndResourceId(
        //        ProductRefId,
        //        productId.ToString(),
        //        language,
        //        string.Empty);

        //    return string.IsNullOrEmpty(resourceData)
        //        ? string.Format(ProductDefaultFormat, productId)
        //        : resourceData;
        //}

        //public string GetSportTypeName(int sportTypeId, string language)
        //{
        //    if (sportTypeId <= 0)
        //    {
        //        return string.Empty;
        //    }

        //    var sportTypeName = _remotingRef.GetSportName(sportTypeId.ToString(), language);

        //    return string.IsNullOrEmpty(sportTypeName)
        //        ? string.Format(SportTypeDefaultFormat, sportTypeId)
        //        : sportTypeName;
        //}

        //public string GetBetTypeName(int betTypeId, string language)
        //{
        //    if (betTypeId <= 0)
        //    {
        //        return string.Empty;
        //    }

        //    var betTypeName = _remotingRef.GetBetTypeName(betTypeId.ToString(), language);

        //    return string.IsNullOrEmpty(betTypeName)
        //        ? string.Format(BetTypeDefaultFormat, betTypeId)
        //        : betTypeName;
        //}

        //public string GetResourceData(string refId, string resourceId, string language)
        //{
        //    if (string.IsNullOrEmpty(refId) || string.IsNullOrEmpty(resourceId))
        //    {
        //        return string.Empty;
        //    }

        //    var resourceData = _remotingRef.GetResourceDataByRefIdAndResourceId(
        //        refId,
        //        resourceId,
        //        language,
        //        string.Empty);

        //    return string.IsNullOrEmpty(resourceData)
        //        ? string.Format(RefResourceDefaultFormat, refId, resourceId)
        //        : resourceData;
        //}
        public string GetBetTypeName(int betTypeId, string language)
            => $"{betTypeId}_{language}";

        public string GetProductName(byte productId, string language)
            => $"{productId}_{language}";

        public string GetResourceData(string refId, string resourceId, string language)
            => $"{refId}_{resourceId}_{language}";

        public string GetSportTypeName(int sportTypeId, string language)
            => $"{sportTypeId}_{language}";
    }
}
