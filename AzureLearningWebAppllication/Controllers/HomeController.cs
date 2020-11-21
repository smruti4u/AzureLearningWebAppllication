using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AzureLearningWebAppllication.Models;
using Microsoft.Extensions.Configuration;
using CacheService.Redis;

namespace AzureLearningWebAppllication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IConfiguration configuration;

        private readonly ICacheService cacheService;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, ICacheService cacheService)
        {
            _logger = logger;
            this.cacheService = cacheService;
            this.configuration = configuration;
        }

        public IActionResult Index()
        {
            CacheData cachedData = cacheService.GetData<CacheData>("Redis").GetAwaiter().GetResult();

            if(cachedData == null)
            {
                return Content("Cache has not been set");
            }


            return Content($"The Data Retrieved From cache Is {cachedData.Id} { cachedData.Name} ");
        }

        public IActionResult SetCache()
        {
            var cacheData = new CacheData()
            {
                Id = "1234234",
                Name = "Bob"
            };
            cacheService.SetData<CacheData>("Redis", cacheData, TimeSpan.FromMinutes(3));
            return Content("Cache Has been Set");
        }

        public IActionResult Privacy()
        {
            throw new Exception("Some thing went wrong");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
