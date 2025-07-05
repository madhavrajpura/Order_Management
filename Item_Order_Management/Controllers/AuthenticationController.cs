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

            TempData["ErrorMessage"] = NotificationMessage.InvalidCredentials;
            return View();
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(UserViewModel model)
    {
        string? generatedToken = await _userService.Login(model);

        CookieOptions option = new CookieOptions();
        option.Expires = DateTime.Now.AddHours(30);

        if (generatedToken != null)
        {
            UserInfoViewModel? userInfo = _userService.GetUserInformation(generatedToken);
            Response.Cookies.Append("JWTToken", generatedToken, option);
            Response.Cookies.Append("ProfileImage", userInfo.ProfileImage, option);
            Response.Cookies.Append("UserName", userInfo.UserName, option);

            string? roleName = _jwtService.GetClaimValue(generatedToken, "role");

            if (roleName == "User")
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
        Response.Cookies.Delete("ProfileImage");
        Response.Cookies.Delete("UserName");
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
        bool isUserExists = await _userService.IsUserExists(model.UserName, model.Email);
        if (isUserExists)
        {
            TempData["ErrorMessage"] = NotificationMessage.AlreadyExists.Replace("{0}", "Username or Email");
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

    #region ForgotPassword

    public string GetEmail(string Email)
    {
        ForgotPasswordViewModel forgotPasswordViewModel = new ForgotPasswordViewModel();
        forgotPasswordViewModel.Email = Email;
        TempData["Email"] = Email;
        return Email;
    }

    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotpassword)
    {
        UserViewModel? user = new UserViewModel();
        user.Email = forgotpassword.Email;

        bool isEmailExists = await _userService.IsEmailExists(user.Email);

        if (!isEmailExists)
        {
            TempData["ErrorMessage"] = NotificationMessage.DoesNotExist.Replace("{0}", "Email");
            return View("ForgotPassword");
        }

        string? userPassword = _userService.GetPassword(user.Email);

        if (!string.IsNullOrEmpty(userPassword))
        {

            string? resetLink = Url.Action("ResetPassword", "Authentication", new { reset_token = _jwtService.GenerateResetToken(user.Email, userPassword) }, Request.Scheme);
            bool userEmail = await _userService.SendEmail(forgotpassword, resetLink!);
            if (userEmail)
            {
                TempData["SuccessMessage"] = NotificationMessage.EmailSentSuccessfully;
                return RedirectToAction("Login", "Authentication");
            }
            TempData["ErrorMessage"] = NotificationMessage.EmailSendingFailed;
            return View("ForgotPassword");
        }
        TempData["ErrorMessage"] = NotificationMessage.EmailSendingFailed;
        return View("ForgotPassword");
    }

    #endregion

    #region ResetPassword
    public IActionResult ResetPassword(string reset_token)
    {
        string? email = _jwtService.GetClaimValue(reset_token, "email");
        string? password = _jwtService.GetClaimValue(reset_token, "password");
        string? dbPassword = _userService.GetPassword(email!);

        if (dbPassword == password)
        {
            ResetPasswordViewModel resetPassData = new ResetPasswordViewModel();
            resetPassData.Email = email!;
            return View(resetPassData);
        }
        TempData["ErrorMessage"] = NotificationMessage.ResetPasswordChangedError;
        return RedirectToAction("Login", "Authentication");
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPassword)
    {
        if (ModelState.IsValid)
        {
            bool isEmailExistsStatus = await _userService.IsEmailExists(resetPassword.Email);

            if (!isEmailExistsStatus)
            {
                TempData["ErrorMessage"] = NotificationMessage.DoesNotExist.Replace("{0}", "Email");
                return View("ResetPassword");
            }

            if (resetPassword.Password == resetPassword.ConfirmPassword)
            {
                bool resetPasswordStatus = await _userService.ResetPassword(resetPassword);
                if (resetPasswordStatus)
                {
                    TempData["SuccessMessage"] = NotificationMessage.PasswordChanged;
                    return RedirectToAction("Login");
                }
                TempData["ErrorMessage"] = NotificationMessage.PasswordChangeFailed;
                return View("ResetPassword");
            }
        }
        return View("ResetPassword");
    }

    #endregion

}