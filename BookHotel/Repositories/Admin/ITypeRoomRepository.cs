using BookHotel.DTOs;
using BookHotel.Models;
using System.Security.Claims;

namespace BookHotel.Repositories.Admin
{
    public interface ITypeRoomRepository
    {
        Task<IEnumerable<TypeRoom>> GetAllAsync();
        
        Task<TypeRoom> GetByIdAsync(int id);
        Task<TypeRoom> CreateAsync(TypeRoom typeRoom);
        Task<bool> UpdateAsync(int id, TypeRoom updatedTypeRoom);
        Task<bool> DeleteAsync(int id);

    }
}
