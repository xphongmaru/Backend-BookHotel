using System.ComponentModel.DataAnnotations;

namespace BookHotel.DTOs
{
    public class TypeRoomCreateDto
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }

}
