using BookHotel.DTOs;
using BookHotel.Models;
namespace BookHotel.Repositories.Admin
{
    public interface IRoomRepository
    {
        Task<Room?> GetRoomDetailsAsync(int roomId);


        // Phương thức lấy danh sách phòng có phân trang
        Task<(List<Room>, int)> GetRoomsAsync(int pageNumber, int pageSize);

        // Phương thức lấy danh sách phòng bán chạy
        Task<List<Room>> GetBestSellingRoomsAsync(int topN);
    }
}
