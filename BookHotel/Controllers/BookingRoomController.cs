using Microsoft.AspNetCore.Mvc;
using BookHotel.Repositories.Admin;
using BookHotel.Models;
using BookHotel.DTOs;
using System.Linq;
using System.Threading.Tasks;
using BookHotel.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BookHotel.Controllers
{
    [Route("api/booking-room")]
    [ApiController]
    public class BookingRoomController : ControllerBase
    {
        private readonly IBookingRoomRepository _bookingRoomRepository;
        private readonly AppDbContext _context;
        public BookingRoomController(AppDbContext context, IBookingRoomRepository bookingRoomRepository)
        {
            _bookingRoomRepository = bookingRoomRepository;
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetBookingRooms()
        {
            var bookingRoomList = await _bookingRoomRepository.GetBookingRoom();
            return Ok(new ApiResponse(true, bookingRoomList, null));
        }

        [HttpGet("details")]
        public  ActionResult GetBookingDetails(int id)
        {
            var bookingRoomList =  _context.Booking_Rooms.Where(br => br.Booking_id == id);
            return Ok(new ApiResponse(true, bookingRoomList, null));

        }





    }

   
}
