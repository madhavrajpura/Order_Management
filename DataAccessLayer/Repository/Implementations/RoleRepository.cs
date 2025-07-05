using DataAccessLayer.Models;
using DataAccessLayer.Repository.Interfaces;

namespace DataAccessLayer.Repository.Implementations;

public class RoleRepository : IRoleRepository
{
    private readonly NewItemOrderDbContext _db;
    public RoleRepository(NewItemOrderDbContext db) => _db = db;

    public List<Role> GetRoles()
    {
        return _db.Roles.ToList() ?? throw new Exception();
    }

    public string GetRoleById(int roleId)
    {
        string? roleName = string.Empty;
        Role? Role = _db.Roles.Find(roleId);
        return Role != null ? (roleName = Role.Name) : roleName = null;
    }

}