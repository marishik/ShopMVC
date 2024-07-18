using Microsoft.AspNetCore.Mvc;
using Practice.Client;

namespace ShopMVC.Controllers
{
    public class ProductController : Controller
    {

        static HttpClient _httpClient = new HttpClient();
        private readonly ILogger<ProductController> _logger;
        private readonly PracticeClient _practiceClient;
        public ProductController(ILogger<ProductController> logger, IPracticeClientFactory practiceClientFactory) {
            _practiceClient = practiceClientFactory.CreateClient();
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var product = _practiceClient.GetProductAsync().Result.Products.ToList();
            return View((product));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public RedirectResult Create(Product product)
        {
            _practiceClient.PostProductAsync(product);

            return Redirect("/Product/Index");
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            var products = _practiceClient.GetProductAsync().Result.Products;
            var product = products?.FirstOrDefault(p => p.Id == id);

            return View(product);
        }

        [HttpPost]
        public RedirectResult Edit(Product product)
        {
            _practiceClient.PutProductAsync(product);

            return Redirect("/Product/Index");
        }
    }
}
