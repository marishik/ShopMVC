using Microsoft.AspNetCore.Mvc;
using Practice.Client;
using static NuGet.Packaging.PackagingConstants;

namespace ShopMVC.Controllers
{
    public class ProductController : Controller
    {

        static HttpClient _httpClient = new HttpClient();
        static PracticeClient _practiceClient = new PracticeClient("http://192.168.98.78:5064/", _httpClient);

        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var product = _practiceClient.GetProductAsync().Result.Products;
            return View(product);
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
