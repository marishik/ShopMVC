using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Practice.Client;
using ShopMVC.Models;

namespace ShopMVC.Controllers
{
    public class ReportController : Controller
    {
        static HttpClient _httpClient = new HttpClient();
        static PracticeClient _practiceClient = new PracticeClient("http://192.168.98.78:5064/", _httpClient);

        public SomeModel someModel = new SomeModel
        {
            Orders = (List<Order>)_practiceClient.GetOrdersAsync().Result.Orders,
            Payments = (List<Payment>)_practiceClient.GetPaymentAsync().Result.Payments,
            Persons = (List<Person>)_practiceClient.GetPersonAsync().Result.Persons,
            Products = (List<Product>)_practiceClient.GetProductAsync().Result.Products,
        };

        private readonly ILogger<ReportController> _logger;

        [HttpGet]
        public IActionResult Index()
        {
            return View(someModel);
        }

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
