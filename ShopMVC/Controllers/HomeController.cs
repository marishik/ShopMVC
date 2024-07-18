using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Practice.Client;
using ShopMVC.Models;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Practice.Client;

namespace ShopMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private static HttpClient _client = new();
        private PracticeClient _practiceClient = new("http://192.168.98.78:5064", _client); //URL NEEDS TO REF TO API ADDRESS

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
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
        [HttpGet]
        public async Task<IActionResult> Shopping()
        {
            var productsResponse = await _practiceClient.GetProductAsync();
            var products = productsResponse.Products.ToList();
            return View((products, (double)0));
        }

        [HttpPost]
        public async Task<IActionResult> Shopping(string[] productsId, int[] productsCount)
        {
            var productsResponse = await _practiceClient.GetProductAsync();
            var products = productsResponse.Products.ToList();
            int temp = 0;

            IEnumerable<int> selectedIds = productsId.Where(id => int.TryParse(id, out temp)).Select(i => temp).ToList();
            var filteredProducts = products.Where(m => selectedIds.Contains(m.Id)).ToList();


            double productsSum = 0.0;
            for (int i = 0; i < productsId.Length; i++)
            {
                productsSum += filteredProducts[i].Price * productsCount[i];
            }


            return View((products, productsSum));
        }

        public double PostPayment(int productId, int count, int orderId)
        {
            var price = _practiceClient
                    .GetProductAsync().Result.Products
                    .First(x => x.Id == productId).Price;

            _practiceClient.PostPaymentAsync(new Payment()
            {
                Id = 0,
                OrderId = orderId,
                Price = price,
                ProductId = productId,
                Count = count
            });

            return price;
        }

        public async Task<Order> PostOrder(string email)
        {
            Order order = new Order()
            {
                Id = 0,
                Total = 0,
                PersonId = await FindByEmail(email),
                Date = DateTime.Now.ToUniversalTime(),
            };

            if (order.Id != -1)
            {
                await _practiceClient.PostOrderAsync(order);
                return order;
            }

            return null;
        }

        public async Task<int> FindByEmail(string email)
        {
            var persons = await _practiceClient.GetPersonAsync();
            var firstPerson = persons.Persons.Where(p => p.Email == email).FirstOrDefault();

            if (firstPerson != null)
            {
                return firstPerson.Id;
            }
            return -1;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Expences()
        {
            var email = Request.HttpContext.User.Claims.FirstOrDefault().Value;

            var persons = _practiceClient.GetPersonAsync().Result.Persons;
            var orders = _practiceClient.GetOrdersAsync().Result.Orders.ToList();
            var payment = _practiceClient.GetPaymentAsync().Result.Payments.ToList();
            var product = _practiceClient.GetProductAsync().Result.Products.ToList();

            var person = persons?.FirstOrDefault(p => p.Email == email);

            double productsSum = 0.0;
            foreach (var order in orders)
            {
                if (person.Id == order.PersonId)
                {
                    productsSum += order.Total;
                }
            }
            return View((person, orders, payment, product, productsSum));
        }
    }
}
