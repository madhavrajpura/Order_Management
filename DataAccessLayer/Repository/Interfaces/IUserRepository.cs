using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace DataAccessLayer.Repository.Interfaces;

public interface IUserRepository
{
    Task<bool> Register(UserViewModel model);
    User GetUserByEmail(string email);
    Task<bool> IsEmailExists(string email);
    Task<bool> IsUsernameExists(string Username);
    Task<bool> ChangePassword(ChangePasswordViewModel changepassword, string Email);
    Task<bool> UpdateUserProfile(UserViewModel user, string Email);
    Task<UserViewModel> GetUserProfileDetails(string Email);
    Task<bool> ResetPassword(User userData);
    List<User> GetAllUsers();
}
