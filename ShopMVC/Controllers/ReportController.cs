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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var orders =  (await _practiceClient.GetOrdersAsync()).Orders.ToList();
            var persons = (await _practiceClient.GetPersonAsync()).Persons.ToList();

            return View((orders, persons));
        }


        [HttpGet]
        public async Task<IActionResult> Show(int? id)
        {
            var order = await _practiceClient.GetOrdersByIdAsync(id);
            var targetOrder = order.Orders.First();
            var person = await _practiceClient.GetPersonByIdAsync(targetOrder.PersonId);
            var toglePerson = person.Persons.First();
            var payments = (await _practiceClient.GetPaymentByOrderIdAsync(targetOrder.Id)).Payments.ToList();
            var products = (await _practiceClient.GetProductAsync()).Products.ToList();

            double productsSum = 0.0;
            foreach (var paymentTotal in payments)
            {
                if (paymentTotal.OrderId == targetOrder.Id)
                {
                    productsSum += paymentTotal.Price * paymentTotal.Count;
                }
            }
            return View((targetOrder, toglePerson, payments, products, productsSum));
        }
    }
}
