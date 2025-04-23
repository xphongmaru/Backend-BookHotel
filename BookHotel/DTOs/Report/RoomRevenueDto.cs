namespace BookHotel.DTOs.Report
{
    public class RoomRevenueDto
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalBookings { get; set; }
    }
}
