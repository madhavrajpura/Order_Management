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
        return await _userRepository.Register(model);
    }

    public async Task<string> Login(UserViewModel model)
    {
        User? user = _userRepository.GetUserByEmail(model.Email);

        if (user != null && user.IsDelete == false)
        {
            if (user.Password == Encryption.EncryptPassword(model.Password))
            {
                string? roleName = _roleRepository.GetRoleById(model.RoleId);
                if (string.IsNullOrEmpty(roleName))
                {
                    roleName = "3";
                }
                string token = _JWTService.GenerateToken(model.Email, roleName,user.Id);
                return token;
            }
            return null;
        }
        return null;
    }

    public async Task<bool> IsEmailExists(string email) => await _userRepository.IsEmailExists(email);

    public async Task<bool> IsUsernameExists(string Username) => await _userRepository.IsUsernameExists(Username);

    public int GetUserIdFromToken(string token)
    {
        string? Email = _JWTService.GetClaimValue(token, "email");

        if (string.IsNullOrEmpty(Email)) return 0;

        User? user = _userRepository.GetUserByEmail(Email!);

        if (user.Email == Email) return user.Id;

        return 0;
    }

}