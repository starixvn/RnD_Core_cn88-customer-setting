using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CN88.CustomerSetting.WebApi.Controllers
{
    public class CustomerSettingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}