using Microsoft.EntityFrameworkCore;
using BookHotel.Data;
using BookHotel.Models;
using BookHotel.DTOs;

namespace BookHotel.Repositories.Admin
{
    public class PermissionRepository:IPermissionRepository
    {
        public AppDbContext _context;
        public PermissionRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Permission>> GetPermissions()
        {
            return await _context.Permissions.ToListAsync();
        }

        public async Task<PermissionGetRequest> GetPermissionByID(int id)
        {
            return await _context.Permissions
                 .Where(p => p.Permission_id == id)
                 .Select(p => new PermissionGetRequest
                 {
                     Permission_id = p.Permission_id,
                     Name = p.Name,
                 }).FirstOrDefaultAsync();
        }

        public Task<Permission> CreatePermission(PermissionCreateRequest request)
        {
            var permission = new Permission {Name= request.Name };
            _context.Permissions.Add(permission);
            return Task.FromResult(permission);
        }

        public async Task UpdatePermission(Permission permission, PermissionCreateRequest request)
        {
            if(!string.IsNullOrEmpty(request.Name))
            {
                permission.Name = request.Name;
            }
            _context.Entry(permission).State = EntityState.Modified;
        }
        public async Task DeletePermission(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission != null) {
                _context.Remove(permission);
            }
        }

    }
}
