using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace BusinessLogicLayer.Services.Interfaces;

public interface IUserService
{
    Task<bool> Register(UserViewModel model);
    Task<string> Login(UserViewModel model);
    Task<bool> IsEmailExists(string email);
    Task<bool> IsUsernameExists(string Username);
    int GetUserIdFromToken(string token);
    Task<bool> ChangePassword(ChangePasswordViewModel changepassword, string Email);
    Task<bool> UpdateUserProfile(UserViewModel user, string Email);
    Task<UserViewModel> GetUserProfileDetails(string Email);
    string GetProfileImage(string token);
    string GetUserName(string token);
    string GetPassword(string Email);
    Task<bool> SendEmail(ForgotPasswordViewModel forgotpassword, string resetLink);
    Task<bool> ResetPassword(ResetPasswordViewModel resetPassword);
    List<User> GetAllUsers();

}