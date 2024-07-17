using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
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


        [HttpPost]
        public IActionResult Index(string name, DateTime date, string email, int status) {
            try {
                var responsePerson = practiceClient.PostPersonAsync(new Person() {
                    Id = 0,
                    Name = name,
                    Status = Practice.Client.Status._0,
                    DateOfBirth = date.ToUniversalTime(),
                    RecordStatus = 0,
                    Email = email
                });
            } catch (HttpRequestException e) {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

            return View();
        }

        


        [HttpGet]
        public async Task<IActionResult> Shopping() {
            var productsResponse = await practiceClient.GetProductAsync();
            var products = productsResponse.Products.ToList();
            return View((products, (double)0));
        }

        [HttpPost]
        public async Task<IActionResult> Shopping(string[] productsId, int[] productsCount) {
            var productsResponse = await practiceClient.GetProductAsync();
            var products = productsResponse.Products.ToList();
            int temp = 0;

            IEnumerable<int> selectedIds = productsId.Where(id => int.TryParse(id, out temp)).Select(i => temp).ToList();
            var filteredProducts = products.Where(m => selectedIds.Contains(m.Id)).ToList();


            double productsSum = 0.0;
            for (int i = 0; i < productsId.Length; i++) {
                productsSum += filteredProducts[i].Price * productsCount[i];
            }

            
            return View((products, productsSum));
        }

        public double PostPayment(int productId, int count, int orderId) {
            var price = practiceClient
                    .GetProductAsync().Result.Products
                    .First(x => x.Id == productId).Price;

            practiceClient.PostPaymentAsync(new Payment() {
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
                await practiceClient.PostOrderAsync(order);
                return order;
            }

            return null;
        }

        public async Task<int> FindByEmail(string email) {
            var persons = await practiceClient.GetPersonAsync();
            var firstPerson = persons.Persons.Where(p => p.Email == email).FirstOrDefault();

            if (firstPerson != null) {
                return firstPerson.Id;
            }
            return -1;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
            public IActionResult Error() {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
