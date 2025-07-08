using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace BusinessLogicLayer.Services.Interfaces;

public interface IUserService
{
    Task<bool> Register(UserViewModel model);
    Task<string> Login(UserViewModel model);
    Task<bool> IsEmailExists(string email);
    Task<bool> IsUserExists(string Username, string Email);
    int GetUserIdFromToken(string token);
    Task<bool> ChangePassword(ChangePasswordViewModel changepassword, string Email);
    Task<bool> UpdateUserProfile(UserViewModel user, int UserId);
    Task<UserViewModel> GetUserProfileDetails(int UserId);
    UserInfoViewModel GetUserInformation(string token);
    string GetPassword(string Email);
    Task<bool> SendEmail(ForgotPasswordViewModel forgotpassword, string resetLink);
    Task<bool> ResetPassword(ResetPasswordViewModel resetPassword);
    List<User> GetAllUsers();

    // nEWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWwwwwwwwwwwwwwwwwwwwww
    Task<List<UserMainViewModel>> GetUsers(int offset, int limit, string search);
    Task<int> GetTotalUserCount(string search);

}