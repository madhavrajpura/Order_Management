using BusinessLogicLayer.Helper;
using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Item_Order_Management.Controllers;

public class AuthenticationController : Controller
{
    private readonly IUserService _userService;
    private readonly IJWTService _jwtService;

    public AuthenticationController(IUserService userService, IJWTService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }

    #region Login

    [HttpGet]
    public IActionResult Login()
    {
        if (Request.Cookies.ContainsKey("JWTToken"))
        {
            string? token = Request.Cookies["JWTToken"];
            System.Security.Claims.ClaimsPrincipal? claims = _jwtService.GetClaimsFromToken(token!);


            if (claims != null)
            {
                if (User.IsInRole("User"))
                {
                    return RedirectToAction("Dashboard", "Items");
                }
                return RedirectToAction("Dashboard", "Admin");
            }
            else
            {
                TempData["ErrorMessage"] = NotificationMessage.InvalidCredentials;
                return View();
            }
        }
        return View();
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

            string? RoleName = _jwtService.GetClaimValue(verification_token, "role");

            if (RoleName == "User")
            {
                TempData["SuccessMessage"] = NotificationMessage.LoginSuccess;
                return RedirectToAction("Dashboard", "Items");
            }
            TempData["SuccessMessage"] = NotificationMessage.LoginSuccess;
            return RedirectToAction("Dashboard", "Admin");
        }

        TempData["ErrorMessage"] = NotificationMessage.InvalidCredentials;
        return RedirectToAction("Login", "Authentication");
    }

    public IActionResult Logout()
    {
        Response.Cookies.Delete("JWTToken");
        Response.Headers["Clear-Site-Data"] = "\"cache\", \"cookies\", \"storage\"";
        TempData["SuccessMessage"] = NotificationMessage.LogoutSuccess;

        return RedirectToAction("Login", "Authentication");
    }

    #endregion

    #region Register

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(UserViewModel model)
    {
        bool isEmailExists = await _userService.IsEmailExists(model.Email);
        bool isUsernameExists = await _userService.IsUsernameExists(model.UserName);

        if (isEmailExists)
        {
            TempData["ErrorMessage"] = NotificationMessage.AlreadyExists.Replace("{0}", "Email");
            return RedirectToAction("Register", "Authentication");
        }
        else if (isUsernameExists)
        {
            TempData["ErrorMessage"] = NotificationMessage.AlreadyExists.Replace("{0}", "Username");
            return RedirectToAction("Register", "Authentication");
        }
        else
        {
            bool result = await _userService.Register(model);

            if (result)
            {
                TempData["SuccessMessage"] = NotificationMessage.RegistrationSuccess;
                return RedirectToAction("Login", "Authentication");
            }

            TempData["ErrorMessage"] = NotificationMessage.RegistrationFailed;
            return RedirectToAction("Register", "Authentication");
        }
    }

    #endregion
    
}