using BookHotel.DTOs;
using BookHotel.Data;
namespace BookHotel.Repositories.Admin
{
    public class LoginAdmin: ILoginAdmin
    {
        private readonly AppDbContext _context;
        public LoginAdmin(AppDbContext context)
        {
            _context = context;
        }

        public Task<UserGetItem> LoginRequest(AdminLoginRequest request)
        {
            var result = _context.Users
                .Where(u => u.Email == request.Email)
                .Where(u => u.Password == request.Password)
                .Select(u => new UserGetItem
                {
                    User_id = u.User_id,
                    Fullname = u.Fullname,
                    Email = u.Email,
                    Phone = u.Phone,
                    Role_name = u.Role.Name
                })
                .FirstOrDefault();

            return Task.FromResult(result);
        }
    }
}
