namespace BookHotel.DTOs.Report
{
    public class TopRoomRevenueReportDto
    {
        public int Year { get; set; }
        public int? Month { get; set; }
        public int? Week { get; set; }
        public List<RoomRevenueDto> Data { get; set; }
    }
}
