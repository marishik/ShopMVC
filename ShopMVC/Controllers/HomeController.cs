using Microsoft.AspNetCore.Mvc;
using ShopMVC.Models;
using System.Diagnostics;
using Practice.Client;

namespace ShopMVC.Controllers {
    public class HomeController: Controller {
        private readonly ILogger<HomeController> _logger;
        private static HttpClient _client = new();
        private PracticeClient _practiceClient = new("http://192.168.98.78:5064", _client); //URL NEEDS TO REF TO API ADDRESS
        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index() {
            return View();
        }

        [HttpGet]
        public IActionResult Authorization()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Authorization(string email)
        {   
            return Index();
        }
        
        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Registration(Person person)
        {
            _practiceClient.PostPersonAsync(person);
            return Index();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
