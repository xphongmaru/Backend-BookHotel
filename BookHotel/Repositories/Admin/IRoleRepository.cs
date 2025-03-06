using BookHotel.Models;

namespace BookHotel.Repositories.Admin
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetRoles();
        Task<Role> CreateRole(Role role);
    }
}
