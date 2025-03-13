using BookHotel.DTOs;
using BookHotel.Models;

namespace BookHotel.Repositories.Admin
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsers();
        Task<UserGetItem> GetUserById(int id);
        Task<User> CreateUser(UserCreateRequest request);
        Task UpdateUser(User user, UserEditRequest request);
    }
}
