using BookHotel.Data;

namespace BookHotel.Repositories.Admin
{
    public class Control : IControl
    {
        private readonly AppDbContext _context;
        //public IRoleRepository Roles { get; }
        //public IPermissionRepository Permissions { get; }
        //public IUserRepository User { get; }
        //public ILoginAdmin LoginAdmin { get; }
        public Control(AppDbContext context)
        {
            _context = context;
            //    Roles = new RoleRepository(_context);
            //    Permissions = new PermissionRepository(_context);
            //    User = new UserRepository(_context);
            //    LoginAdmin = new LoginAdmin(_context);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
