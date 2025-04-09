using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BookHotel.Models
{
    public class Room : BaseEntity
    {
        [Key]
        public int Room_id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Max_occupancy { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        //lien ket
        [ForeignKey("TypeRoom")]
        public int TypeRoom_id { get; set; }
        [JsonIgnore]
        public TypeRoom TypeRoom { get; set; }

        [JsonIgnore]
        public List<RoomPhoto> RoomPhotos { get; set; } = new();

        [JsonIgnore]
        public List<Room_Amenities> Room_Amenities { get; set; } = new();

        [JsonIgnore]
        public List<Booking_Room> Booking_Rooms { get; set; } = new();

        [JsonIgnore]
        public List<Review> Reviews { get; set; } = new();
    }
}
