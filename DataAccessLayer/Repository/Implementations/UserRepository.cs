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
            User? userData = await _db.Users.FirstOrDefaultAsync(e => e.Email == model.Email);

            if (userData != null)
            {
                return false;
            }

            User? user = new User();
            user.Username = model.UserName;
            user.Email = model.Email;
            user.RoleId = model.RoleId;
            user.Password = model.Password;
            user.CreatedBy = 1;
            user.CreatedAt = DateTime.UtcNow;
            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Register: {ex.Message}");
            return false;
        }
    }

    public User GetUserByEmail(string email)
    {
        try
        {
            // var Items = _db.Items.FirstOrDefault(e => e.Id == 1);

            var users = _db.Users.ToList();

            var roles = _db.Roles.ToList();

            var items = _db.Items.ToList();

            var orders = _db.Orders.ToList();

            User? user = _db.Users.FirstOrDefault(e => e.Email == email);

            if (user == null) return null;

            return user;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetUserByEmail: {ex.Message}");
            return null;
        }
    }



}
