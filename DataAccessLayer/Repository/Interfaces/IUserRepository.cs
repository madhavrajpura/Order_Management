using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace DataAccessLayer.Repository.Interfaces;

public interface IUserRepository
{
    Task<bool> Register(UserViewModel model);
    User GetUserByEmail(string email);
    Task<bool> IsEmailExists(string email);
    Task<bool> IsUsernameExists(string Username);
    bool ChangePassword(ChangePasswordViewModel changepassword, string Email);
    bool UpdateUserProfile(UserViewModel user, string Email);
    List<UserViewModel> GetUserProfileDetails(string Email);
    bool ResetPassword(User userData);
}
