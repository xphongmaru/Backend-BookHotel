using BookHotel.Data;
using BookHotel.DTOs;
using BookHotel.Models;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace BookHotel.Repositories.Admin
{
    public class UserRepository: IUserRepository
    {
        public readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<UserGetItem> GetUserById(int id)
        {
            return await _context.Users
                .Where(u => u.User_id == id)
                .Select(u => new UserGetItem
                {
                    User_id = u.User_id,
                    Username = u.Username,
                    Fullname = u.Fullname,
                    Email = u.Email,
                    Phone = u.Phone,
                    Role_id = u.Role_id,
                })
                .FirstOrDefaultAsync();
        }
        public Task<User> CreateUser(UserCreateRequest request)
        {
            var user = new User
            {
                Username = request.Username,
                Password = request.Password,
                Fullname = request.Fullname,
                Email = request.Email,
                Phone = request.Phone,
                Role_id = request.Role_id
            };
            _context.Users.Add(user);
            return Task.FromResult(user);
        }

        public async Task UpdateUser(User user, UserEditRequest request)
        {
            user.Fullname = request.Fullname;
            user.Email = request.Email;
            user.Phone = request.Phone;
            user.Role_id = request.Role_id;
            _context.Entry(user).State= EntityState.Modified;
        }
    }
}
