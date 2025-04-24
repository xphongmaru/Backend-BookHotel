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
        public string Check_in { get; set; } = string.Empty;
        public string Check_out { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Request { get; set; } = string.Empty;
        public decimal total { get; set; }

     
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
        public string Check_in { get; set; } = string.Empty;
        public string Check_out { get; set; } = string.Empty;
        public int Room_id { get; set; }
        public int Quantity { get; set; }
        public string Name_Guess { get; set; } = string.Empty;
        public string Phone_Guess { get; set; } = string.Empty;
    }

    public class CheckoutCreateRequest
    {
        public string Check_in { get; set; } = string.Empty;
        public string Check_out { get; set; } = string.Empty;
        public string Discount { get; set; } = string.Empty;
        public List<Rooms> Rooms { get; set; }
    }

    public class Rooms
    {
        public int Room_id { get; set; }
        public int Quantity { get; set; }
        public string Name_Guess { get; set; } = string.Empty;
        public string Phone_Guess { get; set; } = string.Empty;
    }

    public class GetBookingRequest
    {
        public string Check_in { get; set; } = string.Empty;
        public string Check_out { get; set; } = string.Empty;
        public string Discount { get; set; } = string.Empty;
        public decimal total { get; set; }
        public int totalDays { get; set; }
        public decimal discount_value { get; set; }
        public decimal totalDiscount { get; set; }
        public List<Rooms> Rooms { get; set; } = new();
    }

    public class BookingRequest
    {
        public string Check_in { get; set; } = string.Empty;
        public string Check_out { get; set; } = string.Empty;
        public string Request { get; set; } = string.Empty;
        public string Discount { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public List<Rooms> Rooms { get; set; } = new();
    }

    public class StatusUpdateRequest
    {
        public int Booking_id { get; set; }

        public string Status { get; set; } = string.Empty;
    }

    public class BookingID
    {
        public int Booking_id { get; set; }
    }

    public class getAllBookingRespone()
    {
        public int Booking_id { get; set; }
        public string Check_in { get; set; } = string.Empty;
        public string Check_out { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Request { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public int guess_id { get; set; }
        public string guess_ava {  get; set; } = string.Empty;
        public string guess_name { get; set; } = string.Empty;
        public string guess_num {  get; set; } = string.Empty;
        public List<Booking_Room> Booking_Rooms { get; set; } = new();

        public int Quantity;
    }

    public class cancelBookingRequest()
    {
        public int Booking_id { get; set; }
        public string request { get; set; }=string.Empty;
    }

    public class getAllBookingRequest
    {
        public int Guess_id { get; set; }

        public string startdate { get; set; } = string.Empty;

        public string enddate { get; set; } = string.Empty;
    }
}