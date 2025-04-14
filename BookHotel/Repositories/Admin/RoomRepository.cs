using BookHotel.DTOs;
using BookHotel.Models;
using BookHotel.Data;
using BookHotel.Constant;
using Microsoft.EntityFrameworkCore;
using BookHotel.Exceptions;
using System.Security.Claims;

namespace BookHotel.Repositories.Admin
{
    public class RoomRepository : IRoomRepository
    {
        private readonly AppDbContext _context;

        public RoomRepository(AppDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin(ClaimsPrincipal user)
        {
            return user.Claims.Any(c =>
                c.Type == ClaimTypes.Role && c.Value.Equals("admin", StringComparison.OrdinalIgnoreCase));
        }

        // Lấy thông tin chi tiết phòng
        public async Task<Room?> GetRoomDetailsAsync(int roomId, ClaimsPrincipal user)
        {
            var isAdmin = IsAdmin(user);

            var room = await _context.Rooms!
                .Include(r => r.TypeRoom)
                .Include(r => r.RoomPhotos)
                .Include(r => r.Room_Amenities).ThenInclude(ra => ra.Amenities)
                .Include(r => r.Reviews).ThenInclude(r => r.Guess)
                .FirstOrDefaultAsync(r => r.Room_id == roomId);

            if (!isAdmin && room?.Status == RoomStatus.Hidden)
                return null;

            return room;
        }

        // Lấy danh sách phòng có phân trang
        public async Task<(List<Room>, int)> GetRoomsAsync(int pageNumber, int pageSize, ClaimsPrincipal user)
        {
            var isAdmin = IsAdmin(user);

            var query = _context.Rooms
                .Include(r => r.RoomPhotos)
                .Include(r => r.TypeRoom)
                .Include(r => r.Reviews)
                .AsQueryable();

            if (!isAdmin)
                query = query.Where(r => r.Status != RoomStatus.Hidden);

            var totalRooms = await query.CountAsync();

            var rooms = await query
                .OrderBy(r => r.Room_id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (rooms, totalRooms);
        }

        // Lấy danh sách phòng bán chạy
        public async Task<List<Room>> GetBestSellingRoomsAsync(ClaimsPrincipal user)
        {
            var isAdmin = IsAdmin(user);

            var query = _context.Rooms
                .Include(r => r.Reviews)
                .Include(r => r.Booking_Rooms)
                .Include(r => r.RoomPhotos)
                .Include(r => r.TypeRoom)
                .AsQueryable();

            if (!isAdmin)
                query = query.Where(r => r.Status != RoomStatus.Hidden);

            return await query.ToListAsync();
        }

        // Phân loại phòng
        public async Task<List<Room>> FilterRoomsAsync(
            string? name, int? maxOccupancy, int? typeRoomId,
            decimal? minPrice, decimal? maxPrice, string? status,
            double? minRating, ClaimsPrincipal user)
        {
            bool isAdmin = IsAdmin(user);

            var query = _context.Rooms
                .Include(r => r.TypeRoom)
                .Include(r => r.RoomPhotos)
                .Include(r => r.Booking_Rooms)
                .Include(r => r.Reviews)
                .AsQueryable();

            query = query
                .Where(r =>
                    (string.IsNullOrWhiteSpace(name) || r.Name.Contains(name)) &&
                    (!maxOccupancy.HasValue || r.Max_occupancy >= maxOccupancy) &&
                    (!typeRoomId.HasValue || r.TypeRoom_id == typeRoomId) &&
                    (!minPrice.HasValue || r.Price >= minPrice) &&
                    (!maxPrice.HasValue || r.Price <= maxPrice) &&
                    (string.IsNullOrWhiteSpace(status) || r.Status.ToLower() == status.ToLower()) &&
                    (!minRating.HasValue || (r.Reviews.Any() && r.Reviews.Average(rev => rev.Rating) >= minRating)) &&
                    (isAdmin || r.Status != RoomStatus.Hidden)
                );

            return await query.ToListAsync();
        }


        //Thêm phòng
        public async Task<Room> CreateRoomAsync(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<(bool Success, string Message)> UpdateRoomAsync(int roomId, UpdateRoomDto dto)
        {
            var room = await _context.Rooms
                .Include(r => r.RoomPhotos)
                .Include(r => r.Room_Amenities)
                .FirstOrDefaultAsync(r => r.Room_id == roomId);

            if (room == null)
                return (false, "Không tìm thấy phòng");

            var typeRoomExists = await _context.TypeRooms.AnyAsync(t => t.TypeRoom_id == dto.TypeRoom_id);
            if (!typeRoomExists)
                return (false, "Kiểu phòng không tồn tại");

            var allAmenitiesExist = await _context.Amenities
                .Where(a => dto.AmenityIds.Contains(a.Amenities_id))
                .CountAsync() == dto.AmenityIds.Count;

            if (!allAmenitiesExist)
                return (false, "Một hoặc nhiều tiện nghi không tồn tại");

            // ✅ Validate trạng thái nếu cần
            var validStatuses = new[] { RoomStatus.Available, RoomStatus.Hidden, RoomStatus.Unavailable };
            if (!validStatuses.Contains(dto.Status))
                return (false, "Trạng thái phòng không hợp lệ");

            // Cập nhật thông tin cơ bản
            room.Name = dto.Name;
            room.Price = dto.Price;
            room.Status = dto.Status; // đã dùng constant bên ngoài
            room.Description = dto.Description;
            room.TypeRoom_id = dto.TypeRoom_id;
            room.Max_occupancy = dto.MaxPeople;

            // Xử lý Thumbnail nếu có
            if (dto.Thumbnail != null)
            {
                var thumbnailFileName = $"{Guid.NewGuid()}_{dto.Thumbnail.FileName}";
                var thumbnailPath = Path.Combine("wwwroot/uploads", thumbnailFileName);

                using (var stream = new FileStream(thumbnailPath, FileMode.Create))
                {
                    await dto.Thumbnail.CopyToAsync(stream);
                }

                room.Thumbnail = $"/uploads/{thumbnailFileName}";
            }

            // Xử lý RoomPhotos nếu có
            if (dto.RoomPhotos != null && dto.RoomPhotos.Any())
            {
                _context.RoomPhotos.RemoveRange(room.RoomPhotos);

                foreach (var photo in dto.RoomPhotos)
                {
                    var photoFileName = $"{Guid.NewGuid()}_{photo.FileName}";
                    var photoPath = Path.Combine("wwwroot/uploads", photoFileName);

                    using (var stream = new FileStream(photoPath, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }

                    room.RoomPhotos.Add(new RoomPhoto
                    {
                        Room_id = roomId,
                        Image_url = $"/uploads/{photoFileName}"
                    });
                }
            }

            // Cập nhật tiện nghi
            room.Room_Amenities.Clear();
            foreach (var id in dto.AmenityIds)
            {
                room.Room_Amenities.Add(new Room_Amenities
                {
                    Room_id = roomId,
                    Amenities_id = id
                });
            }

            await _context.SaveChangesAsync();
            return (true, "Thành công");
        }


        // Xóa phòng
        public async Task DeleteRoomAsync(int roomId)
        {
            var room = await _context.Rooms
                .Include(r => r.RoomPhotos)
                .FirstOrDefaultAsync(r => r.Room_id == roomId);

            if (room == null)
                throw new NotFoundException("Không tìm thấy phòng.");

            var totalBookings = await _context.Booking_Rooms
                .Where(br => br.Room_id == roomId)
                .SumAsync(br => br.Quantity);

            if (totalBookings > 0)
                throw new BadRequestException("Không thể xóa phòng đã từng được đặt.");

            // Xóa file Thumbnail nếu có
            DeleteFileIfExists(room.Thumbnail);

            // Xóa các file ảnh mô tả
            foreach (var photo in room.RoomPhotos)
            {
                DeleteFileIfExists(photo.Image_url);
            }

            // Xóa dữ liệu liên quan khỏi DB
            _context.RoomPhotos.RemoveRange(room.RoomPhotos);

            var amenities = await _context.Room_Amenities
                .Where(ra => ra.Room_id == roomId)
                .ToListAsync();
            _context.Room_Amenities.RemoveRange(amenities);

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
        }

        // Ẩn phòng
        public async Task<(bool Success, string Message)> HideRoomAsync(int roomId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null)
                return (false, "Không tìm thấy phòng");

            room.Status = RoomStatus.Hidden;
            await _context.SaveChangesAsync();

            return (true, "Đã ẩn phòng thành công");
        }

        // Xóa phòng
        private void DeleteFileIfExists(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return;

            var fullPath = Path.Combine("wwwroot", relativePath.TrimStart('/'));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
