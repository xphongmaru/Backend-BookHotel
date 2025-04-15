using System.ComponentModel.DataAnnotations;

namespace BookHotel.DTOs
{
    public class AmenityCreateDto
    {
        [Required(ErrorMessage = "Tên tiện nghi không được để trống")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }  // Có thể cho phép null nếu bạn không bắt buộc
    }
}
