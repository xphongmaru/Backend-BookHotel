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
        public DateTime Check_in { get; set; }
        public DateTime Check_out { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Request { get; set; } = string.Empty;
        public decimal total { get; set; }
        public int Guess_id { get; set; }
        public List<Booking_Room> Booking_Rooms { get; set; } = new();
    }

    public class PaymentRequest
    {
        public string DiscountCode { get; set; }=string.Empty;
        public decimal total { get; set; }
    }


    public class BookingRoomCreateRequest
    {
        public DateTime Check_in { get; set; }
        public DateTime Check_out { get; set; }
        public int Room_id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Name_Guess { get; set; } = string.Empty;
        public string Phone_Guess { get; set; } = string.Empty;
    }

    
}