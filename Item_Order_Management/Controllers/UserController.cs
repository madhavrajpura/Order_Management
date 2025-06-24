using BusinessLogicLayer.Helper;
using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Item_Order_Management.Controllers;

public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly IJWTService _jwtService;


    public UserController(IUserService userService, IJWTService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }

    #region User Profile

    [HttpGet]
    public IActionResult UserProfile()
    {
        string? token = Request.Cookies["JWTToken"];
        System.Security.Claims.ClaimsPrincipal? claims = _jwtService.GetClaimsFromToken(token!);

        string? Email = _jwtService.GetClaimValue(token, "email");

        if (claims == null || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }

        List<UserViewModel>? data = _userService.GetUserProfileDetails(Email);
        return View(data[0]);
    }

    [HttpPost]
    public IActionResult UserProfile(UserViewModel user)
    {
        string? token = Request.Cookies["JWTToken"];
        System.Security.Claims.ClaimsPrincipal? claims = _jwtService.GetClaimsFromToken(token!);

        string? Email = _jwtService.GetClaimValue(token, "email");

        if (claims == null || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }

        if (user.ImageFile != null)
        {
            string[] extension = user.ImageFile.FileName.Split(".");
            string ext = extension[extension.Length - 1].ToLower();

            if (new[] { "jpg", "jpeg", "png", "gif", "webp", "jfif" }.Contains(ext))
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                string fileName = ImageTemplate.GetFileName(user.ImageFile, path);
                user.ImageURL = $"/Uploads/{fileName}";
            }
            else
            {
                TempData["ErrorMessage"] = NotificationMessage.ImageFormat;
                return View();
            }
        }

        if (_userService.UpdateUserProfile(user, Email))
        {
            CookieOptions options = new CookieOptions();
            options.Expires = DateTime.Now.AddMinutes(60);
            if (user.ImageURL != null)
            {
                Response.Cookies.Append("ProfileImage", user.ImageURL, options);
            }
            Response.Cookies.Append("UserName", user.UserName, options);

            TempData["SuccessMessage"] = NotificationMessage.UpdateSuccess.Replace("{0}", "Profile");
            return RedirectToAction("Dashboard", "Items");
        }
        TempData["ErrorMessage"] = NotificationMessage.UpdateFailure.Replace("{0}", "Profile");
        return View();
    }

    #endregion

    #region ChangePassword

    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ChangePassword(ChangePasswordViewModel changepassword)
    {
        string? token = Request.Cookies["JWTToken"];
        System.Security.Claims.ClaimsPrincipal? claims = _jwtService.GetClaimsFromToken(token!);

        string? Email = _jwtService.GetClaimValue(token, "email");

        if (claims == null || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }

        if (changepassword.CurrentPassword == changepassword.NewPassword)
        {
            TempData["ErrorMessage"] = "Current Password and New Password cannot be same";
            return View();
        }
        else
        {
            changepassword.CurrentPassword = Encryption.EncryptPassword(changepassword.CurrentPassword);
            changepassword.NewPassword = Encryption.EncryptPassword(changepassword.NewPassword);
            bool password_verify = _userService.ChangePassword(changepassword, Email);

            if (password_verify)
            {
                TempData["SuccessMessage"] = NotificationMessage.UpdateSuccess.Replace("{0}", "Password");
                return RedirectToAction("Dashboard", "Items");
            }
            else
            {
                TempData["ErrorMessage"] = NotificationMessage.UpdateFailure.Replace("{0}", "Password");
                return View();
            }
        }
    }

    #endregion

}