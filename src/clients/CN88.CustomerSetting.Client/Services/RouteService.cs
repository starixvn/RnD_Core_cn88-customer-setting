namespace CN88.CustomerSetting.Client.Services
{
    using System.Net;
    using Configurations;
    using Exceptions;
    using Helpers;
    using Models.Requests;
    using RestSharp;

    public class RouteService : IRouteService
    {
        private readonly IRestClient _restClient;

        internal RouteService()
            : this(null)
        {
        }

        internal RouteService(IRestClient restClient)
        {
            _restClient = restClient ?? GetRestClient();
        }

        public IRestResponse Redirect(RedirectRequest command)
        {
            if (command == null)
            {
                throw new CustomerSettingException("Redirect Request cannot be null");
            }

            var request = new RestRequest("/route/redirect/", Method.POST);

            request.AddJsonBody(command);
            request.Timeout = CN88CustomerSettingConfiguration.Settings.UpdateSettingTimeout;

            IRestResponse restReponse = _restClient.Execute(request);

            return UpdateTimeOutResponse(restReponse);
        }

        private IRestClient GetRestClient()
        {
            var restClient = new RestClient(RandomHelper.GetRandomUri(CN88CustomerSettingConfiguration.Settings.ServiceUris));

            restClient.Authenticator = new HmacAuthenticator();

            return restClient;
        }

        private IRestResponse UpdateTimeOutResponse(IRestResponse restReponse)
        {
            if (restReponse.StatusCode == 0)
            {
                restReponse.StatusCode = HttpStatusCode.RequestTimeout;
                restReponse.Content = "{ \"message\": \"Process is timeout.\" }";
            }

            return restReponse;
        }
    }
}