using BookHotel.Data;

namespace BookHotel.Repositories.Admin
{
    public class Control: IControl
    {
        private readonly AppDbContext _context;
        public IRoleRepository Roles { get; }

        public Control(AppDbContext context)
        {
            _context = context;
            Roles = new RoleRepository(_context);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
