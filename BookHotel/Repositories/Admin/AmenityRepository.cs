using BookHotel.Data;
using BookHotel.DTOs;
using BookHotel.Exceptions;
using BookHotel.Models;
using Microsoft.EntityFrameworkCore;

namespace BookHotel.Repositories.Admin
{
    public class AmenityRepository : IAmenityRepository
    {
        private readonly AppDbContext _context;

        public AmenityRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Amenities>> GetAllAsync()
        {
            return await _context.Amenities.ToListAsync();
        }

        public async Task<List<AmenitiesDto>> GetAmenitiesByRoomIdAsync(int roomId)
        {
            var amenities = await _context.Room_Amenities
                .Where(ra => ra.Room_id == roomId && ra.Amenities != null)
                .Select(ra => new AmenitiesDto
                {
                    Amenities_id = ra.Amenities.Amenities_id,
                    Name = ra.Amenities.Name,
                    Description = ra.Amenities.Description
                })
                .ToListAsync();

            return amenities;
        }

        public async Task<Amenities?> GetByIdAsync(int id)
        {
            return await _context.Amenities.FindAsync(id);
        }

        public async Task<Amenities> CreateAsync(Amenities amenity)
        {
            var isExist = await _context.Amenities.AnyAsync(a => a.Name == amenity.Name);
            if (isExist)
            {
                throw new BadRequestException("Tên tiện nghi đã tồn tại.");
            }

            _context.Amenities.Add(amenity);
            await _context.SaveChangesAsync();
            return amenity;
        }

        public async Task<bool> UpdateAsync(int id, Amenities amenity)
        {
            var existing = await _context.Amenities.FindAsync(id);
            if (existing == null)
                return false;

            // Kiểm tra trùng tên (trừ chính nó)
            var isDuplicate = await _context.Amenities.AnyAsync(a => a.Amenities_id != id && a.Name == amenity.Name);
            if (isDuplicate)
            {
                throw new BadRequestException("Tên tiện nghi đã tồn tại.");
            }

            existing.Name = amenity.Name;
            existing.Description = amenity.Description;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var amenity = await _context.Amenities
                .Include(a => a.Room_Amenities)
                .FirstOrDefaultAsync(a => a.Amenities_id == id);

            if (amenity == null)
                return false;

            if (amenity.Room_Amenities.Any())
            {
                throw new BadRequestException("Không thể xóa tiện nghi đang được sử dụng.");
            }

            _context.Amenities.Remove(amenity);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
