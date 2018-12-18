namespace CN88.CustomerSetting.Client.Helpers
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using Configurations;
    using Exceptions;
    using RestSharp;
    using RestSharp.Authenticators;

    /// <summary>
    /// http://bitoftech.net/2014/12/15/secure-asp-net-web-api-using-api-key-authentication-hmac-authentication/
    /// </summary>
    internal class HmacAuthenticator : IAuthenticator
    {
        private static string _machineIP;

        public HmacAuthenticator()
        {
            if (!string.IsNullOrEmpty(_machineIP))
            {
                _machineIP = GetLocalIPAddress();
            }
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client), "Rest Client cannot be null");
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Rest Request cannot be null");
            }

            string requestUri = HttpUtility.UrlEncode(client.BuildUri(request).ToString().ToLower());
            string requestHttpMethod = request.Method.ToString();

            // Calculate UNIX time
            var epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;
            string requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

            // create random nonce for each request
            string nonce = Guid.NewGuid().ToString("N");

            // Creating the raw signature string
            string signatureRawData = $"{_machineIP}{requestHttpMethod}{requestUri}{requestTimeStamp}{nonce}";
            var secretKeyByteArray = Convert.FromBase64String(CN88CustomerSettingConfiguration.Settings.SecretKey);
            byte[] signature = Encoding.UTF8.GetBytes(signatureRawData);
            using (HMACSHA256 hmac = new HMACSHA256(secretKeyByteArray))
            {
                byte[] signatureBytes = hmac.ComputeHash(signature);
                string requestSignatureBase64String = Convert.ToBase64String(signatureBytes);

                // Setting the values in the Authorization header using custom scheme (amx)
                string authHeader = $"amx {_machineIP}:{requestSignatureBase64String}:{nonce}:{requestTimeStamp}";
                request.AddParameter("Authorization", authHeader, ParameterType.HttpHeader);
            }
        }

        public string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString().Replace(":", "_");
                }
            }

            throw new CustomerSettingException("Local IP Address Not Found!");
        }
    }
}