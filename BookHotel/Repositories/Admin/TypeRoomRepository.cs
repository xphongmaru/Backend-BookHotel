using BookHotel.DTOs;
using BookHotel.Models;
using BookHotel.Data;
using BookHotel.Constant;
using Microsoft.EntityFrameworkCore;
using BookHotel.Exceptions;
using System.Security.Claims;

namespace BookHotel.Repositories.Admin
{
    public class TypeRoomRepository : ITypeRoomRepository
    {
        private readonly AppDbContext _context;

        public TypeRoomRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TypeRoom>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _context.TypeRooms
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAllAsync()
        {
            return await _context.TypeRooms.CountAsync();
        }


        public async Task<TypeRoom> GetByIdAsync(int id)
        {
            return await _context.TypeRooms.FindAsync(id);
        }

        public async Task<TypeRoom> CreateAsync(TypeRoom typeRoom)
        {
            // Kiểm tra trùng tên
            bool isDuplicate = await _context.TypeRooms
                .AnyAsync(t => t.Name.ToLower() == typeRoom.Name.ToLower());
            if (isDuplicate)
                throw new BadRequestException("Tên loại phòng đã tồn tại");

            _context.TypeRooms.Add(typeRoom);
            await _context.SaveChangesAsync();
            return typeRoom;
        }

        public async Task<bool> UpdateAsync(int id, TypeRoom updatedTypeRoom)
        {
            var existing = await _context.TypeRooms.FindAsync(id);
            if (existing == null) return false;

            // Kiểm tra trùng tên với loại phòng khác
            bool isDuplicate = await _context.TypeRooms
                .AnyAsync(t => t.TypeRoom_id != id && t.Name.ToLower() == updatedTypeRoom.Name.ToLower());
            if (isDuplicate)
                throw new BadRequestException("Tên loại phòng đã tồn tại");

            existing.Name = updatedTypeRoom.Name;
            existing.Description = updatedTypeRoom.Description;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.TypeRooms.FindAsync(id);
            if (existing == null) return false;

            bool isUsed = await _context.Rooms.AnyAsync(r => r.TypeRoom_id == id);
            if (isUsed)
                throw new BadRequestException("Không thể xóa vì có phòng đang sử dụng kiểu phòng này");

            _context.TypeRooms.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
