using Microsoft.VisualBasic;    
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BookHotel.DTOs;
using BookHotel.Models;

namespace BookHotel.DTOs
{
    public class BookingRespone
    {
        public int Booking_id { get; set; }
        public string Check_in { get; set; }=string.Empty;
        public string Check_out { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Request { get; set; } = string.Empty;
        public decimal total { get; set; }
        public int Guess_id { get; set; }
        public List<Booking_Room> Booking_Rooms { get; set; } = new();
    }

    public class BookingRoomDeleteRequest
    {
        public int Booking_id { get; set; }
        
        public int room_id { get; set; }
    }

    public class DiscountRequest
    {
        public string DiscountCode { get; set; } = string.Empty;
        public decimal total { get; set; }
    }

    public class BookingRoomCreateRequest
    {
        public string Check_in { get; set; }= string.Empty;
        public string Check_out { get; set; }= string.Empty;
        public int Room_id { get; set; }
        public int Quantity { get; set; }
        public string Name_Guess { get; set; } = string.Empty;
        public string Phone_Guess { get; set; } = string.Empty;
    }

    
}