using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BookHotel.Models
{
    public class Booking:BaseEntity
    {
        [Key]
        public int Booking_id { get; set; }
        public DateTime Check_in { get; set; }
        public DateTime Check_out { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Request { get; set; } = string.Empty;

        //lien ket
        [ForeignKey("Guess")]
        public int Guess_id { get; set; }

        [JsonIgnore]
        public Guess Guess { get; set; }

        [JsonIgnore]
        public List<Booking_Room> Booking_Rooms { get; set; } = new();

        [JsonIgnore]
        public List<Booking_Discount> Booking_Discounts { get; set; } = new();
        public decimal Total { get; set; } = 0;
        public string DiscountCode { get; set; } = string.Empty;
    }
}

