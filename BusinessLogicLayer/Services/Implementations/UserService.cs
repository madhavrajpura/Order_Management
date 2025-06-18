using BusinessLogicLayer.Helper;
using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;
using DataAccessLayer.Models;

namespace BusinessLogicLayer.Services.Implementations;

public class UserService : IUserService
{
    private readonly IJWTService _JWTService;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public UserService(IJWTService JWTService, IUserRepository userLoginRepository, IRoleRepository roleRepository)
    {
        _userRepository = userLoginRepository;
        _JWTService = JWTService;
        _roleRepository = roleRepository;
    }

    public async Task<bool> Register(UserViewModel model)
    {
        model.Password = Encryption.EncryptPassword(model.Password);
        bool result = await _userRepository.Register(model);

        if (result)
        {
            return true;
        }

        return false;
    }

    public async Task<string> Login(UserViewModel model)
    {
        User? user = _userRepository.GetUserByEmail(model.Email);

        if (user != null && user.IsDelete == false)
        {
            if (user.Password == Encryption.EncryptPassword(model.Password))
            {
                string? roleName = _roleRepository.GetRoleById(model.RoleId);
                string token = _JWTService.GenerateToken(model.Email, roleName);
                return token;
            }
            return null;
        }
        return null;


    }

    public (string UserName, string Email) GetUserData(string email)
    {
        User? data = _userRepository.GetUserByEmail(email);
        if (data == null)
        {
            return (string.Empty, string.Empty);
        }
        string UserName = data.Username;
        string Email = data.Email;
        return (UserName, Email);
    }

    public List<Role> GetRoles()
    {
        List<Role> roles = _roleRepository.GetRoles();
        
        return roles;
    }

}