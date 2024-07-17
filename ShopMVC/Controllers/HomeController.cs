using Microsoft.AspNetCore.Mvc;
using ShopMVC.Models;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Practice.Client;

namespace ShopMVC.Controllers {
    public class HomeController: Controller {
        private readonly ILogger<HomeController> _logger;
        private static HttpClient _client = new();
        private PracticeClient _practiceClient = new("http://192.168.98.78:5064", _client); //URL NEEDS TO REF TO API ADDRESS
        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        [HttpGet] [Authorize]
        public string Index()
        { 
            var testValue = Request.HttpContext.User.Claims.FirstOrDefault().Value;
            return testValue;
        }

        [HttpGet]
        public IActionResult Authorization()
        {
            return View();
        }

        [HttpPost]
        public async Task<IResult> Authorization(string? returnUrl)
        {
            string email = Request.Form["email"]!;

            var persons = _practiceClient.GetPersonAsync().Result.Persons;
            var person = persons.FirstOrDefault(p => p.Email == email);

            if (person == null)
                return Results.BadRequest();

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, person.Email) };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            await Request.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
            return Results.Redirect(returnUrl ?? "Home/Index/");
        }

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IResult> Registration(Person person)
        {
            await _practiceClient.PostPersonAsync(person);
            
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, person.Email) };
            
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            await Request.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
            
            return Results.Redirect($"Home/Index/");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
