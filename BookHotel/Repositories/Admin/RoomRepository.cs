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

        //Lấy danh sách phòng bán chạy
        public async Task<List<Room>> GetBestSellingRoomsAsync(int topN)
        {
            var bestSellingRooms = await _context.Rooms
                .Include(r => r.Booking_Rooms)
                .Include(r => r.Reviews)
                .Include(r => r.RoomPhotos)
                .OrderByDescending(r => r.Booking_Rooms.Count)  // Ưu tiên số lần đặt phòng
                //.ThenByDescending(r => r.Booking_Rooms.Sum(b => b.TotalPrice)) // Ưu tiên doanh thu cao
                .ThenByDescending(r => r.Reviews.Average(rev => rev.Rating)) // Ưu tiên đánh giá cao
                .Take(topN)
                .ToListAsync();

            return bestSellingRooms;
        }
    }

}
