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

       
    }

   
}

