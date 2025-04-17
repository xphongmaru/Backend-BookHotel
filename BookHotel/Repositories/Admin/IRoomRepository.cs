using BookHotel.DTOs;
using BookHotel.Models;
using System.Security.Claims;

namespace BookHotel.Repositories.Admin
{
    public interface IRoomRepository
    {
        // Lấy chi tiết phòng - phân quyền admin
        Task<Room?> GetRoomDetailsAsync(int roomId, ClaimsPrincipal user);

        // Lấy danh sách phòng có phân trang - phân quyền admin
        Task<(List<Room>, int)> GetRoomsAsync(int pageNumber, int pageSize, ClaimsPrincipal user);

        // Lấy danh sách phòng bán chạy - phân quyền admin
        Task<List<Room>> GetBestSellingRoomsAsync(ClaimsPrincipal user);

        // Phân loại phòng - phân quyền admin
        Task<List<Room>> FilterRoomsAsync(FilterRoomDto filterDto, ClaimsPrincipal user);

        // Tạo phòng - chỉ cho admin (controller đã kiểm soát)
        Task<Room> CreateRoomAsync(Room room);

        // Cập nhật phòng - chỉ cho admin (controller đã kiểm soát)
        Task<(bool Success, string Message)> UpdateRoomAsync(int roomId, UpdateRoomDto dto);

        // Xóa phòng - chỉ cho admin (controller đã kiểm soát)
        Task DeleteRoomAsync(int roomId);

        // Ẩn phòng - chỉ cho admin (controller đã kiểm soát)
        Task<(bool Success, string Message)> HideRoomAsync(int roomId);
    }
}
