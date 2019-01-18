namespace CN88.CustomerSetting.WebApi.Models.Binders
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Newtonsoft.Json;

    public class SettingRequestModelBinder<T> : IModelBinder
    {
#pragma warning disable S4457 // Parameter validation in "async"/"await" methods should be wrapped
        public async Task BindModelAsync(ModelBindingContext bindingContext)
#pragma warning restore S4457 // Parameter validation in "async"/"await" methods should be wrapped
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            if (bindingContext.ModelType != typeof(T))
            {
                return;
            }

            string bodyString;
            var request = bindingContext.HttpContext.Request;
            using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8))
            {
                bodyString = await reader.ReadToEndAsync();
            }

            if (string.IsNullOrWhiteSpace(bodyString))
            {
                return;
            }

            var updateSettingRequest = JsonConvert.DeserializeObject<T>(
                bodyString,
                new SettingConverter(),
                new SettingTypeConverter(),
                new SettingItemConverter(),
                new LevelConverter());

            bindingContext.Result = ModelBindingResult.Success(updateSettingRequest);
        }
    }
}