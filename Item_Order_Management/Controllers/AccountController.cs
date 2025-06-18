using BusinessLogicLayer.Helper;
using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Item_Order_Management.Controllers;

public class AccountController : Controller
{
    private readonly IUserService _userService;
    private readonly IJWTService _jwtService;

    public AccountController(IUserService userService, IJWTService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        if (Request.Cookies.ContainsKey("JWTToken"))
        {
            string? token = Request.Cookies["JWTToken"];
            System.Security.Claims.ClaimsPrincipal? claims = _jwtService.GetClaimsFromToken(token!);

            if (claims != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["ErrorMessage"] = NotificationMessage.InvalidCredentials;
                return View();
            }
        }
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(UserViewModel model)
    {
        bool result = await _userService.Register(model);

        if (result)
        {
            TempData["SuccessMessage"] = NotificationMessage.RegistrationSuccess;
            return RedirectToAction("Index", "Account");
        }

        TempData["ErrorMessage"] = NotificationMessage.RegistrationFailed;
        return RedirectToAction("Register", "Account");
    }

    [HttpPost]
    public async Task<IActionResult> Login(UserViewModel model)
    {
        string? verification_token = await _userService.Login(model);

        CookieOptions option = new CookieOptions();
        option.Expires = DateTime.Now.AddHours(30);

        if (verification_token != null)
        {
            Response.Cookies.Append("JWTToken", verification_token, option);

            TempData["SuccessMessage"] = NotificationMessage.LoginSuccess;
            return RedirectToAction("Index", "Home");
        }

        TempData["ErrorMessage"] = NotificationMessage.InvalidCredentials;
        return RedirectToAction("Index", "Account");
    }

    public IActionResult Logout()
    {
        Response.Cookies.Delete("JWTToken");
        Response.Headers["Clear-Site-Data"] = "\"cache\", \"cookies\", \"storage\"";
        TempData["SuccessMessage"] = NotificationMessage.LogoutSuccess;

        return RedirectToAction("Index", "Account");
    }
}