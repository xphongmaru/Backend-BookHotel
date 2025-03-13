using Microsoft.EntityFrameworkCore;
using BookHotel.Data;
using BookHotel.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using BookHotel.DTOs;

namespace BookHotel.Repositories.Admin
{
    public class RoleRepository : IRoleRepository
    {
        public AppDbContext _context;
        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Role>> GetRoles()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<RoleGetItem> GetRole(int id)
        {
            //return await _context.Roles.FindAsync(id);
            return await _context.Roles
                .Where(r => r.Role_id == id)
                .Select(r => new RoleGetItem
                {
                    Role_id = r.Role_id,
                    Name = r.Name,
                    PermissonIds = r.Role_Permissions
                    .Select(rp => rp.Permission.Permission_id)
                    .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public Task<Role> CreateRole(RoleCreateRequest request)
        {
            var role = new Role
            {
                Name = request.Name,
            };
            _context.Roles.Add(role);
            _context.SaveChanges();

            var rolePermissions = request.PermissonIds.Select(permission_id => new Role_Permission
            {
                Role_id = role.Role_id,
                Permission_id = permission_id

            }).ToList();
            _context.Role_Permissions.AddRange(rolePermissions);

            return Task.FromResult(role);
        }
        public async Task UpdateRole(Role role, RoleCreateRequest request) {
            if (!string.IsNullOrEmpty(request.Name))
            {
                role.Name = request.Name;
            }
            _context.Entry(role).State = EntityState.Modified;
            var rolePermisonId = _context.Role_Permissions
                .Where(rp => rp.Role_id == role.Role_id);
            _context.Role_Permissions.RemoveRange(rolePermisonId);

            var rolePermission = request.PermissonIds.Select(
                permissionId => new Role_Permission
                {
                    Role_id = role.Role_id,
                    Permission_id = permissionId
                }).ToList();
            _context.Role_Permissions.AddRange(rolePermission);

        }

        public async Task DeleteRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role != null) {
                var rolePermisonId = _context.Role_Permissions
                    .Where(rp => rp.Role_id == role.Role_id);
                _context.Role_Permissions.RemoveRange(rolePermisonId);
                _context.Remove(role);
            }
        }
    }
}
