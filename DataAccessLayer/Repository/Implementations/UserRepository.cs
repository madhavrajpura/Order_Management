using DataAccessLayer.Models;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository.Implementations;

public class UserRepository : IUserRepository
{

    private readonly NewItemOrderDbContext _db;
    public UserRepository(NewItemOrderDbContext db) => _db = db;

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
        User? user = _db.Users.FirstOrDefault(e => e.Email == email) ?? throw new Exception();
        return user;
    }

    public async Task<bool> IsEmailExists(string email)
    {
        User? user = await _db.Users.FirstOrDefaultAsync(e => e.Email == email) ?? throw new Exception();

        if (user != null)
        {
            return true;
        }
        return false;
    }

    public async Task<bool> IsUserExists(string Username, string Email)
    {
        bool user = await _db.Users.AnyAsync(e => e.Username == Username || e.Email == Email);

        if (user)
        {
            return true;
        }
        return false;
    }

    public async Task<bool> ChangePassword(ChangePasswordViewModel changepassword, string Email)
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

    public async Task<bool> UpdateUserProfile(UserViewModel user, int UserId)
    {
        User? userDetails = await _db.Users.FirstOrDefaultAsync(user => user.Id == UserId) ?? throw new Exception();

        if (userDetails == null)
        {
            return false;
        }

        userDetails.Username = user.UserName;
        userDetails.Email = user.Email;
        userDetails.Address = user.Address;

        if (user.ImageFile != null)
        {
            userDetails.ImageUrl = user.ImageURL;
        }
        userDetails.PhoneNumber = user.PhoneNumber;

        _db.Update(userDetails);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<UserViewModel> GetUserProfileDetails(int UserId)
    {
        UserViewModel? User = await _db.Users.Where(x => x.Id == UserId)
        .Select(
            x => new UserViewModel
            {
                UserName = x.Username,
                Email = x.Email,
                ImageURL = x.ImageUrl,
                PhoneNumber = x.PhoneNumber,
                Address = x.Address
            }
        ).FirstOrDefaultAsync() ?? throw new Exception();

        return User;
    }

    public async Task<bool> ResetPassword(User userData)
    {
        _db.Update(userData);
        await _db.SaveChangesAsync();
        return true;
    }

    public List<User> GetAllUsers()
    {
        return _db.Users.Where(u => !u.IsDelete && u.RoleId == 3).ToList() ?? throw new Exception();
    }

    public async Task<List<UserMainViewModel>> GetUsers(int offset, int limit, string search)
    {
        var query = _db.Users.AsQueryable();
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(u => u.Username.Contains(search));
        }
        return await query
            .Skip(offset)
            .Take(limit == 0 ? int.MaxValue : limit)
            .Select(u => new UserMainViewModel { UserId = u.Id ,CustomerName = u.Username })
            .ToListAsync();
    }

    public async Task<int> GetTotalUserCount(string search)
    {
        var query = _db.Users.AsQueryable();
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(u => u.Username.Contains(search));
        }
        return await query.CountAsync();
    }


}