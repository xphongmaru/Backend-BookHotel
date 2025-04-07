using BookHotel.DTOs;
using BookHotel.Models;
using BookHotel.Data;
using Microsoft.EntityFrameworkCore;



namespace BookHotel.Repositories.Admin
{
    public class RoomRepository : IRoomRepository
    {
        private readonly AppDbContext _context;

        public RoomRepository(AppDbContext context)
        {
            _context = context;
        }

        //Lấy thông tin chi tiết phòng
        public async Task<Room?> GetRoomDetailsAsync(int roomId)
        {
            return await _context.Rooms!
                .Include(r => r.TypeRoom)
                .Include(r => r.RoomPhotos) // Lấy danh sách ảnh
                .Include(r => r.Room_Amenities)
                .ThenInclude(ra => ra.Amenities)
                .Include(r => r.Reviews)
                .FirstOrDefaultAsync(r => r.Room_id == roomId);
        }

        //Lấy danh sách phòng có phân trang
        public async Task<(List<Room>, int)> GetRoomsAsync(int pageNumber, int pageSize)
        {
            var totalRooms = await _context.Rooms.CountAsync(); // Tổng số phòng

            var rooms = await _context.Rooms
                .Include(r => r.TypeRoom)
                .Include(r => r.RoomPhotos) // Lấy danh sách ảnh
                .Skip((pageNumber - 1) * pageSize) // Bỏ qua các trang trước đó
                .Take(pageSize) // Lấy số lượng phòng theo `pageSize`
                .ToListAsync();

            return (rooms, totalRooms);
        }

        //---------
        //Lấy danh sách phòng bán chạy
        public async Task<List<Room>> GetBestSellingRoomsAsync()
        {
            return await _context.Rooms
                .Include(r => r.Reviews)
                .Include(r => r.Booking_Rooms)
                .Include(r => r.RoomPhotos)
                .Include(r => r.TypeRoom)
                .ToListAsync(); // Chỉ lấy dữ liệu, sắp xếp xử lý bên ngoài
        }

        //Phân loại phòng giựa trên kiểu phòng
        public async Task<List<Room>> FilterRoomsAsync(
    string? name,
    int? maxOccupancy,
    int? typeRoomId,
    decimal? minPrice,
    decimal? maxPrice,
    string? status,
    double? minRating
)
        {
            var query = _context.Rooms
                .Include(r => r.TypeRoom)
                .Include(r => r.RoomPhotos)
                .Include(r => r.Booking_Rooms)
                .Include(r => r.Reviews)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(r => r.Name.Contains(name));

            if (maxOccupancy.HasValue)
                query = query.Where(r => r.Max_occupancy >= maxOccupancy);

            if (typeRoomId.HasValue)
                query = query.Where(r => r.TypeRoom_id == typeRoomId);

            if (minPrice.HasValue)
                query = query.Where(r => r.Price >= minPrice);

            if (maxPrice.HasValue)
                query = query.Where(r => r.Price <= maxPrice);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(r => r.Status.ToLower() == status.ToLower());

            if (minRating.HasValue)
                query = query.Where(r => r.Reviews.Any() && r.Reviews.Average(rev => rev.Rating) >= minRating);

            return await query.ToListAsync();
        }




    }

}
