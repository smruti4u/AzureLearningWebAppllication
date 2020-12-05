using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ApplicationInsight.Models;
using System.Net.Http;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ApplicationInsight.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly TelemetryClient client;
        public HomeController(ILogger<HomeController> logger)
        {
            client = new TelemetryClient();
            _logger = logger;
        }

        public IActionResult Index()
        {

            client.TrackTrace("IndexMethod", new Dictionary<string, string>()
            {

                ["RequestId"] = "12345678990",
                ["EventSource"] = "MobileApp"
            });

            CallService().Wait();
            return View();
        }

        public IActionResult Privacy()
        {
            throw new Exception("Something Went Wrong");
            return View();
        }

        private async Task CallService()
        {
            HttpClient client = new HttpClient();
            await client.GetAsync("https://api.github.com/users").ConfigureAwait(false);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
