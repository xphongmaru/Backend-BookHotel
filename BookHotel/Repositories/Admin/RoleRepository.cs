using Microsoft.EntityFrameworkCore;
using BookHotel.Data;
using BookHotel.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BookHotel.Repositories.Admin
{
    public class RoleRepository:IRoleRepository
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

        public Task<Role> CreateRole(Role role)
        {
            _context.Roles.Add(role);
            return Task.FromResult(role);
        }
    }
}
