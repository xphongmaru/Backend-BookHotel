using System.Text.Json.Serialization;

namespace BookHotel.Models
{
    public class Booking_Room:BaseEntity
    {
        public int Booking_id { get; set; }
        [JsonIgnore]
        public Booking Booking { get; set; } 
        public int Room_id { get; set; }
        [JsonIgnore]
        public Room Room { get; set; } 
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Name_Guess { get; set; } = string.Empty;
        public string Phone_Guess { get; set; } = string.Empty;

    }
}


    