using Microsoft.AspNetCore.Mvc;
using ShopMVC.Models;
using System.Diagnostics;
using Practice.Client;

namespace ShopMVC.Controllers {
    public class HomeController: Controller {
        private readonly ILogger<HomeController> _logger;
        private static HttpClient _client = new();
        private PracticeClient _practiceClient = new("http://192.168.0.101:5064", _client); //URL NEEDS TO REF TO API ADDRESS
        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index() {
            return View();
        }

        [HttpGet]
        public IActionResult Authorize()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Authorize(string email)
        {
            var person = _practiceClient
                .GetPersonAsync().Result.Persons
                .FirstOrDefault(p => p.Email == email);

            if (person == null)
                return View();
            
            return Index();
        }
        
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Register(Person person)
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
