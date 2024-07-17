using Microsoft.AspNetCore.Mvc;
using Practice.Client;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace ShopMVC.Controllers;

[Authorize]
public class AdminController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private static HttpClient _client = new();
    private PracticeClient _practiceClient = new("http://192.168.98.78:5064", _client); //URL NEEDS TO REF TO API ADDRESS

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
        var postperson = _practiceClient.PostPersonAsync(person);
        if (postperson.Result.StatusCode == (int)HttpStatusCode.OK)
            return View((_practiceClient.GetPersonAsync().Result.Persons.ToList(), true));
        return View();
    }
}