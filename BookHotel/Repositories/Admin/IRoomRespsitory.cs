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
        Task<List<Room>> GetBestSellingRoomsAsync();

        //Phương thức phân loại phòng dựa theo Typeroom_id
        Task<List<Room>> FilterRoomsAsync(
            string? name,
            int? maxOccupancy,
            int? typeRoomId,
            decimal? minPrice,
            decimal? maxPrice,
            string? status,
            double? minRating);

    }
}
