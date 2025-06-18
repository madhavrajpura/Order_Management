using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Item_Order_Management.Controllers;

public class HomeController : Controller
{
    public HomeController()
    {

    }

    public IActionResult Index()
    {
        return View();
    }

}
