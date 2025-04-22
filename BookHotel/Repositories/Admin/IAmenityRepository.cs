using BookHotel.DTOs;
using BookHotel.Models;

namespace BookHotel.Repositories.Admin
{
    public interface IAmenityRepository
    {
        Task<IEnumerable<Amenities>> GetAllAsync();
        Task<List<AmenitiesDto>> GetAmenitiesByRoomIdAsync(int roomId);
        Task<Amenities?> GetByIdAsync(int id);
        Task<Amenities> CreateAsync(Amenities amenity);
        Task<bool> UpdateAsync(int id, Amenities amenity);
        Task<bool> DeleteAsync(int id);
    }
}
