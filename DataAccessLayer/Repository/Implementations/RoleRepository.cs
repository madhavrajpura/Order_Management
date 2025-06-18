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
        List<Role> roles = _db.Roles.ToList();
        return roles;
    }

    public string GetRoleById(int roleId)
    {
        Role? roleName = _db.Roles.Find(roleId);
        return roleName!.Name;

        // return await _db.Roles.FindAsync(roleId) ?? throw new CustomException("Role not found");
    }

}
