using DataAccessLayer.Models;

namespace DataAccessLayer.Repository.Interfaces;

public interface IRoleRepository
{
    List<Role> GetRoles();
    string GetRoleById(int roleId);
}
