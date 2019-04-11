namespace CN88.CustomerSetting.WebApi.Middlewares
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using CN88.CustomerSetting.AppResources;
    using CN88.CustomerSetting.Models.Results;
    using Fanex.Logging;
    using Fanex.Logging.Extensions.AspNetCore;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        private static JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (AccessViolationException ex)
            {
                await HandleExceptionAsync(context, ex.Message).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await ExceptionHandler.HandleAsync(ex, context).ConfigureAwait(false);
                await HandleExceptionAsync(context).ConfigureAwait(false);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, string message = null)
        {
            const HttpStatusCode code = HttpStatusCode.InternalServerError; // 500 if unexpected

            var result = JsonConvert.SerializeObject(
                Result.CreateErrorResult(string.IsNullOrWhiteSpace(message) 
                ? Resources.SystemError
                : message), 
                serializerSettings);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}