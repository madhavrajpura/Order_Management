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
            Response.Cookies.Append("ProfileImage", _userService.GetProfileImage(verification_token), option);
            Response.Cookies.Append("UserName", _userService.GetUserName(verification_token), option);

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
        bool CheckEmailExists = await _userService.IsEmailExists(user.Email);

        if (!CheckEmailExists)
        {
            TempData["ErrorMessage"] = NotificationMessage.DoesNotExist.Replace("{0}", "Email");
            return View("ForgotPassword");
        }

        string? getpassword = _userService.GetPassword(user.Email);
        if (!string.IsNullOrEmpty(getpassword))
        {

            string? resetLink = Url.Action("ResetPassword", "Authentication", new { reset_token = _jwtService.GenerateResetToken(user.Email, getpassword) }, Request.Scheme);
            bool sendEmail = await _userService.SendEmail(forgotpassword, resetLink!);
            if (sendEmail)
            {
                TempData["SuccessMessage"] = NotificationMessage.EmailSentSuccessfully;
                return RedirectToAction("Login", "Authentication");
            }
            else
            {
                TempData["ErrorMessage"] = NotificationMessage.EmailSendingFailed;
                return View("ForgotPassword");
            }
        }
        TempData["ErrorMessage"] = NotificationMessage.EmailSendingFailed;
        return View("ForgotPassword");
    }

    #endregion

    #region ResetPassword
    public IActionResult ResetPassword(string reset_token)
    {
        string? reset_email = _jwtService.GetClaimValue(reset_token, "email");
        string? reset_password = _jwtService.GetClaimValue(reset_token, "password");
        string? Db_Password = _userService.GetPassword(reset_email);

        if (Db_Password == reset_password)
        {
            ResetPasswordViewModel resetPassData = new ResetPasswordViewModel();
            resetPassData.Email = reset_email;
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
            bool IsEmailExistsStatus = await _userService.IsEmailExists(resetPassword.Email);

            if (!IsEmailExistsStatus)
            {
                TempData["ErrorMessage"] = NotificationMessage.DoesNotExist.Replace("{0}", "Email");
                return View("ResetPassword");
            }

            if (resetPassword.Password == resetPassword.ConfirmPassword)
            {
                bool checkresetpassword = await _userService.ResetPassword(resetPassword);
                if (checkresetpassword)
                {
                    TempData["SuccessMessage"] = NotificationMessage.PasswordChanged;
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["ErrorMessage"] = NotificationMessage.PasswordChangeFailed;
                    return View("ResetPassword");
                }
            }
        }
        return View("ResetPassword");
    }

    #endregion

}