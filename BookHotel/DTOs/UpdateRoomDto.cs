using System.ComponentModel.DataAnnotations;

public class UpdateRoomDto
{
    public string Name { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    public string Description { get; set; }

    public string Status { get; set; }

    [Range(1, int.MaxValue)]
    public int MaxPeople { get; set; }

    public int TypeRoom_id { get; set; }

    public List<int> AmenityIds { get; set; } = new();

    public IFormFile? Thumbnail { get; set; } 

    public List<IFormFile>? RoomPhotos { get; set; } = new(); 
}
