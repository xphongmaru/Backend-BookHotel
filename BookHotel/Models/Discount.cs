using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookHotel.Models
{
    public class Discount:BaseEntity
    {
        [Key]
        public int Discount_id { get; set; }
        public string Code { get; set; } = string.Empty;
        public float Discount_percentage { get; set; }
        public float Price_applies { get; set; }
        public float Max_discount { get; set; }
        public DateTime Start_date{ get; set; }
        public DateTime End_date { get; set; }
        public bool Status { get; set; }
        [JsonIgnore]
        public List<Booking_Discount> Booking_Discounts { get; set; } = new();

        public int Quantity { get; set; }

    }

}

