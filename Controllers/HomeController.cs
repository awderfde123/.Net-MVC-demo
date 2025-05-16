using demo.Models;
using demo.Services;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{

    [HttpGet]
    public ActionResult Index()
    {
        return View();
    }

}
