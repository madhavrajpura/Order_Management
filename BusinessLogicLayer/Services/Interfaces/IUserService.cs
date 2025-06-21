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

}