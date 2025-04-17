using BookHotel.Models;
using System.ComponentModel.DataAnnotations;

namespace BookHotel.DTOs
{
    public class FilterRoomDto
    {
        public string? Name { get; set; }

        public int? MaxOccupancy { get; set; }

        public int? TypeRoomId { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public string? Status { get; set; }

        public double? MinRating { get; set; }

        public List<int>? AmenityIds { get; set; } = new();
    }

}

