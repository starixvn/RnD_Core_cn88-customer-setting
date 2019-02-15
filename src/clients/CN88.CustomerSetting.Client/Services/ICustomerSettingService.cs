namespace CN88.CustomerSetting.Client.Services
{
    using Models.Requests;
    using RestSharp;

    public interface ICustomerSettingService
    {
        IRestResponse QueryCustomerSetting(QuerySettingRequest query);

        IRestResponse UpdateCustomerSetting(UpdateSettingRequest update);

        IRestResponse CreateCustomer(CreateCustomerRequest create);

        IRestResponse QueryCustomerSettingTemplate();

        IRestResponse CheckExistedUsername(string username);

        IRestResponse GetMainShareholderSuggestionNames(string username);
    }
}