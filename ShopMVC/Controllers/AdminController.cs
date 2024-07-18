using Microsoft.AspNetCore.Mvc;
using Practice.Client;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace ShopMVC.Controllers;

[Authorize]
public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;
    private readonly PracticeClient _practiceClient;
    public AdminController(ILogger<AdminController> logger, IPracticeClientFactory practiceClientFactory) {
        _practiceClient = practiceClientFactory.CreateClient();
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