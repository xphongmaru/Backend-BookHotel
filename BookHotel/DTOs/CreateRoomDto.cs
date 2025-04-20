using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class CreateRoomDto
{
    public string Name { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(1, int.MaxValue)]
    public int Max_occupancy { get; set; }

    public string Description { get; set; } 

    public string Status { get; set; }

    public IFormFile Thumbnail { get; set; }

    public List<IFormFile> RoomPhotos { get; set; } = new();

    public int TypeRoom_id { get; set; }

    public List<int> AmenityIds { get; set; } = new();
}
