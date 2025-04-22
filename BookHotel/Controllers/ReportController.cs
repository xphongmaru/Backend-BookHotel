using Microsoft.AspNetCore.Mvc;
using BookHotel.Repositories.Admin;
using BookHotel.Responsee;
using Microsoft.EntityFrameworkCore;
using BookHotel.Data;
using BookHotel.Constant;
using BookHotel.DTOs.Report;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/report")]
public class ReportController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IReportRepository _reportRepository;

    public ReportController(AppDbContext context, IReportRepository reportRepository)
    {
        _context = context;
        _reportRepository = reportRepository;
    }

    //Báo cáo chọn ra Top N phòng có doanh thu cao nhất hiện tại
    [Authorize(Roles = "admin")]
    [HttpGet("admin/top-rooms-with-highest-revenue")]
    public async Task<IActionResult> GetTopRevenueRooms([FromQuery] int top = 5)
    {
        if (top <= 0)
        {
            return BadRequest(new BaseResponse<string>("Số lượng top phòng phải lớn hơn 0.", 400));
        }

        try
        {
            var revenueStatuses = new[]
            {
                BookingConstant.CLOSED,
                BookingConstant.CHECKED_IN,
                BookingConstant.RATED
        };

            var query = await _context.Booking_Rooms
                .Include(br => br.Booking)
                .Include(br => br.Room)
                .Where(br => revenueStatuses.Contains(br.Booking.Status))
                .GroupBy(br => new { br.Room_id, br.Room.Name })
                .Select(g => new RoomRevenueDto
                {
                    RoomId = g.Key.Room_id,
                    RoomName = g.Key.Name,
                    TotalRevenue = g.Sum(x => x.Price * x.Quantity),
                    TotalBookings = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(g => g.TotalRevenue)
                .Take(top)
                .ToListAsync();

            return Ok(new BaseResponse<List<RoomRevenueDto>>(query));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new BaseResponse<string>($"Lỗi hệ thống: {ex.Message}", 400));
        }
    }

    //Báo cáo chọn ra top N phòng có doanh thu cao nhất theo năm, có thể thêm tháng, tuần của năm đó
    [Authorize(Roles = "admin")]
    [HttpGet("admin/top-rooms-revenue-year-month-week")]
    public async Task<IActionResult> GetTopRoomsRevenue([FromQuery] int year, [FromQuery] int? month, [FromQuery] int? week, [FromQuery] int topN = 5)
    {
        if (topN <= 0)
            return BadRequest(new BaseResponse<string>("TopN phải lớn hơn 0.", 400));

        var currentYear = DateTime.Now.Year;
        if (year < 2000 || year > currentYear)
            return BadRequest(new BaseResponse<string>($"Năm không hợp lệ (2000 - {currentYear}).", 400));

        if (month is < 1 or > 12)
            return BadRequest(new BaseResponse<string>("Tháng phải nằm trong khoảng 1 - 12.", 400));

        if (week is < 1 or > 4)
            return BadRequest(new BaseResponse<string>("Tuần trong tháng chỉ được từ 1 đến 4.", 400));

        try
        {
            var rooms = await _reportRepository.GetTopRoomsRevenueAsync(year, month, week, topN);

            var reportResult = new TopRoomRevenueReportDto
            {
                Year = year,
                Month = month,
                Week = week,
                Data = rooms
            };

            return Ok(new BaseResponse<TopRoomRevenueReportDto>(reportResult));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new BaseResponse<string>($"Lỗi hệ thống: {ex.Message}", 400));
        }
    }

}
