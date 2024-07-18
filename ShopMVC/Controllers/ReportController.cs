using Microsoft.AspNetCore.Mvc;
using Practice.Client;
using ShopMVC.Models;

namespace ShopMVC.Controllers
{
    public class ReportController : Controller
    {
        private readonly ILogger<ReportController> _logger;
        private readonly PracticeClient _practiceClient;

        public ReportController(ILogger<ReportController> logger, IPracticeClientFactory practiceClientFactory) {
            _practiceClient = practiceClientFactory.CreateClient();
            _logger = logger;
        }

        // TODO: Переделать
        public SomeModel someModel = new SomeModel {
            Orders = (List<Order>)_practiceClient.GetOrdersAsync().Orders,
            Payments = (List<Payment>)_practiceClient.GetPaymentAsync().Result.Payments,
            Persons = (List<Person>)_practiceClient.GetPersonAsync().Result.Persons,
            Products = (List<Product>)_practiceClient.GetProductAsync().Result.Products,
        };

        [HttpGet]
        public IActionResult Index()
        {
            return View(someModel);
        }


        // TODO: Переделать
        [HttpGet]
        public async Task<IActionResult> Show(int? id)
        {
            var orders = _practiceClient.GetOrdersAsync().Result.Orders;
            var persons = _practiceClient.GetPersonAsync().Result.Persons;

            var order = orders?.FirstOrDefault(p => p.Id == id);
            var person = persons?.FirstOrDefault(p =>p.Id == order.PersonId);

            double productsSum = 0.0;
            foreach (var paymentTotal in _practiceClient.GetPaymentAsync().Result.Payments)
            {
                if (paymentTotal.OrderId == order.Id)
                {
                    productsSum += paymentTotal.Price * paymentTotal.Count;
                }
            }
            return View((someModel, order, person, productsSum));
        }
    }
}
