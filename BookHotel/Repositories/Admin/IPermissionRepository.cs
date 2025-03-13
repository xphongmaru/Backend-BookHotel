using BookHotel.DTOs;
using BookHotel.Models;
namespace BookHotel.Repositories.Admin
{
    public interface IPermissionRepository
    {
        Task<IEnumerable<Permission>> GetPermissions();
        Task<PermissionGetRequest> GetPermissionByID(int id);
        Task<Permission> CreatePermission(PermissionCreateRequest request);
        Task UpdatePermission(Permission permission, PermissionCreateRequest request);
        Task DeletePermission(int id);

    }
}
