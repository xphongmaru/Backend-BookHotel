using BookHotel.DTOs;
using BookHotel.Models;
using System.Security.Claims;

namespace BookHotel.Repositories.Admin
{
    public interface ITypeRoomRepository
    {
        Task<List<TypeRoom>> GetAllAsync(int pageNumber, int pageSize); 
        Task<int> CountAllAsync(); 
        Task<TypeRoom> GetByIdAsync(int id);
        Task<TypeRoom> CreateAsync(TypeRoom typeRoom);
        Task<bool> UpdateAsync(int id, TypeRoom updatedTypeRoom);
        Task<bool> DeleteAsync(int id);

    }
}
