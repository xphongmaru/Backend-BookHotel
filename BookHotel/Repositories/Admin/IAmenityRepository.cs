using BookHotel.DTOs;
using BookHotel.Models;
using System.Security.Claims;

namespace BookHotel.Repositories.Admin
{
    public interface IAmenityRepository
    {
        Task<IEnumerable<Amenities>> GetAllAsync();
        Task<Amenities?> GetByIdAsync(int id);
        Task<Amenities> CreateAsync(Amenities amenity);
        Task<bool> UpdateAsync(int id, Amenities amenity);
        Task<bool> DeleteAsync(int id);
    }
}
