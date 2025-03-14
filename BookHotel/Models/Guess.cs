using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookHotel.Models
{
    public class Guess: BaseEntity
    {
        [Key]
        public int Guess_id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string CCCD { get; set; } = string.Empty;
        public DateOnly Bod { get; set; }
        public bool Gender { get; set; }
        public string Password { get; set; } = string.Empty;

        [JsonIgnore]
        public List<Booking> Bookings { get; set; } = new();

        [JsonIgnore]
        public List<Review> Reviews { get; set; } = new();

    }
}


