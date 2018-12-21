namespace CN88.CustomerSetting.Client.Services
{
    using System.Net;
    using Configurations;
    using Exceptions;
    using Helpers;
    using Models.Requests;
    using RestSharp;

    internal class CustomerSettingService : ICustomerSettingService
    {
        private readonly IRestClient _restClient;

        internal CustomerSettingService()
            : this(null)
        {
        }

        internal CustomerSettingService(IRestClient restClient)
        {
            _restClient = restClient ?? GetRestClient();
        }

        public IRestResponse QueryCustomerSetting(QuerySettingRequest query)
        {
            if (query == null)
            {
                throw new CustomerSettingException("Query Setting Request cannot be null");
            }

            var request = new RestRequest("/customersetting/querysetting", Method.POST);

            request.AddJsonBody(query);

            return _restClient.Execute(request);
        }

        public IRestResponse UpdateCustomerSetting(UpdateSettingRequest update)
        {
            if (update == null)
            {
                throw new CustomerSettingException("Update Setting Request cannot be null");
            }

            var request = new RestRequest("/customersetting/post/", Method.POST);

            request.AddJsonBody(update);
            request.Timeout = CN88CustomerSettingConfiguration.Settings.UpdateSettingTimeout;

            IRestResponse restReponse = _restClient.Execute(request);

            return UpdateTimeOutResponse(restReponse);
        }

        
        private IRestResponse UpdateTimeOutResponse(IRestResponse restReponse)
        {
            if (restReponse.StatusCode == 0)
            {
                restReponse.StatusCode = HttpStatusCode.RequestTimeout;
                restReponse.Content = "{ \"message\": \"Update Process is timeout.\" }";
            }

            return restReponse;
        }

        public IRestResponse QueryCustomerSettingTemplate(bool isIe8)
        {
            IRestRequest request = new RestRequest("/customersetting/gettemplate", Method.GET);

            request.AddParameter("isIe8", isIe8);

            return _restClient.Execute(request);
        }

        private IRestClient GetRestClient()
        {
            var restClient = new RestClient(RandomHelper.GetRandomUri(CN88CustomerSettingConfiguration.Settings.ServiceUris));
            restClient.Authenticator = new HmacAuthenticator();

            return restClient;
        }
    }
}