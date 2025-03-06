using System.Text.Json.Serialization;

namespace BookHotel.Models
{
    public class Booking_Discount:BaseEntity
    {
        public int Booking_id { get; set; }
        [JsonIgnore]
        public Booking Booking { get; set; } = new();

        public int Discount_id { get; set; }
        [JsonIgnore]
        public Discount Discount { get; set; } = new();
    }
}


