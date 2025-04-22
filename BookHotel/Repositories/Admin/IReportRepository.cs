using BookHotel.DTOs.Report;
using BookHotel.Models;
using System.Security.Claims;

namespace BookHotel.Repositories.Admin
{
    public interface IReportRepository
    {
        Task<List<RoomRevenueDto>> GetTopRoomsRevenueAsync(int year, int? month, int? week, int topN);

    }
}
