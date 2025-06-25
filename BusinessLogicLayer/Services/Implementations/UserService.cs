using BusinessLogicLayer.Helper;
using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;
using DataAccessLayer.Models;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace BusinessLogicLayer.Services.Implementations;

public class UserService : IUserService
{
    private readonly IJWTService _JWTService;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IConfiguration _configuration;

    public UserService(IJWTService JWTService, IUserRepository userLoginRepository, IRoleRepository roleRepository, IConfiguration configuration)
    {
        _userRepository = userLoginRepository;
        _JWTService = JWTService;
        _roleRepository = roleRepository;
        _configuration = configuration;
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
                string? roleName = _roleRepository.GetRoleById(user.RoleId);
                if (string.IsNullOrEmpty(roleName))
                {
                    roleName = "User";
                }
                string token = _JWTService.GenerateToken(model.Email, roleName, user.Id);
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

    public async Task<bool> ChangePassword(ChangePasswordViewModel changepassword, string Email)
    {
        return await _userRepository.ChangePassword(changepassword, Email);
    }

    public async Task<bool> UpdateUserProfile(UserViewModel user, string Email)
    {
        return await _userRepository.UpdateUserProfile(user, Email);
    }

    public async Task<UserViewModel> GetUserProfileDetails(string Email)
    {
        return await _userRepository.GetUserProfileDetails(Email);
    }

    public string GetProfileImage(string token)
    {
        string? Email = _JWTService.GetClaimValue(token, "email");

        if (string.IsNullOrEmpty(Email)) return null!;

        User? user = _userRepository.GetUserByEmail(Email!);

        if (user.Email == Email) return user.ImageURL ?? "../images/Default_pfp.svg.png";

        return null!;
    }

    public string GetUserName(string token)
    {
        string? Email = _JWTService.GetClaimValue(token, "email");

        if (string.IsNullOrEmpty(Email)) return null!;

        User? user = _userRepository.GetUserByEmail(Email!);

        if (user.Email == Email) return user.Username!;

        return null!;
    }

    public string GetPassword(string Email)
    {
        if (string.IsNullOrEmpty(Email)) return null!;

        User? user = _userRepository.GetUserByEmail(Email!);

        if (user.Email == Email) return user.Password!;

        return null!;
    }

    //  Used to Send Email
    public async Task<bool> SendEmail(ForgotPasswordViewModel forgotpassword, string resetLink)
    {
        string email = forgotpassword.Email;
        User? user = _userRepository.GetUserByEmail(email);
        string userName = user.Username;
        if (email != null)
        {
            try
            {
                MailAddress senderEmail = new MailAddress(_configuration["smtp:SenderEmail"], "sender");
                MailAddress receiverEmail = new MailAddress(forgotpassword.Email, "reciever");
                string password = _configuration["smtp:Password"];
                string sub = "Forgot Password";
                string body = EmailTemplate.ResetPasswordEmail(resetLink,userName);
                SmtpClient smtp = new SmtpClient
                {
                    Host = _configuration["smtp:Host"],
                    Port = int.Parse(_configuration["smtp:Port"]),
                    EnableSsl = bool.Parse(_configuration["smtp:EnableSsl"]),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = bool.Parse(_configuration["smtp:UseDefaultCredentials"]),
                    Credentials = new NetworkCredential(senderEmail.Address, password)
                };
                using (var mess = new MailMessage(senderEmail, receiverEmail))
                {
                    mess.Subject = sub;
                    mess.Body = body;
                    mess.IsBodyHtml = true;
                    await smtp.SendMailAsync(mess);
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("Exception occured is : ", exp.Message);
                return false;
            }

            return true;
        }
        return false;
    }

    // Used to find If email exists and will update the encrypted password in the DB. 
    public async Task<bool> ResetPassword(ResetPasswordViewModel resetPassword)
    {
        try
        {
            User? data = _userRepository.GetUserByEmail(resetPassword.Email);

            if (data != null && !data.IsDelete)
            {
                // Check if the new password is the same as the old password
                if (data.Password == Encryption.EncryptPassword(resetPassword.Password))
                {
                    return false;
                }

                data.Password = Encryption.EncryptPassword(resetPassword.Password);
                if (await _userRepository.ResetPassword(data)) return true;

                return false;
            }
            return false;
        }
        catch
        {
            throw;
        }
    }

    public List<User> GetAllUsers()
    {
        return _userRepository.GetAllUsers();
    }

}