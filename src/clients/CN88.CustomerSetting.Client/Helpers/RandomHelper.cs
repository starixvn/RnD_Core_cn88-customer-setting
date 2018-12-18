namespace CN88.CustomerSetting.Client.Helpers
{
    using System;
    using System.Linq;
    using Exceptions;

    internal static class RandomHelper
    {
        private static Random _randomHandler = new Random();

        internal static string GetRandomUri(string sourceUris)
        {
            if (string.IsNullOrWhiteSpace(sourceUris))
            {
                throw new CustomerSettingException("Uris cannot be null or white space");
            }

            var uris = sourceUris.Split(',');
            var numOfUri = uris.Count();

            if (numOfUri == 1)
            {
                return uris[0];
            }

            var selectedUri = uris[_randomHandler.Next(0, numOfUri)];

            return selectedUri;
        }
    }
}