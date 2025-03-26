using BookHotel.Models;
using System.ComponentModel.DataAnnotations;

namespace BookHotel.DTOs
{
    public class RoomListDto
    {
        public int Room_id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Thumbnail { get; set; } = string.Empty;
        public List<string> RoomPhotos { get; set; } = new(); // Danh sách ảnh
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public double AvgRating { get; set; }
        // 🔥 Thêm thông tin TypeRoom
        public object TypeRoom { get; set; }
    }

}
