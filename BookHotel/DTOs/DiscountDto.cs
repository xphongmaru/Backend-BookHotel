using Microsoft.VisualBasic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BookHotel.DTOs;
using BookHotel.Models;

namespace BookHotel.DTOs
{
    public class DiscountDto
    {
        public string Code { get; set; } = string.Empty;
        public float Discount_percentage { get; set; }
        public float Price_applies { get; set; }
        public float Max_discount { get; set; }
        public string Start_date { get; set; }=string.Empty;
        public string End_date { get; set; }= string.Empty;
        public int Quantity { get; set; }
    }

    public class DiscountWithIdDto 
    {
        public int Discount_id { get; set; }
        public string Code { get; set; } = string.Empty;
        public float Discount_percentage { get; set; }
        public float Price_applies { get; set; }
        public float Max_discount { get; set; }
        public string Start_date { get; set; }
        public string End_date { get; set; }
        public bool Status { get; set; }
        public int Quantity { get; set; }
    }
    
}