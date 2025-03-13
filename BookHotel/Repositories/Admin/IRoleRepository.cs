using BookHotel.DTOs;
using BookHotel.Models;

namespace BookHotel.Repositories.Admin
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetRoles();
        Task<RoleGetItem> GetRole(int id);
        Task<Role> CreateRole(RoleCreateRequest request);
        Task UpdateRole(Role role, RoleCreateRequest request);
        Task DeleteRole(int id);

    }
}
