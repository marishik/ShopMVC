using Microsoft.AspNetCore.Mvc;
using Practice.Client;
using ShopMVC.Models;

namespace ShopMVC.Controllers
{
    public class ReportController : Controller
    {
        static HttpClient _httpClient = new HttpClient();
        static PracticeClient _practiceClient = new PracticeClient("http://192.168.98.78:5064/", _httpClient);

        private readonly ILogger<ReportController> _logger;

        [HttpGet]
        public IActionResult Index()
        {
            var someModel = new SomeModel
            {
                Orders = (List<Order>)_practiceClient.GetOrdersAsync().Result.Orders,
                Payments = (List<Payment>)_practiceClient.GetPaymentAsync().Result.Payments,
                Persons = (List<Person>)_practiceClient.GetPersonAsync().Result.Persons,
                Products = (List<Product>)_practiceClient.GetProductAsync().Result.Products,
            };

            return View(someModel);
        }

        [HttpGet]
        public async Task<IActionResult> Show(int? id) {

            var orders = _practiceClient.GetOrdersAsync().Result.Orders.ToList();
            foreach (var order in orders)
            {
                if (order.Id == id)
                {
                    var someModel = new SomeModel
                    {
                        OrderId = order.Id,
                        Orders = (List<Order>)_practiceClient.GetOrdersAsync().Result.Orders,
                        Payments = (List<Payment>)_practiceClient.GetPaymentAsync().Result.Payments,
                        Persons = (List<Person>)_practiceClient.GetPersonAsync().Result.Persons,
                        Products = (List<Product>)_practiceClient.GetProductAsync().Result.Products,
                    };

                    double productsSum = 0.0;
                    foreach(var paymentTotal in _practiceClient.GetPaymentAsync().Result.Payments)
                    {
                        if(paymentTotal.OrderId == order.Id)
                        {
                            productsSum += order.Total;
                        }
                    }
                    return View((someModel, productsSum));
                }
            }
            return NotFound();
        }
    }
}
