using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace DataAccessLayer.Repository.Interfaces;

public interface IUserRepository
{
    Task<bool> Register(UserViewModel model);
    User GetUserByEmail(string email);
    Task<bool> IsEmailExists(string email);
    Task<bool> IsUserExists(string Username, string Email);
    Task<bool> ChangePassword(ChangePasswordViewModel changepassword, string Email);
    Task<bool> UpdateUserProfile(UserViewModel user, int UserId);
    Task<UserViewModel> GetUserProfileDetails(int UserId);
    Task<bool> ResetPassword(User userData);
    List<User> GetAllUsers();

    // Newwwwwwwwwwwwwww
    Task<List<UserMainViewModel>> GetUsers(int offset, int limit, string search);
    Task<int> GetTotalUserCount(string search);

}
