using System.ComponentModel.DataAnnotations;


namespace BookHotel.DTOs
{
    public class CreateReview
    {
        public string Comment { get; set; } = string.Empty;
        [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao.")]
        public int Rating { get; set; }
        [Required(ErrorMessage = "Room_id không được để trống.")]
        public int Room_id { get; set; }
        public bool Anonymous { get; set; } = false;
    }

    public class GetAllReviewRoom
    {
        public int Review_id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string Guess_name { get; set; }
        public string Guess_avatar { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class GetAllReview
    {
        public int Review_id { get; set; }
        public int Room_id { get; set; }
        public string Room_name { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string Guess_name { get; set; }
        public string Guess_avatar { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
