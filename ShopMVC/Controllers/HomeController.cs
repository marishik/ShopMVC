using Microsoft.AspNetCore.Mvc;
using ShopMVC.Models;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Practice.Client;

namespace ShopMVC.Controllers {
    public class HomeController: Controller {
        private readonly ILogger<HomeController> _logger;
        private static HttpClient _client = new();
        private PracticeClient _practiceClient = new("http://192.168.0.101:5064", _client); //URL NEEDS TO REF TO API ADDRESS
        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        [HttpGet] [Authorize]
        public IActionResult Index() {
            return View();
        }

        [HttpGet]
        public IActionResult Authorization()
        {
            return View();
        }

        [HttpPost]
        public IResult Authorization(string email)
        {
            var persons = _practiceClient.GetPersonAsync().Result.Persons;
            var person = persons.FirstOrDefault(p => p.Email == email);

            if (person == null)
                return Results.Unauthorized();

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, person.Email) };
            
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = person.Email
            };
            
            return Results.Json(response);
        }
        
        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }
        
        [HttpPost]
        public IResult Registration(Person person)
        {
            _practiceClient.PostPersonAsync(person);
            
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, person.Email) };
            
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = person.Email
            };
            
            return Results.Json(response);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
