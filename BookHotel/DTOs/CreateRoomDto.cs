using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class CreateRoomDto
{
    [Required]
    public string Name { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(1, int.MaxValue)]
    public int Max_occupancy { get; set; }

    public string Description { get; set; }

    [Required]
    public string Status { get; set; }

    [Required]
    public IFormFile Thumbnail { get; set; } // 1 ảnh đại diện

    public List<IFormFile> RoomPhotos { get; set; } // nhiều ảnh mô tả

    [Required]
    public int TypeRoom_id { get; set; }

    public List<int> AmenityIds { get; set; }
}
