
using BookHotel.DTOs;

namespace BookHotel.Repositories.Admin
{
    public interface ILoginAdmin
    {
        Task<UserGetItem> LoginRequest(AdminLoginRequest request);
    }
}
