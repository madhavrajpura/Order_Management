using DataAccessLayer.Context;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.Interfaces;

namespace DataAccessLayer.Repository.Implementations;

public class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext _db;
    public RoleRepository(ApplicationDbContext db) => _db = db;

    public List<Role> GetRoles()
    {
        try
        {
            return _db.Roles.ToList() ?? new List<Role>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetRoles: {ex.Message}");
            throw new Exception("Error retrieving roles", ex);
        }
    }

    public string GetRoleById(int roleId)
    {
        try
        {
            string? roleName = string.Empty;
            Role? Role = _db.Roles.Find(roleId);
            return Role != null ? (roleName = Role.Name) : roleName = null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetRoleById: {ex.Message}");
            throw new Exception("Error retrieving role by ID", ex);
        }

        // return await _db.Roles.FindAsync(roleId) ?? throw new CustomException("Role not found");
    }

}
