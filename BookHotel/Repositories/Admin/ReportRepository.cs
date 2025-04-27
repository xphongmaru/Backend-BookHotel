using BookHotel.Constant;
using BookHotel.Data;
using BookHotel.DTOs.Report;
using BookHotel.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BookHotel.Repositories.Admin
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;

        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<RoomRevenueDto>> GetTopRoomsRevenueAsync(int year, int? month, int? week, int topN)
        {
            var query = _context.Booking_Rooms
                .Include(br => br.Room)
                .Include(br => br.Booking)
                .Where(br => br.Booking.Status == BookingConstant.APPROVED || br.Booking.Status == BookingConstant.RATED ||
                        br.Booking.Status == BookingConstant.CHECKED_IN
                &&
                             br.Booking.CreatedAt.Year == year);
                


            if (month.HasValue)
            {
                if (month < 1 || month > 12)
                    throw new BadRequestException("Hãy chọn tháng từ 1 đến 12");

                query = query.Where(br => br.Booking.CreatedAt.Month == month.Value);

                if (week.HasValue)
                {
                    if (week < 1 || week > 4)
                        throw new BadRequestException("Hãy chọn tuần từ 1 đến 4");

                    var firstDayOfMonth = new DateTime(year, month.Value, 1);
                    var daysInMonth = DateTime.DaysInMonth(year, month.Value);

                    // Tính khoảng ngày của tuần (1–4)
                    var startDay = (week.Value - 1) * 7 + 1;
                    var endDay = week.Value == 4 ? daysInMonth : Math.Min(startDay + 6, daysInMonth);

                    var startDate = new DateTime(year, month.Value, startDay);
                    var endDate = new DateTime(year, month.Value, endDay).AddDays(1); 

                    query = query.Where(br => br.Booking.CreatedAt >= startDate && br.Booking.CreatedAt < endDate);
                }
            }

            var topRooms = await query
                .GroupBy(br => new { br.Room_id, br.Room.Name })
                .Select(g => new RoomRevenueDto
                {
                    RoomId = g.Key.Room_id,
                    RoomName = g.Key.Name,
                    TotalRevenue = g.Sum(x => x.Price * x.Quantity),
                    TotalBookings = g.Count()
                })
                .OrderByDescending(r => r.TotalRevenue)
                .Take(topN)
                .ToListAsync();

            return topRooms;
        }



    }
}
