using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace BusinessLogicLayer.Services.Interfaces;

public interface IUserService
{
    Task<bool> Register(UserViewModel model);
    Task<string> Login(UserViewModel model);
     List<Role> GetRoles();

}
