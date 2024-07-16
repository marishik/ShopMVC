using Microsoft.AspNetCore.Mvc;
using Practice.Client;

namespace ShopMVC.Controllers;

public class AdminController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private static HttpClient _client = new();
    private PracticeClient _practiceClient = new("http://192.168.0.101:5064", _client); //URL NEEDS TO REF TO API ADDRESS

    public AdminController(ILogger<HomeController> logger) {
        _logger = logger;
    }
    
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult PersonAdd()
    {
        return View();
    }

    [HttpPost]
    public IActionResult PersonAdd(Person person)
    {
        _practiceClient.PostPersonAsync(person);
        return View();
    }
}