
namespace BookHotel.DTOs
{
    public class RoomListDto
    {
        public int Room_id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Thumbnail { get; set; } = string.Empty;
        public List<string> RoomPhotos { get; set; } = new(); // Danh sách ảnh
        public int Max_occupancy { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public double AvgRating { get; set; }

        public List<AmenitiesDto> RoomAmenities { get; set; }

        public string Status { get; set; } = string.Empty; // Trường trạng thái phòng

        public TypeRoomDto? TypeRoom { get; set; } // Dùng DTO thay vì object
    }

    public class TypeRoomDto
    {
        public int TypeRoom_id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
