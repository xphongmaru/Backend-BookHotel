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

        public double Rating { get; set; }  // ⭐ Điểm trung bình đánh giá
        public int TotalReviews { get; set; } // Tổng số đánh giá

        public List<AmenitiesDto> RoomAmenities { get; set; } = new(); // Tiện nghi

        public List<ReviewDto> Reviews { get; set; } = new(); // ⭐ Danh sách đánh giá chi tiết
    }

    public class ReviewDto
    {
        public int Review_id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string ReviewerName { get; set; } = string.Empty;
        public string ReviewerThumbnail { get; set; } = string.Empty;
        public string CreatedAt { get; set; }


    }
}

