using Microsoft.AspNetCore.Mvc;
using Practice.Client;
using ShopMVC.Models;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace ShopMVC.Controllers {

    public class HomeController: Controller {
        private static Dictionary<int, int> _productIdCount;
        private readonly ILogger<HomeController> _logger;
        private static HttpClient _client = new();
        private PracticeClient _practiceClient = new("http://192.168.98.78:5064", _client); //URL NEEDS TO REF TO API ADDRESS

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Index() {
            return View();
        }

        #region Authorization and Registration

        [HttpGet]
        public IActionResult Authorization() {
            return View();
        }

        [HttpPost]
        public async Task<IResult> Authorization(string? returnUrl) {
            string email = Request.Form["email"]!;

            // TODO: Валидацию email 

            var responsePersons = (await _practiceClient.GetPersonAsync());
            var persons = responsePersons.Persons;

            var person = persons.FirstOrDefault(p => p.Email == email);

            if (person == null)
                return Results.BadRequest();

            var claims = new List<Claim> { 
                new Claim(ClaimTypes.Name, person.Email) 
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            await Request.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            if (email == "admin@admin") {
                return Results.Redirect("/Admin/");
            }

            if (returnUrl == null) {
                return Results.Redirect("/");
            }

            return Results.Redirect(returnUrl);
        }

        [HttpGet]
        public IActionResult Registration() {
            return View();
        }

        [HttpPost]
        public async Task<IResult> Registration(Person person) {
            await _practiceClient.PostPersonAsync(person);

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, person.Email) };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            await Request.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return Results.Redirect("/");
        }

        public async Task<IResult> Logout() {
            await Request.HttpContext.SignOutAsync();
            return Results.Redirect($"~/Home/Authorization");
        }

        #endregion

        #region Buying product logic

        [HttpGet]
        public async Task<IActionResult> Shopping() {
            var productsResponse = await _practiceClient.GetProductAsync();
            var products = productsResponse.Products.ToList();
            return View((products, (double)0));
        }

        [HttpPost]
        public async Task<IActionResult> Shopping(string[] productsId, int[] productsCount) {
            _productIdCount = new();

            var productsResponse = await _practiceClient.GetProductAsync();
            var products = productsResponse.Products.ToList();

            int temp = 0;

            IEnumerable<int> selectedIds =
                productsId.Where(id => int.TryParse(id, out temp)).Select(i => temp).ToList();
            var filteredProducts = products.Where(m => selectedIds.Contains(m.Id)).ToList();

            double productsSum = 0.0;

            for (int id = 0; id < productsId.Length; id++) {
                for (int i = 0; i < productsCount.Length; i++) {
                    if (productsCount[i] == 0) {
                        continue;
                    }

                    productsSum += productsCount[i] * filteredProducts[id].Price;
                    _productIdCount.Add(int.Parse(productsId[id]), productsCount[i]);

                    productsCount[i] = 0;
                    break;
                }
            }
            return View((products, productsSum));
        }

        [HttpGet]
        public IActionResult Buy() {
            return View();
        }

        public async Task<IResult> BuyShopping() {
            var email = Request.HttpContext.User.Claims.FirstOrDefault()!.Value;
            var order = await PostOrder(email);
            var orderId = _practiceClient.GetOrdersAsync().Result.Orders.Last().Id;
            double total = 0;

            foreach (var product in _productIdCount) {
                var price = PostPayment(product.Key, product.Value, orderId);
                total += price * product.Value;
            }

            await _practiceClient.PutOrderAsync(new Order() {
                Id = orderId,
                Total = total,
                PersonId = order.PersonId,
                Date = DateTime.Now.ToUniversalTime(),
            });

            _productIdCount.Clear();

            return Results.Redirect("~/Home/Buy");
        }

        public double PostPayment(int productId, int count, int orderId) {
            var price = _practiceClient
                .GetProductAsync().Result.Products
                .First(x => x.Id == productId).Price;

            _practiceClient.PostPaymentAsync(new Payment() {
                Id = 0,
                OrderId = orderId,
                Price = price,
                ProductId = productId,
                Count = count
            });

            return price;
        }

        public async Task<Order> PostOrder(string email) {
            Order order = new Order() {
                Id = 0,
                Total = 0,
                PersonId = await FindByEmail(email),
                Date = DateTime.Now.ToUniversalTime(),
            };

            if (order.Id != -1) {
                await _practiceClient.PostOrderAsync(order);
                return order;
            }

            return null;
        }

        public async Task<int> FindByEmail(string email) {
            var persons = await _practiceClient.GetPersonAsync();
            var firstPerson = persons.Persons.Where(p => p.Email == email).FirstOrDefault();

            if (firstPerson != null) {
                return firstPerson.Id;
            }

            return -1;
        }

        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        // TODO: Переделать
        [HttpGet]
        public async Task<IActionResult> Expences() {
            var email = Request.HttpContext.User.Claims.FirstOrDefault().Value;

            var persons = (await _practiceClient.GetPersonAsync()).Persons;
            var orders = (await _practiceClient.GetOrdersAsync()).Orders.ToList();
            var payment = (await _practiceClient.GetPaymentAsync()).Payments.ToList();
            var product = (await _practiceClient.GetProductAsync()).Products.ToList();

            var person = persons?.FirstOrDefault(p => p.Email == email);

            double productsSum = 0.0;
            foreach (var order in orders) {
                if (person.Id == order.PersonId) {
                    productsSum += order.Total;
                }
            }
            return View((person, orders, payment, product, productsSum));
        }
    }
}
