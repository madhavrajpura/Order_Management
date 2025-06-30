using DataAccessLayer.Context;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository.Implementations;

public class UserRepository : IUserRepository
{

    private readonly ApplicationDbContext _db;
    public UserRepository(ApplicationDbContext db) => _db = db;

    public async Task<bool> Register(UserViewModel model)
    {
        try
        {
            User? user = new User();
            user.Username = model.UserName;
            user.Email = model.Email;
            user.RoleId = model.RoleId;
            user.Password = model.Password;
            user.PhoneNumber = model.PhoneNumber;
            user.CreatedAt = DateTime.Now;
            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Register: {ex.Message}");
            throw new Exception("Error registering user", ex);
        }
    }

    public User GetUserByEmail(string email)
    {
        try
        {
            User? user = _db.Users.FirstOrDefault(e => e.Email == email);

            if (user == null) return null;
            return user;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetUserByEmail: {ex.Message}");
            throw new Exception("Error retrieving user by email", ex);
        }
    }

    public async Task<bool> IsEmailExists(string email)
    {
        try
        {
            User? user = await _db.Users.FirstOrDefaultAsync(e => e.Email == email);

            if (user != null)
            {
                return true;
            }
            return false;
        }
        catch
        {
            Console.WriteLine("Error in IsEmailExists");
            throw new Exception("Error checking if email exists");
        }
    }

    public async Task<bool> IsUsernameExists(string Username)
    {
        try
        {
            User? user = await _db.Users.FirstOrDefaultAsync(e => e.Username == Username);

            if (user != null)
            {
                return true;
            }
            return false;
        }
        catch
        {
            Console.WriteLine("Error in IsEmailExists");
            throw new Exception("Error checking if username exists");
        }
    }

    public async Task<bool> ChangePassword(ChangePasswordViewModel changepassword, string Email)
    {
        try
        {
            User? userdetails = GetUserByEmail(Email!);
            if (userdetails != null)
            {
                if (userdetails.Password == changepassword.CurrentPassword)
                {
                    userdetails.Password = changepassword.NewPassword;
                    _db.Update(userdetails);
                    await _db.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            return false;

        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> UpdateUserProfile(UserViewModel user, string Email)
    {
        try
        {
            User userdetails = GetUserByEmail(Email);

            userdetails.Username = user.UserName;
            userdetails.Email = user.Email;
            userdetails.Address = user.Address;
            if (user.ImageFile != null)
            {
                userdetails.ImageURL = user.ImageURL;
            }
            userdetails.PhoneNumber = user.PhoneNumber;

            _db.Update(userdetails);
            await _db.SaveChangesAsync();
            return true;
        }
        catch
        {
            throw;
        }

    }

    public async Task<UserViewModel> GetUserProfileDetails(string Email)
    {
        UserViewModel? data = await _db.Users.Where(x => x.Email == Email)
        .Select(
            x => new UserViewModel
            {
                UserName = x.Username,
                Email = x.Email,
                ImageURL = x.ImageURL,
                PhoneNumber = x.PhoneNumber,
                Address = x.Address
            }
        ).FirstOrDefaultAsync();

        return data;
    }

    public async Task<bool> ResetPassword(User userData)
    {
        try
        {
            _db.Update(userData);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception Exception)
        {
            Console.WriteLine("Exception Occured : ", Exception.Message);
            return false;
        }
    }

    public List<User> GetAllUsers()
    {
        return _db.Users.Where(u => !u.IsDelete && u.RoleId == 3).ToList();
    }


}