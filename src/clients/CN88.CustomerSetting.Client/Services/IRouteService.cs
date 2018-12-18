namespace CN88.CustomerSetting.Client.Services
{
    using Models.Requests;
    using RestSharp;

    public interface IRouteService
    {
        IRestResponse Redirect(RedirectRequest command);
    }
}