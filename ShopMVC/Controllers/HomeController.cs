using Microsoft.AspNetCore.Mvc;
using Practice.Client;
using ShopMVC.Models;
using System.Diagnostics;

namespace ShopMVC.Controllers {
    public class HomeController: Controller {
        private readonly ILogger<HomeController> _logger;

        private static HttpClient httpClient = new HttpClient();
        private PracticeClient practiceClient = new PracticeClient("http://192.168.98.78:5064/", httpClient);

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index() {
            return View();
        }

        [HttpGet]
        public IActionResult Shopping() {
            return View(practiceClient.GetProductAsync().Result.Products);
        }

        [HttpPost]
        public IActionResult Post() {
            return null;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
