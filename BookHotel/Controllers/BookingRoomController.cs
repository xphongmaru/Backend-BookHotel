using Microsoft.AspNetCore.Mvc;
using BookHotel.Repositories.Admin;
using BookHotel.Models;
using BookHotel.DTOs;
using System.Linq;
using System.Threading.Tasks;
using BookHotel.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BookHotel.Controllers
{
    [Route("api/booking-room")]
    [ApiController]
    public class BookingRoomController : ControllerBase
    {
        private readonly IBookingRoomRepository _bookingRoomRepository;
        private readonly AppDbContext _context;
        public BookingRoomController(AppDbContext context,IBookingRoomRepository bookingRoomRepository)
        {
            _bookingRoomRepository = bookingRoomRepository;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetBooking()
        {
            var bookingRoomList = await _bookingRoomRepository.GetBookingRoom();
            return Ok(bookingRoomList);
        }

        [HttpPost("id")]
        public async Task<ActionResult> CreateBooking(int id,[FromBody] BookingRoomCreateRequest request)
        {
            var booking = _context.Bookings.FirstOrDefault(b=>b.Guess_id==id&&b.Status=="open");

            if (booking == null)
            {
                booking = new Booking
                {
                    Status = "open",
                    Guess_id = id
                };
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
            }

            if (booking != null) { 
               booking.Booking_Rooms=_bookingRoomRepository.GetBookingRoombyId(booking.Booking_id).Result;
            }

            var bookingRoom = new Booking_Room
            {
                Booking_id = booking.Booking_id,
                Room_id = request.Room_id,
                Quantity = request.Quantity,
                Price = request.Price,
                Name_Guess = request.Name_Guess,
                Phone_Guess = request.Phone_Guess,
            };

            _context.Booking_Rooms.Add(bookingRoom);
            await _context.SaveChangesAsync();

            var respone = new BookingRespone
            {
                Booking_id=booking.Booking_id,
                Check_in = booking.Check_in,
                Check_out = booking.Check_out,
                Status = booking.Status,
                Description = booking.Description,
                Request=booking.Request,
                Guess_id = booking.Guess_id,
                Booking_Rooms = booking.Booking_Rooms,  
            };
            
            return Ok(respone);
        }

        [HttpPut("id")]
        public async Task<ActionResult> CheckOut(int id)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.Guess_id == id && b.Status == "open");
            booking.Status="closed";

            _context.Entry(booking).State=EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(booking);
        }
    }

   
}

