namespace CN88.CustomerSetting.WebApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        public IActionResult Index()
            => new ContentResult()
            {
                Content = "CN88 Customer Setting API started."
            };
    }
}