using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BookHotel.Models
{
    public class Review: BaseEntity
    {
        [Key]
        public int Review_id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;

        [ForeignKey("Room")]
        public int Room_id { get; set; }
        [JsonIgnore]
        public Room Room { get; set; }

        [ForeignKey("Guess")]
        public int Guess_id { get; set; }

        public bool Anonymous { get; set; } = false;

        [JsonIgnore]
        public Guess Guess { get; set; }
    }
}


