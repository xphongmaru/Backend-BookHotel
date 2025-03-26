using BookHotel.Models;
using System.ComponentModel.DataAnnotations;

namespace BookHotel.DTOs
{
    public class RoomDto
    {
        public int Room_id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Max_occupancy { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public List<string> RoomPhotos { get; set; } = new(); // Danh sách ảnh

        public object TypeRoom { get; set; }
        public double Rating { get; set; }
        public List<AmenitiesDto> RoomAmenities { get; set; }
    }

}
